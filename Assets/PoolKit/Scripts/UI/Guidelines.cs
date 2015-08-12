using UnityEngine;
using System.Collections;

public class Guidelines : MonoBehaviour {
    private static Guidelines m_Instance;

    [SerializeField]
    private RectTransform m_GuideBall;
    [SerializeField]
    private RectTransform m_GuideLine;
    [SerializeField]
    private RectTransform m_WhiteBallDir;
    [SerializeField]
    private RectTransform m_HitBallDir;
    [SerializeField]
    void Awake()
    {
        if(m_Instance)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        gameObject.SetActive(false);
    }

    private Vector3 World2UI(Vector3 v)
    {
        Vector2 sp = Camera.main.WorldToScreenPoint(v);
        return BaseUIController.GetUICamera().ScreenToWorldPoint(sp);
    }

    private void _GuidePointerAt(Vector3 hitPoint, Transform hitTrans, Vector3 hitNormal)
    {
        gameObject.SetActive(true);

        Vector3 shp = World2UI(hitPoint),//hit point
            shtp = World2UI(hitTrans.position),//hitball position
            shwp = World2UI(hitPoint + hitNormal.normalized * PoolKit.WhiteBall.GetRadius()),//whiteball position when hit
            swp = World2UI(PoolKit.WhiteBall.GetPosition());//whiteball current position
        //ball
        m_GuideBall.position = shwp;
        //line
        m_GuideLine.position = shwp;
        Vector2 v1 = m_GuideBall.parent.worldToLocalMatrix.MultiplyPoint(shwp);
        Vector2 v2 = m_GuideBall.parent.worldToLocalMatrix.MultiplyPoint(swp);
        m_GuideLine.sizeDelta = new Vector2(Vector2.Distance(v1, v2), m_GuideLine.rect.height);
        m_GuideLine.right = (swp - shwp).normalized;
        if(hitTrans.name.Contains("Ball"))
        {
            m_HitBallDir.gameObject.SetActive(true);
            m_WhiteBallDir.gameObject.SetActive(true);
            //hitballdir
            m_HitBallDir.position = shtp;
            Vector3 v3 = (shtp - shp).normalized;
            float d = Vector2.Dot(-m_GuideLine.right, v3);
            float d40 = d * 40;
            v3 = v3 * d;
            m_HitBallDir.right = v3;
            m_HitBallDir.sizeDelta = new Vector2(d40, m_HitBallDir.rect.height);
            //whiteballdir
            m_WhiteBallDir.position = shwp;
            m_WhiteBallDir.right = -m_GuideLine.right - v3;
            m_WhiteBallDir.sizeDelta = new Vector2(40 - d40, m_WhiteBallDir.rect.height);
        }
        else
        {
            m_HitBallDir.gameObject.SetActive(false);
            m_WhiteBallDir.gameObject.SetActive(false);
        }
    }

    public static void GuidePointerAt(Vector3 hitPoint, Transform hitTrans, Vector3 hitNormal)
    {
        m_Instance._GuidePointerAt(hitPoint, hitTrans, hitNormal);
    }

    private void _HideAllObjects()
    {
        gameObject.SetActive(false);
    }

    public static void HideAllObjects()
    {
        m_Instance._HideAllObjects();
    }

    public static Vector3 GetPointerDirection()
    {
        return -m_Instance.m_GuideLine.right;
    }
}
