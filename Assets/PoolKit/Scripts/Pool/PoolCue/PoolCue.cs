using UnityEngine;
using System.Collections;
using Debugger;


namespace PoolKit
{
	//the pool cues that you should use should depend on which game you are using -- 8 ball or 9 ball the only real difference is it will check if the ball is okay or not.
	public class PoolCue : MonoBehaviour {
        private static PoolCue m_Instance = null;
		//our line renderer
        //public LineRenderer lineRenderer;

		//ref to the white ball
		protected WhiteBall m_whiteBall;

		//our layer mask
		public LayerMask layerMask;

        //current white ball side
        private Vector2 m_CurrentSideOffset;
        [SerializeField]
        private Vector3 m_RefPoint = new Vector3(0, 0, -1);

		public enum State
		{
			ROTATE,
			ROLL
		};

		//the current state
		protected State m_state;

		/// <summary>
		/// The pool cue model
		/// </summary>
		public GameObject poolCueGO;

		//the audio to play when we hit a pool cue
		public AudioClip onHitCueAC;

		//the inital rotation
		protected Quaternion m_initalRot;

		//the inital psoition
		protected Vector3 m_initalPos;

		//the power scale -- between 1 and 100
		protected float m_powerScalar = 1f;

		//the minimum power we want ot use for this shot. 
		public float minPowerScalar = 0.125f;

		//the gui skin
		public GUISkin skin0;

		//the target ball
		protected PoolBall m_targetBall;

		//the taret position
		protected Vector3 m_targetPos;

        protected float m_CurRotAngle;
        public float CurRotAngle
        {
            set 
            { 
                m_CurRotAngle += value;
                m_CurRotAngle = MathTools.Roll(0, 360, m_CurRotAngle);
            }
        }

		//do we want to rotate
		protected bool m_requestRotate = false;

		//do we want to fire the ball
		protected bool m_requestFire = false;

		//are all the balls down
		public bool areAllBallsDown=false;

		//are we greater then 8
		public int greaterThen8 = 0;

		public void Awake()
		{
            if (m_Instance)
            {
                return;
            }

            m_Instance = this;

            m_initalPos = transform.localPosition;
            m_initalRot = transform.localRotation;
            m_whiteBall = transform.parent.GetComponentInChildren<WhiteBall>();
		}
		public void OnEnable()
		{
			PoolKit.BaseGameManager.onBallStop += onBallStop;
			PoolKit.BaseGameManager.onGameStart += onStartGame;
            FireSlider.OnSliderValueChange += SetPowerScalar;
            FireSlider.OnSliderRelease += requestFire;
			//PoolKit.BaseGameManager.onBallHitBall += onBallHitBall;
		}
		public void OnDisable()
		{
			PoolKit.BaseGameManager.onBallStop -= onBallStop;
			PoolKit.BaseGameManager.onGameStart -= onStartGame;
            FireSlider.OnSliderValueChange -= SetPowerScalar;
            FireSlider.OnSliderRelease -= requestFire;
		}
		public void setPowerAI(float power)
		{
			m_powerScalar = power * 0.01f;
		}

        public void SetPowerScalar(float value)
        {
            m_powerScalar = value;
        }

		public void onStartGame()
		{
            //m_whiteBall =transform.parent.GetComponentInChildren<WhiteBall>();
            //if (m_whiteBall)
            //{
            //    transform.parent = m_whiteBall.transform;
            //}
            //transform.localScale = new Vector3(1, 1, 1);

            //transform.localRotation = m_initalRot;
            //transform.localPosition = m_initalPos;
            //if(lineRenderer && m_whiteBall)
            //    lineRenderer.SetPosition(0,m_whiteBall.transform.position);
		}

		
		void onBallStop()
		{
			ballStopRPC();
		}
		public virtual void ballStopRPC()
		{
			m_state = State.ROTATE;

			if(m_whiteBall)
			{
				transform.parent = m_whiteBall.transform;
			}
			transform.localScale = new Vector3(1,1,1);

			transform.localRotation = m_initalRot;
			transform.localPosition = m_initalPos;
		}
		public virtual  void requestRotateRPC(){m_requestRotate=true;}
		public void requestRotate(){
			requestRotateRPC();
		}
		public virtual void requestFireRPC(){m_requestFire=true;}
		public  void requestFire(){
			requestFireRPC();
		}
        void Update () {
            if(m_state==State.ROTATE)
            {

                if(poolCueGO)
                {
                    Vector3 pos = Vector3.zero;
                    pos.z = Mathf.Lerp(-.005f,-.065f,m_powerScalar);
                    poolCueGO.transform.localPosition = pos;
                }

                if(m_requestRotate)
                {
                    handleRotate();
                    m_requestRotate=false;
                }
                if(m_requestFire)
                {
                    fireBall();
                    m_requestFire=false;
                }

            }

        }


		public void fireBall()
		{
			fireBallRPC();
		}

		public void fireBallRPC()
		{
			m_requestFire=false;
			if(audio)
			{
				audio.PlayOneShot(onHitCueAC,1f);
			}

			//lets set the balls target and the target position. When the white ball hits the first ball we will set the ball to point at the target.
			m_whiteBall.setTarget(m_targetBall,m_targetPos);

			Debug.Log ("FIRE BALL" + m_whiteBall.name);
			m_whiteBall.fireBall(m_powerScalar);
			m_state = State.ROLL;
            poolCueGO.SetActive(false);
            Guidelines.HideAllObjects();
			
			transform.parent = null;

		}

		public void setTarget(PoolBall ball, Vector3 p2)
		{
            //poolCueGO.SetActive(true);
            //lineRenderer.SetColors(Color.green,Color.green);
            //m_lr2.SetColors(Color.blue,Color.blue);
            //lineRenderer.SetPosition(0,m_whiteBall.transform.position);
            //lineRenderer.SetPosition(1,ball.transform.position);


            //if(m_lr2)
            //{
            //    m_lr2.gameObject.SetActive(true);
            //    m_lr2.SetPosition(0,ball.transform.position);		
            //    m_lr2.SetPosition(1,p2);
            //}
		}

		public virtual bool isBallOkay(PoolBall ball)
		{
			return true;
		}
        
        //public void OnDrawGizmos()
        //{ 
        //    Gizmos.DrawSphere(m_targetPos, WhiteBall.GetRadius());
        //}


		void handleRotate()
		{
			if(m_whiteBall && m_whiteBall.sphereCollider)
			{
				poolCueGO.SetActive(true);
                
				SphereCollider sc = m_whiteBall.sphereCollider;
				Ray ray = new Ray(m_whiteBall.transform.position,transform.forward);
                float r = WhiteBall.GetRadius();
				RaycastHit rch;
				if(Physics.SphereCast(ray,r - Mathf.Epsilon,out rch,1000f,layerMask.value))
				{
					Vector3 pos = rch.point;

                    Guidelines.GuidePointerAt(rch.point, rch.transform, rch.normal);

					Vector3 vec =  pos-sc.transform.position;

                    pos = sc.transform.position + vec.normalized * (vec.magnitude - r);

					Vector3 nrm = rch.normal;
					nrm.y=0;
					m_targetBall=null;

					if(rch.collider.name.Contains("Ball"))
					{
                        m_targetBall = rch.transform.GetComponent<PoolBall>();
						m_targetPos = rch.point - nrm;
					}
				}
			 }
		}

        public static Vector3 GetPosition()
        {
            return m_Instance.transform.position;
        }

        public static Transform GetTransform()
        {
            return m_Instance.transform;
        }

        public static Vector3 GetForward()
        {
            return m_Instance.transform.forward;
        }

        void _Siding(Vector2 sideOffset)
        {
            if(sideOffset != Vector2.zero)
            {
                sideOffset.x *= .5f;
                Vector3 v1 = transform.localToWorldMatrix.MultiplyVector(new Vector3(sideOffset.x, sideOffset.y, 0));
                Vector3 v2 = transform.localToWorldMatrix.MultiplyVector(m_RefPoint);
                Vector3 v3 = Vector3.Cross(v2, v1);
                WhiteBall.SetTorque(v3.normalized * sideOffset.sqrMagnitude * .375f); 
            }
            m_CurrentSideOffset = sideOffset;
        }

        private void _ResetSideOffset()
        {
            m_CurrentSideOffset = Vector2.zero;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Guidelines.HideAllObjects();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            //Guidelines.ShowAllObjects();
        }

        public static void ResetSideOffset()
        {
            m_Instance._ResetSideOffset();
        }

        public static void Siding(Vector2 sideOffset)
        {
            m_Instance._Siding(sideOffset);
        }

        public static void UpdateSiding()
        {
            m_Instance._Siding(m_Instance.m_CurrentSideOffset);
        }
	}
}
