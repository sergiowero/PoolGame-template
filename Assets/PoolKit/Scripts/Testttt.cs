using UnityEngine;
using System.Collections;

public class Testttt : MonoBehaviour {
    string filePath;
    [SerializeField]
    GUIStyle style;
    float f = 0;
    void Start()
    {
        filePath = Application.dataPath + "/AssetData/PoolEnvironmentData/PoolPhysical.asset";
    }


    void OnGUI()
    {
        if (GUILayout.Button("Read"))
        {
            Debug.Log(StreamTools.DeserializeObject<PoolDataAsset>(filePath));
        }
        GUI.Box(new Rect(0, 0, 100, 100), "Test box", style);
        f =  GUI.HorizontalSlider(new Rect(180, 5, 100, 10), f, 0, 1);
    }
}
