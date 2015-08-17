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
    private Guidelines m_Guidelines;
    [SerializeField]
    private Siding m_Siding;

    public Text foulText { get { return m_FoulText; } }
    public CueOperateArea cueOperateArea { get { return m_CueOperateArea; } }
    public FireSlider fireSlider { get { return m_FireSlider; } }
    public Guidelines guidelines { get { return m_Guidelines; } }
    public Siding siding { get { return m_Siding; } }

    void Awake()
    {
        if (m_Instance)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        m_FoulText.gameObject.SetActive(false);
        PoolKit.BaseGameManager.onFireBall += OnFireBall;
        PoolKit.BaseGameManager.onPlayerTurn += OnStartRound;
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

    public static void ShowFoulText()
    {
        m_Instance.m_FoulText.gameObject.SetActive(true);
    }
}
