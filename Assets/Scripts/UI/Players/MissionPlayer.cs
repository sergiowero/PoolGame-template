using UnityEngine;
using System.Collections;

public class MissionPlayer : MonoBehaviour, IPlayer
{
    public int playerID {  get;  set;  }

    [System.Serializable]
    public class PlayerData
    {
        public int FireCount;
        public int PottedCount;
        public int HitRate;
        public int Link;
        public int MaxLink;
        public int ShotsRemain = ConstantData.MissionShotCount;
        public int HighScore;
        public int Score;
        public int Star;
    }

    public PlayerData m_PlayerData = new PlayerData();

    public int ShotsRemain 
    { 
        get { return m_PlayerData.ShotsRemain; }
        set { m_PlayerData.ShotsRemain = value < 0 ? 0 : value; }
    }

    public int FireCount
    {
        get { return m_PlayerData.FireCount; }
        set { m_PlayerData.FireCount = value; }
    }

    public int PottedCount
    {
        get { return m_PlayerData.PottedCount; }
        set { m_PlayerData.PottedCount = value; }
    }

    public int HitRate
    {
        get { return m_PlayerData.HitRate; }
        set { m_PlayerData.HitRate = value; }
    }

    public int Link
    {
        get { return m_PlayerData.Link; }
        set 
        {
            m_PlayerData.Link = value;
            if (m_PlayerData.MaxLink < m_PlayerData.Link)
                m_PlayerData.MaxLink = m_PlayerData.Link;
        }
    }

    public void AddScore(int score)
    {
        m_PlayerData.Score += score * (m_PlayerData.Link + 1);
    }

    void Awake()
    {
        PoolRulesBase.onGameOver += GameOver;
        PoolRulesBase.onFireBall += FireBall;
    }

    void OnDestroy()
    {
        PoolRulesBase.onGameOver -= GameOver;
        PoolRulesBase.onFireBall -= FireBall;
    }

    protected void GameOver(IPlayer player)
    {
        if(player != null && player.playerID == playerID)
        {
            ShowWinUI();
        }
        else
        {
            ShowLoseUI();
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    protected void FireBall()
    {
        m_PlayerData.FireCount++;
    }

    protected void ShowWinUI()
    {
        Debug.Log("Win");
    }

    protected void ShowLoseUI()
    {
        Debug.Log("Lose");
    }
}
