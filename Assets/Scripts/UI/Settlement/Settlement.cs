﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Settlement : MonoBehaviour 
{
    [SerializeField]
    private SettlementGameover m_GameOverUI;

    [SerializeField]
    private SettlementCongratulation m_CongratulationUI;

    [SerializeField]
    private SettlementRegret m_RegretUI;

    void Awake()
    {
        m_CongratulationUI.gameObject.SetActive(false);
        m_GameOverUI.gameObject.SetActive(false);
        m_RegretUI.gameObject.SetActive(false);
    }

    public void SwitchUI()
    {
        m_CongratulationUI.gameObject.SetActive(false);
        m_RegretUI.gameObject.SetActive(false);
        m_GameOverUI.gameObject.SetActive(true);
    }

    public void SetPlayerData(QuickFirePlayer.PlayerData playerData)
    {
        m_GameOverUI.SetData(playerData);
    }

    public void ShowRegretUI(float score, float highScore, float friendScore = 0)
    {
        m_RegretUI.gameObject.SetActive(true);
        m_RegretUI.SetScore(score, highScore);
        m_CongratulationUI.gameObject.SetActive(false);
        m_GameOverUI.gameObject.SetActive(false);
    }

    public void ShowCongratulationUI(float score, float friendScore = 0)
    {
        m_CongratulationUI.gameObject.SetActive(true);
        m_CongratulationUI.SetScore(score);
        m_RegretUI.gameObject.SetActive(false);
        m_GameOverUI.gameObject.SetActive(false);
    }
}