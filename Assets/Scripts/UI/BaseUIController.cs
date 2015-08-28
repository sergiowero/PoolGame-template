using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BaseUIController : MonoBehaviour {

    private static BaseUIController m_Instance = null;

    [SerializeField]
    private Camera m_UICamera;

    [SerializeField]
    private TextArea m_Text;
    [SerializeField]
    private CueOperateArea m_CueOperateArea;
    [SerializeField]
    private FireSlider m_FireSlider;
    [SerializeField]
    private CueAndGuidelines m_CueAndLines;
    [SerializeField]
    private Siding m_Siding;
    [SerializeField]
    private Transform m_TopMenuRoot;
    [SerializeField]
    [HideInInspector]
    private TopMenu m_TopMenu;

    public static TextArea text { get { return m_Instance.m_Text; } }
    public static CueOperateArea cueOperateArea { get { return m_Instance.m_CueOperateArea; } }
    public static FireSlider fireSlider { get { return m_Instance.m_FireSlider; } }
    public static CueAndGuidelines cueAndLines { get { return m_Instance.m_CueAndLines; } }
    public static Siding siding { get { return m_Instance.m_Siding; } }
    public static  Transform TopMenuRoot { get { return m_Instance.m_TopMenuRoot; } }
    public static TopMenu topMenu 
    {
        set 
        { 
            m_Instance.m_TopMenu = value;
        }
        get { return m_Instance.m_TopMenu; } 
    }

    void Awake()
    {
        if (m_Instance)
        {
            Debug.LogError("two " + gameObject.name + " in the scene. check the code");
        }
        m_Instance = this;
        m_Text.gameObject.SetActive(false);
        PoolRulesBase.onFireBall += OnFireBall;
        PoolRulesBase.onNewTurn += OnStartRound;
    }

    void OnDestroy()
    {
        PoolRulesBase.onFireBall -= OnFireBall;
        PoolRulesBase.onNewTurn -= OnStartRound;
        m_Instance = null;
    }

    private void OnFireBall()
    {
        siding.gameObject.SetActive(false);
        fireSlider.gameObject.SetActive(false);
        cueOperateArea.gameObject.SetActive(false);
    }

    private void OnStartRound(int playerIndex)
    {
        cueOperateArea.gameObject.SetActive(true);
        fireSlider.gameObject.SetActive(true);
        siding.gameObject.SetActive(true);
    }

    public static Camera GetUICamera()
    {
        return m_Instance.m_UICamera;
    }

    public static Transform RootTransform()
    {
        return m_Instance.transform;
    }
}
