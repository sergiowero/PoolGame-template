using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PoolHuman = PoolKit.HumanPlayer;

public class CueOperateArea : MonoBehaviour {

    private static CueOperateArea m_Instance = null;

    private Vector2 m_poolBallsp;

    private Vector2 m_LastTouchsp;


    private bool m_Dragging = false;

    void Awake()
    {
        if(m_Instance)
        {
            Debug.Log("two dragarea in the scene");
            return;
        }
        m_Instance = this;
    }


    public void OnAreaDragEnd(BaseEventData data)
    {
        m_Dragging = false;
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
        if (float.IsNaN(ag) || float.IsInfinity(ag) || ag == 0)
            return;
        float fag = ag * 100 / Mathf.Max(v2.magnitude,100);
        float z = Vector3.Cross(v1, v2).z;
        if (z < 0) PoolHuman.CueRotate(fag);
        else PoolHuman.CueRotate(-fag);
        m_LastTouchsp = ped.position;
    }

    public void OnAreaDragBegin(BaseEventData data)
    {
        m_Dragging = true;
        m_poolBallsp = PoolKit.WhiteBall.GetScreenPosition();
        m_LastTouchsp = default(Vector2);
    }

    public void OnAreaPointerClick(BaseEventData data)
    {
        if (m_Dragging)
            return;
        _PointerAt(((PointerEventData)data).position);
    }

    public static void PointerAt(Vector3 point)
    {
        Vector3 v = Camera.main.WorldToScreenPoint(point);
        v.z = 0;
        Debug.Log(v);
        m_Instance._PointerAt(v);
    }

    private void _PointerAt(Vector3 point)
    {
           Vector3 vec = point - PoolKit.WhiteBall.GetScreenPosition(),
            dir = Guidelines.GetPointerDirection();

        float angle = Mathf.Acos(Vector3.Dot(vec.normalized, dir.normalized)) * Mathf.Rad2Deg;

        //pass too small angle
        if (float.IsNaN(angle) || float.IsInfinity(angle) || angle < 1)
            return;

        float z = Vector3.Cross(vec, dir).z;
        if (z < 0) PoolHuman.CueRotate(-angle);
        else PoolHuman.CueRotate(angle);
    }
}
