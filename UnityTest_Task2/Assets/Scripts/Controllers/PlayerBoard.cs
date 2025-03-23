using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoard : MonoBehaviour
{
    private Vector2[] m_cellPosition;
    
    private (NormalItem, Cell)[] m_playerCells;
    
    private int m_cellCount;

    private int m_curBoardCount;
    public int g_curBoardCount
    {
        get
        {
            return m_curBoardCount;
        }
        set
        {
            if (value >= 0)
            {
                m_curBoardCount = value;
            }
        }
    }

    public bool g_isLoosing;
    
    private GameManager m_gameManager;

    private bool m_autoPlay;

    private bool m_autoLose;

    private NormalItem.eNormalType m_currentType;

    private Board m_board;

    private float m_timeCounter;

    private float m_timeDelay;

    private HashSet<NormalItem.eNormalType> m_selected;

    public void StartGame (GameManager gameManager, BoardController boardController)
    {
        m_timeCounter = 0.5f;
        
        m_timeDelay = 0.5f;

        m_currentType = NormalItem.eNormalType.TYPE_ONE; 
        
        m_selected = new HashSet<NormalItem.eNormalType>();
            
        m_board = boardController.g_board;
        
        m_gameManager = gameManager;

        m_gameManager.StateChangedAction += OnGameStateChange;
        
        m_cellCount = 5;
        
        m_cellPosition = new Vector2[m_cellCount];
        
        m_playerCells = new (NormalItem, Cell)[m_cellCount];
        
        RectTransform[] rt = this.gameObject.GetComponentsInChildren<RectTransform>();
        for (int i = 1; i < rt.Length; i++)
        {
            Vector3[] corners = new Vector3[4];
            rt[i].GetWorldCorners(corners);

            m_cellPosition[i-1] = (corners[0] + corners[2]) / 2f;
        }
    }

    private void Update()
    {
        if (m_autoPlay)
        {
            if (m_board.Count() == 0)
            {
                return;
            }
            
            m_timeCounter -= Time.deltaTime;

            if (m_timeCounter <= 0)
            {
                (NormalItem,Cell) item;
               if((item = m_board.SearchItemByType(m_currentType)) != (null,null))
               {
                   PlaceInBoard(item.Item2, item.Item1);
               }
               else
               {
                   int nextType = (int)m_currentType +1;
                   if (Enum.IsDefined(typeof(NormalItem.eNormalType), nextType))
                   {
                       m_currentType = (NormalItem.eNormalType)nextType;
                   }
                   else
                   {
                       return;
                   }
               }
                m_timeCounter = m_timeDelay;
            }
            
        }else if (m_autoLose)
        {
            if (m_board.Count() == 0)
            {
                return;
            }
            
            m_timeCounter -= Time.deltaTime;

            if (m_timeCounter <= 0)
            {
                if (!m_selected.Contains(m_currentType))
                {
                    (NormalItem,Cell) item;
                    if((item = m_board.SearchItemByType(m_currentType)) != (null,null))
                    {
                        PlaceInBoard(item.Item2, item.Item1);
                    }
                    m_selected.Add(m_currentType);
                    
                    int nextType= (int)m_currentType +1;
                    if (Enum.IsDefined(typeof(NormalItem.eNormalType), nextType))
                    {
                        m_currentType = (NormalItem.eNormalType)nextType;
                    }
                    else
                    {
                        return;
                    }
                    
                }
                m_timeCounter = m_timeDelay;
            }

        }
    }

    public void Return(Vector2 cellPos)
    {
        float minDst = float.MaxValue;
        int minIndex = 0;
        for (int i = 0; i < m_cellPosition.Length; i++)
        {
            float dst = Vector2.Distance(cellPos, m_cellPosition[i]);
            if (dst <= minDst)
            {
                minDst = Vector2.Distance(cellPos, m_cellPosition[i]);
                minIndex = i;
            }
        }

        if (m_playerCells[minIndex] == (null, null)
            || m_playerCells[minIndex].Item2 == null
            || m_playerCells[minIndex].Item1 == null)
        {
            return;
        }
        
        //Move the object back to board
        m_playerCells[minIndex].Item1.AnimationMoveToPosition(m_playerCells[minIndex].Item2.transform.position);
        m_playerCells[minIndex].Item2.Assign(m_playerCells[minIndex].Item1);
        
        for (int j = minIndex + 1; j < m_playerCells.Length; j++)
        {
            int targetIndex = Mathf.Max(j - 1,0);
            m_playerCells[targetIndex] = m_playerCells[j];
                        
            if (m_playerCells[j].Item1 != null)
            {
                m_playerCells[j].Item1.AnimationMoveToPosition(m_cellPosition[targetIndex]);
            }
                        
            m_playerCells[j] = (null, null);
        }
        
        m_playerCells[minIndex] = (null, null);
    }
    
    
    /// <summary>
    /// Place in board, called by board controller
    /// If found, auto arrange,
    /// If not, find the leftest empty slot
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="item"></param>
    public void PlaceInBoard(Cell cell, NormalItem item)
    {
        bool isSimilar = false;

        for (int i = 0; i < m_playerCells.Length; i++)
        {
            if (m_playerCells[i] != (null, null))
            {
                if (m_playerCells[i].Item1.ItemType == item.ItemType)
                {
                    isSimilar = true;
                }else if(m_playerCells[i].Item1.ItemType != item.ItemType && isSimilar)
                {
                    //Auto arrange item
                    for (int j = m_playerCells.Length - 1; j > i; j--)
                    {
                        m_playerCells[j] = m_playerCells[j - 1];
                        if (m_playerCells[j-1].Item1 != null)
                        {
                            m_playerCells[j-1].Item1.AnimationMoveToPosition(m_cellPosition[j ]);
                        }
                    }
                
                    cell.Free();
                    m_playerCells[i] = (item, cell);
                    item.AnimationMoveToPosition(m_cellPosition[i]);
                    StartCoroutine(ClearTriple());

                    m_curBoardCount--;
                    
                    return;
                }
            }
            
        }

        for (int i = 0; i < m_playerCells.Length; i++)
        {
            if (m_playerCells[i] == (null, null))
            {
                cell.Free();
                m_playerCells[i] = (item, cell);
                item.AnimationMoveToPosition(m_cellPosition[i]);
                StartCoroutine(ClearTriple());
                
                m_curBoardCount--;
                return;
            }
        }
    }
    
    /// <summary>
    /// Clear 3 identical items, shrink the board after that
    /// If player board isFull, it about to lose, but the added one clear something, it does not lose
    /// </summary>
    private IEnumerator ClearTriple()
    {
        yield return new WaitForSeconds(0.2f); // Wait for moving animation

        bool aboutToLose = false;

        if (this.Count() == m_cellCount)
        {
            aboutToLose = true;
        }
        
        for (int i = 0; i < m_playerCells.Length - 2; i++)
        {
            if (m_playerCells[i] != (null, null) && m_playerCells[i + 1] != (null, null) &&
                m_playerCells[i + 2] != (null, null))
            {
                if (m_playerCells[i].Item1.ItemType == m_playerCells[i + 1].Item1.ItemType
                    && m_playerCells[i].Item1.ItemType == m_playerCells[i + 2].Item1.ItemType)
                {

                    aboutToLose = false;
                    
                    m_playerCells[i].Item1.ShowDisappearAnimation();
                    m_playerCells[i+1].Item1.ShowDisappearAnimation();
                    m_playerCells[i+2].Item1.ShowDisappearAnimation();
                       
                    m_playerCells[i] = (null, null);
                    m_playerCells[i + 1] = (null, null);
                    m_playerCells[i + 2] = (null, null);
                    
                    //Move after item forward
                    for (int j = i + 3; j < m_playerCells.Length; j++)
                    {
                        int targetIndex = Mathf.Max(j - 3,0);
                        m_playerCells[targetIndex] = m_playerCells[j];
                        
                        if (m_playerCells[j].Item1 != null)
                        {
                            m_playerCells[j].Item1.AnimationMoveToPosition(m_cellPosition[targetIndex]);
                        }
                        
                        m_playerCells[j] = (null, null);
                    }
                    
                }
            }
            
        }

        if (aboutToLose)
        {
            g_isLoosing = true;
        }
    }
    
    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_LOSE:
                case GameManager.eStateGame.GAME_WIN:
                m_timeCounter = 0.5f;
                m_currentType = NormalItem.eNormalType.TYPE_ONE;
                g_isLoosing = false;
                m_autoLose = false;
                m_autoPlay = false;
                m_curBoardCount = 0;
                m_playerCells = new (NormalItem, Cell)[m_cellCount];
                m_cellPosition = new Vector2[m_cellCount];
                m_selected = new HashSet<NormalItem.eNormalType>();
                break;
            case GameManager.eStateGame.AUTO_PLAY:
                m_autoPlay = true;
                break;
            case GameManager.eStateGame.AUTO_LOSE:
                m_autoLose = true;
                break;
        }
    }

    private int Count()
    {
        int cnt = 0;
        for (int i = 0; i < m_playerCells.Length; i++)
        {

            if (m_playerCells[i] != (null, null))
            {
                cnt++;
            }
        }

        return cnt;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < m_playerCells.Length; i++)
        {
            if (m_playerCells[i] != (null, null))
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawCube(m_cellPosition[i], Vector3.one * 0.2f);

        }
    }
}
