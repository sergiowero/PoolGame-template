using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementGameover : MonoBehaviour 
{
    [SerializeField]
    protected NumericalEffect m_Accuracy;
    [SerializeField]
    protected NumericalEffect m_Combo;
    [SerializeField]
    protected NumericalEffect m_Score;
    [SerializeField]
    protected NumericalEffect m_HighScore;
    [SerializeField]
    protected NumericalEffect m_FriendHighScore;
    [SerializeField]
    private NumericalEffect m_ShotCount;
    [SerializeField]
    private NumericalEffect m_PottedCount;
    [SerializeField]
    private NumericalEffect m_PlayTime;
    [SerializeField]
    private NumericalEffect m_Round;

    //缺少文字说明， 先这个样子
    public void SetData(QuickFirePlayer.PlayerData playerData)
    {
        m_ShotCount.value = playerData.ShotCount;
        m_PottedCount.value = playerData.PottedCount;
        m_Accuracy.value = (int)(playerData.HitRate * 100);
        m_Combo.value = playerData.combo;
        m_PlayTime.value = (int)playerData.PlayTime;
        m_Score.value = playerData.Score;
        m_HighScore.value = playerData.HighScore;
        m_Round.value = playerData.MaxRank;
    }

    public void OnBack()
    {
        Application.LoadLevel(0);
    }

    public void OnTryAgain()
    {
        Application.LoadLevel(1);
    }
}
