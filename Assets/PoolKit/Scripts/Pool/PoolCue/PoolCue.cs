using UnityEngine;
using System.Collections;
using Debugger;


	//the pool cues that you should use should depend on which game you are using -- 8 ball or 9 ball the only real difference is it will check if the ball is okay or not.
	public class PoolCue : MonoBehaviour {
        private static PoolCue m_Instance = null;
		//our line renderer
        //public LineRenderer lineRenderer;

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

        [SerializeField]
        protected Transform m_CueImage;

		//the inital rotation
		protected Quaternion m_initalRot;

		//the inital psoition
		protected Vector3 m_initalPos;

		//the power scale -- between 1 and 100
		protected float m_powerScalar = 1f;

		//the minimum power we want ot use for this shot. 
		public float minPowerScalar = 0.125f;

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

        #region Gizmos--------------------------------------------
        [SerializeField]
        protected Color m_GizmosColor;
        #endregion


        public void Awake()
		{
            if (m_Instance)
            {
                return;
            }

            m_Instance = this;

            m_initalPos = transform.localPosition;
            m_initalRot = transform.localRotation;
            m_CurRotAngle = 0;
            FireSlider.OnSliderValueChange += SetPowerScalar;
            FireSlider.OnSliderRelease += Fire;
        }

        public void OnDestroy()
        {
            FireSlider.OnSliderValueChange -= SetPowerScalar;
            FireSlider.OnSliderRelease -= Fire;
        }
        
		public void setPowerAI(float power)
		{
			m_powerScalar = power * 0.01f;
		}

        public void SetPowerScalar(float value)
        {
            m_powerScalar = value;
            BaseUIController.cueAndLines.AdjustingCue(value);
        }
		
		void OnBallStop()
		{
            m_state = State.ROTATE;
            transform.parent = Pools.CueBall.transform;
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = m_initalRot;
            transform.localPosition = m_initalPos;
		}

		public void Fire()
		{
			m_requestFire=false;

			//lets set the balls target and the target position. When the white ball hits the first ball we will set the ball to point at the target.
            Pools.CueBall.setTarget(m_targetBall, m_targetPos);
            Pools.CueBall.fireBall(m_powerScalar);
			m_state = State.ROLL;
            BaseUIController.cueAndLines.gameObject.SetActive(false);
			transform.parent = null;
		}

		public virtual bool isBallOkay(PoolBall ball)
		{
			return true;
		}

        public void Rotate(float angle)
        {
            CurRotAngle = angle;
            transform.RotateAround(Pools.CueBall.GetPosition(), Vector3.up, angle);
            HandleRotate();
            UpdateSiding();
        }

		void HandleRotate()
		{
            if (Pools.CueBall && Pools.CueBall.sphereCollider)
			{
                SphereCollider sc = Pools.CueBall.sphereCollider;
                Ray ray = new Ray(Pools.CueBall.transform.position, transform.forward);
                float r = Pools.CueBall.GetRadius();
				RaycastHit rch;
				if(Physics.SphereCast(ray,r - Mathf.Epsilon,out rch,1000f,layerMask.value))
				{
					Vector3 pos = rch.point;

                    BaseUIController.cueAndLines.GuidePointerAt(rch.point, rch.transform, rch.normal);

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

        public void Siding(Vector2 sideOffset)
        {
            if(sideOffset != Vector2.zero)
            {
                sideOffset.x *= .5f;
                Vector3 v1 = transform.localToWorldMatrix.MultiplyVector(new Vector3(sideOffset.x, sideOffset.y, 0));
                Vector3 v2 = transform.localToWorldMatrix.MultiplyVector(m_RefPoint);
                Vector3 v3 = Vector3.Cross(v2, v1);
                Pools.CueBall.SetTorque(v3.normalized * sideOffset.sqrMagnitude * .375f); 
            }
            m_CurrentSideOffset = sideOffset;
        }

        public void ResetSideOffset()
        {
            m_CurrentSideOffset = Vector2.zero;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            BaseUIController.cueAndLines.gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            BaseUIController.cueAndLines.gameObject.SetActive(true);
        }

        public void UpdateSiding()
        {
            Siding(m_CurrentSideOffset);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = m_GizmosColor;
            Gizmos.DrawRay(transform.position, transform.forward);
        }
	}
