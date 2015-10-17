using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static PoolRulesBase Rules;

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
            ConstantData.GType = m_DebugGameType;
            LevelDataIndex.CurrentLevel = m_DebugLevelData;
        }
#endif //#if UNITY_EDITOR 20
    }

    void Start()
    {
        switch (ConstantData.GType)
        {
            case GameType.QuickFire:
                BaseUIController.topMenu = SupportTools.AddChild<TopMenuQuickFire>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuQuickFire");
                Rules = gameObject.AddComponent<PoolRulesQuickFire>();
                break;
            case GameType.Standard:
                BaseUIController.topMenu = SupportTools.AddChild<TopMenuStandard>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuStandard");
                Rules = gameObject.AddComponent<PoolRulesStandard>();
                break;
            case GameType.Mission:
                BaseUIController.topMenu = SupportTools.AddChild<TopMenuMission>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuMission");
                Rules = gameObject.AddComponent<PoolRulesMission>();
                break;
        }
        Rules.SetPlayers(BaseUIController.topMenu.GetPlayers());
    }
}
