using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BaseUIController : MonoBehaviour {

    private static BaseUIController m_Instance = null;

    [SerializeField]
    private Camera m_UICamera;

    [SerializeField]
    private Text m_FoulText;

    void Awake()
    {
        if (m_Instance)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        m_FoulText.gameObject.SetActive(false);
    }

    public static Camera GetUICamera()
    {
        return m_Instance.m_UICamera;
    }

    public static Transform RootTransform()
    {
        return m_Instance.transform;
    }

    public static void ShowFoulText()
    {
        m_Instance.m_FoulText.gameObject.SetActive(true);
    }
}
