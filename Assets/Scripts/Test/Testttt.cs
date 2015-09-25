using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class Testttt : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.Label(Camera.main.WorldToScreenPoint(transform.position).ToString());
    }
}