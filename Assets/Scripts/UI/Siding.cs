 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Siding : MonoBehaviour 
{
    [SerializeField]
    private RectTransform m_SideballO;//sideball operator
    [SerializeField]
    private RectTransform m_AnchorO; //anchor to sideball operator
    [SerializeField]
    private SidingAnchor m_SidingAnchor;
    [SerializeField]
    private Text m_MasseText;
    [SerializeField]
    private RectTransform m_MasseCue;
    [SerializeField]
    private RectTransform m_MasseIcon;
    [SerializeField]
    private int m_PrevValue;


    private Vector2 m_LastCoord;


    private const float m_FadeTime = .5f;

    public int MasseAngle
    {
        get
        {
            return (int)m_MasseCue.localEulerAngles.z;
        }
        set
        {
            m_MasseCue.localEulerAngles = new Vector3(0, 0, value);
            m_MasseText.text = value.ToString() + "°";
            m_MasseText.transform.rotation = Quaternion.identity;
            Pools.Cue.VerticalRotate(value);
            iTween.RotateTo(m_MasseIcon.gameObject, m_MasseCue.localEulerAngles, .2f);
        }
    }

    void Awake()
    {
        m_SidingAnchor.OnMovingDown = AnchorMoveDown;
        PoolRulesBase.onNewTurn += ResetAnchorOffset;
        m_MasseCue.localEulerAngles = Vector3.zero;
        m_MasseIcon.localEulerAngles = Vector3.zero;
    }

    void OnDestroy()
    {
        m_SidingAnchor.OnMovingDown = null;
        //PoolRulesBase.onFireBall -= ResetAnchorOffset;
        PoolRulesBase.onNewTurn -= ResetAnchorOffset;
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
        }
        _SidingAnchorOffset(coord);
    }

    public void OnSiding(BaseEventData data)
    {
        PointerEventData ped = data as PointerEventData;
        CalculateAnchorOffset(ped.position);
    }

    private void AnchorMoveDown()
    {
        Pools.Cue.Siding(m_SidingAnchor.GetAnchorOffset());
    }

    private void _SidingAnchorOffset(Vector2 offset)
    {
        m_SidingAnchor.SetTargetPosition(offset);
    }

    private void _ResetVerticalDegreeSlider()
    {
        m_MasseCue.localEulerAngles = Vector3.zero;
    }

    private void ResetAnchorOffset(int i)
    {
        _SidingAnchorOffset(Vector2.zero);
        _ResetVerticalDegreeSlider();
        MasseAngle = 0;
        Pools.Cue.Siding(Vector3.zero);
    }

    public void OnCueDrag(BaseEventData data)
    {
        PointerEventData pd = data as PointerEventData;
        Vector3 worldPosition = BaseUIController.GetUICamera().ScreenToWorldPoint(pd.position);
        Vector3 v = (worldPosition - m_MasseCue.position).normalized;
        int angle;
        if (Vector3.Cross(v, Vector3.right).z > 0)
            angle = 0;
        else
            angle = (int)Vector3.Angle(Vector3.right, v);
        angle = Mathf.Clamp(angle, 0, 90);
        if (m_PrevValue != angle)
        {
            m_PrevValue = angle;
            MasseAngle = angle;
        }
    }
}
