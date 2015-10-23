using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementMissionFail : MonoBehaviour
{
    [SerializeField]
    private Text m_Score;
    [SerializeField]
    private Text m_FriendHighScore;


    public void SetData(MissionPlayer.PlayerData playerData)
    {
        m_Score.text = playerData.Score.ToString();
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
