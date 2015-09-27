using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionPlayer : MonoBehaviour, IPlayer
{
    private MissionRecords m_Records = null;
    public MissionRecords Records { get { return m_Records; } }

    [SerializeField]
    private Text m_Combo;
    [SerializeField]
    private Text m_MaxCombo;
    [SerializeField]
    private Text m_Score;

    public int playerID {  get;  set;  }

    [System.Serializable]
    public class PlayerData : IPlayerData
    {
        public int ShotCount;
        public int PottedCount;
        public float HitRate;
        public int Link;
        public int MaxLink;
        public int ShotsRemain = ConstantData.MissionShotCount;
        public int HighScore;
        public int Score;
        public int Star;


        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("FireCount : ").Append(ShotCount).Append('\n')
                .Append("Potted count : ").Append(PottedCount).Append('\n')
                .Append("Hit rate : ").Append(HitRate).Append('\n')
                .Append("Link : ").Append(Link).Append('\n')
                .Append("Max link : ").Append(MaxLink).Append('\n')
                .Append("Shot remain : ").Append(ShotsRemain).Append('\n')
                .Append("Hight score : ").Append(HighScore).Append('\n')
                .Append("Score : ").Append(Score).Append('\n')
                .Append("Star : ").Append(Star);
            return builder.ToString();
        }
    }

    protected PlayerData m_PlayerData = new PlayerData();

    public PlayerData Data { get { return m_PlayerData; } }

    public int ShotsRemain 
    { 
        get { return m_PlayerData.ShotsRemain; }
        set { m_PlayerData.ShotsRemain = value < 0 ? 0 : value; }
    }

    public int FireCount
    {
        get { return m_PlayerData.ShotCount; }
        set { m_PlayerData.ShotCount = value; }
    }

    public int PottedCount
    {
        get { return m_PlayerData.PottedCount; }
        set { m_PlayerData.PottedCount = value; }
    }

    public float HitRate
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
            m_Combo.text = "Combo : " + m_PlayerData.Link;
            m_MaxCombo.text = "Max combo : " + m_PlayerData.MaxLink;
        }
    }

    public void AddScore(int score)
    {
        m_PlayerData.Score += (int)(score * (m_PlayerData.Link + 4) * .2f);
        if (m_PlayerData.HighScore < m_PlayerData.Score)
            m_PlayerData.HighScore = m_PlayerData.Score;
        m_Score.text = "Score : " + m_PlayerData.Score;
    }

    void Awake()
    {
        PoolRulesBase.onGameOver += GameOver;
        BombBall.GameoverWithBoom += GameOver;
        PoolRulesBase.onFireBall += FireBall;
#if UNITY_ANDROID && !UNITY_EDITOR
        string filePath = StreamTools.GetStreamingAssetsPath() + ConstantData.MissionLevelDataRecordPath;
        StartCoroutine(StreamTools.LoadBytes<MissionRecords>(filePath, OnLoadedMissionRecordsOnAndroid));
#else
        LoadMissionRecords();
#endif
    }

    void OnDestroy()
    {
        PoolRulesBase.onGameOver -= GameOver;
        BombBall.GameoverWithBoom -= GameOver;
        PoolRulesBase.onFireBall -= FireBall;
    }

    protected void GameOver(IPlayer player)
    {
        string name = LevelDataIndex.CurrentLevel.FileName;
        m_PlayerData.Star = LayoutConfiguration.instance[name].GetStarWithScore(m_PlayerData.Score);
        m_PlayerData.HitRate = m_PlayerData.ShotCount == 0 ? 0 : Mathf.Round(((float)m_PlayerData.PottedCount / (float)m_PlayerData.ShotCount) * 1000f) / 1000f;
        int star = 0;
        if(player != null && player.playerID == playerID)
        {
            AddScore(ShotsRemain * 200);
            star = Mathf.Max(m_Records.GetStar(name), m_PlayerData.Star);
            m_Records.Record(name, star, m_PlayerData.HighScore);
            StreamTools.SerializeObject(m_Records, ConstantData.MissionLevelDataRecordPath);
            BaseUIController.MSettlement.ShowCongratulationUI(m_PlayerData.Score);
            PlayerPrefs.SetString(ConstantData.MissionProgressKeyName, ConstantData.LevelDatas.Next(LevelDataIndex.CurrentLevel).FileName);
        }
        else
        {
            BaseUIController.MSettlement.ShowRegretUI(m_PlayerData.Score, m_PlayerData.HighScore);
        }
        BaseUIController.MSettlement.SetMissionData(m_PlayerData);
    }

    protected void OnLoadedMissionRecordsOnAndroid(MissionRecords records)
    {
        if (records != null)
        {
            m_Records = records;
        }
        else
        {
            m_Records = new MissionRecords();
        }
        int highScoreRecord = m_Records.GetHighScore(LevelDataIndex.CurrentLevel.FileName);
        m_PlayerData.HighScore = highScoreRecord;
    }

    protected void LoadMissionRecords()
    {
        m_Records = StreamTools.DeserializeObject<MissionRecords>(ConstantData.MissionLevelDataRecordPath);
        if(m_Records == null)
        {
            m_Records = new MissionRecords();
        }
        int highScoreRecord = m_Records.GetHighScore(LevelDataIndex.CurrentLevel.FileName);
        m_PlayerData.HighScore = highScoreRecord;
    }

    protected void FireBall()
    {
        m_PlayerData.ShotCount++;
    }
}
