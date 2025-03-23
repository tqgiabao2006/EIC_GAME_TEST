using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMoves : LevelCondition
{
    private int m_moves;

    
    private PlayerBoard m_playerBoard;

    public override void Setup(float value, Text txt, PlayerBoard playerBoard)
    {
        base.Setup(value, txt);

        m_moves = (int)value;
        m_playerBoard = playerBoard;
        
        UpdateText();
    }

    private void Update()
    {
        Debug.Log("OnMove");
        if (m_playerBoard.g_curBoardCount == 0)
        {
            OnWinConditionComplete();
        }else if (m_playerBoard.g_isLoosing)
        {
            OnLoseConditionComplete();
        }
        // if (m_conditionCompleted) return;
        //
        // m_moves--;
        //
        // UpdateText();
        //
        // if(m_moves <= 0)
        // {
        //     OnConditionComplete();
        // }
    }

    protected override void UpdateText()
    {
        m_txt.text = string.Format("MOVES:\n{0}", m_moves);
    }
}
