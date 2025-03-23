using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{ 
    [SerializeField] private Button btnAttackTime;

    [SerializeField] private Button btnMoves;
    
    [SerializeField] private Button btnAutoplay;

    [SerializeField] private Button btnAutoLose;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnMoves.onClick.AddListener(OnClickMoves);
        btnAttackTime.onClick.AddListener(OnClickAttackTime);
        btnAutoplay.onClick.AddListener(SetAutoPlay);
        btnAutoLose.onClick.AddListener(SetAutoLose);
    }

    private void OnDestroy()
    {
        if (btnMoves) btnMoves.onClick.RemoveAllListeners();
        if (btnAttackTime) btnAttackTime.onClick.RemoveAllListeners();
    }

    private void SetAutoPlay()
    {
        m_mngr.LoadAutoPlay();
    }

    private void SetAutoLose()
    {
        m_mngr.LoadAutoLose();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickAttackTime()
    {
        m_mngr.LoadLevelAttackTime();
    }

    private void OnClickMoves()
    {
        m_mngr.LoadLevelMoves();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
