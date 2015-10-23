using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementUIManager : MonoBehaviour 
{
    [SerializeField]
    private SettlementGameover m_GameOverUI;

    [SerializeField]
    private SettlementQuickFireWin m_QuickFireWinUI;

    [SerializeField]
    private SettlementQuickFireLose m_QuickFireLoseUI;

    [SerializeField]
    private SettlementMissionComplete m_MissionCompleteUI;

    [SerializeField]
    private SettlementMissionFail m_MissionFailUI;

    [SerializeField]
    private SettlementStandardGameover m_StandardGameoverUI;

    [SerializeField]
    private Animator m_Animator;

    public void QuickWin(QuickFirePlayer.PlayerData playerData)
    {
        m_Animator.SetTrigger("QuickWin");
    }

    public void QuickLose(QuickFirePlayer.PlayerData playerData)
    {
        m_Animator.SetTrigger("QuickLose");
    }

    public void GameOver(QuickFirePlayer.PlayerData playerData)
    {
        m_Animator.SetTrigger("GameOver");
        m_GameOverUI.SetData(playerData);
    }

    public void MissionComplete(MissionPlayer.PlayerData playerData)
    {
        m_Animator.SetTrigger("MissionComplete");
        m_MissionCompleteUI.SetData(playerData);
    }

    public void MissionFail(MissionPlayer.PlayerData playerData)
    {
        m_Animator.SetTrigger("MissionFail");
        m_MissionFailUI.SetData(playerData);
    }

    public void StandardGameOver(BasePlayer player)
    {
        m_Animator.SetTrigger("StandardGameOver");
        m_StandardGameoverUI.SetWinner(player.playerName.text);
    }
}
