using UnityEngine;
using System.Collections;
public class PoolBall : MonoBehaviour 
	{
        //the balls Id
        [SerializeField]
        protected int m_BallID = -1;

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
        public bool hitWall = false;

		//has the ball been pocketed
		public bool pocketed=false;

        protected BallShadowRenderer m_ShadowRenderer;

        //protected BallPhysicalDrag m_BallPhysicalDrag;
        protected PhysicalSupportTools m_BallPhysicalDrag;

        protected PhysicMaterial m_PhysicMaterial;

        [SerializeField]
        private float m_MaxYAxis;


		public virtual void Awake()
		{
			m_rigidbody =gameObject.GetComponent<Rigidbody>();
            m_ShadowRenderer = GetComponent<BallShadowRenderer>();
            //m_BallPhysicalDrag = GetComponent<BallPhysicalDrag>();
            m_PhysicMaterial = collider.sharedMaterial;
		}
		public virtual void Start() 
		{
			m_initalPos = transform.position;
			m_initalRot = transform.rotation;
            m_BallPhysicalDrag = PhysicalSupportTools.PhysicalDragTo(gameObject, ConstantData.GetPoolDatas().BallDrag, ConstantData.GetPoolDatas().BallAngularDrag);
            m_rigidbody.useConeFriction = true;
		}

        IEnumerator RecordYValue()
        {
            yield return new WaitForSeconds(1);
            m_MaxYAxis = m_rigidbody.position.y;
            yield return null;
        }

        private void YValueDrag()
        {
            Vector3 v = m_rigidbody.position;
            v.y = m_MaxYAxis;
            m_rigidbody.position = v;
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
                //BaseGameManager.ballHitWall(rigidbody.velocity);
                if(!hitWall)
                {
                    GameManager.Rules.BallHitRail();
                    hitWall = true;
                }
			}
			if (col.gameObject.name.Contains("Ball"))
			{
                //BaseGameManager.ballHitBall(rigidbody.velocity);
			}
		}

		public void OnEnable()
		{
            PoolRulesBase.onFireBall += OnFireBall;
            PoolRulesBase.onNewTurn += OnNewTurn;
            OpenRenderer();
		}


		public void OnDisable()
		{
            PoolRulesBase.onFireBall -= OnFireBall;
            PoolRulesBase.onNewTurn -= OnNewTurn;
		}

		public void OnFireBall()
		{
            m_slowTime = 0;
		}

        public void OnNewTurn(int turnIndex)
        {
            hitWall = false;
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

        void LateUpdate()
        {
            if (m_rigidbody.position.y > m_MaxYAxis)
                YValueDrag();
        }

		public void Potted()
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
            ReversePhysicalMaterial();
            PhysicalSupportTools.Remove(gameObject, PhysicalSupportType.MaxSpeedLimit);
            enabled = true;
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
