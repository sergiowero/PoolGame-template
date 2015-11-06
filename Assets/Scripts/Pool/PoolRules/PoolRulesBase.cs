using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Flags]
public enum GlobalState
{
    NONE = 1 << 0,
    ROLLING = 1 << 1,
    DRAG_WHITEBALL = 1 << 2,
    IDLE = 1 << 3,
    GAMEOVER = 1 << 4
}

public enum CallPockets
{
    None = 0,
    NotUse = 1,
    BlackBallOnly = 2,
    AllShots = 3
}

public enum PocketType
{
    None = 0,
    Punishment = 1,
    Reward = 2, 
    BlockOff
}

[System.Flags]
public enum PocketIndexes
{
    None = 0,
    TopLeft = 1 << 0,
    TopCenter = 1 << 1,
    TopRight = 1 << 2,
    BottomLeft = 1 << 3,
    BottomCenter = 1 << 4,
    BottomRight = 1 << 5
}

public enum BallType
{
    NONE = 99,
    STRIPE = 15,
    SOLID = 7,
    BLACK = 8,
    REDCUSTOM = 20,
    BLUECUSTOM = 30,
    YELLOWCUSTOM = 40,
    BOMB = 70,
    ABSORB = 71,
    SINGULARITY = 72,
    JIANGYOU = 73,
    DEMON = 74,
    WHITE = 0
}

public enum GameType
{
    None = 0,
    QuickFire = 1,
    Mission = 2,
    Standard = 3,
    AI = 4,
    Multiple = 5
}

public enum AIDifficulty
{
    VeryLow = 30,
    Low = 50,
    Medium = 70
}

public abstract class PoolRulesBase : MonoBehaviour
{
    public static Delegate1Args<int> onNewTurn;
    public static Delegate0Args onFireBall;
    public static Delegate1Args<IPlayer> onGameOver;
    public static Delegate0Args onCueballPotted;

    protected GlobalState m_State = GlobalState.NONE;
    protected GlobalState m_PrevState;

    public GlobalState State 
    { 
        get { return m_State;}
        set
        {
            m_PrevState = m_State;
            m_State = value;
            //if (ConstantData.GType == GameType.Standard)
            BaseUIController.cueOperateArea.ChangeOperationType(value);
        }
    }
    public void ReversePrevState()
    {
        m_State = m_PrevState;
    }
    protected int m_Turn = 0;
    public bool firstRound { get { return m_Turn == 1; } }

    //public int round { get { return m_Turn; } }

    protected bool m_GameOver = false;

    protected bool m_WhiteBallPotted = false;

    protected float m_Time;

    protected bool m_TimeOut = false;

    protected Dictionary<int, PoolBall> m_PottedBallList = new Dictionary<int, PoolBall>();
    public Dictionary<int, PoolBall> PottedBallList { get { return m_PottedBallList; } }

    protected List<PoolBall> m_PottedBallListThisRound = new List<PoolBall>();
    public List<PoolBall> PottedBallListThisRound { get { return m_PottedBallListThisRound; } }

    protected bool m_HandleWhiteball = true;
    public bool HandleWhiteBall { get { return m_HandleWhiteball; } }

    #region Virtual methods.............................
    protected virtual void Start()
    {
        Pools.ResetAllBalls(false, true);
        Invoke("TurnBegin", .01f);
        //TurnBegin();
    }

    protected virtual void Update()
    {
        if (State == GlobalState.ROLLING && !m_GameOver)
        {
            bool rollingDone = CheckIsBallsDoneRolling(Pools.BallsArray) && CheckIsBallsDoneRolling(Pools.CustomBallsArray);
            if (rollingDone)
            {
                State = GlobalState.IDLE;
                StartCoroutine(CheckResultAndChangeTurn(ConstantData.TurnWaitTime));
            }
        }
        CustomUpdate();
    }

    protected bool CheckIsBallsDoneRolling(PoolBall[] balls)
    {
        for (int i = 0, length = balls.Length; i < length; i++)
        {
            if (balls[i])
            {
                if (balls[i].BallState == PoolBall.State.HIDE)
                    continue;

                if (!balls[i].IsDoneRolling())
                {
                    return false;
                }
            }
        }
        return true;
    }

    public virtual void PotBall(PoolBall ball, PocketIndexes pocket)
    {
        ball.Potted(pocket);

        if (ball.ballType == BallType.WHITE)
        {
            m_WhiteBallPotted = true;
            if (onCueballPotted != null)
                onCueballPotted();
        }
        else
            m_PottedBallListThisRound.Add(ball);
    }

    public virtual void OnBallFired()
    {
        State = GlobalState.ROLLING;
        if (onFireBall != null)
        {
            onFireBall();
        }
    }

    protected virtual void TurnBegin()
    {
        m_Turn++;
        //
        //reset game state
        //
        if (m_WhiteBallPotted)
            Pools.CueBall.Reset();
        m_WhiteBallPotted = false;
        m_TimeOut = false;
        if (onNewTurn != null)
        {
            onNewTurn(m_Turn);
        }
    }

    protected virtual IEnumerator CheckResultAndChangeTurn(float time)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
        {
            PoolBall pb = m_PottedBallListThisRound[i];
            m_PottedBallList.Add(pb.GetBallID(), pb);
        }
        m_PottedBallListThisRound.Clear();
        TurnBegin();
    }
    #endregion

    #region Abstract methods......................
    public abstract void CueBallHitRail();

    public abstract void SetPlayers(params IPlayer[] players);

    public abstract void BallHitRail();

    //protected abstract void OnTimeOut();

    protected abstract void CustomUpdate();

    public abstract void WhiteBallHitBall(PoolBall ball);

    protected abstract void CallPocket();

    protected abstract bool HandleFouls();

    public abstract bool CheckGameOver();
    #endregion
}
