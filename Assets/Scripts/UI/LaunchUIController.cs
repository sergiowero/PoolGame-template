using UnityEngine;
using System.Collections;

public class LaunchUIController : MonoBehaviour
{
    void Awake()
    {
        ConstantData.GType = GameType.None;
    }

    public void LoadQuickFire()
    {
        ConstantData.GType = GameType.QuickFire;
        Application.LoadLevel(1);
    }

    public void LoadStandard()
    {
        ConstantData.GType = GameType.Standard;
        Application.LoadLevel(1);
    }
}
