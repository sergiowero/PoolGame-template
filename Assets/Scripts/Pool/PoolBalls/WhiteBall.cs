using UnityEngine;
using System.Collections;
public class WhiteBall : PoolBall
{
    private BallDragger m_BallDragger;

    public static bool CueBallSiding = false;

    Vector3 screenPoint;
    Vector3 offset;

    private Constraint m_constraint;
    private PoolBall m_targetBall;
    private Vector3 m_targetPos;

    public LayerMask layermask;
    public bool Foul = true;

    [SerializeField]
    private Vector3 m_TestVelocity;

    [SerializeField]
    private bool m_UseTracker;


    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetScreenPosition()
    {
        Vector3 v = Pools.SceneCamera.WorldToScreenPoint(GetPosition());
        v.z = 0;
        return v;
    }

    public override void Start()
    {
        base.Start();
        m_constraint = gameObject.GetComponent<Constraint>();
        m_constraint.enabled = false;
        m_constraint.adjustment = GetRadius();
        Reset();

        if(m_UseTracker)
        {
            Debugger.Tracker t = Debugger.DBERP.GetComponentWithType(Debugger.DebuggerType.Tracker, gameObject) as Debugger.Tracker;
            t.SetTransform(gameObject.transform);
        }
    }

    public void setTarget(PoolBall ball, Vector3 targetPos)
    {
        if (ball)
        {
            m_targetBall = ball;
            m_targetPos = targetPos;
            m_targetPos.y = transform.position.y;
        }
    }

    public override void OnCollisionEnter(Collision col)
    {
        if (GameManager.Rules.State == GlobalState.DRAG_WHITEBALL)
            return;

        #region Might be useful
        if (col.gameObject.name.Contains("Rail"))
        {
            AudioHelper.m_Instance.onBallHitWall(m_rigidbody.velocity);
            GameManager.Rules.CueBallHitRail();
            GameManager.Rules.BallHitRail();
        }
        #endregion
        if (col.transform.CompareTag("Ball"))
        {
            if (AudioEnable) AudioHelper.m_Instance.onBallHitBall(m_rigidbody.velocity);
            PoolBall ball = col.gameObject.GetComponent<PoolBall>();
            GameManager.Rules.WhiteBallHitBall(ball);
            if (ball && ball == m_targetBall)
            {
                m_targetBall.PointAtTarget(m_targetPos);
                m_targetBall = null;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        m_TestVelocity = m_rigidbody.angularVelocity;
    }

    public void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.DrawRay(m_rigidbody.position, m_TestVelocity);
        }
    }

    public override void Reset()
    {
        base.Reset();
        //transform.position = Pools.CueBallOrigin.position;
        Vector3 p = Pools.CueBallOrigin.position;
        Pools.PutBallToThePoint(this, ref p);
    }

    public void fireBall(float powerScalar, Vector3 fireDir, Vector3 hitPoint)
    {
        AudioHelper.m_Instance.onFireBall();
        GameManager.Rules.OnBallFired();
        m_slowTime = 0;

        Vector3 fireForce = fireDir * powerScalar * ConstantData.GetPoolDatas().MaxImpulse;
        m_rigidbody.AddForceAtPosition(fireForce, hitPoint, ForceMode.Impulse);
        m_state = State.ROLL;
        OpenDrag();
    }
}
