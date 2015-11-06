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

    private float m_Time;
    [SerializeField]
    private float m_TimeInterval = 1;

    private Queue<Rigidbody> m_BallQueue = new Queue<Rigidbody>();

    //private bool m_Down = false;

    void Awake()
    {
        m_Force = (m_ForcePointTrans.position - m_InitTrans.position).normalized * m_BallInitForceStrength;
        m_Time = m_TimeInterval;
    }

    void Update()
    {
        if (m_BallQueue.Count > 0)
        {
            m_Time -= Time.deltaTime;
            if (m_Time < 0)
            {
                Rigidbody rb = m_BallQueue.Dequeue();
                WhiteBall cueball;
                if ((cueball = rb.GetComponent<WhiteBall>()) && cueball.BallState != PoolBall.State.POTTED) //ball has already reset
                {
                    return;
                }
                rb.GetComponent<PoolBall>().LightRenderer.Open();
                rb.collider.enabled = true;
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = m_Force;
                rb.renderer.enabled = true;
                rb.gameObject.AddComponent<RackBallCollision>();
                PhysicalSupportTools.MaxSpeedLimitTo(rb.gameObject, m_BallMaxSpeed);
                m_Time = m_TimeInterval;
            }
        }
    }

    public void Add(PoolBall ball)
    {
        ball.AudioEnable = true;
        ball.LightRenderer.Close();
        Rigidbody rb = ball.rigidbody;
        rb.position = m_InitTrans.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collider.enabled = false;
        rb.renderer.enabled = false;
        m_BallQueue.Enqueue(rb);
    }

    public void Remove(PoolBall ball)
    {
        ball.Reset();
    }

    public void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = m_BallQueue.Count > 0 ? Color.green : Color.red;
            Gizmos.DrawCube(transform.position, Vector3.one * .2f);
        }
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
