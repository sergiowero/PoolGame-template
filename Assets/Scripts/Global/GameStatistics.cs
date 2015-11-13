using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameStatistics
{
    public int shots;                       //击球数
    public int pottedBalls;             //进球数
    public int maxCombo;                //一场比赛最大连杆
    public int quickFireHighScore;      //时间赛最高分
    public int quickFireHighRound;          //时间赛最高回合
    public int missionStars;                //闯关赛总星数
    public int cueballPottedCount;      //白球进袋数
    public int cueballHitNoBallCount;           //击球后白球一个球都没打到
    public int cueballHitRailCount;                 //白球撞岸边次数
    public int cueballHitBallCount;             //白球击打到的球次数
    public int pottedJiangyouBallCount;     //把酱油球打到袋里的次数
    public int AIBattleWins;            //AI对战获胜次数
    public int AIBattleCount;               //AI对战次数

    public static void Serialize()
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        StreamTools.SerializeObject(ConstantData.gameStatisticsData, ConstantData.gameStatisticsDataPath);
    }

    public static void MarkPottedJiangyouBallCount(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.pottedJiangyouBallCount += value;
    }

    public static void MarkAIBattleWins(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.AIBattleWins += value;
    }

    public static void MarkAIBattleCount(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.AIBattleCount += value;
    }

    public static void MarkShot(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.shots += value;
    }

    public static void MarkPottedBalls(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.pottedBalls += value;
    }

    public static void MarkMaxCombo(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        if (ConstantData.gameStatisticsData.maxCombo < value)
            ConstantData.gameStatisticsData.maxCombo = value;
    }

    public static void MarkQuickFireHighScore(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        if (ConstantData.gameStatisticsData.quickFireHighScore < value)
            ConstantData.gameStatisticsData.quickFireHighScore = value;
    }

    public static void MarkQuickFireHighRound(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        if (ConstantData.gameStatisticsData.quickFireHighRound < value)
            ConstantData.gameStatisticsData.quickFireHighRound = value;
    }

    public static void MarkMissionStars(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.missionStars += value;
    }

    public static void MarkCueballHitNoBall(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.cueballHitNoBallCount += value;
    }

    public static void MarkCueballPotted(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.cueballPottedCount += value;
    }

    public static void MarkCueballHitRail(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.cueballHitRailCount += value;
    }

    public static void MarkCueballHitBall(int value)
    {
        if (ConstantData.gameStatisticsData == null)
            return;
        ConstantData.gameStatisticsData.cueballHitBallCount += value;
    }
}
