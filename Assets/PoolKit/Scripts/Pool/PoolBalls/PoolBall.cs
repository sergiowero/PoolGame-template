using UnityEngine;
using System.Collections;
	public class PoolBall :MonoBehaviour 
	{
        //the balls Id
        [SerializeField]
        protected int m_BallID = -1;

        public enum BallType
		{
			STRIPE,
			SOLID,
			BLACK,
			WHITE
		};
		//the type of ball
		public BallType ballType;

		public Vector3 ballTorque;
		protected Rigidbody m_rigidbody;

		//the inital position
		protected Vector3 m_initalPos;

		//the inital rotation 
		protected Quaternion m_initalRot;
		public enum State
		{
			ROLL,
			IDLE
		};

		//what state is the ball in
		protected State m_state;

		//the current time the ball has to be slowed down
		protected float m_slowTime;

		//the time the ball has to be slowed down before its considered stopped
		public float slowTime = 1;

		//did we hit the wall.
		public bool hitWall=false;

		//has the ball been pocketed
		public bool pocketed=false;

        protected BallShadowRenderer m_ShadowRenderer;

        protected BallPhysicalDrag m_BallPhysicalDrag;

        protected PhysicMaterial m_PhysicMaterial;

		public virtual void Awake()
		{
			m_rigidbody =gameObject.GetComponent<Rigidbody>();
            m_ShadowRenderer = GetComponent<BallShadowRenderer>();
            m_BallPhysicalDrag = GetComponent<BallPhysicalDrag>();
            m_PhysicMaterial = collider.sharedMaterial;
		}
		public virtual void Start() 
		{
			m_initalPos = transform.position;
			m_initalRot = transform.rotation;
			m_rigidbody.useConeFriction=true;
		}


		//point the ball at the target
		public void PointAtTarget(Vector3 target)
		{
			Vector3 vel = m_rigidbody.velocity;
			float mag = vel.magnitude;
			Vector3 newDir = target - transform.position;
			m_rigidbody.velocity = newDir.normalized * mag;
		}

        public int GetBallID()
        {
            return m_BallID;
        }

		public virtual void OnCollisionEnter (Collision col){
            if (col.gameObject.name.Contains("Rail"))
			{
				//we hit the wall.
				BaseGameManager.ballHitWall(rigidbody.velocity);
				hitWall=true;
			}
			if (col.gameObject.name.Contains("Ball"))
			{
				BaseGameManager.ballHitBall(rigidbody.velocity);
                //m_ShadowRenderer.enabled = true;
			}
		}

		public void OnEnable()
		{
			BaseGameManager.onBallStop += OnBallStop;
			BaseGameManager.onFireBall	+= OnFireBall;
            OpenRenderer();
		}


		public void OnDisable()
		{
			BaseGameManager.onBallStop -= OnBallStop;
			BaseGameManager.onFireBall	-= OnFireBall;
		}

		public void OnFireBall()
		{
            m_slowTime = 0;
		}

		public void Update()
        {
            if (m_rigidbody.velocity.sqrMagnitude < .01f && m_rigidbody.angularVelocity.sqrMagnitude < .01f)
            {
                if (m_state == State.ROLL)
                {
                    if (m_slowTime < slowTime)
                        m_slowTime += Time.deltaTime;
                    if (m_slowTime >= slowTime)
                    {
                        m_state = State.IDLE;
                        if (ballType != BallType.WHITE) m_ShadowRenderer.enabled = false;
                        CloseDrag();
                    }
                }
            }
            else
            {
                if(m_state != State.ROLL)
                {
                    OpenDrag();
                    if (ballType != BallType.WHITE) m_ShadowRenderer.enabled = true;
                    m_state = State.ROLL;
                    m_slowTime = 0;
                }
            }
		}

		public void EnterPocket()
		{
			if(m_rigidbody)
			{
				pocketed=true;
				m_rigidbody.velocity = Vector3.zero;
                m_rigidbody.angularVelocity = Vector3.zero;
				m_state = State.IDLE;
                CloseDrag();
                CloseRenderer();
                enabled = false;
                RemovePhysicalMaterial();
                //gameObject.SetActive(false);
			}
		}

        public virtual void Reset()
        {
            pocketed = false;
            m_slowTime = 0;
            m_state = State.IDLE;
            transform.position = m_initalPos;
            transform.rotation = m_initalRot;
            m_rigidbody.constraints = RigidbodyConstraints.None;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.velocity = Vector3.zero;
            OpenRenderer();
            enabled = true;
        }

		public virtual void OnBallStop()
		{
			m_rigidbody.angularVelocity = Vector3.zero;
			m_rigidbody.velocity = Vector3.zero;
		}

		public bool IsDoneRolling()
		{
			return m_state == State.IDLE;
		}

        public void CloseRenderer()
        {
            if(m_ShadowRenderer) m_ShadowRenderer.Close();
        }

        public void OpenRenderer()
        {
            if(m_ShadowRenderer) m_ShadowRenderer.Open();
        }

        public void CloseDrag()
        {
            m_BallPhysicalDrag.enabled = false;
        }

        public void OpenDrag()
        {
            m_BallPhysicalDrag.enabled = true;
        }

        public void RemovePhysicalMaterial()
        {
            collider.sharedMaterial = null;
        }

        public void ReversePhysicalMaterial()
        {
            collider.sharedMaterial = m_PhysicMaterial;
        }
	}
