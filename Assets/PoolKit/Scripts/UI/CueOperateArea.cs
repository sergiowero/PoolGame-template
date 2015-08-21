using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// need initializtion
/// </summary>
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

    private void RoundBegin(int playerID)
    {
        BaseUIController.cueAndLines.gameObject.SetActive(true);
        PoolBall ball9 = Pools.Balls[9];
        if (ball9 != null && ball9.gameObject.activeInHierarchy)
            PointerAtWorld(Pools.Balls[9].transform.position);
        else
        {
            for (int i = 0; i < 16; i++)
            {
                if (i == 8) continue;
                PoolBall ball = Pools.Balls[i];
                if (ball != null && ball.gameObject.activeInHierarchy)
                {
                    PointerAtWorld(ball.transform.position);
                    break;
                }
            }
        }
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

    public void PointerAtWorld(Vector3 point)
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
