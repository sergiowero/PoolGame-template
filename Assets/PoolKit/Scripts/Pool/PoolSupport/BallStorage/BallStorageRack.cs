using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallStorageRack : MonoBehaviour {

    Dictionary<int, Rigidbody> Balls = new Dictionary<int, Rigidbody>();

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

    public void Add(Rigidbody ball)
    {
        PoolBall pBall = ball.GetComponent<PoolBall>();
        pBall.EnterPocket();
        BallInPocket pocket = ball.GetComponent<BallInPocket>();
        if (pocket) Destroy(pocket);

        ball.position = m_InitTrans.position;
        ball.velocity = m_Force;
        Balls.Add(pBall.GetBallID(), ball);

        PhysicalSupportTools.MaxSpeedLimitTo(ball.gameObject, m_BallMaxSpeed);
    }

    public PoolBall Remove(int ballID)
    {
        if(Balls.ContainsKey(ballID))
        {
            PoolBall ball = Balls[ballID].GetComponent<PoolBall>();
            ball.Reset();
            Balls.Remove(ballID);

            PhysicalSupportTools.Remove(ball.gameObject, PhysicalSupportType.MaxSpeedLimit);
            return ball;
        }
        return null;
    }
}
