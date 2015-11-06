using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


public class Testttt : MonoBehaviour
{
    [SerializeField]
    RectTransform test1;




    void OnGUI()
    {
        if(GUILayout.Button("fdsafdsafdsa"))
        {
            Debug.Log(Random.Range(0, 2));
        }
    }



    public void Test()
    {
        Debug.Log("Click");
    }
}