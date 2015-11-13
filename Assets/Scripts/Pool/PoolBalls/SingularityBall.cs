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
            HOAudioManager.BallhitRail(m_rigidbody.velocity);
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
                HOAudioManager.Break();
            }
            else
            {
                HOAudioManager.BallhitBall(m_rigidbody.velocity);
            }
        }
    }
}
