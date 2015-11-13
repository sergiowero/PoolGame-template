using UnityEngine;
using System.Collections;
using System.Text;

public enum GLanguage
{
    zhCN,
    zhTW,
    ruRU,
    enUS,
    enGB
}

public class ConstantData
{
    public static GLanguage GameLanguage = GLanguage.enUS;

    public static string BattleSceneType = null;

    public const string ToPratice = "ToPratice";

    public const string ToLevel = "ToLevel";

    public const string ToAI = "ToAI";

    public static int LoadedChapter = 0;

    public const string MusicVolumnKey = "music_volumn";

    public const string SoundVolumnKey = "sound_volumn";

    public const int True = 1;

    public const int False = 0;

    private static int _Sound = 1;
    public static bool Sound
    {
        set
        {
            _Sound = value ? True : False;
            PlayerPrefs.SetInt(SoundVolumnKey, _Sound);
        }
        get
        {
            return _Sound == True;
        }
    }

    private static int _Music = 1;
    public static bool Music
    {
        get
        {
            return _Music == True;
        }
        set
        {
            _Music = value ? True : False;
            PlayerPrefs.SetInt(MusicVolumnKey, _Music);
        }
    }

    public const float BallRadiusAdjustment = .004f;

    public const float BallDragRayCastRadius = 1f;

    public const float TurnWaitTime = .1f;

    public const float TimePerRoundLow = 30;

    public const float TimePerRoundHigh = 60;

    public const float TimeLimitQuickFire = 60 * 2; 

    public const int QuickFireBallPottedPoint = 100;

    public const float QuickFireComboRewards = .5f;

    public const string MissionProgressKeyName = "MissionProgress";

    public const int MissionFireBallPottedPoint = 100;

    public const int MissionShotCount = 5;

    public const int GuidelineLength = 60;

    public const int MissionCueballPottedPunishment = -2;

    public const int MissionFireShots = -1;

    public const int MissionFoulTimeWait = 2;

    #region Ball points......
    public const int MissionPottedPoint = 100;

    public const int MissionSingularityPottedPoint = 300;

    public const int MissionBombPottedPoint = 300;

    public const int MissionAbsorbPottedPoint = 0;

    public const int MissionYellowBallPoint = 150;

    public const int MissionRedBallPoint = 100;

    public const int MissionBlueBallPoint = 200;

    public const int MissionDemonBallPoint = 0;

    public const int MissionJiangYouBallPoint = 0;
    #endregion //Ball points......

    public const int MissionBombBallDuration = 90;

    public const int SpecialPocketProbability = 30;

    public const int PunitiveShots = -2;

    public const int RewardShots = 1;

    public const int PhysicalRecoverInterval = 20 * 60;

    private static int _OutlineAndBallLayer = -1;
    public static int OulineAndBallLayer
    {
        get 
        { 
            if(_OutlineAndBallLayer == -1)
            {
                _OutlineAndBallLayer = 1 << LayerMask.NameToLayer("Outline") | 1 << LayerMask.NameToLayer("Ball");
            }
            return _OutlineAndBallLayer;
        }
    }
    public static string achieveDataRecordPath
    {
        get
        {
            return StreamTools.GetPersistentDataPath() + "GameRecords/AchieveData";
        }
    }

    private static PoolDataAsset PoolDatas = null;
    public static string PoolDataAssetsFile { get { return StreamTools.GetStreamingAssetsPath(true) + "PoolEnvironmentData/PoolPhysical.asset"; } }
    public static string quickFireGameRecordPath 
    {
        get 
        { 
            return StreamTools.GetPersistentDataPath() + "GameRecords/QuickFireData";
        } 
    }

    public static string gameStatisticsDataPath 
    { 
        get
        { 
        return StreamTools.GetStreamingAssetsPath() + "GameRecords/GameStatisticsData"; 
        }
    }

    public static string missionLevelDataRecordPath
    { 
        get 
        {
            return StreamTools.GetPersistentDataPath() + "GameRecords/MissionData";
        } 
    }
    public static string MissionLevelDataPath { get { return "LevelDatas/"; } }
    public static string MissionLevelDataIndexPath { get { return "LevelDatas/LevelDataIndex.asset"; } }


    private static int _Physical = 20;

    public const int physicalSubstact = 2;

    public static int physical
    {
        set
        {
            _Physical = value;
            if (_Physical > maxPhysical)
                _Physical = maxPhysical;
            LaunchUIController.SetPhysical(_Physical);
        }
        get { return _Physical; }
    }

    public const int maxPhysical = 20;

    public static LevelDataIndex LevelDatas;

    private static GameStatistics _GameStatisticsData = null;
    public static GameStatistics gameStatisticsData
    {
        set
        {
            if(_GameStatisticsData == null)
                _GameStatisticsData = value;
        }
        get { return _GameStatisticsData;  }
    }

    private static AchieveRecords _AchieveRecords;
    public static AchieveRecords achieveRecords
    {
        set
        {
            _AchieveRecords = value;
            StreamTools.SerializeObject(_AchieveRecords, ConstantData.achieveDataRecordPath);
        }
        get { return _AchieveRecords; }
    }

    private static MissionRecords _MissionRecords;
    public static MissionRecords missionRecords
    {
        set 
        {
            _MissionRecords = value;
            StreamTools.SerializeObject(_MissionRecords, ConstantData.missionLevelDataRecordPath);
        }
        get { return _MissionRecords; }
    }

    private static QuickFirePlayer.PlayerData _QuickFireRecords;

    public static QuickFirePlayer.PlayerData quickFireRecords
    {
        set
        {
            _QuickFireRecords = value;
            StreamTools.SerializeObject(_QuickFireRecords, ConstantData.quickFireGameRecordPath);
        }
        get { return _QuickFireRecords; }
    }

    public static PoolDataAsset GetPoolDatas()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        if (PoolDatas == null)
        {
            PoolDatas = StreamTools.DeserializeObject<PoolDataAsset>(StreamTools.GetStreamingAssetsPath() +  PoolDataAssetsFile);
            if (PoolDatas == null)
            {
                UnityEngine.Debug.Log("Pool asset data is null");
                PoolDatas = new PoolDataAsset();
            }
        }
#endif
        return PoolDatas;
    }

    public static void SetPoolDatas(PoolDataAsset data)
    {
        PoolDatas = data;
    }
}

[System.Serializable]
public class PoolDataAsset
{
    public PoolDataAsset()
    {
        MaxImpulse = 60;
        BallAngularDrag = 6;
        BallDrag = .5f;
        BallBounciness = .95f;
        RailBounciness = .8f;
        HorizontalSidingStrength = .5f;
        VerticalSidingStrength = 1;
    }

    public float MaxImpulse;

    public float BallAngularDrag;

    public float BallDrag;

    public float BallBounciness;

    public float RailBounciness;

    public float HorizontalSidingStrength;

    public float VerticalSidingStrength;

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Max impulse : ").Append(MaxImpulse)
            .Append("\nWhite ball angular drag : ").Append(BallAngularDrag)
            .Append("\nWhite ball drag : ").Append(BallDrag)
            .Append("\nBall bounciness : ").Append(BallBounciness)
            .Append("\nRail bounciness : ").Append(RailBounciness)
            .Append("\nHorizontal siding strength : ").Append(HorizontalSidingStrength)
            .Append("\nVertical siding strength : ").Append(VerticalSidingStrength);
        return builder.ToString();
    }
}

