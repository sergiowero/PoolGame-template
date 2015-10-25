using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class Testttt : MonoBehaviour
{
    [SerializeField]
    Vector3 m_v;

    void OnGUI()
    {
        if(GUILayout.Button("Test ray case"))
        {
            GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}