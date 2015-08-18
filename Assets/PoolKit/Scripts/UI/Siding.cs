using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Siding : MonoBehaviour 
{
    private static Siding m_Instance = null;

    [SerializeField]
    private RectTransform m_SideballT;//sideball toggle ,just for show
    [SerializeField]
    private RectTransform m_SideballO;//sideball operator
    [SerializeField]
    private RectTransform m_AnchorT;//anchor to sideball toggle
    [SerializeField]
    private RectTransform m_AnchorO; //anchor to sideball operator
    [SerializeField]
    private RectTransform m_BlackMask; //showing when sideballO is showing, for operator sideballO
    [SerializeField]
    private RectTransform m_MaskDeactiveObject; //disable mask touch component
    [SerializeField]
    private SidingAnchor m_SidingAnchor; 

    private bool m_SidingDrag = false;

    private bool m_TouchWhiteArea = false;

    private Vector3 m_SideballO_Orip;

    private Vector2 m_LastCoord;


    private const float m_FadeTime = .5f;

    void Awake()
    {
        if(!m_Instance)
        {
            m_Instance = this;
            m_SideballO_Orip = m_SideballO.localPosition;
            Color c = m_SideballO.GetComponent<Image>().color;
            c.a = 0;
            m_SideballO.GetComponent<Image>().color = c;
            m_SideballO.gameObject.SetActive(false);
            m_BlackMask.gameObject.SetActive(false);
            m_SidingAnchor.OnMovingDown = AnchorMoveDown;
        }
        else
        {
            Debug.LogError("there two siding object in the scene!");
        }
    }

    public void OnToggleClick()
    {
        if (WhiteBall.CueBallSiding)
        {
            m_MaskDeactiveObject.gameObject.SetActive(true);
            iTween.FadeTo(m_SideballO.gameObject, 0, m_FadeTime);
            iTween.FadeTo(m_AnchorO.gameObject, 0, m_FadeTime);
            iTween.FadeTo(m_BlackMask.gameObject, 0, m_FadeTime * .5f);
            iTween.ScaleTo(m_SideballO.gameObject, Vector3.one, m_FadeTime);
            iTween.MoveTo(m_SideballO.gameObject, iTween.Hash("position", m_SideballO_Orip, "time", m_FadeTime, "oncomplete", "OnFadeoffDown", "oncompletetarget", gameObject, "islocal", true));
        }
        else
        {
            m_SideballO.gameObject.SetActive(true);
            m_BlackMask.gameObject.SetActive(true);
            m_MaskDeactiveObject.gameObject.SetActive(false);
            iTween.FadeTo(m_BlackMask.gameObject, 0.4f, m_FadeTime * .5f);
            iTween.FadeTo(m_SideballO.gameObject, 1, m_FadeTime);
            iTween.FadeTo(m_AnchorO.gameObject, 1, m_FadeTime);
            iTween.MoveTo(m_SideballO.gameObject, iTween.Hash("position", Vector3.zero, "time", m_FadeTime, "islocal", false));
            iTween.ScaleTo(m_SideballO.gameObject, Vector3.one * 2, m_FadeTime);
        }
        WhiteBall.CueBallSiding = !WhiteBall.CueBallSiding;
    }

    private void OnFadeoffDown()
    {
        m_SideballO.gameObject.SetActive(false);
        m_BlackMask.gameObject.SetActive(false);
        m_MaskDeactiveObject.gameObject.SetActive(false);
    }

    private void CalculateAnchorOffset(Vector3 position)
    {
        Vector2 vc = BaseUIController.GetUICamera().ScreenToWorldPoint(position);
        Vector2 coord, newCoord = m_SideballO.worldToLocalMatrix.MultiplyPoint(vc);
        float mag = newCoord.magnitude;
        float r = m_SideballO.rect.width * .5f - m_AnchorO.rect.width * .5f;
        if (mag > r)
        {
            float ang = Mathf.Acos(Mathf.Abs(newCoord.x) / mag);
            coord.x = r * Mathf.Cos(ang) * (newCoord.x >= 0 ? 1 : -1);
            coord.y = r * Mathf.Sin(ang) * (newCoord.y >= 0 ? 1 : -1);
        }
        else
        {
            coord = newCoord;
            m_TouchWhiteArea = true;
        }
        if (m_TouchWhiteArea)
            _SidingAnchorOffset(coord);
    }

    public void BlackMaskDragBegin(BaseEventData data)
    {
        m_SidingDrag = true;
    }

    public void BlackMaskDragEnd(BaseEventData data)
    {
        m_SidingDrag = false;
        if(!m_TouchWhiteArea)
        {
            OnToggleClick();
        }
        else
        {
            PointerEventData ped = data as PointerEventData;
            CalculateAnchorOffset(ped.position);
        }
    }

    public void BlackMaskDrag(BaseEventData data)
    {
        PointerEventData ped = data as PointerEventData;
        CalculateAnchorOffset(ped.position);
    }

    private void AnchorMoveDown()
    {
        Pools.Cue.Siding(m_AnchorO.localPosition);
        if(!m_SidingDrag)
        {
            OnToggleClick();
        }
    }

    private void _SidingAnchorOffset(Vector2 offset)
    {
        m_SidingAnchor.SetTargetPosition(offset);
    }

    public static void ResetAnchorOffset()
    {
        m_Instance._SidingAnchorOffset(Vector2.zero);
        Pools.Cue.ResetSideOffset();
    }
}
