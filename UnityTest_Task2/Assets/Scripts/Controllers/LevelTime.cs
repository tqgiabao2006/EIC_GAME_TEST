using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTime : LevelCondition
{
    private float m_time;

    private GameManager m_mngr;
    
    private PlayerBoard m_playerBoard;

    public override void Setup(float value, Text txt, GameManager mngr, PlayerBoard playerBoard)
    {
        base.Setup(value, txt, mngr, playerBoard);

        m_mngr = mngr;

        m_time = value;
        
        m_playerBoard = playerBoard;

        UpdateText();
    }

    private void Update()
    {
        if (m_conditionCompleted) return;

        if (m_mngr.State != GameManager.eStateGame.ATTACK) return;

        m_time -= Time.deltaTime;

        UpdateText();

        if (m_time <= 0f)
        {
            OnLoseConditionComplete();
            
        }else if (m_playerBoard.g_curBoardCount <= 0)
        {
            OnWinConditionComplete();
        }
    }

    protected override void UpdateText()
    {
        if (m_time < 0f) return;
        Debug.Log("UpdateText");
        m_txt.text = string.Format("TIME:\n{0:00}", m_time);
    }
}
