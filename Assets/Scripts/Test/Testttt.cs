using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class Testttt : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_Ri;

    [SerializeField]
    private Collider m_C;

    void OnGUI()
    {
        if(GUILayout.Button("Test ray case"))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            m_C.enabled = false;
            if(Physics.Raycast(ray))
            {
                Debug.Log("hit");
            }
            m_C.enabled = true;
        }
    }
}