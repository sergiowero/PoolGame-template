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
        //not use now
        public AIPlayer player;
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
            List<KeyValuePair<PoolBall, Vector3>> shootableBalls = GetShootableBalls();
            dMsg.drag = GameManager.Rules.State == GlobalState.DRAG_WHITEBALL;
            dMsg.considerBalls = considerBalls;
            dMsg.shootableBalls = shootableBalls;
            PoolBall targetBall;
            PocketTrigger targetPocket;
            if (dMsg.drag)
                dMsg.hitPoint = ConsiderHitPointWithDrag(considerBalls, out dMsg.cueballPosition, out targetBall, out targetPocket);
            else
            {
                dMsg.cueballPosition = Pools.CueBall.GetPosition();
                dMsg.hitPoint = ConsiderHitPoint(considerBalls, dMsg.cueballPosition, out targetBall, out targetPocket);
            }
            if (dMsg.hitPoint != Vector3.zero)
            {
                dMsg.targetBall = targetBall;
                dMsg.targetPocket = targetPocket;
                dMsg.powerScale = ConsiderPowerScale(targetBall, targetPocket, dMsg.hitPoint, dMsg.cueballPosition);
            }
            else
            {
                if(dMsg.shootableBalls.Count != 0)
                {
                    int i = Mathf.Max(Random.Range(0, dMsg.shootableBalls.Count - 1), 0);
                    dMsg.targetBall = dMsg.shootableBalls[i].Key;
                    dMsg.hitPoint = dMsg.shootableBalls[i].Value;
                }
                else//如果目标类型的球无球可打,则随便找一个能打到的球
                {
                    int i = Mathf.Max(Random.Range(0, cMsg.ballList.Count - 1));
                    dMsg.targetBall = cMsg.ballList[i];
                    dMsg.hitPoint = dMsg.targetBall.transform.position;
                }
                int j = Random.Range(0, Pools.PocketTriggers.Count - 1);
                dMsg.targetPocket = Pools.PocketTriggers[j];
                dMsg.cueballPosition = Pools.CueBall.GetPosition();
                dMsg.powerScale = 1;
            }
            if(BaseUIController.Instance.debugAI)
            {
                BaseUIController.Instance.targetBallImage.transform.position = MathTools.World2UI(dMsg.targetBall.transform.position);
                BaseUIController.Instance.targetPocketImage.transform.position = MathTools.World2UI(dMsg.targetPocket.transform.position);
                BaseUIController.Instance.hitPointImage.transform.position = MathTools.World2UI(dMsg.hitPoint);
            }
            if (Random.Range(0, 100) > (int)AIPlayer.difficulty)
            {
                dMsg.hitPoint += new Vector3(Random.Range(-player.m_Deviation, player.m_Deviation), 0, Random.Range(-player.m_Deviation, player.m_Deviation));
            }
            if(BaseUIController.Instance.debugAI)
                BaseUIController.Instance.newHitPointImage.transform.position = MathTools.World2UI(dMsg.hitPoint);
            return dMsg;
        }

        private Vector3 ConsiderHitPointWithDrag(Dictionary<PoolBall, List<PocketTrigger>> considerBalls, out Vector3 cueBallPosition, out PoolBall targetBall, out PocketTrigger targetTrigger)
        {
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
                    targetBall = kvp.Key;
                    targetTrigger = kvp.Value;
                    if (!targetTrigger) continue;
                    Vector3 dir = targetBall.transform.position - targetTrigger.pointerPosition;
                    //白球摆成和目标球和袋口一条直线
                    cueBallPosition = targetBall.transform.position + (dir.normalized * Mathf.Min(GetPlacedSpace(targetBall, targetBall.transform.position - targetTrigger.pointerPosition), 5 * targetBall.GetRadius()));
                    //这里随机会跳出循环，代表随机取一个球的意思， optimalBalls.count 为了使概率平均
                    if (Random.Range(1, optimalBalls.Count) == 1)
                        break;
                }
                if (!targetTrigger)
                {
                    return Vector3.zero;
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
                //夹角越大， 旋转击打这个球的概率就越小，所以这里用90 - angle
                angle = 90 - angle;
                totleAngle += angle;
                balls.Add(ball);
                angles.Add(angle);
            }
            //随机选择一个击打球
            float rand = Random.Range(0, totleAngle), threshold = 0;
            for (int i = 0, count = angles.Count; i < count; i++)
            {
                threshold += angles[i];
                if (rand < threshold)
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

        private List<KeyValuePair<PoolBall, Vector3>> GetShootableBalls()
        {
            List<KeyValuePair<PoolBall, Vector3>> shootableBalls = new List<KeyValuePair<PoolBall, Vector3>>();
            Vector3 cueBallPosition = Pools.CueBall.GetPosition();
            for(int i = 0; i < cMsg.ballList.Count; i++)
            {
                PoolBall ball = cMsg.ballList[i];
                Vector3 hitPoint = Vector3.zero;
                if (ball.BallState != PoolBall.State.IDLE)
                    continue;

                //求出dir的2条Y坐标为0的垂直向量, 得到目标球正对于白球的左侧和右侧的击球点,判断白球是否能击中 2个点或者白球中心的点的其中一个
                Vector3 dir = ball.transform.position - cueBallPosition;
                Vector2[] perpendicularVectors = MathTools.PerpendicularVector2D(new Vector2(dir.x, dir.z));


                Vector3[] point3 = new Vector3[] 
                { 
                    ball.transform.position, 
                    ball.transform.position + new Vector3(perpendicularVectors[0].x, 0, perpendicularVectors[0].y) * ball.GetRadius(),
                    ball.transform.position + new Vector3(perpendicularVectors[1].x, 0, perpendicularVectors[1].y) * ball.GetRadius()
                };

                for(int j = 0; j < point3.Length; j++)
                {
                    hitPoint = point3[j];
                    if(!CheckObastacleBetweenTargetPositionAndCueball(ball, hitPoint, cueBallPosition))
                    {
                        shootableBalls.Add(new KeyValuePair<PoolBall, Vector3>(ball, hitPoint));
                        break;
                    }
                }
            }
            return shootableBalls;
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

            //float angle = MathTools.AngleBetween(hitPoint - cueBallPosition, pocketPosition - targetBallPosition);
            float mag = (targetBallPosition - cueBallPosition).magnitude + (pocketPosition - targetBallPosition).magnitude;
            return Mathf.Min(1, mag / Pools.GetTableSize().x);
        }

        private Vector3 GetRandomHitPoint()
        {
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
        public PoolBall targetBall;
        public PocketTrigger targetPocket;

        public Dictionary<PoolBall, List<PocketTrigger>> considerBalls = new Dictionary<PoolBall, List<PocketTrigger>>();
        public List<KeyValuePair<PoolBall, Vector3>> shootableBalls = new List<KeyValuePair<PoolBall, Vector3>>();
    }
    #endregion //Decider

    #region Executor
    protected class AIExecutor
    {
        //not use now
        public AIPlayer player;

        DecidedMessage msg;

        Queue<ExecuteBehaviour> behaviours;

        public bool deQueue;

        public AIExecutor(AIPlayer _player)
        {
            player = _player;
            behaviours = new Queue<ExecuteBehaviour>();
            deQueue = false;
        }

        public void SetDecision(DecidedMessage _msg)
        {
            msg = _msg;
            if(msg.drag)
            {
                    if (Random.Range(0, 3) <= 2)
                        behaviours.Enqueue(new ExecuteDragThink(msg, player));
                behaviours.Enqueue(new ExecuteDrag(msg, player));
            }
            behaviours.Enqueue(new ExecuteThink(msg, player, Random.Range(1.1f,1.5f)));
            for (int i = 0; i < 3; i++ )
            {
                if (Random.Range(0, 3) <=2)
                    behaviours.Enqueue(new ExecuteAimThink(msg, player));
            }
            behaviours.Enqueue(new ExecuteAim(msg, player));
            behaviours.Enqueue(new ExecuteThink(msg, player));
            behaviours.Enqueue(new ExecuteFire(msg, player));
        }

        public void Execute()
        {
            if (behaviours.Count == 0)
                return;

            if (behaviours.Peek().Preparing())
            {
                behaviours.Dequeue().Do();
            }
        }
    }

    protected abstract class ExecuteBehaviour
    {
        protected DecidedMessage msg;

        protected AIPlayer player; 

        public ExecuteBehaviour(DecidedMessage _msg, AIPlayer _player)
        {
            msg = _msg;
            player = _player;
        }


        public abstract void Do();

        public virtual bool Preparing() { return true; }
    }

    protected class ExecuteDrag : ExecuteBehaviour
    {
        public ExecuteDrag(DecidedMessage _msg, AIPlayer _player) : base(_msg, _player) { }

        public override void Do()
        {
            iTween.MoveTo(Pools.CueBall.gameObject, msg.cueballPosition, 2);
        }
    }

    protected class ExecuteDragThink : ExecuteThink
    {
        public ExecuteDragThink(DecidedMessage _msg, AIPlayer _player)
            : base(_msg, _player)
        {
            m_ThinkTime = 0.5f;
        }

        public override void Do()
        {
            Vector3 v = new Vector3(Random.Range(0,.5f), 0, Random.Range(0,.5f));
            iTween.MoveTo(Pools.CueBall.gameObject, msg.cueballPosition + v, 2);
        }
    }

    protected class ExecuteThink : ExecuteBehaviour
    {
        protected float m_ThinkTime;
        public ExecuteThink(DecidedMessage _msg, AIPlayer _player) : base(_msg, _player) 
        {
            m_ThinkTime = Random.Range(2, 3);
        }

        public ExecuteThink(DecidedMessage _msg, AIPlayer _player, float _thinkTime) : base(_msg, _player)
        {
            m_ThinkTime = _thinkTime;
        }
        public override void Do() { }
        public override bool Preparing()
        {
            m_ThinkTime -= Time.deltaTime;
            if (m_ThinkTime <= 0)
                return true;
            return false;
        }
    }

    protected class ExecuteAimThink : ExecuteThink
    {
        public ExecuteAimThink(DecidedMessage _msg, AIPlayer _player) : base(_msg, _player) 
        {
            m_ThinkTime = 1;
        }

        public override void Do()
        {
            //List<PoolBall> list = player.BallsList;
            float angle = BaseUIController.cueOperateArea.pointerOperation.GetRotationAngleAtWorld(msg.hitPoint + msg.targetBall.GetRadius() * Random.onUnitSphere);
            Vector3 v = new Vector3(0, Pools.Cue.transform.eulerAngles.y + angle, 0);
            iTween.RotateTo(Pools.Cue.gameObject, v ,1);
        }
    }

    protected class ExecuteAim : ExecuteBehaviour
    {
        public ExecuteAim(DecidedMessage _msg, AIPlayer _player) : base(_msg, _player) { }
        public override void Do()
        {
            GameManager.Rules.State = GlobalState.IDLE;
            float angle = BaseUIController.cueOperateArea.pointerOperation.GetRotationAngleAtWorld(msg.hitPoint);
            Vector3 v = new Vector3(0, Pools.Cue.transform.eulerAngles.y + angle, 0);
            iTween.RotateTo(Pools.Cue.gameObject, v, 1);
        }
    }

    protected class ExecuteFire : ExecuteBehaviour
    {
        public ExecuteFire(DecidedMessage _msg, AIPlayer _player) : base(_msg, _player) 
        {
        }

        private float m_Time = .2f;
        private float _Time;

        public override void Do()
        {
            BaseUIController.cueAndLines.ReleaseCue();
        }

        public override bool Preparing()
        {
            //if (m_Push)
            //{
            BaseUIController.cueAndLines.AdjustingChange(Mathf.Lerp(0, msg.powerScale, _Time / m_Time));
            _Time += Time.deltaTime;
            if (_Time > m_Time)
            {
                //m_Push = false;
                return true;
            }
            //}
            return false;
        }
    }
    #endregion //Executor

    protected AIExecutor m_Executor;
    protected AICollector m_Collector;
    protected AIDecider m_Decider;

    public Vector3 hitpoint;

    public static AIDifficulty difficulty = AIDifficulty.Medium;

    [SerializeField]
    [Range(0,0.05f)]
    private float m_Deviation;

    protected override void Awake()
    {
        base.Awake();
        m_Executor = new AIExecutor(this);
        m_Collector = new AICollector(this);
        m_Decider = new AIDecider(this);
    }

    public override void Begin()
    {
        if (GameManager.Rules.State == GlobalState.GAMEOVER)
            return;

        m_Outline.enabled = true;
        BaseUIController.GlobalMask = true;
        DecidedMessage decideMsg = m_Decider.Decide(m_Collector.Collect());
        hitpoint = decideMsg.hitPoint;
        m_Executor.SetDecision(decideMsg);
    }

    public override void End()
    {
        base.End();
    }

    public override void PlayerUpdate()
    {
        m_Executor.Execute();
    }
}
