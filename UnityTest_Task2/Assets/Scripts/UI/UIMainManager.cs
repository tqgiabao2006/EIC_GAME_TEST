using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

public class UIMainManager : MonoBehaviour
{
    private IMenu[] m_menuList;

    private GameManager m_gameManager;

    private void Awake()
    {
        m_menuList = GetComponentsInChildren<IMenu>(true);
    }

    void Start()
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            m_menuList[i].Setup(this);
        }
    }

    internal void ShowMainMenu()
    {
        m_gameManager.ClearLevel();
        m_gameManager.SetState(GameManager.eStateGame.MAIN_MENU);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_gameManager.State == GameManager.eStateGame.GAME_STARTED)
            {
                m_gameManager.SetState(GameManager.eStateGame.PAUSE);
            }
            else if (m_gameManager.State == GameManager.eStateGame.PAUSE)
            {
                m_gameManager.SetState(GameManager.eStateGame.GAME_STARTED);
            }
        }
    }

    internal void Setup(GameManager gameManager)
    {
        m_gameManager = gameManager;
        m_gameManager.StateChangedAction += OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.SETUP:
                break;
            case GameManager.eStateGame.MAIN_MENU:
                ShowMenu<UIPanelMain>();
                break;
            case GameManager.eStateGame.GAME_STARTED:
            case GameManager.eStateGame.AUTO_LOSE:
            case GameManager.eStateGame.AUTO_PLAY:
            case GameManager.eStateGame.ATTACK:
                ShowMenu<UIPanelGame>();
                break;
            case GameManager.eStateGame.PAUSE:
                ShowMenu<UIPanelPause>();
                break;
            case GameManager.eStateGame.GAME_LOSE:
                UIPanelGameOver gameOver = FindMenu(typeof(UIPanelGameOver)) as UIPanelGameOver;
                gameOver.GetComponentInChildren<Text>().text = "YOU LOSE";
                ShowMenu<UIPanelGameOver>();
                break;
            case GameManager.eStateGame.GAME_WIN:
                UIPanelGameOver gameWin = FindMenu(typeof(UIPanelGameOver)) as UIPanelGameOver;
                gameWin.GetComponentInChildren<Text>().text = "YOU WIN";
                ShowMenu<UIPanelGameOver>();
                break;
        }
    }

    private void ShowMenu<T>() where T : IMenu
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            IMenu menu = m_menuList[i];
            if(menu is T)
            {
                menu.Show();
            }
            else
            {
                menu.Hide();
            }            
        }
    }

    private IMenu FindMenu(Type type)
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            if (m_menuList[i].GetType() == type)
            {
                return m_menuList[i];
            }
        }
        return null;
    }

    internal Text GetLevelConditionView()
    {
        UIPanelGame game = m_menuList.Where(x => x is UIPanelGame).Cast<UIPanelGame>().FirstOrDefault();
        if (game)
        {
            return game.LevelConditionView;
        }

        return null;
    }

    internal void ShowPauseMenu()
    {
        m_gameManager.SetState(GameManager.eStateGame.PAUSE);
    }

    internal void LoadLevelMoves()
    {
        m_gameManager.LoadLevel(GameManager.eLevelMode.MOVES, GameManager.eStateGame.GAME_STARTED);
    }

    internal void LoadLevelAttackTime()
    {
        m_gameManager.LoadLevel(GameManager.eLevelMode.TIMER, GameManager.eStateGame.ATTACK);
    }

    internal void ShowGameMenu()
    {
        m_gameManager.SetState(GameManager.eStateGame.GAME_STARTED);
    }

    internal void LoadAutoPlay()
    {
        m_gameManager.LoadLevel(GameManager.eLevelMode.MOVES, GameManager.eStateGame.AUTO_PLAY);
    }

    internal void LoadAutoLose()
    {
        m_gameManager.LoadLevel(GameManager.eLevelMode.MOVES, GameManager.eStateGame.AUTO_LOSE);
    }
}
