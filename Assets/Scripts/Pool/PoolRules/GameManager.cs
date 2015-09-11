using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static PoolRulesBase Rules;

    public int m_TargetFream = 300;

    [SerializeField]
    private GameType m_DebugGameType;

    void Awake()
    {
        Application.targetFrameRate = m_TargetFream;
        if (m_DebugGameType != GameType.None)
        {
            ConstantData.GType = m_DebugGameType;
        }
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
                break;
        }
        Rules.SetPlayers(BaseUIController.topMenu.GetPlayers());
        Rules.Initialize();
    }
}
