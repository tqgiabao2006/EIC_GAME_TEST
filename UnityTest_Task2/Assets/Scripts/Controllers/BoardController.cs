using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public bool IsBusy { get; private set; }

    public Board g_board;

    private GameManager m_gameManager;

    private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private List<Cell> m_potentialMatch;

    private float m_timeAfterFill;

    private bool m_hintIsShown;

    private bool m_gameOver;
    
    private PlayerBoard m_playerBoard;
    
    private bool m_hasCliked = false;

    private bool m_autoPlay;
    
    private bool m_autoLose;

    private bool m_attack;

    public void StartGame(GameManager gameManager, GameSettings gameSettings, PlayerBoard playerBoard)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        g_board = new Board(this.transform, gameSettings);
        
        m_playerBoard = playerBoard;
        
        m_hasCliked = false;

        Fill();
    }

    private void Fill()
    {
        g_board.Fill();
        
        m_playerBoard.g_curBoardCount = g_board.Count();
        
        // FindMatchesAndCollapse();
    }
    
    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.AUTO_PLAY:
                m_autoPlay = true;
                break;
            case GameManager.eStateGame.AUTO_LOSE:
                m_autoLose = true;
                break;
            case GameManager.eStateGame.ATTACK:
                m_attack = true;
                break;
            case GameManager.eStateGame.GAME_LOSE:
                m_gameOver = true;
                m_autoLose = false;
                m_autoPlay = false;
                m_attack = false;
                break;
        }
    }


    public void Update()
    { 
        //Block player pick item
        if (m_autoLose || m_autoPlay) return;
        if (m_gameOver) return;
        if (IsBusy) return;

        // if (!m_hintIsShown)
        // {
        //     m_timeAfterFill += Time.deltaTime;
        //     if (m_timeAfterFill > m_gameSettings.TimeForHint)
        //     {
        //         m_timeAfterFill = 0f;
        //         ShowHint();
        //     }
        // }

        if (Input.GetMouseButtonDown(0) && !m_hasCliked)
        {
            m_hasCliked = true;
            
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                m_isDragging = true;
                m_hitCollider = hit.collider;
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null)
                {
                    if (cell.Item is NormalItem)
                    { 
                        m_playerBoard.PlaceInBoard(cell, cell.Item as NormalItem);
                    }
                }
                else if (hit.collider.gameObject.CompareTag(Constants.PLAYER_ITEM_TAG) && m_attack)
                {
                    m_playerBoard.Return(hit.collider.gameObject.transform.position);
                }
                
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_hasCliked = false;
            ResetRayCast();
        }
        //
        // if (Input.GetMouseButton(0) && m_isDragging)
        // {
        //     var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //     if (hit.collider != null)
        //     {
        //         if (m_hitCollider != null && m_hitCollider != hit.collider)
        //         {
        //             StopHints();
        //
        //             Cell c1 = m_hitCollider.GetComponent<Cell>();
        //             Cell c2 = hit.collider.GetComponent<Cell>();
        //             if (AreItemsNeighbor(c1, c2))
        //             {
        //                 IsBusy = true;
        //                 SetSortingLayer(c1, c2);
        //                 m_board.Swap(c1, c2, () =>
        //                 {
        //                     FindMatchesAndCollapse(c1, c2);
        //                 });
        //
        //                 ResetRayCast();
        //             }
        //         }
        //     }
        //     else
        //     {
        //         ResetRayCast();
        //     }
        // }
    }

    private void ResetRayCast()
    {
        m_isDragging = false;
        m_hitCollider = null;
    }

    private void FindMatchesAndCollapse(Cell cell1, Cell cell2)
    {
        if (cell1.Item is BonusItem)
        {
            cell1.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else if (cell2.Item is BonusItem)
        {
            cell2.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else
        {
            List<Cell> cells1 = GetMatches(cell1);
            List<Cell> cells2 = GetMatches(cell2);

            List<Cell> matches = new List<Cell>();
            matches.AddRange(cells1);
            matches.AddRange(cells2);
            matches = matches.Distinct().ToList();

            if (matches.Count < m_gameSettings.MatchesMin)
            {
                g_board.Swap(cell1, cell2, () =>
                {
                    IsBusy = false;
                });
            }
            else
            {
                // OnMoveEvent();

                CollapseMatches(matches, cell2);
            }
        }
    }

    private void FindMatchesAndCollapse()
    {
        List<Cell> matches = g_board.FindFirstMatch();

        if (matches.Count > 0)
        {
            CollapseMatches(matches, null);
        }
        else
        {
            m_potentialMatch = g_board.GetPotentialMatches();
            if (m_potentialMatch.Count > 0)
            {
                IsBusy = false;

                m_timeAfterFill = 0f;
            }
            else
            {
                //StartCoroutine(RefillBoardCoroutine());
                StartCoroutine(ShuffleBoardCoroutine());
            }
        }
    }

    private List<Cell> GetMatches(Cell cell)
    {
        List<Cell> listHor = g_board.GetHorizontalMatches(cell);
        if (listHor.Count < m_gameSettings.MatchesMin)
        {
            listHor.Clear();
        }

        List<Cell> listVert = g_board.GetVerticalMatches(cell);
        if (listVert.Count < m_gameSettings.MatchesMin)
        {
            listVert.Clear();
        }

        return listHor.Concat(listVert).Distinct().ToList();
    }

    private void CollapseMatches(List<Cell> matches, Cell cellEnd)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].ExplodeItem();
        }

        if(matches.Count > m_gameSettings.MatchesMin)
        {
            g_board.ConvertNormalToBonus(matches, cellEnd);
        }

        StartCoroutine(ShiftDownItemsCoroutine());
    }

    private IEnumerator ShiftDownItemsCoroutine()
    {
        g_board.ShiftDownItems();

        yield return new WaitForSeconds(0.2f);

        g_board.FillGapsWithNewItems();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator RefillBoardCoroutine()
    {
        g_board.ExplodeAllItems();

        yield return new WaitForSeconds(0.2f);

        g_board.Fill();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator ShuffleBoardCoroutine()
    {
        g_board.Shuffle();

        yield return new WaitForSeconds(0.3f);

        FindMatchesAndCollapse();
    }


    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher();
        if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    private bool AreItemsNeighbor(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }

    internal void Clear()
    {
        g_board.Clear();
    }

    // private void ShowHint()
    // {
    //     m_hintIsShown = true;
    //     foreach (var cell in m_potentialMatch)
    //     {
    //         cell.AnimateItemForHint();
    //     }
    // }

    // private void StopHints()
    // {
    //     m_hintIsShown = false;
    //     foreach (var cell in m_potentialMatch)
    //     {
    //         cell.StopHintAnimation();
    //     }
    //
    //     m_potentialMatch.Clear();
    // }
}
