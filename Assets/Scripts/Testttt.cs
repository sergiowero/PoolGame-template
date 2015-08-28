using UnityEngine;
using System.Collections;

public class Testttt : MonoBehaviour
{
    public static int num = 0;

    void OnGUI()
    {
        if(GUI.Button(new Rect(100,100, 300, 300), "player n player"))
        {
            num = 1;
            Application.LoadLevel("PoolGame");
        }
        if(GUI.Button(new Rect(100,400,300,300), "Quick fire"))
        {
            num = 2;
            Application.LoadLevel("PoolGame");
        }
    }

}
