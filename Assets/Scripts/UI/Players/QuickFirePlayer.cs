using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuickFirePlayer : MonoBehaviour, IPlayer
{
    [System.Serializable]
    public class PlayerData : IPlayerData
    {
        public int ShotCount;
        public int PottedCount;
        public float HitRate;
        public int MaxLink;
        public float PlayTime;
        public int AverageTime;
        public int Score;
        public int HighScore;
        public int MaxRank = 1;
    }

    public PlayerData m_PlayerData;

    public int playerID { get; set; }

    protected Text m_Multiplier, m_Rack, m_Time, m_Score;

    protected float m_MultiplierValue, m_TimeValue;

    protected float m_TimeShowing;
    [SerializeField]
    protected int m_TimeTurningSpeed = 60;
    protected System.TimeSpan m_TimeSpan;

    protected bool m_PottedBallThisRound;

    public float PlayTime 
    {
        set { m_PlayerData.PlayTime = value; }
        get { return m_PlayerData.PlayTime; }
    }

    [SerializeField]
    protected Image[] m_BallIcons;

    public int Rank 
    {
        set { m_PlayerData.MaxRank = value; }
        get { return m_PlayerData.MaxRank; }
    }

    void Awake()
    {
        m_Multiplier = transform.FindChild("Multiplier").FindChild("Value").GetComponent<Text>();
        m_Rack = transform.FindChild("Rack").FindChild("Value").GetComponent<Text>();
        m_Time = transform.FindChild("Time").FindChild("Value").GetComponent<Text>();
        m_Score = transform.FindChild("Score").FindChild("Value").GetComponent<Text>();
        PoolRulesBase.onGameOver += OnGameOver;
        PoolRulesBase.onFireBall += OnFireBall;
        PoolRulesBase.onCueballPotted += ComboBreak;
        m_MultiplierValue = 1;
        m_PlayerData = new PlayerData();
#if UNITY_ANDROID && !UNITY_EDITOR
        string filePath = StreamTools.GetStreamingAssetsPath() + ConstantData.QuickFireGameRecordPath;
        StartCoroutine(StreamTools.LoadBytes<PlayerData>(filePath, OnLoadedPlayerDataOnAndroid));
#else
        LoadPlayerData();
#endif
    }

    void OnDestroy()
    {
        PoolRulesBase.onGameOver -= OnGameOver;
        PoolRulesBase.onFireBall -= OnFireBall;
        PoolRulesBase.onCueballPotted -= ComboBreak;
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

    protected void OnLoadedPlayerDataOnAndroid(PlayerData data)
    {
        if(data != null)
        {
            m_PlayerData.HighScore = data.HighScore;
        }
    }

    protected void LoadPlayerData()
    {
        PlayerData data = StreamTools.DeserializeObject<PlayerData>(ConstantData.QuickFireGameRecordPath);
        if(data != null)
        {
            m_PlayerData.HighScore = data.HighScore;
        }
    }

    protected void OnGameOver(IPlayer player)
    {
        m_PlayerData.HitRate = m_PlayerData.ShotCount == 0 ? 0 :  Mathf.Round((m_PlayerData.PottedCount / m_PlayerData.ShotCount) * 1000f) / 1000f;
        if(m_PlayerData.HighScore < m_PlayerData.Score)
        {
            m_PlayerData.HighScore = m_PlayerData.Score;
            StreamTools.SerializeObject(m_PlayerData, ConstantData.QuickFireGameRecordPath);
            BaseUIController.MSettlement.ShowCongratulationUI(m_PlayerData.Score);
        }
        else
        {
            BaseUIController.MSettlement.ShowRegretUI(m_PlayerData.Score, m_PlayerData.HighScore);
        }
        BaseUIController.MSettlement.SetPlayerData(m_PlayerData);
    }

    public void SetTime(float time, bool immediate = false)
    {
        m_TimeValue = time > 0 ? time : 0;
        if (immediate)
            m_TimeShowing = m_TimeValue;
    }

    public void AddScore(float score)
    {
        m_PlayerData.Score += (int)(score * m_MultiplierValue);
        m_Score.text = m_PlayerData.Score.ToString();
    }

    #region Add ball -------------------------------------------------
    public void AddBall(int ballID)
    {
        for(int i = 0, length = m_BallIcons.Length; i < length; i++)
        {
            if(m_BallIcons[i].sprite == null)
            {
                m_BallIcons[i].sprite = Pools.BallIcons[ballID - 1];
                if (i == length - 1)
                    iTween.FadeTo(m_BallIcons[i].gameObject, iTween.Hash("alpha", 1, "time", .5f, "oncomplete", "Combo", "oncompletetarget", gameObject));
                else
                    iTween.FadeTo(m_BallIcons[i].gameObject, 1, .5f);
                break;
            }
        }
        m_PlayerData.PottedCount++;
        m_PlayerData.MaxLink++;
    }

    protected void Combo()
    {
        m_MultiplierValue += ConstantData.QuickFireComboRewards;
        m_Multiplier.text = m_MultiplierValue.ToString() + "x";
        BallIconFadeoff();
    }

    public void ComboBreak()
    {
        m_MultiplierValue -= ConstantData.QuickFireComboRewards;
        if(m_MultiplierValue < 1)
            m_MultiplierValue = 1;
        m_Multiplier.text = m_MultiplierValue.ToString() + "x";
        BallIconFadeoff();
    }

    public void BallIconFadeoff()
    {
        for (int i = 0, length = m_BallIcons.Length; i < length - 1; i++)
        {
            iTween.FadeTo(m_BallIcons[i].gameObject, 0, .5f);
        }
        iTween.FadeTo(m_BallIcons[m_BallIcons.Length - 1].gameObject, iTween.Hash("alpha", 0, "time", .5f, "oncomplete", "ClearBallIcons", "oncompletetarget", gameObject));
    }

    protected void ClearBallIcons()
    {
        for (int i = 0, length = m_BallIcons.Length; i < length; i++)
        {
            m_BallIcons[i].sprite = null;
        }
    }
    #endregion

    protected void OnFireBall()
    {
        m_PlayerData.ShotCount++;
    }
}
