using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class FireSlider : MonoBehaviour {
    public static Delegate1Args<float> OnSliderValueChange;
    public static Delegate0Args OnSliderRelease;

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
        float percentage = p.y / -m_MaxPower;
        DiscreteValue(ref percentage);
        //float power = Mathf.Lerp(0, 1, percentage);
        //Debug.Log("power : " + power);
        m_Background.color = Color.Lerp(m_MinColor, m_MaxColor, percentage);

        if(OnSliderValueChange != null && m_LastValue != percentage)
        {
            OnSliderValueChange(percentage);
        }
        m_LastValue = percentage;
    }

    public void OnFireSliderDragEnd(BaseEventData data)
    {
        if (m_LastValue > .02f && OnSliderRelease != null)
        {
            OnSliderRelease();
            m_LastValue = 0;
        }
        iTween.MoveTo(m_SliderButton.gameObject, iTween.Hash("y", 0, "time", m_FadeTime, "islocal", true));
        iTween.ColorTo(m_Background.gameObject, iTween.Hash("color",m_MinColor, "time", m_FadeTime, "includechildren", false));
    }

    private void DiscreteValue(ref float value)
    {
        value = Mathf.Pow(value, 1.1f);
    }
}
