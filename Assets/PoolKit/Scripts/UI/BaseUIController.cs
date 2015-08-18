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
    [SerializeField]
    private CueOperateArea m_CueOperateArea;
    [SerializeField]
    private FireSlider m_FireSlider;
    [SerializeField]
    private CueAndGuidelines m_CueAndLines;
    [SerializeField]
    private Siding m_Siding;

    public static Text foulText { get { return m_Instance.m_FoulText; } }
    public static CueOperateArea cueOperateArea { get { return m_Instance.m_CueOperateArea; } }
    public static FireSlider fireSlider { get { return m_Instance.m_FireSlider; } }
    public static CueAndGuidelines cueAndLines { get { return m_Instance.m_CueAndLines; } }
    public static Siding siding { get { return m_Instance.m_Siding; } }

    void Awake()
    {
        if (m_Instance)
        {
            Debug.LogError("two " + gameObject.name + " in the scene. check the code");
        }
        m_Instance = this;
        m_FoulText.gameObject.SetActive(false);
        BaseGameManager.onFireBall += OnFireBall;
        BaseGameManager.onNewRoundBegin += OnStartRound;
    }

    void OnDestroy()
    {
        BaseGameManager.onFireBall -= OnFireBall;
        BaseGameManager.onNewRoundBegin -= OnStartRound;
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
