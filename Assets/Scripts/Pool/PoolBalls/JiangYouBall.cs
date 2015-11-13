using UnityEngine;
using System.Collections;

public class JiangYouBall : PoolBall
{
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
            GameStatistics.MarkPottedJiangyouBallCount(1);
        }
    }
}
