using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Testttt : MonoBehaviour
{
    public static int num = 0;

    [SerializeField]
    public float nummm = 0;

    void OnGUI()
    {
        if(GUILayout.Button("Get Value!"))
        {
            Debug.Log("Key : " + nummm + " Value : " + Mathf.RoundToInt(nummm));
        }
    }

    void Start()
    {
    }

    public void LoadQuickFire()
    {
        num = 1;
        LoadScene();
    }

    public void LoadStandard()
    {
        num = 2;
        LoadScene();
    }

    private void LoadScene()
    {
        Application.LoadLevel(1);
    }
}
