using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolRulesMission : PoolRulesBase
{

    protected MissionPlayer m_Player;

    protected IList<int> m_TargetBalls = new List<int>();

    protected bool m_WhiteballHitBallThisRound = false;

    protected int m_ScoreThisRound = 0;

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

    protected override void Start()
    {
        Pools.DisableStandardBalls();
        List<LevelData.BallData> lp = LevelDataIndex.CurrentLevel.ballDatas;
        Transform ooRoot = GameObject.Find("8Ball/OtherObjects").transform;
        for (int i = 0, count = lp.Count; i < count; i++)
        {
            LevelData.BallData d = lp[i];
            GameObject o = Resources.Load(d.Type.ToString()) as GameObject;
            GameObject oo = Instantiate(o) as GameObject;
            oo.transform.SetParent(ooRoot);
            d.Position.y = 0;
            oo.transform.position = d.Position;
            PoolBall pb = oo.GetComponent<PoolBall>();
            pb.SetBallID(d.ID);
            pb.ballType = d.Type;
            if(pb.ballType != BallType.JIANGYOU && pb.ballType != BallType.DEMON)
                m_TargetBalls.Add(d.ID);
            Pools.CustomBalls.Add(pb.GetBallID(), pb);
        }
        Pools.CueBall.transform.position = LevelDataIndex.CurrentLevel.cueBallData.Position;
        PocketTrigger.MarkPocketType(LevelDataIndex.CurrentLevel.StartPunishmentPocket, LevelDataIndex.CurrentLevel.StartRewardPocket);
        PocketTrigger.Block(LevelDataIndex.CurrentLevel.BlockPockets);
        m_Player.ShotsRemain = LevelDataIndex.CurrentLevel.shotCount;
        TurnBegin();
    }

    protected override IEnumerator CheckResultAndChangeTurn(float time)
    {
        yield return new WaitForSeconds(time);

        if (m_PottedBallListThisRound.Count == 0) m_Player.Link = 0; 

        if(m_WhiteBallPotted)
        {
            m_Player.Score -= m_ScoreThisRound;
            BaseUIController.text.Show(string.Format(HOLocalizationConfiguration.GetValue(106), m_ScoreThisRound));
            yield return new WaitForSeconds(ConstantData.MissionFoulTimeWait);
            for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
            {
                m_PottedBallListThisRound[i].BackToPrevRoundState();
            }
        }
        else
        {
            for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
            {
                PoolBall pb = m_PottedBallListThisRound[i];
                m_PottedBallList.Add(pb.GetBallID(), pb);
            }
        }
        m_PottedBallListThisRound.Clear();

        if (CheckGameOver())
        {
            m_GameOver = true;
            if (onGameOver != null)
            {
                 if (m_Player.ShotsRemain == 0)
                    onGameOver(null);
                else if (m_TargetBalls.Count == 0)
                    onGameOver(m_Player);
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
        {
            //m_PunishmentCountThisRound++;
            m_Player.AddCues(ConstantData.PunitiveShots, PocketTrigger.GetPocketWithIndexes(pocket));
        }
        if ((pocket & PocketTrigger.RewardPocket) != 0)
        {
            //m_RewardCountThisRound++;
            m_Player.AddCues(ConstantData.RewardShots, PocketTrigger.GetPocketWithIndexes(pocket));
        }
        int score = 0;
        if (ball.ballType == BallType.WHITE)
        {
            m_Player.Link = 0;
            m_Player.AddCues(ConstantData.MissionCueballPottedPunishment, PocketTrigger.GetPocketWithIndexes(pocket));
            return;
        }
        else if (ball.ballType == BallType.BOMB)
            score = ConstantData.MissionBombPottedPoint;
        else if (ball.ballType == BallType.SINGULARITY)
            score = ConstantData.MissionSingularityPottedPoint;
        else if (ball.ballType == BallType.ABSORB)
        {
            score = ConstantData.MissionAbsorbPottedPoint;
            AbsorbBall b = (AbsorbBall)ball;
            for (int i = 0, count = b.AbsorbList.Count; i < count; i++)
            {
                PotBall(b.AbsorbList[i], pocket);
                Pools.StorageRack.Add(b.AbsorbList[i]);
            }
            b.AbsorbList.Clear();
        }
        else if (ball.ballType == BallType.REDCUSTOM)
            score = ConstantData.MissionRedBallPoint;
        else if (ball.ballType == BallType.BLUECUSTOM)
            score = ConstantData.MissionBlueBallPoint;
        else if (ball.ballType == BallType.YELLOWCUSTOM)
            score = ConstantData.MissionYellowBallPoint;
        else if (ball.ballType == BallType.JIANGYOU)
            score = ConstantData.MissionJiangYouBallPoint;
        else if (ball.ballType == BallType.DEMON)
            score = ConstantData.MissionDemonBallPoint;
        if(score > 0)
        {
            m_Player.Link++;
            m_Player.AddScore(score, PocketTrigger.GetPocketWithIndexes(pocket));
            m_ScoreThisRound = score;
        }

        if (m_Player.Link >= 5)
        {
            BaseUIController.GenerateTips("Great!");
        }

        m_Player.PottedCount++;
        if (m_TargetBalls.Contains(ball.GetBallID())) 
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

    public override void CueBallHitRail()
    {
    }
}
