using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementMissionGameover : MonoBehaviour
{
    [SerializeField]
    private Text m_ShotsRemain;

    [SerializeField]
    private Text m_Star;

    [SerializeField]
    private Text m_HitRate;
    [SerializeField]
    private Text m_MaxLink;
    [SerializeField]
    private Text m_Score;
    [SerializeField]
    private Text m_HighScore;
    [SerializeField]
    private Text m_FriendHighScore;


    public void SetData(MissionPlayer.PlayerData playerData)
    {
        m_ShotsRemain.text = "剩余杆数:" + playerData.ShotsRemain.ToString();
        m_Star.text = "星级:" + playerData.Star.ToString();
        m_HitRate.text = "命中率:" + (playerData.HitRate * 100).ToString() + "%";
        m_MaxLink.text = "最大连杆:" + playerData.MaxLink.ToString();
        m_Score.text = "分数:" + playerData.Score.ToString();
        m_HighScore.text = "最高分:" + playerData.HighScore.ToString();
    }

    public void OnNextLevel()
    {
        LevelData levelData = ConstantData.LevelDatas.Next(LevelDataIndex.CurrentLevel);
        if(levelData != null)
        {
            LevelDataIndex.CurrentLevel = levelData;
            Application.LoadLevel(1);
        }
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
