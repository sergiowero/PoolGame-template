using UnityEngine;
using System.Collections;

public class Testttt : MonoBehaviour
{
    public static int num = 0;

    public GUIStyle style;

    public Texture2D texture;

    public Color c;

    void OnGUI()
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
