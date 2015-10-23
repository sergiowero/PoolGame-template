using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementGameover : MonoBehaviour 
{
    [SerializeField]
    protected Text m_Accuracy;
    [SerializeField]
    protected Text m_Combo;
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
    private Text m_Round;

    //缺少文字说明， 先这个样子
    public void SetData(QuickFirePlayer.PlayerData playerData)
    {
        m_ShotCount.text = playerData.ShotCount.ToString();
        m_PottedCount.text = playerData.PottedCount.ToString();
        m_Accuracy.text = playerData.HitRate.ToString() + "%";
        m_Combo.text =  "x" + playerData.MaxLink.ToString();
        m_PlayTime.text = playerData.PlayTime.ToString() + "s";
        m_Score.text = playerData.Score.ToString();
        m_HighScore.text = playerData.HighScore.ToString();
        m_Round.text = playerData.MaxRank.ToString();
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
