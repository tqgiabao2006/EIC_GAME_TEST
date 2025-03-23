using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public event Action OnLoseConditionCompleteEvent = delegate { };
    public event Action OnWinCOnditionCompleteEvent = delegate { };

    protected Text m_txt;

    protected bool m_conditionCompleted = false;

    public virtual void Setup(float value, Text txt)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mng, PlayerBoard playerBoard)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt,PlayerBoard playerBoard)
    {
        m_txt = txt;
    }

    protected virtual void UpdateText() { }

    protected void OnLoseConditionComplete()
    {
        m_conditionCompleted = true;

        OnLoseConditionCompleteEvent();
    }

    protected void OnWinConditionComplete()
    {
        m_conditionCompleted = true;
        
        OnWinCOnditionCompleteEvent();
    }

    protected virtual void OnDestroy()
    {

    }
}
