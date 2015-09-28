using UnityEngine;
using System.Collections;

public class SingularityBall : PoolBall 
{
    public static Delegate1Args<PoolBall> SingularityBreakBall;

    public override void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.Contains("Rail"))
        {
            //we hit the wall.
            //BaseGameManager.ballHitWall(rigidbody.velocity);
            AudioHelper.m_Instance.onBallHitWall(m_rigidbody.velocity);
            if (!hitWall)
            {
                GameManager.Rules.BallHitRail();
                hitWall = true;
            }
        }
        if (col.gameObject.CompareTag("Ball"))
        {
            PoolBall ball = col.transform.GetComponent<PoolBall>();
            if (ball.ballType != BallType.WHITE && BallState == State.ROLL)
            {
                if (SingularityBreakBall != null)
                    SingularityBreakBall(ball);
                if (AudioEnable) AudioHelper.m_Instance.onBreak();
            }
            else
            {
                if (AudioEnable) AudioHelper.m_Instance.onBallHitBall(m_rigidbody.velocity);
            }
        }
    }
}
