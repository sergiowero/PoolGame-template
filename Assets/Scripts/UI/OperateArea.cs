using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class OperateArea : MonoBehaviour {
    interface IAreaOperation
    {
        void DragBegin(Vector2 position);
        void Drag(Vector2 position);
        void DragEnd(Vector2 position);
        void Click(Vector2 position);
        void Begin();
        void End();
    }

    /// <summary>
    /// Active default
    /// </summary>
    class PointerOperation : IAreaOperation
    {
        private Vector2 poolBallsp;
        private Vector2 lastTouchsp;

        public void DragBegin(Vector2 position)
        {
            Debug.Log("DragBegin");
            poolBallsp = Pools.CueBall.GetScreenPosition();
            lastTouchsp = position;
        }

        public void Drag(Vector2 position)
        {
            //if (lastTouchsp == default(Vector2))
            //{
            //    lastTouchsp = position;
            //    return;
            //}
            Debug.Log("Drag");
            Vector3 v1 = lastTouchsp - poolBallsp, v2 = position - poolBallsp;
            float ag = Mathf.Acos(Vector3.Dot(v1.normalized, v2.normalized)) * Mathf.Rad2Deg;
            if (float.IsNaN(ag) || float.IsInfinity(ag) || ag == 0)
                return;
            float fag = ag * 100 / Mathf.Max(v2.magnitude, 100);
            float z = Vector3.Cross(v1, v2).z;
            if (z < 0) Pools.Cue.Rotate(fag);
            else Pools.Cue.Rotate(-fag);
            lastTouchsp = position;
        }

        public void DragEnd(Vector2 position)
        {
            Debug.Log("DebugEnd");
        }

        public void Click(Vector2 position)
        {
            Debug.Log("Click");
            PointerAt(position);
        }

        public void PointerAt(Vector3 point)
        {
            Debug.Log("PointerAt");
            Vector3 vec = point - Pools.CueBall.GetScreenPosition(),
                dir = BaseUIController.cueAndLines.GetPointerDirection();
            float angle = Mathf.Acos(Vector3.Dot(vec.normalized, dir.normalized)) * Mathf.Rad2Deg;
            PointerAtAngle(angle, Vector3.Cross(vec, dir).z < 0);
        }

        public void PointerAtWorld(Vector3 point)
        {
            Debug.Log("PointerAtWorld");
            Vector3 v = Pools.SceneCamera.WorldToScreenPoint(point);
            PointerAt(v);
        }

        public void PointerAtAngle(float angle, bool anticlockwise)
        {
            Debug.Log("PointerAtAngle");
            //pass too small angle                                              /*this is meaningless*/
            if (float.IsNaN(angle) || float.IsInfinity(angle)/* || angle < 1*/)
                return;

            if (anticlockwise) Pools.Cue.Rotate(-angle);
            else Pools.Cue.Rotate(angle);
        }

        public void Begin()
        {
        }

        public void End()
        {
        }
    }
    /// <summary>
    /// Active when cueball can be dragged
    /// </summary>
    class DragOperation : IAreaOperation
    {
        private Vector3 ballDelta = new Vector3();

        private WhiteBall cueBall;
        private Constraint constraint;

        private Vector2 prevScreenPosition;

        private float y;

        private bool droppable = true;

        public DragOperation()
        {
            cueBall = Pools.CueBall;
            constraint = Pools.CueBall.GetComponent<Constraint>();
        }

        public void DragBegin(Vector2 position)
        {
            prevScreenPosition = position;
        }

        public void Drag(Vector2 position)
        {
            Vector3 delta = Pools.SceneCamera.ScreenToWorldPoint(position) - Pools.SceneCamera.ScreenToWorldPoint(prevScreenPosition);
            ballDelta.Set(delta.x, delta.y, delta.z);

            prevScreenPosition = position;

            cueBall.transform.position += ballDelta;
            cueBall.transform.position = MathTools.Clamp3(cueBall.transform.position, constraint.min, constraint.max);
            if (RayCast(Pools.SceneCamera.WorldToScreenPoint(cueBall.transform.position)))
            {
                droppable = false;
                BaseUIController.hand.ChangeState(1);
            }
            else
            {
                droppable = true;
                BaseUIController.hand.ChangeState(0);
            }
        }

        public void DragEnd(Vector2 position)
        {

        }

        public void Click(Vector2 position)
        {
            if (!droppable)
                return;

            Vector3 v = cueBall.transform.position;
            v.y = y;
            cueBall.transform.position = v;
            GameManager.Rules.State = GlobalState.IDLE;
        }

        public void Begin()
        {
            Pools.Cue.Hide();
            BaseUIController.hand.gameObject.SetActive(true);
            y = cueBall.transform.position.y;
            cueBall.yConstrain = false;
            Vector3 v = cueBall.transform.position;
            v.y = 0;
            cueBall.transform.position = v;
            constraint.enabled = true;
            cueBall.rigidbody.useGravity = false;
            cueBall.rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            Pools.AllBallsKinematic();
        }

        public void End()
        {
            Pools.Cue.Show();
            Vector3 v = cueBall.transform.position;
            v.y = y;
            cueBall.transform.position = v;
            BaseUIController.hand.gameObject.SetActive(false);
            cueBall.rigidbody.useGravity = true;
            cueBall.rigidbody.constraints = RigidbodyConstraints.None;
            constraint.enabled = false;
            Pools.AllBallsNonKinematic();
        }

        private bool RayCast(Vector2 p)
        {
            Vector3 ori = Pools.SceneCamera.ScreenToWorldPoint(p);
            Vector3 dir = Vector3.down;
            Ray ray = new Ray(ori, dir);
            if (Physics.SphereCast(ray, Pools.CueBall.GetRadius(), 1000, 1 << LayerMask.NameToLayer("Ball")))
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Do nothing
    /// </summary>
    class DontDoAnyOperation : IAreaOperation
    {
        public void DragBegin(Vector2 position){}
        public void Drag(Vector2 position){}
        public void DragEnd(Vector2 position){}
        public void Click(Vector2 position){}
        public void Begin(){}
        public void End(){}
    }

    private Delegate1Args<Vector2> m_OnDrag;
    private Delegate1Args<Vector2> m_OnDragBegin;
    private Delegate1Args<Vector2> m_OnDragEnd;
    private Delegate1Args<Vector2> m_OnClick;


    private PointerOperation m_PointerOperation;
    private DragOperation m_DragOperation;
    private DontDoAnyOperation m_DontDoAnyOperation;

    private bool m_Dragging = false;

    private IAreaOperation m_Operation;

    void Awake()
    {
        PoolRulesBase.onNewTurn += RoundBegin;
    }

    void Start()
    {
        m_PointerOperation = new PointerOperation();
        m_DragOperation = new DragOperation();
        m_DontDoAnyOperation = new DontDoAnyOperation();
        SetOpeartion(m_PointerOperation);
        BaseUIController.hand.followObject = Pools.CueBall.transform;
    }

    void OnDestroy()
    {
        PoolRulesBase.onNewTurn -= RoundBegin;
    }

    private void RoundBegin(int playerID)
    {
        BaseUIController.cueAndLines.gameObject.SetActive(true);
        if (GameManager.Rules.FirstRound)
            Pools.Cue.Reset(); 
        else
        {
            for (int i = 0; i < 16; i++)
            {
                PoolBall ball = Pools.Balls[i];
                if (ball.ballType == BallType.WHITE || ball.ballType == BallType.BLACK) 
                    continue;

                if (ball != null && ball.gameObject.activeInHierarchy && ball.BallState == PoolBall.State.IDLE)
                {
                    m_PointerOperation.PointerAtWorld(ball.transform.position);
                    return;
                }
            }
            m_PointerOperation.PointerAtAngle(0, true);
        }
    }

    public void ChangeOperationType(GlobalState state)
    {
        Debug.Log("Change operation type : " + state);
        switch (state)
        {
            case GlobalState.DRAG_WHITEBALL:
                SetOpeartion(m_DragOperation);
                break;
            case GlobalState.IDLE:
                SetOpeartion(m_PointerOperation);
                break;
            default:
                SetOpeartion(m_DontDoAnyOperation);
                break;
        }
    }

    private void SetOpeartion(IAreaOperation operation)
    {
        if(m_Operation != null)
            m_Operation.End();
        m_Operation = operation;
        m_OnDrag = m_Operation.Drag;
        m_OnClick = m_Operation.Click;
        m_OnDragBegin = m_Operation.DragBegin;
        m_OnDragEnd = m_Operation.DragEnd;
        m_Operation.Begin();
    }

    public void OnAreaDragEnd(BaseEventData data)
    {
        if(m_OnDragEnd != null && m_Dragging)
        {
            m_OnDragEnd(((PointerEventData)data).position);
        }
        m_Dragging = false;
    }

    public void OnAreaDrag(BaseEventData data)
    {
        PointerEventData ped = data as PointerEventData;
        if(m_OnDrag != null)
        {
            m_OnDrag(ped.position);
        }
    }

    public void OnAreaDragBegin(BaseEventData data)
    {
        m_Dragging = true;
        if(m_OnDragBegin != null)
        {
            m_OnDragBegin(((PointerEventData)data).position);
        }
    }

    public void OnAreaPointerClick(BaseEventData data)
    {
        if (!m_Dragging && m_OnClick != null)
        {
            m_OnClick(((PointerEventData)data).position);
        }
    }

}
