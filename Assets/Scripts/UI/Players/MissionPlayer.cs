using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionPlayer : MonoBehaviour, IPlayer
{
    [SerializeField]
    private Text m_ShotsRemain;
    [SerializeField]
    private Text m_LevelName;
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
        set 
        {
            m_PlayerData.ShotsRemain = value < 0 ? 0 : value;
            m_ShotsRemain.text = m_PlayerData.ShotsRemain.ToString();
        }
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
        }
    }

    public int Score
    {
        get { return m_PlayerData.Score; }
        set
        {
            if(value <= 0)
                m_PlayerData.Score = 0;
            else
                m_PlayerData.Score = value;
            if (m_PlayerData.HighScore < m_PlayerData.Score)
                m_PlayerData.HighScore = m_PlayerData.Score;
            m_Score.text = Score.ToString();
        }
    }

    public void AddCues(int cue, Vector3 position)
    {
        if (cue == 0)
            return;

        ShotsRemain += cue;
        Color c;
        string f = string.Format(HOLocalizationConfiguration.GetValue(113), cue);
        if (cue > 0)
            c = Color.yellow;
        else
        {
            m_PlayerData.Link = 0;
            c = Color.red;
        }
        BaseUIController.GenerateTips(f, c, position);
    }

    public void AddCues(int cue, PocketTrigger pocket)
    {
        if (cue == 0)
            return;

        AddCues(cue, MathTools.World2UI(pocket.GetRealPosition()));
    }

    public void AddScore(int score, PocketTrigger pocket)
    {
        int s = (int)(score * (m_PlayerData.Link + 4) * .2f);
        Score += s;

        if(pocket)
        {
            string tips;
            if (Link > 1)
                tips = "COMBO x " + Link + "\n+" + s;
            else
                tips = "+" + s;
            BaseUIController.GenerateTips(tips, Color.yellow, MathTools.World2UI(pocket.GetRealPosition()));
        }
    }

    void Awake()
    {
        PoolRulesBase.onGameOver += GameOver;
        BombBall.GameoverWithBoom += GameOver;
        DemonBall.GameOverWithPotted += GameOver;
        PoolRulesBase.onFireBall += FireBall;
        m_LevelName.text = LevelDataIndex.CurrentLevel.FileName;
        m_ShotsRemain.text = m_PlayerData.ShotsRemain.ToString();
        m_Score.text = "0";
//#if UNITY_ANDROID && !UNITY_EDITOR
//        string filePath = StreamTools.GetStreamingAssetsPath() + ConstantData.MissionLevelDataRecordPath;
//        StartCoroutine(StreamTools.LoadBytes<MissionRecords>(filePath, OnLoadedMissionRecordsOnAndroid));
//#else
//        LoadMissionRecords();
//#endif
    }

    void OnDestroy()
    {
        PoolRulesBase.onGameOver -= GameOver;
        BombBall.GameoverWithBoom -= GameOver;
        DemonBall.GameOverWithPotted -= GameOver;
        PoolRulesBase.onFireBall -= FireBall;
    }

    protected void GameOver(IPlayer player)
    {
        string name = LevelDataIndex.CurrentLevel.FileName;
        m_PlayerData.HitRate = m_PlayerData.ShotCount == 0 ? 0 : Mathf.Round(((float)m_PlayerData.PottedCount / (float)m_PlayerData.ShotCount) * 1000f) / 1000f;
        int star = 0;
        if(player != null && player.playerID == playerID)
        {
            AddScore(ShotsRemain * 200, null);
            m_PlayerData.Star = LayoutConfiguration.instance[name].GetStarWithScore(m_PlayerData.Score);
            star = Mathf.Max(ConstantData.missionRecords.GetStar(name), m_PlayerData.Star);
            ConstantData.missionRecords.Record(name, star, m_PlayerData.HighScore);
            StreamTools.SerializeObject(ConstantData.missionRecords, ConstantData.missionLevelDataRecordPath);
            BaseUIController.MSettlement.MissionComplete(m_PlayerData);
            string nextMission = ConstantData.LevelDatas.Next(LevelDataIndex.CurrentLevel).FileName;
            if(ConstantData.LevelDatas.Comp(PlayerPrefs.GetString(ConstantData.MissionProgressKeyName), nextMission) == -1)
                PlayerPrefs.SetString(ConstantData.MissionProgressKeyName, nextMission);
            
            //about achieve
            if(GameManager.Rules.firstRound)
                ConstantData.achieveRecords.Mark(name, AchieveType.FinishWithTriangularPole);
            else
                ConstantData.achieveRecords.Mark(name, AchieveType.Finish);
        }
        else
        {
            BaseUIController.MSettlement.MissionFail(m_PlayerData);
        }
        //BaseUIController.MSettlement.SetMissionData(m_PlayerData);
    }

    protected void FireBall()
    {
        m_PlayerData.ShotCount++;
        AddCues(ConstantData.MissionFireShots, m_ShotsRemain.transform.position);
    }
}
