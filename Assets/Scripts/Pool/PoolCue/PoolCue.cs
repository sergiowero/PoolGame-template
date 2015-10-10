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
    [SerializeField]
    private Transform m_FirePoint;
    [SerializeField]
    private Transform m_SidingPoint;
    [SerializeField]
    [Range(0,0.01f)]
    private float m_SidingPointOffsetRange;

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

    //do we want to rotate
    protected bool m_requestRotate = false;

    //do we want to fire the ball
    protected bool m_requestFire = false;

    #region Gizmos--------------------------------------------
    [SerializeField]
    protected Color m_GizmosColor;
    #endregion

    //void Update()
    //{
    //    Vector3 v = m_CurrentSideOffset * m_SidingPointOffsetRange;
    //    v.z = -.013f;
    //    m_SidingPoint.localPosition = v;
        
    //    Pools.CueBall.SetHitPoint(m_SidingPoint.position);
    //}

    public void Awake()
    {
        m_initalPos = transform.localPosition;
        m_initalRot = transform.localRotation;
        m_CurRotAngle = 0;
        FireSlider.OnSliderValueChange += SetPowerScalar;
        FireSlider.OnSliderRelease += Fire;
        PoolRulesBase.onNewTurn += RoundBegin;
    }

    public void OnDestroy()
    {
        FireSlider.OnSliderValueChange -= SetPowerScalar;
        FireSlider.OnSliderRelease -= Fire;
        PoolRulesBase.onNewTurn -= RoundBegin;
    }

    public void SetPowerScalar(float value)
    {
        m_powerScalar = value;
        BaseUIController.cueAndLines.AdjustingCue(value);
    }

    private void RoundBegin(int i)
    {
        transform.position = Pools.CueBall.transform.position;
        Pools.CueBall.SetHitPoint(m_SidingPoint.position);
    }

    void OnEnable()
    {
        transform.position = Pools.CueBall.transform.position;
    }

    public Vector3 GetFireDirection()
    {
        return m_FirePoint.forward;
    }

    public void Fire()
    {
        m_requestFire = false;

        //lets set the balls target and the target position. When the white ball hits the first ball we will set the ball to point at the target.
        Pools.CueBall.setTarget(m_targetBall, m_targetPos);
        Pools.CueBall.fireBall(m_powerScalar);
        m_state = State.ROLL;
        BaseUIController.cueAndLines.gameObject.SetActive(false);
        transform.parent = null;
    }

    public void Rotate(float angle)
    {
        CurRotAngle = angle;
        transform.RotateAround(Pools.CueBall.GetPosition(), Vector3.up, angle);
        HandleRotate();
        Pools.CueBall.SetHitPoint(m_SidingPoint.position);
    }

    public void VerticalRotate(float angle)
    {
        m_FirePoint.localRotation = Quaternion.identity;
        m_FirePoint.localPosition = new Vector3(0, 0, -.013f);
        m_FirePoint.RotateAround(Pools.CueBall.GetPosition(), transform.right, angle);
        Pools.CueBall.SetHitPoint(m_SidingPoint.position);

        //Vector2 op = new Vector2(Pools.CueBall.GetPosition().z, Pools.CueBall.GetPosition().y);
    }

    public void Reset()
    {
        transform.localEulerAngles = new Vector3(0, 90, 0);
        HandleRotate();
    }

    public void ResetPosition()
    {
        transform.position = Pools.CueBall.GetPosition();
    }

    public void Siding(Vector2 sideOffset)
    {
        m_SidingPoint.localPosition = sideOffset * m_SidingPointOffsetRange;
        Pools.CueBall.SetHitPoint(m_SidingPoint.position);
    }

    void HandleRotate()
    {
        if (Pools.CueBall && Pools.CueBall.sphereCollider)
        {
            SphereCollider sc = Pools.CueBall.sphereCollider;
            Ray ray = new Ray(Pools.CueBall.transform.position, transform.forward);
            float r = Pools.CueBall.GetRadius();
            RaycastHit rch;
            if (Physics.SphereCast(ray, r - Mathf.Epsilon, out rch, 1000f, layerMask.value))
            {
                Vector3 pos = rch.point;

                BaseUIController.cueAndLines.GuidePointerAt(rch.point, rch.transform, rch.normal);

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
            }
        }
    }

    public void ResetSideOffset()
    {
        //m_CurrentSideOffset = Vector2.zero;
        m_SidingPoint.localPosition = Vector3.zero;
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
        //Siding(m_CurrentSideOffset, true);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = m_GizmosColor;
        Gizmos.DrawRay(m_SidingPoint.position, -m_SidingPoint.forward);
    }
}
