using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public static PoolRulesBase Rules;

    public int m_TargetFream = 300;

    void Awake()
    {
        Application.targetFrameRate = m_TargetFream;
    }

    void Start()
    {
        if(Testttt.num == 1)
        {
            BaseUIController.topMenu = SupportTools.AddChild<TopMenuQuickFire>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuQuickFire");
            Rules = gameObject.AddComponent<PoolRulesQuickFire>();
        }
        if(Testttt.num == 2)
        {
            BaseUIController.topMenu = SupportTools.AddChild<TopMenuStandard>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuStandard");
            Rules = gameObject.AddComponent<PoolRulesStandard>();
        }
        else
        {
            BaseUIController.topMenu = SupportTools.AddChild<TopMenuQuickFire>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuQuickFire");
            Rules = gameObject.AddComponent<PoolRulesQuickFire>();
        }
        Rules.SetPlayers(BaseUIController.topMenu.GetPlayers());
        Rules.Initialize();
    }
}
