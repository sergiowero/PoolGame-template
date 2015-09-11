using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementGameover : MonoBehaviour 
{
    [SerializeField]
    private Text m_ShotCount;
    [SerializeField]
    private Text m_PottedCount;
    [SerializeField]
    private Text m_HitRate;
    [SerializeField]
    private Text m_MaxLink;
    [SerializeField]
    private Text m_PlayTime;
    [SerializeField]
    private Text m_AverageTime;
    [SerializeField]
    private Text m_Score;
    [SerializeField]
    private Text m_HighScore;
    [SerializeField]
    private Text m_MaxRank;
    [SerializeField]
    private Text m_FriendHighScore;

    //缺少文字说明， 先这个样子
    public void SetData(QuickFirePlayer.PlayerData playerData)
    {
        m_ShotCount.text = playerData.ShotCount.ToString();
        m_PottedCount.text = playerData.PottedCount.ToString() ;
        m_HitRate.text = playerData.HitRate.ToString();
        m_MaxLink.text = playerData.MaxLink.ToString();
        m_PlayTime.text = playerData.PlayTime.ToString();
        m_AverageTime.text = playerData.AverageTime.ToString();
        m_Score.text = playerData.Score.ToString();
        m_HighScore.text = playerData.HighScore.ToString();
        m_MaxRank.text = playerData.MaxRank.ToString();
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
