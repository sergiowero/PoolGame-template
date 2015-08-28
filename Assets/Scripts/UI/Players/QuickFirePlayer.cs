using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuickFirePlayer : MonoBehaviour, IPlayer
{
    public int playerID { get; set; }

    protected Text m_Multiplier, m_Rack, m_Time, m_Score;

    protected float m_MultiplierValue, m_RackValue, m_TimeValue, m_ScoreValue;

    protected float m_TimeShowing;
    [SerializeField]
    protected int m_TimeTurningSpeed = 60;
    protected System.TimeSpan m_TimeSpan;

    protected bool m_PottedBallThisRound;

    [SerializeField]
    protected Image[] m_BallIcons;

    void Awake()
    {
        m_Multiplier = transform.FindChild("Multiplier").FindChild("Value").GetComponent<Text>();
        m_Rack = transform.FindChild("Rack").FindChild("Value").GetComponent<Text>();
        m_Time = transform.FindChild("Time").FindChild("Value").GetComponent<Text>();
        m_Score = transform.FindChild("Score").FindChild("Value").GetComponent<Text>();
        m_MultiplierValue = 1;
        m_RackValue = 1;
        m_ScoreValue = 0;
    }

    void Update()
    {
        if (m_TimeShowing == m_TimeValue)
            return;

        if(m_TimeShowing < m_TimeValue)
        {
            m_TimeShowing += Time.deltaTime * m_TimeTurningSpeed;
            if (m_TimeShowing > m_TimeValue)
                m_TimeShowing = m_TimeValue;
        }
        else if(m_TimeShowing > m_TimeValue)
        {
            m_TimeShowing -= Time.deltaTime * m_TimeTurningSpeed;
            if (m_TimeShowing < m_TimeValue)
                m_TimeShowing = m_TimeValue;
        }
        m_TimeSpan = System.TimeSpan.FromSeconds(m_TimeShowing);
        m_Time.text = m_TimeSpan.Minutes + ":" + m_TimeSpan.Seconds;
    }

    public void SetTime(float time, bool immediate = false)
    {
        m_TimeValue = time > 0 ? time : 0;
        if (immediate)
            m_TimeShowing = m_TimeValue;
    }

    public void AddScore(float score)
    {
        m_ScoreValue += score * m_MultiplierValue;
        m_Score.text = m_ScoreValue.ToString();
    }

    public void AddBall(int ballID)
    {
        for(int i = 0, length = m_BallIcons.Length; i < length; i++)
        {
            if(m_BallIcons[i].sprite == null)
            {
                m_BallIcons[i].sprite = Pools.BallIcons[ballID - 1];
                iTween.FadeTo(m_BallIcons[i].gameObject, 1, .5f);
                break;
            }
        }
    }

    public void Clear()
    {

    }
}
