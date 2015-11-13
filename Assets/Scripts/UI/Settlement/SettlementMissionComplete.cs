using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementMissionComplete : MonoBehaviour
{
    [SerializeField]
    private NumericalEffect m_ShotsRemain;

    private int m_Star;

    [SerializeField]
    private NumericalEffect m_HitRate;
    [SerializeField]
    private NumericalEffect m_MaxLink;
    [SerializeField]
    //private Text m_Score;
    private NumericalEffect m_Score;
    [SerializeField]
    private NumericalEffect m_HighScore;
    [SerializeField]
    private NumericalEffect m_FriendHighScore;
    [SerializeField]
    private Animator m_Animator;

    void Start()
    {
        m_Animator.SetInteger("Star", m_Star);
    }

    public void SetData(MissionPlayer.PlayerData playerData)
    {
        m_ShotsRemain.value = playerData.ShotsRemain;
        m_HitRate.value = (int)(playerData.HitRate * 100);
        m_MaxLink.value = playerData.MaxCombo;
        m_Score.value = playerData.Score;
        m_HighScore.value = playerData.HighScore;
        m_Star = playerData.Star;
    }

    public void OnNextLevel()
    {
        LevelData levelData = ConstantData.LevelDatas.Next(LevelDataIndex.CurrentLevel);
        if(levelData != null)
        {
            LevelDataIndex.CurrentLevel = levelData;
            //Application.LoadLevel(1);
            BaseUIController.Instance.RestartScene();
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
