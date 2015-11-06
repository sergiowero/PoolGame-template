using UnityEngine;
using System.Collections;
using Debugger;


public class PoolCue : MonoBehaviour
{
    //our layer mask
    public LayerMask layerMask;

    //current white ball side
    //private Vector2 m_CurrentSideOffset;
    //[SerializeField]
    //private Vector3 m_RefPoint = new Vector3(0, 0, -1);
    private Transform m_CueTrans;
    [SerializeField]
    private Transform m_FirePoint;
    [SerializeField]
    private Transform m_SidingPoint;
    [SerializeField]
    private float m_SidingPointOffset;
    private Vector3 m_CurrentSidingOffset;

    [SerializeField]
    private Vector2 m_SidingPointRange = new Vector2(.006f, .01f); //x: min ; y : max

    public enum State
    {
        ROTATE,
        ROLL
    };

    //the current state
    protected State m_state;

    //the inital rotation
    protected Quaternion m_initalRot;

    //the inital psoition
    protected Vector3 m_initalPos;

    //the power scale -- between 1 and 100
    protected float m_powerScalar = 1f;

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

    protected Vector3 m_CurHitPoint;

    #region Gizmos--------------------------------------------
    [SerializeField]
    protected Color m_GizmosColor;
    #endregion

    public void Awake()
    {
        m_CueTrans = transform;
        m_initalPos = m_CueTrans.localPosition;
        m_initalRot = m_CueTrans.localRotation;
        m_CurRotAngle = 0;
        m_SidingPointOffset = m_SidingPointRange.x;
    }

    protected Vector3 GetHitPoint()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(m_SidingPoint.position, m_FirePoint.forward,out hitInfo,float.MaxValue,  1 << LayerMask.NameToLayer("WhiteBall")))
        {
            return hitInfo.point;
        }
        BaseUIController.text.Show("The ray can not cast the white ball. need debug it");
        return Vector3.zero;
    }

    public void LateUpdate()
    {
        m_CueTrans.position = Pools.CueBall.transform.position;
        HandleRotate();
        //Pools.CueBall.SetHitPoint(m_SidingPoint.position);
    }

    public void SetPowerScalar(float value)
    {
        m_powerScalar = value;
    }

    public void Fire()
    {
        //lets set the balls target and the target position. When the white ball hits the first ball we will set the ball to point at the target.
        Pools.CueBall.setTarget(m_targetBall, m_targetPos);
        float powerScalar = m_powerScalar;
        if(GameManager.Rules.firstRound)
        {
            powerScalar *= Random.Range(.8f, 1.5f);
        }
        Pools.CueBall.fireBall(powerScalar, m_FirePoint.forward, GetHitPoint());
        m_state = State.ROLL;
        m_CueTrans.parent = null;
    }

    public void Rotate(float angle)
    {
        CurRotAngle = angle;
        m_CueTrans.RotateAround(Pools.CueBall.GetPosition(), Vector3.up, angle);
    }

    public void VerticalRotate(float angle)
    {
        m_FirePoint.localRotation = Quaternion.identity;
        m_FirePoint.localPosition = new Vector3(0, 0, -.013f);
        m_FirePoint.RotateAround(Pools.CueBall.GetPosition(), m_CueTrans.right, angle);
        m_SidingPointOffset = Mathf.Lerp(m_SidingPointRange.x, m_SidingPointRange.y, angle / 90);
        Siding(m_CurrentSidingOffset);
    }

    public void Reset()
    {
        m_CueTrans.localEulerAngles = new Vector3(0, 90, 0);
    }

    public void Siding(Vector2 sideOffset)
    {
        m_CurrentSidingOffset = sideOffset;
        m_SidingPoint.localPosition = m_CurrentSidingOffset * m_SidingPointOffset;
    }

    void HandleRotate()
    {
        if (Pools.CueBall && Pools.CueBall.sphereCollider)
        {
            SphereCollider sc = Pools.CueBall.sphereCollider;
            Ray ray = new Ray(Pools.CueBall.transform.position, m_CueTrans.forward);
            float r = Pools.CueBall.GetRadius();
            RaycastHit rch;
            if (Physics.SphereCast(ray, r - Mathf.Epsilon, out rch, 1000f, layerMask.value))
            {
                Vector3 pos = rch.point;

                BaseUIController.cueAndLines.GuidePointerAt(rch.point, rch.transform, rch.normal, m_FirePoint.forward);

                Vector3 vec = pos - sc.transform.position;

                pos = sc.transform.position + vec.normalized * (vec.magnitude - r);

                Vector3 nrm = rch.normal;
                nrm.y = 0;
                m_targetBall = null;

                if (rch.collider.CompareTag("Ball"))
                {
                    m_targetBall = rch.transform.GetComponent<PoolBall>();
                    m_targetPos = rch.point - nrm;
                }

                if (ConstantData.GType >= GameType.Standard && m_targetBall != null)
                {
                    BasePlayer player = ((PoolRulesStandard)GameManager.Rules).CurrentPlayer;
                    bool b1 = player.TargetBallType == BallType.NONE && m_targetBall.ballType == BallType.BLACK;
                    bool b2 = player.TargetBallType != BallType.NONE && player.TargetBallType != m_targetBall.ballType;
                    if (b1 || b2)
                        BaseUIController.cueAndLines.Forbidden();
                    else
                        BaseUIController.cueAndLines.Allow();
                }
                else
                    BaseUIController.cueAndLines.Allow();
            }
        }
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

    public void OnDrawGizmos()
    {
        Gizmos.color = m_GizmosColor;
        Gizmos.DrawRay(m_SidingPoint.position, -m_SidingPoint.forward);
    }
}
