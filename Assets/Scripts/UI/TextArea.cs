using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextArea : MonoBehaviour 
{
    [SerializeField]
    private Text m_Text;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public bool Show(string s)
    {
        m_Text.text = s;
        gameObject.SetActive(true);
        return true;
    }
}
