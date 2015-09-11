using UnityEngine;
using System.Collections;

public class CueAndGuidelines : MonoBehaviour {
    private Vector3 m_Pivot;

    [SerializeField]
    private RectTransform m_GuideBall;
    [SerializeField]
    private RectTransform m_GuideLine;
    [SerializeField]
    private RectTransform m_WhiteBallDir;
    [SerializeField]
    private RectTransform m_HitBallDir;
    [SerializeField]
    private RectTransform m_Cue;
    void Awake()
    {
        gameObject.SetActive(false);
        m_GuideLine.right = Vector3.down;
    }


    public void AdjustingCue(float offset)
    {
        m_Cue.position = m_Pivot - m_Cue.right * offset * ConstantData.AdjustingCueScalar;
    }

    public void GuidePointerAt(Vector3 hitPoint, Transform hitTrans, Vector3 hitNormal)
    {
        //gameObject.SetActive(true);

        Vector3 shp = MathTools.World2UI(hitPoint),//hit point
            shtp = MathTools.World2UI(hitTrans.position),//hitball position
            shwp = MathTools.World2UI(hitPoint + hitNormal.normalized * Pools.CueBall.GetRadius()),//whiteball position when hit
            swp = MathTools.World2UI(Pools.CueBall.GetPosition());//whiteball current position
        //pivot
        m_Pivot = swp;
        //ball
        m_GuideBall.position = shwp;
        //line
        m_GuideLine.position = shwp;
        Vector2 v1 = m_GuideBall.parent.worldToLocalMatrix.MultiplyPoint(shwp);
        Vector2 v2 = m_GuideBall.parent.worldToLocalMatrix.MultiplyPoint(swp);
        m_GuideLine.sizeDelta = new Vector2(Vector2.Distance(v1, v2), m_GuideLine.rect.height);
        m_GuideLine.right = (swp - shwp).normalized;
        //cue
        m_Cue.position = MathTools.World2UI(Pools.CueBall.GetPosition());
        m_Cue.right = -m_GuideLine.right;
        if(hitTrans.name.Contains("Ball"))
        {
            m_HitBallDir.gameObject.SetActive(true);
            m_WhiteBallDir.gameObject.SetActive(true);
            //hitballdir
            m_HitBallDir.position = shtp;
            Vector3 v3 = (shtp - shp).normalized;
            float d = Vector2.Dot(-m_GuideLine.right, v3);
            float d40 = d * ConstantData.GuidelineLength;
            v3 = v3 * d;
            m_HitBallDir.right = v3;
            m_HitBallDir.sizeDelta = new Vector2(d40, m_HitBallDir.rect.height);
            //whiteballdir
            m_WhiteBallDir.position = shwp;
            m_WhiteBallDir.right = -m_GuideLine.right - v3;
            m_WhiteBallDir.sizeDelta = new Vector2(ConstantData.GuidelineLength - d40, m_WhiteBallDir.rect.height);
        }
        else
        {
            m_HitBallDir.gameObject.SetActive(false);
            m_WhiteBallDir.gameObject.SetActive(false);
        }
    }

    public Vector3 GetPointerDirection()
    {
        return -m_GuideLine.right;
    }
}
