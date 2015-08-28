using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public static PoolRulesBase Rules;

    void Start()
    {
        if(Testttt.num == 1)
        {
            BaseUIController.topMenu = SupportTools.AddChild<TopMenuStandard>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuStandard");
            Rules = gameObject.AddComponent<PoolRulesStandard>();
            Rules.SetPlayers(BaseUIController.topMenu.GetPlayers());
            Rules.Initialize();
        }
        else if (Testttt.num == 2)
        {
            BaseUIController.topMenu = SupportTools.AddChild<TopMenuQuickFire>(BaseUIController.TopMenuRoot.gameObject, "TestRes/TopMenuQuickFire");
            Rules = gameObject.AddComponent<PoolRulesQuickFire>();
            Rules.SetPlayers(BaseUIController.topMenu.GetPlayers());
            Rules.Initialize();
        }
    }
}
