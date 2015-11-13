using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbsorbBall : PoolBall 
{
    private List<PoolBall> m_AbsorbList = new List<PoolBall>();

    public List<PoolBall> AbsorbList { get { return m_AbsorbList; } }

    public override void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.Contains("Rail"))
        {
            //we hit the wall.
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
            if(ball.ballType != BallType.WHITE && BallState == State.ROLL)
            {
                m_AbsorbList.Add(ball);
                //ball.Hide();
                TemporarySlot.Add(ball);
            }
            else
            {
                HOAudioManager.BallhitBall(m_rigidbody.velocity);
            }
        }
    }
}
