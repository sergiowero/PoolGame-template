﻿using UnityEngine;
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

        if (col.gameObject.name.Contains("Rail"))
        {
            HOAudioManager.BallhitRail(m_rigidbody.velocity);
            GameManager.Rules.CueBallHitRail();
            GameManager.Rules.BallHitRail();
            GameStatistics.MarkCueballHitRail(1);
        }
        if (col.transform.CompareTag("Ball"))
        {
            HOAudioManager.BallhitBall(m_rigidbody.velocity);
            GameStatistics.MarkCueballHitBall(1);
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
        m_TestVelocity = m_rigidbody.velocity;
    }

    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
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
        HOAudioManager.FireBall();
        GameManager.Rules.OnBallFired();
        m_slowTime = 0;

        Vector3 fireForce = fireDir * powerScalar * ConstantData.GetPoolDatas().MaxImpulse;
        m_rigidbody.AddForceAtPosition(fireForce, hitPoint, ForceMode.Impulse);
        m_state = State.ROLL;
        OpenDrag();
    }

    public override void Potted(PocketIndexes pocketIndex)
    {
        if (m_rigidbody)
        {
            m_state = State.POTTED;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            CloseDrag();
            CloseRenderer();
            enabled = false;
            RemovePhysicalMaterial();
            GameStatistics.MarkCueballPotted(1);
        }
    }
}
