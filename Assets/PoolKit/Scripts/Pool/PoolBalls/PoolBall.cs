using UnityEngine;
using System.Collections;
namespace PoolKit
{

	public class PoolBall :MonoBehaviour 
	{
		public enum BallType
		{
			STRIPE,
			SOLID,
			BLACK,
			WHITE
		};
		//the type of ball
		public BallType ballType;

		//the ball texture to use.
		public Texture ballTex;
		public Vector3 ballTorque;
//		protected PhotonView m_view;
		protected Rigidbody m_rigidbody;

		//the inital position
		protected Vector3 m_initalPos;

        protected Transform m_Shadow;
        protected Transform m_ShadowRenderer;
        protected Vector3 m_ShadowRendererLocalPosition;

        [SerializeField]
        protected float m_DecreaseFactor;

		//the inital rotation 
		protected Quaternion m_initalRot;
		public enum State
		{
			IDLE,
			ROLL,
			DONE
		};

		//what state is the ball in
		protected State m_state;

		//the minimum speed
		public float minSpeed = 0.5f;

        public float m_MinAngularSpeed = .1f;

		//the current time the ball has to be slowed down
		protected float m_slowTime;

		//the time the ball has to be slowed down before its considered stopped
		public float slowTime = 1;

		//the balls last position
		protected Vector3 lastPosition = Vector3.zero;

        protected Vector3 lastEuler = Vector3.zero;

		//the balls currnet speed.
		protected float Speed = 0;

        protected float angularSpeed = 0;
		
		//did we hit the wall.
		public bool hitWall=false;

		//has the ball been pocketed
		public bool pocketed=false;

		//the balls index
		public int ballIndex=-1;
		public virtual void Awake()
		{
			m_rigidbody =gameObject.GetComponent<Rigidbody>();
            m_Shadow = transform.FindChild("Shadow");
            if (m_Shadow)
            {
                m_ShadowRenderer = m_Shadow.FindChild("Renderer");
                m_ShadowRendererLocalPosition = m_ShadowRenderer.localPosition;
                RenderShadow();
            }
		}
		public virtual void Start()
		{
			m_initalPos = transform.position;
			m_initalRot = transform.rotation;
			m_rigidbody.useConeFriction=true;
			//m_view = gameObject.GetComponent<PhotonView>();
			
			if(name.Length>4 && ballType!=BallType.WHITE)
			{
				string str = name.Substring(4,name.Length-4);
				ballIndex = int.Parse(str);
			}
		}
		//point the ball at the target
		public void pointAtTarget(Vector3 target)
		{
			Vector3 vel = m_rigidbody.velocity;
			float mag = vel.magnitude;
			Vector3 newDir = target - transform.position;
			m_rigidbody.velocity = newDir.normalized * mag;
		}


		public virtual void OnCollisionEnter (Collision col){
            if (col.gameObject.name.Contains("Rail"))
			{
				//we hit the wall.
				PoolKit.BaseGameManager.ballHitWall(rigidbody.velocity);
				hitWall=true;
			}
			if (col.gameObject.name.Contains("Ball"))
			{
				PoolKit.BaseGameManager.ballHitBall(rigidbody.velocity);
			}
			
		}


		public void OnEnable()
		{
			PoolKit.BaseGameManager.onBallStop += onBallStop;
			PoolKit.BaseGameManager.onFireBall	+= onFireBall;
		}
		public void OnDisable()
		{
			PoolKit.BaseGameManager.onBallStop -= onBallStop;
			PoolKit.BaseGameManager.onFireBall	-= onFireBall;
		}
		public void onFireBall()
		{
            m_rigidbody.isKinematic = false;
            m_slowTime = 0;
		}
		public void Update()
		{
                if (m_rigidbody.velocity.sqrMagnitude < .01f && m_rigidbody.angularVelocity.sqrMagnitude < .01f)
                {
                    if(m_slowTime < slowTime)
                        m_slowTime += Time.deltaTime;
                    if (m_slowTime >= slowTime)
                    {
                        m_state = State.DONE;
                    }
                }
                else
                {
                    m_state = State.ROLL;
                    m_slowTime = 0;
                }
		}

        // LateUpdate is called every frame, if the Behaviour is enabled
        public void LateUpdate()
        {
            if (m_Shadow)
            {
                RenderShadow();
            }
        }

        private void RenderShadow()
        {
            m_Shadow.rotation = Quaternion.identity;
            m_ShadowRendererLocalPosition.x = (transform.position.x - LightSource.Position.x) * .0015f;
            m_ShadowRendererLocalPosition.z = (transform.position.z - LightSource.Position.z) * .0015f;
            m_ShadowRenderer.localPosition = m_ShadowRendererLocalPosition;
        }

		void FixedUpdate()
		{
		}

		
		public void enterPocket()
		{
			if(m_rigidbody)
			{
				pocketed=true;
				//we entered a pocket lets freeze the x and z constraints so it doesnt bounce around. 
				m_rigidbody.velocity = Vector3.zero;		
				m_state = State.DONE;
				if(ballType!=BallType.WHITE)
				{
					Destroy(gameObject);
				}else{
                    //transform.position = m_initalPos;
                    gameObject.SetActive(false);
				}
			}
		}
		public virtual void onBallStop()
		{
			m_rigidbody.angularVelocity = Vector3.zero;
			m_rigidbody.velocity = Vector3.zero;
			m_rigidbody.isKinematic=true;
		}

		public bool isDoneRolling()
		{
			return m_state == State.DONE;
		}


	}
}
