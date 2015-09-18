using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolRulesMission : PoolRulesBase
{

    protected MissionPlayer m_Player;

    protected IList<int> m_TargetBalls = new List<int>();

    protected bool m_WhiteballHitBallThisRound = false;

    void Awake()
    {
    }

    void OnDestroy()
    {
    }

    protected override void Start()
    {
        List<LevelData.DisplayDatas> ld = LevelData.CurrentLevel.BallsDrawList;
        List<LevelData.PositionDatas> lp = LevelData.CurrentLevel.BallsPosition;
        List<LevelData.OtherObjectDatas> lo = LevelData.CurrentLevel.OtherObjectsPosition;
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
        for (int i = 0, count = lo.Count; i < count; i++ )
        {
            LevelData.OtherObjectDatas d = lo[i];
            GameObject o = Resources.Load(d.Type.ToString()) as GameObject;
            GameObject oo = Instantiate(o) as GameObject;
            oo.transform.SetParent(GameObject.Find("8Ball/OtherObjects").transform);
            oo.transform.position = d.Position;
            oo.GetComponent<PoolBall>().SetBallID(d.ID);
            oo.GetComponent<PoolBall>().ballType = d.Type;
        }
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
        Debug.Log("Turn begin");
        m_WhiteballHitBallThisRound = false;
    }

    public override void SetPlayers(params IPlayer[] players)
    {
        m_Player = players[0] as MissionPlayer;
    }

    public override void PotBall(PoolBall ball, PocketIndexes pocket)
    {
        base.PotBall(ball, pocket);
        if(ball.ballType == BallType.WHITE)
        {
            m_Player.Link = 0;
        }
        else
        {
            m_Player.AddScore(ConstantData.MissionPottedScore);
            m_Player.Link++;
            m_Player.PottedCount++;
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
