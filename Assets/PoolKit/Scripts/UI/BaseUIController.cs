using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BaseUIController : MonoBehaviour {

    private static BaseUIController m_Instance;

    [SerializeField]
    private RectTransform m_SideObject;
    //private Animator m_SideObjectAnimator;
    [SerializeField]
    private RectTransform m_SideButton;
    [SerializeField]
    private RectTransform m_Anchor;
    [SerializeField]
    private RectTransform m_AnchorTemplate;
    [SerializeField]
    private Camera m_UICamera;

    [SerializeField]
    private float m_FadeTime = .5f;

    void Awake()
    {
        if (m_Instance)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        Button button = m_SideButton.GetComponent<Button>();
        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnSideButtonClick);
        }
        //m_SideObjectAnimator = m_SideObject.transform.parent.GetComponent<Animator>();
    }

	void Start () {
        Color c = m_SideObject.GetComponent<Image>().color;
        c.a = 0;
        m_SideObject.GetComponent<Image>().color = c;
        m_SideObject.gameObject.SetActive(false);
	}
	
	void Update () {
	
	}

    public void OnSideButtonClick()
    {
        Debug.Log("Button click");
        if(PoolKit.WhiteBall.CueBallSiding)
        {
            iTween.FadeTo(m_SideObject.gameObject, 0, m_FadeTime);
            iTween.FadeTo(m_Anchor.gameObject, 0, m_FadeTime);
            iTween.ScaleTo(m_SideObject.gameObject, Vector3.one, m_FadeTime);
            iTween.MoveTo(m_SideObject.gameObject, iTween.Hash("position", Vector3.zero, "time", m_FadeTime, "oncomplete", "OnFadeoffDown", "oncompletetarget", gameObject, "islocal", true));
        }
        else
        {
            m_SideObject.gameObject.SetActive(true);
            iTween.FadeTo(m_SideObject.gameObject, 1, m_FadeTime);
            iTween.FadeTo(m_Anchor.gameObject, 1, m_FadeTime);
            iTween.MoveTo(m_SideObject.gameObject, iTween.Hash("position", Vector3.zero, "time", m_FadeTime, "islocal", false));
            iTween.ScaleTo(m_SideObject.gameObject, Vector3.one * 2, m_FadeTime);
        }
        PoolKit.WhiteBall.CueBallSiding = !PoolKit.WhiteBall.CueBallSiding;
        //PoolKit.WhiteBall.CueBallSiding = !m_SideObjectAnimator.GetBool("CueSideIconShow");
        //m_SideObjectAnimator.SetBool("CueSideIconShow", PoolKit.WhiteBall.CueBallSiding);
    }

    private void OnFadeoffDown()
    {
        m_SideObject.gameObject.SetActive(false);
    }

    public void OnAnchorDrag(BaseEventData data)
    {
        PointerEventData ped = data as PointerEventData;
        Vector2 vc = m_UICamera.ScreenToWorldPoint(ped.position);
        Vector2 cood, newCood = m_SideObject.worldToLocalMatrix.MultiplyPoint(vc);
        float mag = newCood.magnitude;
        float r = m_SideObject.rect.width * .5f - m_Anchor.rect.width * .5f;
        if (mag > r)
        {
            float ang = Mathf.Acos(Mathf.Abs(newCood.x)/mag);
            cood.x = r * Mathf.Cos(ang) * (newCood.x >= 0 ? 1 : -1);
            cood.y = r * Mathf.Sin(ang) * (newCood.y >= 0 ? 1 : -1);
        }
        else
        {
            cood = newCood;
        }
        m_Anchor.localPosition = cood;
        m_AnchorTemplate.localPosition = cood;
        PoolKit.PoolCue.Siding(cood);
    }

    public static Vector2 TableCoord2ScreenCoord(Vector3 c)
    {
        return Camera.main.WorldToScreenPoint(c);
    }

    public static Camera GetUICamera()
    {
        return m_Instance.m_UICamera;
    }
}
