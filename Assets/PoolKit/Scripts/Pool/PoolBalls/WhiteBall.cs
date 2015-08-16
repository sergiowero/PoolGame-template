using UnityEngine;
using System.Collections;
namespace PoolKit
{
	public class WhiteBall : PoolBall
	{
        private BallDragger m_BallDragger;

        private static WhiteBall m_Instance = null;

        public static bool CueBallSiding = false;

		public PoolBall ball;
		public SphereCollider sphereCollider;

		Vector3 screenPoint;
		Vector3 offset;
		public float dragThreshold = 20;
		public PoolCue m_cue;

		private Constraint m_constraint;
		private bool m_hitBall=false;
		private PoolBall m_targetBall;
		private Vector3 m_targetPos;

        //private bool m_fired=false;
		public LayerMask layermask;
		public bool foul=true;

        [SerializeField]
        private float power = 20;
		public override void Awake()
		{
            if(m_Instance == null)
            {
                base.Awake();
                m_Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
		}
		//lets get the radius
		public float getRadius()
		{
            return sphereCollider.radius * transform.localScale.x - ConstantData.BallRadiusAdjustment;//BallRadiusAdjustment is the collision adjustment, otherwise colliding can not be happen
		}

        public static float GetRadius()
        {
            return m_Instance.getRadius();
        }

        public static Vector3 GetPosition()
        {
            return m_Instance.transform.position;
        }

        public static Vector3 GetScreenPosition()
        {
            Vector3 v = Camera.main.WorldToScreenPoint(GetPosition());
            v.z = 0;
            return v;
        }

		public void setPoolCue(PoolCue cue)
		{
			if(m_cue && cue!=m_cue)
			{
				m_cue.gameObject.SetActive(false);
			}else{
				cue.gameObject.SetActive(true);
			}
			m_cue = cue;
		}

		public override void Start()
		{
			base.Start();
            InitBallDragger();
			sphereCollider = gameObject.GetComponent<SphereCollider>();
			ball = gameObject.GetComponent<PoolBall>();
			m_constraint = gameObject.GetComponent<Constraint>();
			m_constraint.enabled=false;
            m_constraint.adjustment = getRadius();
            m_cue = (PoolCue)GameObject.FindObjectOfType(typeof(PoolCue));
            stopBall();
            PoolGameScript8Ball.Shuffle();
		}

        private void InitBallDragger()
        {
            m_BallDragger = gameObject.GetComponent<BallDragger>();
            if (!m_BallDragger)
                m_BallDragger = gameObject.AddComponent<BallDragger>();
            m_BallDragger.layermask = gameObject.layer;
            m_BallDragger.dragBegin = OnDragBegin;
            m_BallDragger.drag = OnDrag;
            m_BallDragger.dragEnd = OnDragEnd;
        }

		//the mouse is down lets get the screen point and offset
		void OnDragBegin(BallDraggerData data)
		{
			if(foul && PoolGameScript8Ball.Instance.GlobalState != PoolGameScript.State.ROLLING )
			{
				m_constraint.enabled=true;
				m_rigidbody.useGravity=false;
				screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(data.Position.x, data.Position.y, screenPoint.z));
                PoolGameScript8Ball.Instance.GlobalState = PoolGameScript.State.DRAG_WHITEBALL;
			}	
		}

        void OnDragEnd(BallDraggerData data)
		{
            if (foul && PoolGameScript8Ball.Instance.GlobalState != PoolGameScript.State.ROLLING)
			{
				m_rigidbody.useGravity=true;
				m_constraint.enabled=false;
                if (m_cue)
                    m_cue.Show();
                collider.isTrigger = false;
                PoolGameScript8Ball.Instance.ReturnPrevousState();
			}
		}

        void OnDrag(BallDraggerData data)
		{
            if (foul && PoolGameScript8Ball.Instance.GlobalState != PoolGameScript.State.ROLLING)
			{
                Vector3 curScreenPoint = new Vector3(data.Position.x, data.Position.y, screenPoint.z);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                curPosition.y = transform.position.y;

                Collider[] contantacts = Physics.OverlapSphere(curPosition, getRadius(), layermask.value);
                if (contantacts.Length == 0)
                {
                    transform.position = curPosition;
                    collider.isTrigger = true;
                    if (m_cue)
                        m_cue.Hide();
                }
			}
		}

		public bool isRolling()
		{
			return m_state==State.ROLL;
		}

		public void setTarget(PoolBall ball, Vector3 targetPos)
		{
			if(ball)
			{
				m_targetBall = ball;
				m_targetPos = targetPos;
				m_targetPos.y = transform.position.y;
			}
		}
		public override void OnCollisionEnter (Collision col){
            //float vel = Mathf.Max(rigidbody.velocity.x,rigidbody.velocity.z);
            string name = col.gameObject.name;
            if(!name.Contains("surface"))
                StartCoroutine("TouchTable", 0);
            if (name.Contains("Rail"))
            {
                //we hit the wall.
                PoolKit.BaseGameManager.ballHitWall(rigidbody.velocity);
                hitWall = true;
            }
            if (name.Contains("Ball"))
			{

				PoolBall ball = col.gameObject.GetComponent<PoolBall>();
//				Debug.Log ("whiteBallHit"+ ball.name + m_hitBall);

				if(ball && m_hitBall==false)
				{
					PoolKit.BaseGameManager.whiteBallHitBall(m_hitBall,ball);
					m_hitBall=true;
				}
				//we hit our target, lets set it to the target position
				if(ball && ball==m_targetBall)
				{
					PoolKit.BaseGameManager.ballHitBall(rigidbody.velocity);
					m_targetBall.pointAtTarget(m_targetPos);
					m_targetBall=null;
				}
			}
			
		}
		public void clear()
		{
			m_hitBall=false;
			m_rigidbody.constraints = RigidbodyConstraints.None;
			m_slowTime=0;
			m_state = State.IDLE;

            transform.rotation = Quaternion.identity;
			if(m_rigidbody)
			{
				m_rigidbody.isKinematic=false;
				m_rigidbody.angularVelocity = Vector3.zero;
				m_rigidbody.velocity = Vector3.zero;
				m_rigidbody.isKinematic=true;
			}
            //m_fired=false;
		}
		public void reset()
		{
            gameObject.SetActive(true);
			m_hitBall=false;
			m_rigidbody.constraints = RigidbodyConstraints.None;
			m_slowTime=0;
			m_state = State.IDLE;
            //m_fired=false;
			transform.position = m_initalPos;
			transform.rotation = Quaternion.identity;
			if(m_rigidbody)
			{
				m_rigidbody.isKinematic=false;
				m_rigidbody.angularVelocity = Vector3.zero;
				m_rigidbody.velocity = Vector3.zero;
				m_rigidbody.isKinematic=true;
			}


		}

		public void stopBall()
		{
            transform.rotation = Quaternion.identity;
			if(m_rigidbody)
			{
				m_rigidbody.isKinematic=false;
				m_rigidbody.angularVelocity = Vector3.zero;
				m_rigidbody.velocity = Vector3.zero;
				m_rigidbody.isKinematic=true;
			}

		}

		public override void onBallStop()
		{
			base.onBallStop();
            transform.rotation = Quaternion.identity;

		}

        public void fireBall(float powerScalar)
		{
            _fireBall(powerScalar);
		}
		
		void _fireBall(float powerScalar)
		{
			m_rigidbody.isKinematic=false;
            if(powerScalar > 1)
            {
                m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
                m_rigidbody.useGravity = false;
                StartCoroutine("TouchTable", powerScalar * .1f);
            }
            //m_fired=true;
			m_slowTime=0;
			m_rigidbody.AddForce(PoolCue.GetForward() * powerScalar * power , ForceMode.Impulse);
			m_rigidbody.AddTorque(ballTorque);
			m_state = State.ROLL;
			PoolKit.BaseGameManager.fireBall();
            Debugger.Tracker tracker = Debugger.DBERP.GetComponentWithType(Debugger.DebuggerType.Tracker, gameObject) as Debugger.Tracker;
            tracker.Clear();
            tracker.SetTransform(transform);
            ballTorque = Vector3.zero;
            Siding.ResetAnchorOffset();
		}

        IEnumerator TouchTable(float time)
        {
            yield return new WaitForSeconds(time);
            m_rigidbody.constraints = RigidbodyConstraints.None;
            m_rigidbody.useGravity = true;
        }

        public static void SetTorque(Vector3 torque)
        {
            m_Instance._SetTorque(torque);
        }

        private void _SetTorque(Vector3 torque)
        {
            ballTorque = torque;
        }
	}
}