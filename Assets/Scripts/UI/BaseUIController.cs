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
    private OperateArea m_CueOperateArea;
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
    [SerializeField]
    private SettlementUIManager m_Settlement;
    [SerializeField]
    private TextTips m_Tips;
    [SerializeField]
    private GameObject m_GlobalMask;
    [SerializeField]
    private BallInHand m_Hand;
    [SerializeField]
    private GameObject m_FadeMask;
    [SerializeField]
    private GameObject m_RiseMask;
    [SerializeField]
    private Animator m_Menu;


    [SerializeField]
    public Image targetPocketImage;
    [SerializeField]
    public Image targetBallImage;
    [SerializeField]
    public Image hitPointImage;
    [SerializeField]
    public Image newHitPointImage;
    [SerializeField]
    public bool debugAI;

    public static BaseUIController Instance { get { return m_Instance; } }


    public static TextArea text { get { return m_Instance.m_Text; } }
    public static OperateArea cueOperateArea { get { return m_Instance.m_CueOperateArea; } }
    public static FireSlider fireSlider { get { return m_Instance.m_FireSlider; } }
    public static CueAndGuidelines cueAndLines { get { return m_Instance.m_CueAndLines; } }
    public static Siding siding { get { return m_Instance.m_Siding; } }
    public static  Transform TopMenuRoot { get { return m_Instance.m_TopMenuRoot; } }
    public static SettlementUIManager MSettlement { get { return m_Instance.m_Settlement; } }
    public static bool GlobalMask 
    {
        get
        {
            return m_Instance.m_GlobalMask.activeInHierarchy;
        }
        set
        {
            cueAndLines.SetFade(value);
            m_Instance.m_GlobalMask.SetActive(value);
        }
    }


    public static BallInHand hand { get { return m_Instance.m_Hand; } }
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
        //hand.gameObject.SetActive(false);
        m_FadeMask.SetActive(true);
        PoolRulesBase.onFireBall += OnFireBall;
        PoolRulesBase.onNewTurn += OnStartRound;

        if(!debugAI)
        {
            targetPocketImage.gameObject.SetActive(false); 
            targetBallImage.gameObject.SetActive(false);
            hitPointImage.gameObject.SetActive(false);
            newHitPointImage.gameObject.SetActive(false);
        }
    }


    void OnDestroy()
    {
        PoolRulesBase.onFireBall -= OnFireBall;
        PoolRulesBase.onNewTurn -= OnStartRound;
        m_Instance = null;
    }

    private void OnFireBall()
    {
        fireSlider.gameObject.SetActive(false);
        GlobalMask = true;
    }

    private void OnStartRound(int playerIndex)
    {
        fireSlider.gameObject.SetActive(true);
        GlobalMask= false;
    }

    public static Camera GetUICamera()
    {
        return m_Instance.m_UICamera;
    }

    public static Transform RootTransform()
    {
        return m_Instance.transform;
    }

    public void Back2MainScene()
    {
        m_RiseMask.GetComponent<AnimationTools>().sceneIndex = 0;
        m_RiseMask.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        m_RiseMask.GetComponent<AnimationTools>().sceneIndex = 1;
        m_RiseMask.gameObject.SetActive(true);
    }

    public static void GenerateTips(string text, Color c, Vector3 position, bool stationary = false)
    {
        TextTips tips = SupportTools.AddChild<TextTips>(m_Instance.gameObject, m_Instance.m_Tips.gameObject);
        tips.transform.position = position;
        tips.SetText(text, c, stationary);
    }

    public static void GenerateTips(string text, Color c, bool stationary = false)
    {
        GenerateTips(text, c, Vector3.zero, stationary);
    }

    public static void GenerateTips(string text)
    {
        GenerateTips(text, Color.white);
    }
}
