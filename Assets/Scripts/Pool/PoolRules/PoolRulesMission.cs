using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolRulesMission : PoolRulesBase
{

    protected MissionPlayer m_Player;

    protected IList<int> m_TargetBalls = new List<int>();

    protected bool m_WhiteballHitBallThisRound = false;

    protected int m_PunishmentCountThisRound = 0;// the count that ball is potted at punitive pocket per round

    protected int m_RewardCountThisRound = 0;//the count that ball is potted at reward pocket per round

    void Awake()
    {
        SingularityBall.SingularityBreakBall += OnSingularityBreakBall;
    }

    void OnDestroy()
    {
        SingularityBall.SingularityBreakBall -= OnSingularityBreakBall;
    }

    void OnGUI()
    {
        GUILayout.Label(m_Player.Data.ToString());
    }

    protected override void Start()
    {
        List<LevelData.DisplayDatas> ld = LevelDataIndex.CurrentLevel.BallsDrawList;
        List<LevelData.PositionDatas> lp = LevelDataIndex.CurrentLevel.BallsPosition;
        List<LevelData.OtherObjectDatas> lo = LevelDataIndex.CurrentLevel.OtherObjectsPosition;
        for (int i = 0, count = ld.Count; i < count; i++ )
        {
            if (ld[i].ID == 0) continue;
            Pools.Balls[ld[i].ID].Reset();
            if(ld[i].Draw)
                m_TargetBalls.Add(ld[i].ID);
            else
            {
                Pools.Balls[ld[i].ID].Hide();
            }
        }
        for (int i = 0, count = lp.Count; i < count; i++ )
        {
            Pools.Balls[lp[i].ID].transform.position = lp[i].Positon;
        }
        Transform ooRoot = GameObject.Find("8Ball/OtherObjects").transform;
        for (int i = 0, count = lo.Count; i < count; i++ )
        {
            LevelData.OtherObjectDatas d = lo[i];
            GameObject o = Resources.Load(d.Type.ToString()) as GameObject;
            GameObject oo = Instantiate(o) as GameObject;
            oo.transform.SetParent(ooRoot);
            oo.transform.position = d.Position;
            PoolBall pb = oo.GetComponent<PoolBall>();
            pb.SetBallID(d.ID);
            pb.ballType = d.Type;
            m_TargetBalls.Add(d.ID);
        }
        PocketTrigger.MarkPocketType(LevelDataIndex.CurrentLevel.StartPunishmentPocket, LevelDataIndex.CurrentLevel.StartRewardPocket);
        PocketTrigger.Block(LevelDataIndex.CurrentLevel.BlockPockets);
        TurnBegin();
    }

    protected override IEnumerator CheckResultAndChangeTurn(float time)
    {
        yield return new WaitForSeconds(time);

        if (m_WhiteBallPotted || !m_WhiteballHitBallThisRound)
        {
            m_Player.ShotsRemain -= ConstantData.MissionCueballPottedPunishment;
        }
        else if (m_PottedBallListThisRound.Count == 0)
        {
            m_Player.ShotsRemain -= ConstantData.MissionNoBallHittedPunishment;
        }

        m_Player.ShotsRemain += ConstantData.RewardShots * m_RewardCountThisRound - ConstantData.PunitiveShots * m_PunishmentCountThisRound;

        for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
        {
            PoolBall pb = m_PottedBallListThisRound[i];
            m_PottedBallList.Add(pb.GetBallID(), pb);
        }
        m_PottedBallListThisRound.Clear();

        if (CheckGameOver())
        {
            m_GameOver = true;
            if (onGameOver != null)
            {
                if (m_TargetBalls.Count == 0)
                    onGameOver(m_Player);
                else if (m_Player.ShotsRemain == 0)
                    onGameOver(null);
            }
        }
        else
            TurnBegin();
    }

    protected override void TurnBegin()
    {
        base.TurnBegin();
        m_WhiteballHitBallThisRound = false;
        m_PunishmentCountThisRound = 0;
        m_RewardCountThisRound = 0;

        if(m_Turn > 1)
        {
            //generate punitive pocket and reward pocket
            //punishment
            PocketIndexes punishment = PocketIndexes.None;
            if (Random.Range(0, 100) > ConstantData.SpecialPocketProbability)
            {
                punishment |= (PocketIndexes)(1 << Random.Range(0, 5));
                if (Random.Range(0, 1) == 1)
                {
                    punishment |= (PocketIndexes)(1 << Random.Range(0, 5));
                }
            }
            //reward
            PocketIndexes reward = PocketIndexes.None;
            if (Random.Range(0, 100) > ConstantData.SpecialPocketProbability)
            {
                reward |= (PocketIndexes)(1 << Random.Range(0, 5));
                if (Random.Range(0, 1) == 1)
                {
                    reward |= (PocketIndexes)(1 << Random.Range(0, 5));
                }
            }

            PocketTrigger.MarkPocketType(punishment, reward);
        }
    }

    public override void SetPlayers(params IPlayer[] players)
    {
        m_Player = players[0] as MissionPlayer;
    }

    public override void PotBall(PoolBall ball, PocketIndexes pocket)
    {
        base.PotBall(ball, pocket);

        if ((pocket & PocketTrigger.PunitivePocket) != 0)
            m_PunishmentCountThisRound++;
        if ((pocket & PocketTrigger.RewardPocket) != 0)
            m_RewardCountThisRound++;

        if (ball.ballType == BallType.WHITE)
        {
            m_Player.Link = 0;
            return;
        }
        else if (ball.ballType == BallType.BOMB)
            m_Player.AddScore(ConstantData.MissionBombPottedPoint);
        else if (ball.ballType == BallType.SINGULARITY)
            m_Player.AddScore(ConstantData.MissionSingularityPottedPoint);
        else if(ball.ballType == BallType.ABSORB)
        {
            m_Player.AddScore(ConstantData.MissionAbsorbPottedPoint);
            AbsorbBall b = (AbsorbBall)ball;
            for(int i = 0, count = b.AbsorbList.Count; i < count; i++)
            {
                PotBall(b.AbsorbList[i], pocket);
                Pools.StorageRack.Add(b.AbsorbList[i]);
            }
            b.AbsorbList.Clear();
        }
        else
            m_Player.AddScore(ConstantData.MissionPottedPoint);

        m_Player.Link++;
        m_Player.PottedCount++;
        m_TargetBalls.Remove(ball.GetBallID());
    }

    protected void OnSingularityBreakBall(PoolBall ball)
    {
        if(ball)
        {
            Destroy(ball.gameObject);
            m_TargetBalls.Remove(ball.GetBallID());
        }
    }

    public override void BallHitRail()
    {
        
    }

    protected override void CustomUpdate()
    {
        
    }

    public override void WhiteBallHitBall(PoolBall ball)
    {
        m_WhiteballHitBallThisRound = true;
    }

    protected override void CallPocket()
    {
    }

    protected override bool HandleFouls()
    {
        return false;
    }

    public override bool CheckGameOver()
    {
        return m_TargetBalls.Count == 0 || m_Player.ShotsRemain == 0;
    }
}
