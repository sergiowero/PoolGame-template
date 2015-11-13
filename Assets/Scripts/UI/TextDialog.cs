using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextDialog : MonoBehaviour 
{
    private Animator m_Animator;

    [SerializeField]
    private Text m_Text;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetTrigger("Show");
    }


    private void _Show(string text)
    {
        m_Text.text = text;
    }

    public static void Show(string text)
    {
        TextDialog dialog = SupportTools.AddChild<TextDialog>(GameManager.CurrentUIRoot.gameObject, "UI/TextDialog");
        dialog._Show(text);
    }

    public static void Show(int id)
    {
        Show(HOLocalizationConfiguration.GetValue(id));
    }
}
