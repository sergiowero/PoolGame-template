﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static PoolRulesBase Rules;

    public static Canvas CurrentUIRoot = null;

    public static GameType GType = GameType.None;

    public int m_TargetFream = 300;

#if UNITY_EDITOR
    [SerializeField]
    public GameType m_DebugGameType;
#endif//#if UNITY_EDITOR 13
    [SerializeField]
    private LevelData m_DebugLevelData;

    void Awake()
    {
        Application.targetFrameRate = m_TargetFream;
#if UNITY_EDITOR
        if (m_DebugGameType != GameType.None)
        {
            GType = m_DebugGameType;
            LevelDataIndex.CurrentLevel = m_DebugLevelData;
        }
#endif //#if UNITY_EDITOR 20
        PoolRulesBase.onGameOver += OnGameOver;
    }

    void OnDestroy()
    {
        PoolRulesBase.onGameOver -= OnGameOver;
    }

    void Start()
    {
        switch (GType)
        {
            case GameType.QuickFire:
                BaseUIController.topMenu = SupportTools.AddChild<TopMenuQuickFire>(BaseUIController.TopMenuRoot.gameObject, "UI/BattleScene/TopMenuQuickFire");
                Rules = gameObject.AddComponent<PoolRulesQuickFire>();
                break;
            case GameType.Standard:
                BaseUIController.topMenu = SupportTools.AddChild<TopMenuStandard>(BaseUIController.TopMenuRoot.gameObject, "UI/BattleScene/TopMenuStandard");
                Rules = gameObject.AddComponent<PoolRulesStandard>();
                break;
            case GameType.Mission:
                BaseUIController.topMenu = SupportTools.AddChild<TopMenuMission>(BaseUIController.TopMenuRoot.gameObject, "UI/BattleScene/TopMenuMission");
                Rules = gameObject.AddComponent<PoolRulesMission>();
                break;
            case GameType.AI:
                BaseUIController.topMenu = SupportTools.AddChild<TopMenuStandard>(BaseUIController.TopMenuRoot.gameObject, "UI/BattleScene/TopMenuMultipleAI");
                Rules = gameObject.AddComponent<PoolRulesMultiple>();
                break;
        }
        Rules.SetPlayers(BaseUIController.topMenu.GetPlayers());
    }

    private void OnGameOver(IPlayer player)
    {
        if (GType == GameType.AI)
        {
            GameStatistics.MarkAIBattleCount(1);
            if (!(player is AIPlayer))
                GameStatistics.MarkAIBattleWins(1);
        }
        Rules.State = GlobalState.GAMEOVER;
    }
}
