using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BasePlayer : MonoBehaviour, IPlayer
{
    protected BallType m_TargetBallType;
    public virtual BallType TargetBallType
    {
        set 
        { 
            m_TargetBallType = value; 
        }
        get { return m_TargetBallType; }
    }

    protected List<PoolBall> m_TargetBalls = new List<PoolBall>();
    public List<PoolBall> BallsList { get { return m_TargetBalls; } }

    private int m_Combo = 0;

    public int combo
    {
        set 
        {
            m_Combo = value;
            GameStatistics.MarkMaxCombo(m_Combo);
        }
        get { return m_Combo; }
    }

    public int playerID { get; set; }
    [SerializeField]
    protected CountdownOutline m_Outline;
    [SerializeField]
    protected Image m_Avatar;
    [SerializeField]
    public Text playerName;
    [SerializeField]
    protected GridLayoutGroup m_SlotsRoot;

    protected virtual void Awake()
    {
        m_TargetBallType = BallType.NONE;
        playerID = GetInstanceID();
        playerName.text = name;
        //m_Outline.enabled = false;
        PoolRulesBase.onNewTurn += TurnBegin;
    }

    protected virtual void OnDestroy()
    {
        PoolRulesBase.onNewTurn -= TurnBegin;
    }

    protected virtual void TurnBegin(int turn)
    {
        if(((PoolRulesStandard)GameManager.Rules).CurrentPlayer.playerID != playerID)
        {
            m_Outline.enabled = false;
        }
        else
        {
            m_Outline.enabled = true;
        }
    }

    protected void AddBalls(int min, int max)
    {
        for (int i = min; i <= max; i++)
        {
            if(!Pools.Balls[i].IsBallDisable())
            {
                m_TargetBalls.Add(Pools.Balls[i]);
                Image o = SupportTools.AddChild<Image>(m_SlotsRoot.gameObject, "UI/BattleScene/Slot");
                o.sprite = Resources.Load<Sprite>("BallsIcon/" + i.ToString());
                o.transform.name = i.ToString();
            }
        }
    }

    public void UpdateBallIcon()
    {
        if (m_TargetBallType == BallType.NONE) 
            return;

        if(m_TargetBalls.Count != 0)
        {
            for (int i = 0; i < m_TargetBalls.Count; )
            {
                if (m_TargetBalls[i].BallState == PoolBall.State.POTTED)
                {
                    Transform t = m_SlotsRoot.transform.FindChild(m_TargetBalls[i].GetBallID().ToString());
                    if (t) Destroy(t.gameObject);
 
                    m_TargetBalls.RemoveAt(i);
                }
                else  i++;
            }
            if (m_TargetBalls.Count == 0 && m_TargetBallType != BallType.BLACK)
                m_TargetBallType = BallType.BLACK;
        }

        if(m_TargetBalls.Count == 0)
        {
            if (m_TargetBallType == BallType.BLACK) AddBalls(8, 8);
            if (m_TargetBallType == BallType.SOLID) AddBalls(1, 7);
            if (m_TargetBallType == BallType.STRIPE) AddBalls(9, 15);
        }
    }

    public void Countdown(float percentage)
    {
        if(m_Outline.enabled)
            m_Outline.SetValue(percentage);
    }

    public virtual void Begin()
    {
        if(!m_Outline.enabled)
            HOAudioManager.PlayClip("Ready", 1);
        //m_Outline.enabled = true;
    }

    public virtual void End()
    {
        //m_Outline.enabled = false;
    }

    /// <summary>
    /// use for the network and aiplayer
    /// </summary>
    public virtual void PlayerUpdate() { }
}		

