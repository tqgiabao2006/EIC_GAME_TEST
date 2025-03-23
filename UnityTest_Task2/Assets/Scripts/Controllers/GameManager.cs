﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        TIMER,
        MOVES
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        AUTO_PLAY,
        AUTO_LOSE,
        GAME_LOSE,
        GAME_WIN,
        ATTACK
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }


    private GameSettings m_gameSettings;


    private BoardController m_boardController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;
    
    private PlayerBoard m_playerBoard;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);
        
        m_playerBoard = FindObjectOfType<PlayerBoard>(true);
        
        m_uiMenu = FindObjectOfType<UIMainManager>();
        
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if(State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode, eStateGame state)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings, m_playerBoard);
        m_playerBoard.StartGame(this, m_boardController);

        if (mode == eLevelMode.MOVES)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelMoves>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_playerBoard);
        }
        else if (mode == eLevelMode.TIMER)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(60, m_uiMenu.GetLevelConditionView(), this, m_playerBoard);
        }

        m_levelCondition.OnLoseConditionCompleteEvent += GameLose;
        m_levelCondition.OnWinCOnditionCompleteEvent += GameWin;

        State = state;
    }

    public void GameLose()
    {
        StartCoroutine(WaitBoardController(false));
    }

    public void GameWin()
    {
        StartCoroutine(WaitBoardController(true));
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }

    private IEnumerator WaitBoardController(bool isWin)
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);
        
        ClearLevel();

        if (isWin)
        {
            State = eStateGame.GAME_WIN;
        }
        else
        {
            State = eStateGame.GAME_LOSE;
        }

        if (m_levelCondition != null)
        {
            m_levelCondition.OnLoseConditionCompleteEvent -= GameLose;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
