using UnityEngine;
using System.Collections;

public struct BallDraggerData
{
    public Vector2 DeltaPosition;
    public Vector2 Position;

    public override string ToString()
    {
        return "position : " + Position + " ; position delta : " + DeltaPosition;
    }
}

public class BallDragger : MonoBehaviour {

    public LayerMask layermask;

    public System.Action<BallDraggerData> dragBegin;
    public System.Action<BallDraggerData> dragEnd;
    public System.Action<BallDraggerData> drag;

    private BallDraggerData m_DraggerData = new BallDraggerData();

    private bool m_Drag;

	void Update () 
    {
        if (Input.touchSupported)
            ProcessTouchEvents();
        else
            ProcessMouseEvents();
	}


    private void ProcessMouseEvents()
    {
        if(Input.GetMouseButtonDown(0))
        {
            BeginEvent(Input.mousePosition);
        }
        else if(Input.GetMouseButton(0))
        {
            ProcessEvent(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            EndEvent(Input.mousePosition);
        }
    }


    private void ProcessTouchEvents()
    {
        if(Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if(t.phase == TouchPhase.Began)
            {
                BeginEvent(t.position);
            }
            else if(t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                ProcessEvent(t.position);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                EndEvent(t.position);
            }
        }
    }

    private void BeginEvent(Vector2 position)
    {
        m_DraggerData.Position = position;
        m_DraggerData.DeltaPosition = Vector2.zero;



        if (RayCast(position) && dragBegin != null)
        {
            dragBegin(m_DraggerData);
            m_Drag = true;
        }
    }

    private void ProcessEvent(Vector2 position)
    {
        m_DraggerData.DeltaPosition = position - m_DraggerData.Position;
        m_DraggerData.Position = position;
        if (m_DraggerData.DeltaPosition.sqrMagnitude == 0)
            return;
        if (m_Drag && drag != null)
        {
            drag(m_DraggerData);
        }
    }

    private void EndEvent(Vector2 position)
    {
        m_DraggerData.Position = Vector2.zero;
        m_DraggerData.DeltaPosition = Vector2.zero;
        if (m_Drag && dragEnd != null)
        {
            dragEnd(m_DraggerData);
            m_Drag = false;
        }
    }

    private bool RayCast(Vector2 p)
    {
        Vector3 ori = Pools.SceneCamera.ScreenToWorldPoint(p);
        Vector3 dir = Vector3.down;
        Ray ray = new Ray(ori, dir);
        RaycastHit hit;
        if (Physics.SphereCast(ray, Pools.CueBall.GetRadius(), out hit, 1000, 1 << layermask.value))
        {
            if (hit.collider.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                return true;
        }
        return false;
    }
}
