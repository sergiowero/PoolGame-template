using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


public class Testttt : MonoBehaviour
{
    void OnGUI()
    {
        if(GUILayout.Button("Generate tips"))
        {
            BaseUIController.GenerateTips("100",Color.yellow,MathTools.World2UI(Pools.CueBall.GetPosition()));
        }
    }
}