using UnityEngine;
using System.Collections;

public class PoolRulesQuickFire : PoolRulesBase
{
    protected QuickFirePlayer m_Player;

    //protected  

    public override void SetPlayers(params IPlayer[] players)
    {
        m_Player = (QuickFirePlayer)players[0];
    }

    public override void Initialize()
    {
        m_Time = ConstantData.TimeLimitQuickFire;
        m_Player.PlayTime = ConstantData.TimeLimitQuickFire;
        m_Player.SetTime(m_Time, true);
        m_HandleWhiteball = false;
    }

    protected override void CustomUpdate()
    {
        if (m_Time > 0)
            m_Time -= Time.deltaTime;

        m_Player.SetTime(m_Time);

        if (m_Time <= 0 && !m_GameOver)
        {
            m_GameOver = true;
            if (onGameOver != null)
            {
                onGameOver(m_Player);
            }
        }
    }

    public override bool CheckGameOver()
    {
        return false;
    }

    public override void PotBall(PoolBall ball, PocketIndexes pocket)
    {
        base.PotBall(ball, pocket);
        if(ball.ballType != BallType.WHITE)
        {
            m_Player.AddBall(ball.GetBallID());
            m_Player.AddScore(ConstantData.QuickFireBallPottedScore);
            m_Time += 10;
            m_Player.PlayTime += 10;
        }
        else
        {
            m_Player.ComboBreak();
        }
    }

    public override void BallHitRail()
    {
    }

    protected override void CallPocket()
    {
    }

    protected override IEnumerator CheckResultAndChangeTurn(float time)
    {
        yield return new WaitForSeconds(time);
        TurnBegin();
    }

    protected override bool HandleFouls()
    {
        return false;
    }

    public override void OnBallFired()
    {
        base.OnBallFired();
    }

    protected override void TurnBegin()
    {
        if(m_WhiteBallPotted)
        {
            m_Time -= 30;
        }
        if (m_PottedBallListThisRound.Count == 0 && !m_WhiteBallPotted)
        {
            m_Player.ComboBreak();
        }
        base.TurnBegin();
        if(m_PottedBallList.Count == 14)
        {
            Pools.ResetAllBalls(true, true);
            m_PottedBallList.Clear();
            m_Player.Rank++;
        }
    }

    public override void WhiteBallHitBall(PoolBall ball)
    {
    }
}
