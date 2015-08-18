using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CueOperateArea : MonoBehaviour {

    private static CueOperateArea m_Instance = null;

    private Vector2 m_poolBallsp;

    private Vector2 m_LastTouchsp;


    private bool m_Dragging = false;

    void Awake()
    {
        if(m_Instance)
        {
            Debug.LogError("two dragarea in the scene");
            return;
        }
        m_Instance = this;
        BaseGameManager.onNewRoundBegin += RoundBegin;
    }

    void OnDestroy()
    {
        BaseGameManager.onNewRoundBegin -= RoundBegin;
    }

    //这里需要判断是否是游戏开始， 游戏开始指向9号球 不然保持之前的角度不变
    private void RoundBegin(int playerID)
    {
        BaseUIController.cueAndLines.gameObject.SetActive(true);
        PointAtWorld(Pools.Balls[9].transform.position);
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
        if (z < 0) Pools.Cue.Rotate(fag);
        else Pools.Cue.Rotate(-fag);
        m_LastTouchsp = ped.position;
    }

    public void OnAreaDragBegin(BaseEventData data)
    {
        m_Dragging = true;
        m_poolBallsp = Pools.CueBall.GetScreenPosition();
        m_LastTouchsp = default(Vector2);
    }

    public void OnAreaPointerClick(BaseEventData data)
    {
        if (m_Dragging)
            return;
        PointerAt(((PointerEventData)data).position);
    }

    public void PointAtWorld(Vector3 point)
    {
        Vector3 v = Pools.SceneCamera.WorldToScreenPoint(point);
        PointerAt(v);
    }
    
    public void PointerAt(Vector3 point)
    {
        Vector3 vec = point - Pools.CueBall.GetScreenPosition(),
            dir = BaseUIController.cueAndLines.GetPointerDirection();

        float angle = Mathf.Acos(Vector3.Dot(vec.normalized, dir.normalized)) * Mathf.Rad2Deg;

        //pass too small angle
        if (float.IsNaN(angle) || float.IsInfinity(angle) || angle < 1)
            return;

        float z = Vector3.Cross(vec, dir).z;
        if (z < 0) Pools.Cue.Rotate(-angle);
        else Pools.Cue.Rotate(angle);
    }
}
