using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : BasePlayer
{

    #region Collector
    protected class AICollector
    {
        AIPlayer player;

        public AICollector(AIPlayer _player)
        {
            player = _player;
        }

        public CollectedMessage Collect()
        {
            CollectedMessage msg = new CollectedMessage();
            if(player.m_TargetBalls.Count == 0)
            {
                msg.ballList = Pools.GetSolidAndStripeBalls();
            }
            else
            {
                msg.ballList = player.m_TargetBalls;
            }
            return msg;
        }
    }

    protected class CollectedMessage
    {
        public List<PoolBall> ballList;

        public CollectedMessage()
        {
            ballList = new List<PoolBall>();
        }
    }
    #endregion //Collector

    #region Decider
    protected class AIDecider
    {
        AIPlayer player;
        CollectedMessage cMsg;

        public AIDecider(AIPlayer _player)
        {
            player = _player;
        }

        public DecidedMessage Decide(CollectedMessage _cMsg)
        {
            cMsg = _cMsg;
            DecidedMessage dMsg = new DecidedMessage();

            Dictionary<PoolBall, List<PocketTrigger>> considerBalls = GetConsiderBalls();
            dMsg.drag = GameManager.Rules.State == GlobalState.DRAG_WHITEBALL;
            PoolBall targetBall;
            PocketTrigger targetPocket;
            if (dMsg.drag)
                dMsg.hitPoint = ConsiderHitPointWithDrag(considerBalls, out dMsg.cueballPosition, out targetBall, out targetPocket);
            else
            {
                dMsg.cueballPosition = Pools.CueBall.GetPosition();
                dMsg.hitPoint = ConsiderHitPoint(considerBalls, dMsg.cueballPosition, out targetBall, out targetPocket);
            }

            if(dMsg.hitPoint != Vector3.zero)
                dMsg.powerScale = ConsiderPowerScale(targetBall, targetPocket, dMsg.hitPoint, dMsg.cueballPosition);
            else
            {
                dMsg.hitPoint = GetRandomHitPoint();
                dMsg.powerScale = 1;
            }
            return dMsg;
        }

        private Vector3 ConsiderHitPointWithDrag(Dictionary<PoolBall, List<PocketTrigger>> considerBalls, out Vector3 cueBallPosition, out PoolBall targetBall, out PocketTrigger targetTrigger)
        {
            Debug.Log("AI : consider hit point with drag");
            Dictionary<PoolBall, PocketTrigger> optimalBalls = FilterTheBestPocketForEachBall(considerBalls);
            cueBallPosition = Vector3.zero;
            targetTrigger = null;
            targetBall = null;
            if(optimalBalls.Count == 0)
            {
                return Vector3.zero;
            }
            else
            {
                foreach (KeyValuePair<PoolBall, PocketTrigger> kvp in optimalBalls)
                {
                    PoolBall ball = kvp.Key;
                    PocketTrigger trigger = kvp.Value;
                    Vector3 dir = ball.transform.position - trigger.pointerPosition;
                    //白球摆成和目标球和袋口一条直线
                    cueBallPosition = ball.transform.position + (dir.normalized * Mathf.Min(GetPlacedSpace(ball, ball.transform.position - trigger.pointerPosition), 5 * ball.GetRadius()));
                    targetBall = ball;
                    targetTrigger = trigger;
                    //这里随机会跳出循环，代表随机取一个球的意思， optimalBalls.count 为了使概率平均
                    if (Random.Range(1, optimalBalls.Count) == 1)
                        break;
                }
                return ConsiderHitPoint(targetBall, targetTrigger);
            }
        }

        private Dictionary<PoolBall, PocketTrigger> FilterTheBestPocketForEachBall(Dictionary<PoolBall, List<PocketTrigger>> considerBalls)
        {
            Dictionary<PoolBall, PocketTrigger> optimalBalls = new Dictionary<PoolBall, PocketTrigger>();
            foreach (var v in considerBalls)
            {
                PoolBall ball = v.Key;
                float bestDistance = float.MaxValue;
                PocketTrigger bestPocket = null;
                for (int i = 0, count = v.Value.Count; i < count; i++)
                {
                    PocketTrigger pocket = v.Value[i];

                    if (GetPlacedSpace(ball, ball.transform.position - pocket.pointerPosition) < ball.GetRadius() * 2)
                        continue;

                    float distance = Vector3.Distance(ball.transform.position, pocket.pointerPosition);
                    if (distance < bestDistance)
                    {
                        bestPocket = pocket;
                        bestDistance = distance;
                    }
                }
                optimalBalls.Add(ball, bestPocket);
            }
            return optimalBalls;
        }

        private Vector3 ConsiderHitPoint(PoolBall ball, PocketTrigger trigger)
        {
            Vector3 vec = ball.transform.position - trigger.pointerPosition;
            return ball.transform.position + vec.normalized * ball.GetRadius() * 2;
        }

        private Vector3 ConsiderHitPoint(Dictionary<PoolBall, List<PocketTrigger>> considerBalls, Vector3 cueBallPosition, out PoolBall targetBall, out PocketTrigger targetTrigger)
        {
            Debug.Log("AI : consider hit point");
            Dictionary<PoolBall, PocketTrigger> optimalBalls = FilterTheBestPocketForEachBallWithCueBall(considerBalls, cueBallPosition);
            //Dictionary<PoolBall, float> ballCuebalAngles = new Dictionary<PoolBall, float>();
            List<float> angles = new List<float>();
            List<PoolBall> balls = new List<PoolBall>();
            float totleAngle = 0;
            foreach(var v in optimalBalls)
            {
                PoolBall ball = v.Key;
                PocketTrigger trigger = v.Value;

                if (!trigger)
                    continue;

                float angle = MathTools.AngleBetween(ball.transform.position - cueBallPosition, trigger.pointerPosition - ball.transform.position);
                totleAngle += angle;
                balls.Add(ball);
                angles.Add(angle);
            }
            float rand = Random.Range(0, totleAngle);
            for (int i = 0, count = angles.Count; i < count; i++)
            {
                if(rand < angles[i])
                {
                    PoolBall k = balls[i];
                    targetBall = k;
                    targetTrigger = optimalBalls[k];
                    return ConsiderHitPoint(targetBall, targetTrigger);
                }
            }
            targetTrigger = null;
            targetBall = null;
            return Vector3.zero;
        }

        private Dictionary<PoolBall, PocketTrigger> FilterTheBestPocketForEachBallWithCueBall(Dictionary<PoolBall, List<PocketTrigger>> considerBalls, Vector3 cueBallPosition)
        {
            Dictionary<PoolBall, PocketTrigger> optimalBalls = new Dictionary<PoolBall,PocketTrigger>();
            foreach(var v in considerBalls)
            {
                PoolBall ball = v.Key;
                float bestAngle = 70;
                PocketTrigger bestPocket = null;
                for(int i = 0, count = v.Value.Count; i < count; i++)
                {
                    PocketTrigger pocket = v.Value[i];
                    // is any obstacle between the ball hit position and the cueball ?
                    bool b1 = !CheckObastacleBetweenTargetPositionAndCueball(ball, ConsiderHitPoint(ball, pocket), cueBallPosition);
                    if(b1)
                    {
                        float angle = MathTools.AngleBetween(ball.transform.position - cueBallPosition, pocket.pointerPosition - ball.transform.position);
                        if(angle < bestAngle)
                        {
                            bestPocket = pocket;
                            bestAngle = angle;
                        }
                    }
                }
                optimalBalls.Add(ball, bestPocket);
            }
            return optimalBalls;
        }

        private float GetPlacedSpace(PoolBall ball, Vector3 dir)
        {
            ball.collider.enabled = false;
            ball.rigidbody.isKinematic = true;
            RaycastHit hit;
            float space = 0;
            if (Physics.SphereCast(ball.transform.position, ball.GetRadius(), dir, out hit, ConstantData.OulineAndBallLayer))
            {
                space = (hit.transform.position - ball.transform.position).magnitude * .5f;
            }
            ball.collider.enabled = true;
            ball.rigidbody.isKinematic = false;
            return space;
        }

        private Dictionary<PoolBall, List<PocketTrigger>> GetConsiderBalls()
        {
            Dictionary<PoolBall, List<PocketTrigger>> considerableBalls = new Dictionary<PoolBall, List<PocketTrigger>>();
            for (int i = 0; i < cMsg.ballList.Count; i++)
            {
                PoolBall ball = cMsg.ballList[i];
                if (ball.BallState != PoolBall.State.IDLE)
                    continue;

                List<PocketTrigger> triggerList = new List<PocketTrigger>();
                for (int j = 0; j < Pools.PocketTriggers.Count; j++)
                {
                    PocketTrigger trigger = Pools.PocketTriggers[j];
                    //is any obstacle between the ball and the pocket ? 
                    bool b1 = !CheckObastacleBetweenBallAndPocket(ball, trigger);
                    if (b1)
                    {
                        triggerList.Add(trigger);
                    }
                }
                if (triggerList.Count != 0)
                {
                    considerableBalls.Add(ball, triggerList);
                }
            }
            return considerableBalls;
        }

        private bool CheckObastacleBetweenBallAndPocket(PoolBall originBall, PocketTrigger pocket)
        {
            Vector3 dir = pocket.pointerPosition - originBall.transform.position;
            RaycastHit[] hit = Physics.SphereCastAll(originBall.transform.position, originBall.GetRadius(),
                dir, dir.magnitude, ConstantData.OulineAndBallLayer);
            foreach (RaycastHit h in hit)
            {
                //Debug.Log("Obastacle between " + originBall.name + " and " + pocket.name + " : " + h.transform.name);
                if (h.collider.name.CompareTo(originBall.name) != 0)
                    return true;
            }
            return false;
        }

        private bool CheckObastacleBetweenTargetPositionAndCueball(PoolBall ball, Vector3 targetPosition, Vector3 cueBallPosition)
        {
            Vector3 dir = targetPosition - cueBallPosition;
            RaycastHit[] hit = Physics.SphereCastAll(cueBallPosition, Pools.CueBall.GetRadius(),
                dir, dir.magnitude, ConstantData.OulineAndBallLayer);
            foreach (RaycastHit h in hit)
            {
                //Debug.Log("Obastacle between hit point-" + GetHitPoint(ball, pocket) + " and cue ball : " + h.transform.name);
                if (h.collider.name.CompareTo(ball.name) != 0)//the sphere always hit the cue ball
                    return true;
            }
            return false;
        }

        private float ConsiderPowerScale(PoolBall ball, PocketTrigger trigger, Vector3 hitPoint, Vector3 cueBallPosition)
        {
            Vector3 targetBallPosition = ball.transform.position,
            pocketPosition = trigger.pointerPosition;

            float angle = MathTools.AngleBetween(hitPoint - cueBallPosition, pocketPosition - targetBallPosition);
            float mag = (targetBallPosition - cueBallPosition).magnitude + (pocketPosition - targetBallPosition).magnitude;
            return Mathf.Max(1, mag / Pools.GetTableSize().x * 1.5f);
        }

        private Vector3 GetRandomHitPoint()
        {
            Debug.Log("AI : get random hit point");
            int i = Random.Range(0, cMsg.ballList.Count - 1), j = Random.Range(0, Pools.PocketTriggers.Count - 1);
            return ConsiderHitPoint(cMsg.ballList[i], Pools.PocketTriggers[j]);
        }
    }

    protected class DecidedMessage
    {
        public Vector3 hitPoint;
        public Vector3 cueballPosition;
        public float powerScale;
        public bool drag;
    }
    #endregion //Decider

    #region Executor
    protected class AIExecutor
    {
        AIPlayer player;

        DecidedMessage msg;

        Vector3 startVector;
        Vector3 endVector;

        float thinkingTime;

        enum Step
        {
            None = 0,
            Drag,
            Change2Aim,
            AdjustDirection,
            Fire
        }
        Step step;

        public AIExecutor(AIPlayer _player)
        {
            player = _player;
            startVector = Vector3.zero;
            endVector = Vector3.zero;
            step = Step.None;
        }

        public void SetDecision(DecidedMessage _msg)
        {
            Debug.Log("Executor set decision");
            msg = _msg;
            thinkingTime = GenerateThinkingTime();
            if(msg.drag)
            {
                step = Step.Drag;
            }
            else
            {
                step = Step.AdjustDirection;
            }
        }

        public void Execute()
        {
            if(step != Step.None)
            {
                if(thinkingTime > 0)
                    thinkingTime -= Time.deltaTime;
                if(thinkingTime <= 0)
                {
                    switch(step)
                    {
                        case Step.Drag:
                            iTween.MoveTo(Pools.CueBall.gameObject, msg.cueballPosition, .5f);
                            thinkingTime = GenerateThinkingTime();
                            step = Step.Change2Aim;
                            break;
                        case Step.Change2Aim:
                            thinkingTime = GenerateThinkingTime();
                            GameManager.Rules.State = GlobalState.IDLE;
                            step = Step.AdjustDirection;
                            break;
                        case Step.AdjustDirection:
                            float angle = BaseUIController.cueOperateArea.pointerOperation.GetRotationAngleAtWorld(msg.hitPoint);
                            Vector3 v = new Vector3(startVector.x, Pools.Cue.transform.eulerAngles.y + angle, startVector.z);
                            iTween.RotateTo(Pools.Cue.gameObject, v, .5f);
                            thinkingTime = GenerateThinkingTime();
                            step = Step.Fire;
                            break;
                        case Step.Fire:
                            Debug.Log("AI fire, power scalar : " + msg.powerScale);
                            Pools.Cue.SetPowerScalar(msg.powerScale);
                            Pools.Cue.Fire();
                            step = Step.None;
                            break;
                    }
                }
            }
        }

        private float GenerateThinkingTime()
        {
            return Random.Range(.5f, 2);
        }
    }
    #endregion //Executor

    protected AIExecutor m_Executor;
    protected AICollector m_Collector;
    protected AIDecider m_Decider;

    public Vector3 hitpoint;

    protected override void Awake()
    {
        base.Awake();
        m_Executor = new AIExecutor(this);
        m_Collector = new AICollector(this);
        m_Decider = new AIDecider(this);
    }

    public override void Begin()
    {
        base.Begin();
        //m_OperateMask.SetActive(true);
        //BaseUIController.GlobalMask.SetActive(true);
        DecidedMessage decideMsg = m_Decider.Decide(m_Collector.Collect());
        hitpoint = decideMsg.hitPoint;
        m_Executor.SetDecision(decideMsg);
    }

    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
            Gizmos.DrawSphere(hitpoint, .1f);
    }

    public override void End()
    {
        base.End();
        //m_OperateMask.SetActive(false);
    }

    public override void PlayerUpdate()
    {
        m_Executor.Execute();
    }
}
