using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class FireSlider : MonoBehaviour {
    public static System.Action<float> OnSliderValueChange;
    public static System.Action OnSliderRelease;

    [SerializeField]
    private Image m_Background;
    [SerializeField]
    private Image m_SliderButton;
    private RectTransform m_SliderTrans;
    private float m_MaxPower;

    [SerializeField]
    private Color m_MinColor;
    [SerializeField]
    private Color m_MaxColor;

    private float m_LastValue;

    private float m_FadeTime = .2f;

    void Awake()
    {
        m_SliderTrans = m_SliderButton.rectTransform;
        m_MaxPower = m_SliderTrans.rect.height - 10;
    }

	void Start () 
    {
        m_Background.color = m_MinColor;
	}

    public void OnFireSliderDrag(BaseEventData data)
    {
        PointerEventData ped = data as PointerEventData;
        Vector3 delta = ped.delta;
        Vector3 pp = BaseUIController.GetUICamera().WorldToScreenPoint(m_SliderButton.transform.position);
        pp.y += delta.y;
        m_SliderButton.transform.position = BaseUIController.GetUICamera().ScreenToWorldPoint(pp);
        Vector3 p = m_SliderTrans.localPosition;
        p.y = Mathf.Clamp(p.y, -m_MaxPower, 0);//y position is negative
        m_SliderTrans.localPosition = p;
        float power = p.y / -m_MaxPower;
        m_Background.color = Color.Lerp(m_MinColor, m_MaxColor, power);

        if(OnSliderValueChange != null && m_LastValue != power)
        {
            OnSliderValueChange(power);
        }
        m_LastValue = power;
    }

    public void OnFireSliderDragEnd(BaseEventData data)
    {
        if (m_LastValue > .1f && OnSliderRelease != null)
        {
            OnSliderRelease();
        }
        iTween.MoveTo(m_SliderButton.gameObject, iTween.Hash("y", 0, "time", m_FadeTime, "islocal", true));
        iTween.ColorTo(m_Background.gameObject, iTween.Hash("color",m_MinColor, "time", m_FadeTime, "includechildren", false));
    }
}
