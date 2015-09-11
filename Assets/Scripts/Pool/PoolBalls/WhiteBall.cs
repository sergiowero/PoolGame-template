using UnityEngine;
using System.Collections;
	public class WhiteBall : PoolBall
	{
        private BallDragger m_BallDragger;

        public static bool CueBallSiding = false;

		Vector3 screenPoint;
		Vector3 offset;

		private Constraint m_constraint;
		private PoolBall m_targetBall;
		private Vector3 m_targetPos;

        //private bool m_fired=false;
		public LayerMask layermask;
		public bool Foul=true;


        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Vector3 GetScreenPosition()
        {
            Vector3 v = Pools.SceneCamera.WorldToScreenPoint(GetPosition());
            v.z = 0;
            return v;
        }

		public override void Start()
		{
			base.Start();
            InitBallDragger();
			m_constraint = gameObject.GetComponent<Constraint>();
			m_constraint.enabled=false;
            m_constraint.adjustment = GetRadius();
            Reset();
		}


        #region ball darg event--------------------------------------
        private void InitBallDragger()
        {
            m_BallDragger = gameObject.GetComponent<BallDragger>();
            if (!m_BallDragger)
                m_BallDragger = gameObject.AddComponent<BallDragger>();
            m_BallDragger.layermask.value = gameObject.layer;
            m_BallDragger.dragBegin = OnDragBegin;
            m_BallDragger.drag = OnDrag;
            m_BallDragger.dragEnd = OnDragEnd;
        }

		void OnDragBegin(BallDraggerData data)
		{
            if (GameManager.Rules.HandleWhiteBall && GameManager.Rules.State != GlobalState.ROLLING)
			{
				m_constraint.enabled=true;
				m_rigidbody.useGravity=false;
                screenPoint = Pools.SceneCamera.WorldToScreenPoint(gameObject.transform.position);
                offset = gameObject.transform.position - Pools.SceneCamera.ScreenToWorldPoint(new Vector3(data.Position.x, data.Position.y, screenPoint.z));
                GameManager.Rules.State = GlobalState.DRAG_WHITEBALL;
                Pools.Cue.Hide();
            }	
		}

        void OnDragEnd(BallDraggerData data)
		{
            if (GameManager.Rules.HandleWhiteBall && GameManager.Rules.State != GlobalState.ROLLING)
			{
				m_rigidbody.useGravity=true;
				m_constraint.enabled=false;
                Pools.Cue.Show();
                collider.isTrigger = false;
                GameManager.Rules.ReversePrevState();
			}
		}

        void OnDrag(BallDraggerData data)
		{
            if (GameManager.Rules.HandleWhiteBall && GameManager.Rules.State != GlobalState.ROLLING)
			{
                Vector3 curScreenPoint = new Vector3(data.Position.x, data.Position.y, screenPoint.z);
                Vector3 curPosition = Pools.SceneCamera.ScreenToWorldPoint(curScreenPoint) + offset;
                curPosition.y = transform.position.y;

                Collider[] contantacts = Physics.OverlapSphere(curPosition, GetRadius(), layermask.value);
                if (contantacts.Length == 0)
                {
                    transform.position = curPosition;
                    collider.isTrigger = true;
                }
			}
		}
        #endregion

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
            string name = col.gameObject.name;
            if(!name.Contains("surface") && gameObject.activeInHierarchy)
                StartCoroutine("TouchTable", 0);
            #region Might be useful
            //if (name.Contains("Rail"))
            //{
            //    AudioHelper.m_Instance.onBallHitWall(m_rigidbody.velocity);
            //    hitWall = true;
            //}
            #endregion
            if (name.Contains("Ball"))
			{
                AudioHelper.m_Instance.onBallHitBall(m_rigidbody.velocity);
				PoolBall ball = col.gameObject.GetComponent<PoolBall>();
                GameManager.Rules.WhiteBallHitBall(ball);
				if(ball && ball==m_targetBall)
				{
					m_targetBall.PointAtTarget(m_targetPos);
					m_targetBall=null;
				}
			}
		}

        //void FixedUpdate()
        //{
        //    if (m_state == State.ROLL)
        //    {
        //        Vector3 direction = m_rigidbody.position - m_LastPosition;
        //        RaycastHit hit;
        //        if (Physics.SphereCast(m_LastPosition, m_Radius, direction, out hit, direction.magnitude, 1 << LayerMask.NameToLayer("Ball")))
        //        {
        //            m_rigidbody.position = hit.point + hit.normal.normalized * m_Radius;
        //        }
        //    }
        //    m_LastPosition = m_rigidbody.position;
        //}


		public void clear()
		{
			m_rigidbody.constraints = RigidbodyConstraints.None;
			m_slowTime=0;
            m_state = State.IDLE;

            transform.rotation = Quaternion.identity;
			if(m_rigidbody)
			{
				m_rigidbody.angularVelocity = Vector3.zero;
				m_rigidbody.velocity = Vector3.zero;
			}
		}

        public override void Reset()
        {
            Debug.Log("Reset white ball");
            base.Reset();
            transform.position = m_initalPos;
        }

        public void fireBall(float powerScalar)
		{
            if (powerScalar > .3f)
            {
                m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
                m_rigidbody.useGravity = false;
                StartCoroutine("TouchTable", powerScalar * .1f);
            }
            //BaseGameManager.fireBall();
            AudioHelper.m_Instance.onFireBall();
            GameManager.Rules.OnBallFired();
            m_slowTime = 0;
            m_rigidbody.AddForce(Pools.Cue.transform.forward * powerScalar * ConstantData.GetPoolDatas().MaxImpulse, ForceMode.Impulse);
            m_rigidbody.AddTorque(ballTorque * powerScalar);
            m_state = State.ROLL;
            //m_BallPhysicalDrag.enabled = true;
            OpenDrag();
            ballTorque = Vector3.zero;
            Siding.ResetAnchorOffset();
		}
		
        IEnumerator TouchTable(float time)
        {
            yield return new WaitForSeconds(time);
            m_rigidbody.constraints = RigidbodyConstraints.None;
            m_rigidbody.useGravity = true;
        }

        public void SetTorque(Vector3 torque)
        {
            ballTorque = torque;
        }
	}
