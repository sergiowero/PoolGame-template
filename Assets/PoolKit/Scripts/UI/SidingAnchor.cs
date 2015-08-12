using UnityEngine;
using System.Collections;

public class SidingAnchor : MonoBehaviour {

    public System.Action OnMovingDown;

    [Range(0, 1)]
    [SerializeField]
    private float m_Time = .2f;
    private float m_CurTime;

    [SerializeField]
    private RectTransform m_AnchorO;
    [SerializeField]
    private RectTransform m_AnchorT;

    private Vector3 m_TargetPosition;
    private Vector3 m_LastPosition;

    public void SetTargetPosition(Vector3 v)
    {
        m_TargetPosition = v;
        m_LastPosition = m_AnchorO.localPosition;
        m_CurTime = 0;
        enabled = true;
    }

	void Start () 
    {
        enabled = false;
        m_TargetPosition = Vector3.zero;
	}
	
	void Update () 
    {
        m_CurTime += Time.deltaTime;
        float percentage = m_CurTime / m_Time;
        m_AnchorO.localPosition = Vector3.Lerp(m_LastPosition, m_TargetPosition, percentage);
        m_AnchorT.localPosition = m_AnchorO.localPosition;
        if (m_CurTime > m_Time)
        {
            if (OnMovingDown != null && m_TargetPosition != Vector3.zero)
                OnMovingDown();
            enabled = false;
        }
	}
}
