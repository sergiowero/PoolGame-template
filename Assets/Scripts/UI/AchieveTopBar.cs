using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchieveTopBar : MonoBehaviour {

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
        AchieveTopBar topBar = SupportTools.AddChild<AchieveTopBar>(GameManager.CurrentUIRoot.gameObject, "UI/AchieveTopBar");
        topBar._Show(text);
    }

    public static void Show(int id)
    {
        Show(HOLocalizationConfiguration.GetValue(id));
    }
}
