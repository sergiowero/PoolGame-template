using UnityEngine;
using System.Collections;

public class PoolRulesStandard : PoolRulesBase
{
    protected float m_TimePerRound;

    protected bool m_Countdown = true;

    protected bool m_CueBallHitRail = false;

    protected bool m_UseGuidelines;

    protected int m_HittingRailBallsCount;

    protected CallPockets m_UseCallPocket = CallPockets.None;

    protected BallType m_WhiteHitBallType = BallType.NONE;

    protected int m_CurPlayerIndex;

    protected int m_OpponentPlayerIndex
    {
        get { return MathTools.Roll(0, m_Players.Length, m_CurPlayerIndex + 1); }
    }

    protected BasePlayer[] m_Players;

    public BasePlayer CurrentPlayer
    {
        get { return m_Players[m_CurPlayerIndex]; }
    }

    public BasePlayer OpponentPlayer
    {
        get { return m_Players[m_OpponentPlayerIndex]; }
    }

    protected override void Start()
    {
        base.Start();
        m_TimePerRound = ConstantData.TimePerRoundLow;
        m_Time = m_TimePerRound;
    } 


    private bool AnyBallWithTypeEnterPocket(BallType type)
    {
        if(type == BallType.NONE)
        {
            if (m_PottedBallListThisRound.Count > 0)
                return true;
            else
                return false;
        }
        else
        {
            for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
            {
                if (m_PottedBallListThisRound[i].ballType == type)
                    return true;
            }
            return false;
        }
    }

    protected override void CustomUpdate()
    {
        if (m_Countdown)
        {
            m_Time -= Time.deltaTime;
            CurrentPlayer.Countdown(m_Time / m_TimePerRound);
            if (m_Time <= 0)
            {
                m_Countdown = false;
                m_TimeOut = true;
                //check the result immediately
                StartCoroutine(CheckResultAndChangeTurn(0));
            }
        }
    }

    public override void SetPlayers(params IPlayer[] players)
    {
        int length = players.Length;
        m_Players = new BasePlayer[length];
        for(int i = 0; i < length; i++)
        {
            m_Players[i] = (BasePlayer)players[i];
        }
    }

    protected override void TurnBegin()
    {
        base.TurnBegin();
        m_Time = m_TimePerRound;
        m_Countdown = true;
        m_CueBallHitRail = false;
        m_WhiteHitBallType = BallType.NONE;
        m_HittingRailBallsCount = 0;
        if(m_HandleWhiteball)
        {
            State = GlobalState.DRAG_WHITEBALL;
        }
        else
        {
            State = GlobalState.IDLE;
        }
        for (int i = 0, length = m_Players.Length; i < length; i++)
        {
            m_Players[i].UpdateBallIcon();
        }
        CurrentPlayer.Begin();
    }

    protected override IEnumerator CheckResultAndChangeTurn(float time)
    {
        yield return new WaitForSeconds(time);


        if (CheckGameOver()) // the game is over
        {
            GameManager.Rules.State = GlobalState.GAMEOVER;
            if (onGameOver != null)
            {
                onGameOver(CurrentPlayer);
            }
            BaseUIController.MSettlement.StandardGameOver(CurrentPlayer);
        }
        else //not yet
        {
            m_HandleWhiteball = HandleFouls();

            if (m_HandleWhiteball || !AnyBallWithTypeEnterPocket(CurrentPlayer.TargetBallType))
            {
                CurrentPlayer.combo = 0;
                //change player(if there is more than 2 players in the game)
                CurrentPlayer.End();
                m_CurPlayerIndex = MathTools.Roll(0, m_Players.Length, m_CurPlayerIndex + 1);
            }

            for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
            {
                PoolBall pb = m_PottedBallListThisRound[i];
                m_PottedBallList.Add(pb.GetBallID(), pb);
            }
            m_PottedBallListThisRound.Clear();

            TurnBegin();
        }
    }

    public override void BallHitRail()
    {
        if(m_WhiteHitBall)
            m_HittingRailBallsCount++;
    }

    public override bool CheckGameOver()
    {
        //black 8 not enter the pocket yet
        if (Pools.Balls[8].BallState != PoolBall.State.POTTED )
            return false;

        CurrentPlayer.UpdateBallIcon();
        //black 8 enter the pocket 

        //current player hits the black 8 in the pocket at the first round, current player win
        if (firstRound)
            return true;

        //current player has the target balls or current player hit the cueball in the pocket
        if (CurrentPlayer.BallsList.Count > 0 || Pools.CueBall.BallState == PoolBall.State.POTTED)
        {
            //opponent win
            m_CurPlayerIndex = m_OpponentPlayerIndex;
            return true;
        }

        //current player win
        return true;
    }

    protected override bool HandleFouls()
    {
        //if cue ball enter the pocket
        if (m_WhiteBallPotted) return BaseUIController.text.Show(string.Format(HOLocalizationConfiguration.GetValue(409), CurrentPlayer.name));
        //time out
        if (m_TimeOut) return BaseUIController.text.Show(HOLocalizationConfiguration.GetValue(412));

        if (firstRound)
        {
            //if there is no at least 4 balls hit the wall
            if (m_HittingRailBallsCount < 4 && !m_CueBallHitRail &&m_PottedBallListThisRound.Count == 0) return BaseUIController.text.Show(HOLocalizationConfiguration.GetValue(413));
        }
        else
        {
            //if (m_HittingRailBallsCount == 0 && m_PottedBallListThisRound.Count == 0 && m_WhiteHitBallType == BallType.NONE) return BaseUIController.text.Show("白球没到打到球");
            //if cue ball desn't hit any balls after shotting
            if (!m_WhiteHitBall) return BaseUIController.text.Show(HOLocalizationConfiguration.GetValue(403));
            //if white ball doesn't hit the target type ball
            if ((m_WhiteHitBallType != CurrentPlayer.TargetBallType && CurrentPlayer.TargetBallType != BallType.NONE)
                || (CurrentPlayer.TargetBallType == BallType.NONE && m_WhiteHitBallType == BallType.BLACK)) return BaseUIController.text.Show(string.Format(HOLocalizationConfiguration.GetValue(408), CurrentPlayer.name));
            //it must be at least once that ball hitted the rail after first hitted
            if (m_WhiteHitBallType != BallType.NONE && m_HittingRailBallsCount == 0 && m_PottedBallListThisRound.Count == 0) return BaseUIController.text.Show(HOLocalizationConfiguration.GetValue(407));
        }
        return false;
    }

    protected override void CallPocket()
    {

    }

    public override void OnBallFired()
    {
        base.OnBallFired();
        m_Countdown = false;
    }

    public override void WhiteBallHitBall(PoolBall ball)
    {
        if (!m_WhiteHitBall)
        {
            m_WhiteHitBallType = ball.ballType;
        }
        base.WhiteBallHitBall(ball);
    }

    public override void PotBall(PoolBall ball, PocketIndexes pocket)
    {
        base.PotBall(ball, pocket);
        if (CurrentPlayer.TargetBallType == BallType.NONE && ball.ballType != BallType.WHITE && ball.ballType != BallType.BLACK && !firstRound)
        {
            CurrentPlayer.TargetBallType = ball.ballType;
            OpponentPlayer.TargetBallType = ball.ballType == BallType.SOLID ? BallType.STRIPE : BallType.SOLID;
        }
        BallType b = ball.ballType;
        if (b != BallType.WHITE)
        {
            CurrentPlayer.combo++;
        }
    }

    public override void CueBallHitRail()
    {
        if(m_WhiteHitBall)
            m_CueBallHitRail = true;
    }
}
