using UnityEngine;
using System.Collections;
	public class WhiteBall : PoolBall
	{
        private BallDragger m_BallDragger;

        public static bool CueBallSiding = false;

		public SphereCollider sphereCollider;

		Vector3 screenPoint;
		Vector3 offset;

		private Constraint m_constraint;
		private bool m_hitBall=false;
		private PoolBall m_targetBall;
		private Vector3 m_targetPos;

        //private bool m_fired=false;
		public LayerMask layermask;
		public bool foul=true;

		public float GetRadius()
		{
            return sphereCollider.radius * transform.localScale.x - ConstantData.BallRadiusAdjustment;//BallRadiusAdjustment is the collision adjustment, otherwise colliding can not be happen
		}

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
			sphereCollider = gameObject.GetComponent<SphereCollider>();
			m_constraint = gameObject.GetComponent<Constraint>();
			m_constraint.enabled=false;
            m_constraint.adjustment = GetRadius();
            reset();
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
			if(foul && PoolGameScript8Ball.Instance.GlobalState != PoolGameScript.State.ROLLING )
			{
				m_constraint.enabled=true;
				m_rigidbody.useGravity=false;
                screenPoint = Pools.SceneCamera.WorldToScreenPoint(gameObject.transform.position);
                offset = gameObject.transform.position - Pools.SceneCamera.ScreenToWorldPoint(new Vector3(data.Position.x, data.Position.y, screenPoint.z));
                PoolGameScript8Ball.Instance.GlobalState = PoolGameScript.State.DRAG_WHITEBALL;
                Pools.Cue.Hide();
            }	
		}

        void OnDragEnd(BallDraggerData data)
		{
            if (foul && PoolGameScript8Ball.Instance.GlobalState != PoolGameScript.State.ROLLING)
			{
				m_rigidbody.useGravity=true;
				m_constraint.enabled=false;
                Pools.Cue.Show();
                collider.isTrigger = false;
                PoolGameScript8Ball.Instance.ReturnPrevousState();
			}
		}

        void OnDrag(BallDraggerData data)
		{
            if (foul && PoolGameScript8Ball.Instance.GlobalState != PoolGameScript.State.ROLLING)
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
            if(!name.Contains("surface"))
                StartCoroutine("TouchTable", 0);
            if (name.Contains("Rail"))
            {
                BaseGameManager.ballHitWall(rigidbody.velocity);
                hitWall = true;
            }
            if (name.Contains("Ball"))
			{
				PoolBall ball = col.gameObject.GetComponent<PoolBall>();
				if(ball && m_hitBall==false)
				{
					BaseGameManager.whiteBallHitBall(m_hitBall,ball);
					m_hitBall=true;
				}
				if(ball && ball==m_targetBall)
				{
					BaseGameManager.ballHitBall(rigidbody.velocity);
					m_targetBall.PointAtTarget(m_targetPos);
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
				m_rigidbody.angularVelocity = Vector3.zero;
				m_rigidbody.velocity = Vector3.zero;
			}
		}
		public void reset()
		{
            gameObject.SetActive(true);
			m_hitBall=false;
			m_rigidbody.constraints = RigidbodyConstraints.None;
			m_slowTime=0;
			m_state = State.IDLE;
			transform.position = m_initalPos;
			transform.rotation = Quaternion.identity;
			if(m_rigidbody)
			{
				m_rigidbody.angularVelocity = Vector3.zero;
				m_rigidbody.velocity = Vector3.zero;
			}
		}

		public override void OnBallStop()
		{
			base.OnBallStop();
            //transform.rotation = Quaternion.identity;
		}

        public void fireBall(float powerScalar)
		{
            if (powerScalar > .3f)
            {
                m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
                m_rigidbody.useGravity = false;
                StartCoroutine("TouchTable", powerScalar * .5f);
            }
            BaseGameManager.fireBall();
            m_slowTime = 0;
            Debug.Log("add force : " + ConstantData.GetPoolDatas().MaxImpulse);
            m_rigidbody.AddForce(Pools.Cue.transform.forward * powerScalar * ConstantData.GetPoolDatas().MaxImpulse, ForceMode.Impulse);
            m_rigidbody.AddTorque(ballTorque);
            m_state = State.ROLL;
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
