using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PoolHuman = PoolKit.HumanPlayer;

public class DragArea : MonoBehaviour {

    private Vector2 m_poolBallsp;

    private Vector2 m_LastTouchsp;
	void Start () {
	
	}

    public void OnAreaDrag(BaseEventData data)
    {
        PointerEventData ped = data as PointerEventData;
        if (m_LastTouchsp == default(Vector2))
        {
            m_LastTouchsp = ped.position;
            return;
        }

        Vector3 v1 = m_LastTouchsp - m_poolBallsp,
            v2 = ped.position - m_poolBallsp;
        float ag = Mathf.Acos(Vector3.Dot(v1.normalized, v2.normalized)) * Mathf.Rad2Deg;
        if (float.IsNaN(ag) || float.IsInfinity(ag))
            return;
        float fag = ag * 100 / Mathf.Max(v2.magnitude,100);
        float z = Vector3.Cross(v1, v2).z;
        if (z < 0) PoolHuman.CueRotate(fag);
        else PoolHuman.CueRotate(-fag);
        m_LastTouchsp = ped.position;
    }

    public void OnAreaDragBegin(BaseEventData data)
    {
        m_poolBallsp = Camera.main.WorldToScreenPoint(PoolKit.WhiteBall.GetPosition());
        m_LastTouchsp = default(Vector2);
    }
}
