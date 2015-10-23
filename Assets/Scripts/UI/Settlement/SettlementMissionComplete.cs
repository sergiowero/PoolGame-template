using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementMissionComplete : MonoBehaviour
{
    [SerializeField]
    private Text m_ShotsRemain;

    private int m_Star;

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
    [SerializeField]
    private Animator m_Animator;

    void Start()
    {
        m_Animator.SetInteger("Star", m_Star);
    }

    public void SetData(MissionPlayer.PlayerData playerData)
    {
        m_ShotsRemain.text = playerData.ShotsRemain.ToString();
        m_HitRate.text = (playerData.HitRate * 100).ToString() + "%";
        m_MaxLink.text = "x" + playerData.MaxLink.ToString();
        m_Score.text = playerData.Score.ToString();
        m_HighScore.text = playerData.HighScore.ToString();
        m_Star = playerData.Star;
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
