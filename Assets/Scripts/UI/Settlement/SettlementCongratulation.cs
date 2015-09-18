using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementCongratulation : MonoBehaviour 
{
    [SerializeField]
    protected Text m_ScoreText;

    [SerializeField]
    protected Text m_FriendScoreText;

    [SerializeField]
    protected Image m_FriendAvatar;

    public virtual void SetScore(params float[] score)
    {
        m_ScoreText.text = score[0].ToString();
    }

    public void OnUseTime()
    {

    }

    public void OnWatchAd()
    {

    }
    
    public void OnBack()
    {
        BaseUIController.MSettlement.SwitchUI();
    }

}
