using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextID : MonoBehaviour 
{
    [SerializeField]
    private int ID;

    void Start()
    {
        GetComponent<Text>().text = HOLocalizationConfiguration.GetValue(ID);
    }
}
