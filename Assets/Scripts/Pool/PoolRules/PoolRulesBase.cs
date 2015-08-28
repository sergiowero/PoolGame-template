using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Flags]
public enum GlobalState
{
    NONE = 1 << 0,
    ROLLING = 1 << 1,
    DRAG_WHITEBALL = 1 << 2,
    IDLE = 1 << 3
}

public enum CallPockets
{
    None = 0,
    NotUse = 1,
    BlackBallOnly = 2,
    AllShots = 3
}

public enum PocketIndexes
{
    TopLeft = 1,
    TopCenter = 2,
    TopRight = 3,
    BottomLeft = 4,
    BottomCenter = 5,
    BottomRight = 6
}

public enum BallType
{
    NONE = 99,
    STRIPE = 9,
    SOLID = 1,
    BLACK = 8,
    WHITE = 0
}

public abstract class PoolRulesBase : MonoBehaviour
{
    public static System.Action<int> onNewTurn;
    public static System.Action onFireBall;
    public static System.Action<IPlayer> onGameOver;

    protected GlobalState m_State = GlobalState.NONE;
    protected GlobalState m_PrevState;

    public GlobalState State 
    { 
        get { return m_State;}
        set { m_PrevState = m_State; m_State = value; }
    }
    public void ReversePrevState()
    {
        m_State = m_PrevState;
    }
    protected int m_Turn = 0;
    public bool FirstRound { get { return m_Turn == 1; } }

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

    void Start()
    {
        TurnBegin();
    }

    void Update()
    {
        if (State == GlobalState.ROLLING && !m_GameOver)
        {
            bool rollingDone = true;
            PoolBall[] balls = Pools.BallsArray;
            for (int i = 0, length = balls.Length; i < length; i++)
            {
                if (balls[i] && !balls[i].IsDoneRolling())
                {
                    rollingDone = false;
                    break;
                }
            }
            if (rollingDone)
            {
                State = GlobalState.IDLE;
                StartCoroutine(CheckResultAndChangeTurn(ConstantData.TurnWaitTime));
            }
        }
        CustomUpdate();
    }

    #region Virtual methods.............................
    public virtual void PotBall(PoolBall ball, PocketIndexes pocket)
    {
        ball.Potted();
        BallInPocket ballInPocket = ball.GetComponent<BallInPocket>();
        if (ballInPocket) Destroy(ballInPocket);

        if (ball.ballType == BallType.WHITE)
            m_WhiteBallPotted = true;
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

        if (onNewTurn != null)
        {
            onNewTurn(m_Turn);
        }

        //
        //reset game state
        //
        if (m_WhiteBallPotted)
            Pools.CueBall.Reset();
        m_WhiteBallPotted = false;
        m_TimeOut = false;
        for (int i = 0, count = m_PottedBallListThisRound.Count; i < count; i++)
        {
            PoolBall pb = m_PottedBallListThisRound[i];
            m_PottedBallList.Add(pb.GetBallID(), pb);
        }
        m_PottedBallListThisRound.Clear();
    }
    #endregion

    #region Abstract methods......................
    public abstract void SetPlayers(params IPlayer[] players);

    public abstract void BallHitRail();

    protected abstract IEnumerator CheckResultAndChangeTurn(float time);

    //protected abstract void OnTimeOut();

    public abstract void Initialize();

    protected abstract void CustomUpdate();

    public abstract void WhiteBallHitBall(PoolBall ball);

    protected abstract void CallPocket();

    protected abstract bool HandleFouls();

    public abstract bool CheckGameOver();
    #endregion
}
