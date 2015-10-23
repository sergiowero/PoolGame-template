using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementQuickFireLose : SettlementQuickFireWin
{
    [SerializeField]
    protected Text m_HighScore;

    public override void SetScore(params float[] score)
    {
        base.SetScore(score);
        m_HighScore.text = score[1].ToString();
    }

}
