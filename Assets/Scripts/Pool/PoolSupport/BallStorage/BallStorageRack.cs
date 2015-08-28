using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallStorageRack : MonoBehaviour {

    [SerializeField]
    private float m_BallMaxSpeed;

    [SerializeField]
    private float m_BallInitForceStrength;

    [SerializeField]
    private Transform m_InitTrans;

    [SerializeField]
    private Transform m_ForcePointTrans;

    private Vector3 m_Force;

    void Awake()
    {
        m_Force = (m_ForcePointTrans.position - m_InitTrans.position).normalized * m_BallInitForceStrength;
    }

    public void Add(PoolBall ball)
    {
        Rigidbody rb = ball.rigidbody;
        rb.position = m_InitTrans.position;
        rb.velocity = m_Force;
        //GameManager.Rules.BallAtPocket.Add(pBall.GetBallID(), pBall);

        PhysicalSupportTools.MaxSpeedLimitTo(ball.gameObject, m_BallMaxSpeed);
    }

    public void Remove(PoolBall ball)
    {
        ball.Reset();
        PhysicalSupportTools.Remove(ball.gameObject, PhysicalSupportType.MaxSpeedLimit);
    }

    //public PoolBall Remove(int ballID)
    //{
    //    if (GameManager.Rules.BallAtPocket.ContainsKey(ballID))
    //    {
    //        PoolBall ball = GameManager.Rules.BallAtPocket[ballID].GetComponent<PoolBall>();
    //        ball.Reset();
    //        GameManager.Rules.BallAtPocket.Remove(ballID);

    //        PhysicalSupportTools.Remove(ball.gameObject, PhysicalSupportType.MaxSpeedLimit);
    //        return ball;
    //    }
    //    return null;
    //}
}
