using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementGameover : MonoBehaviour 
{
    [SerializeField]
    protected Text m_HitRate;
    [SerializeField]
    protected Text m_MaxLink;
    [SerializeField]
    protected Text m_Score;
    [SerializeField]
    protected Text m_HighScore;
    [SerializeField]
    protected Text m_FriendHighScore;
    [SerializeField]
    private Text m_ShotCount;
    [SerializeField]
    private Text m_PottedCount;
    [SerializeField]
    private Text m_PlayTime;
    [SerializeField]
    private Text m_AverageTime;
    [SerializeField]
    private Text m_MaxRank;

    //缺少文字说明， 先这个样子
    public void SetData(QuickFirePlayer.PlayerData playerData)
    {
        m_ShotCount.text = "击球数:" + playerData.ShotCount.ToString();
        m_PottedCount.text = "进球数:" + playerData.PottedCount.ToString() ;
        m_HitRate.text = "命中率:" + playerData.HitRate.ToString() + "%";
        m_MaxLink.text = "最大连击数:" + playerData.MaxLink.ToString();
        m_PlayTime.text = "游戏时间:" + playerData.PlayTime.ToString();
        m_AverageTime.text = "平均游戏时间:" + playerData.AverageTime.ToString();
        m_Score.text = "分数:" + playerData.Score.ToString();
        m_HighScore.text = "最高分数:" + playerData.HighScore.ToString();
        m_MaxRank.text = "最高Rank:" + playerData.MaxRank.ToString();
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
