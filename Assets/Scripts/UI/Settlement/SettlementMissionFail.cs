using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementMissionFail : MonoBehaviour
{
    [SerializeField]
    private NumericalEffect m_Score;
    [SerializeField]
    private NumericalEffect m_FriendHighScore;


    public void SetData(MissionPlayer.PlayerData playerData)
    {
        m_Score.value = playerData.Score;
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
