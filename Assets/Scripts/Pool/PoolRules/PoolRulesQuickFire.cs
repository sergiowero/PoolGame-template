using UnityEngine;
using System.Collections;

public class PoolRulesQuickFire : PoolRulesBase
{
    protected QuickFirePlayer m_Player;

    //protected  

    protected override void Start()
    {
        base.Start();
        m_Time = ConstantData.TimeLimitQuickFire;
        m_Player.PlayTime = ConstantData.TimeLimitQuickFire;
        m_Player.SetTime(m_Time, true);
        m_HandleWhiteball = false;
    }

    public override void SetPlayers(params IPlayer[] players)
    {
        m_Player = (QuickFirePlayer)players[0];
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
            m_Player.AddScore(ConstantData.QuickFireBallPottedPoint);
            m_Time += 10;
            m_Player.PlayTime += 10;
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
        if (m_WhiteBallPotted)
        {
            m_Time -= 30;
        }
        if (m_PottedBallListThisRound.Count == 0 && !m_WhiteBallPotted)
        {
            m_Player.ComboBreak();
        }
        for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
        {
            PoolBall pb = m_PottedBallListThisRound[i];
            m_PottedBallList.Add(pb.GetBallID(), pb);
        }
        m_PottedBallListThisRound.Clear();
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

    public override void CueBallHitRail()
    {
        throw new System.NotImplementedException();
    }
}
