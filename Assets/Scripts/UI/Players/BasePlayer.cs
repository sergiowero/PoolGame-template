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

    protected string playerName = "Player 1";

    protected List<PoolBall> m_TargetBalls = new List<PoolBall>();
    public List<PoolBall> BallsList { get { return m_TargetBalls; } }

    [System.NonSerialized]
    public PocketIndexes m_TargetPocketIndex = 0;

    public int playerID { get; set; }
    protected CountdownOutline m_Outline;
    protected Image m_Avatar;
    protected Text m_PlayerName;
    [SerializeField]
    protected GridLayoutGroup m_SlotsRoot;

    protected virtual void Awake()
    {
        m_Outline = transform.FindChild("Outline").GetComponent<CountdownOutline>();
        m_Avatar = transform.FindChild("Avatar").GetComponent<Image>();
        m_PlayerName = transform.FindChild("PlayerName").GetComponent<Text>();
        m_TargetBallType = BallType.NONE;
        playerID = GetInstanceID();
        m_PlayerName.text = name;
    }

    protected void AddBalls(int min, int max)
    {
        for (int i = min; i <= max; i++)
        {
            if(!Pools.Balls[i].pocketed)
            {
                m_TargetBalls.Add(Pools.Balls[i]);
                Image o = SupportTools.AddChild<Image>(m_SlotsRoot.gameObject, "TestRes/Slot");
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
                if (m_TargetBalls[i].pocketed)
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
        m_Outline.SetValue(percentage);
    }

    public virtual void Begin()
    {
        m_Outline.enabled = true;
    }

    public virtual void End()
    {
        m_Outline.enabled = false;
    }
}		

