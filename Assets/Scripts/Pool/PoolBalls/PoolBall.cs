using UnityEngine;
using System.Collections;
public class PoolBall : MonoBehaviour
{
    public Delegate0Args onReset;

    public bool yConstrain = true;

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

    public SphereCollider sphereCollider;

    public bool AudioEnable = true;

    [System.Flags]
    public enum State
    {
        NONE = 0,
        ROLL = 1 << 0 , // the ball is rolling
        IDLE = 1 << 1,  // the ball is idling
        POTTED = 1 << 2, //the ball is potted
        HIDE = 1 << 3,//do we hide the ball
    };

    [SerializeField]
    //what state is the ball in
    protected State m_state;
    public State BallState { get { return m_state; } }

    //the current time the ball has to be slowed down
    protected float m_slowTime;

    //the time the ball has to be slowed down before its considered stopped
    public float slowTime = 1;

    //did we hit the wall.
    public bool hitWall = false;

    protected BallShadowRenderer m_ShadowRenderer;

    protected Follower m_LightRenderer;
    public Follower LightRenderer { get { return m_LightRenderer; } }

    protected Follower m_FocusRenderer;
    public Follower focusRenderer { get { return m_FocusRenderer; } }

    //protected BallPhysicalDrag m_BallPhysicalDrag;
    protected PhysicalSupportTools m_BallPhysicalDrag;

    protected PhysicalSupportTools m_AngularVelocityCorrection;

    protected PhysicMaterial m_PhysicMaterial;

    [SerializeField]
    private float m_MaxYAxis;

    protected Renderer m_Mesh;

    protected Vector3 m_LastPosition;

    protected float m_Radius;

    public virtual void Awake()
    {
        m_rigidbody = gameObject.GetComponent<Rigidbody>();
        m_ShadowRenderer = GetComponent<BallShadowRenderer>();
        m_LightRenderer = transform.FindChild("RefLight").GetComponent<Follower>();
        m_FocusRenderer = transform.FindChild("Focus").GetComponent<Follower>();
        m_FocusRenderer.Close();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        m_Mesh = GetComponent<MeshRenderer>();
        //m_BallPhysicalDrag = GetComponent<BallPhysicalDrag>();
        m_Radius = sphereCollider.radius * transform.localScale.x;
        m_BallPhysicalDrag = PhysicalSupportTools.PhysicalDragTo(gameObject, ConstantData.GetPoolDatas().BallDrag, ConstantData.GetPoolDatas().BallAngularDrag);
        //m_AngularVelocityCorrection = PhysicalSupportTools.AngularVelocityCorrectionTo(gameObject, m_rigidbody, m_Radius);
        m_PhysicMaterial = collider.sharedMaterial;
    }
    public virtual void Start()
    {
        m_initalPos = transform.position;
        m_initalRot = transform.rotation;
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

    public void SetBallID(int id)
    {
        m_BallID = id;
    }

    public virtual void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.Contains("Rail"))
        {
            //we hit the wall.
            //BaseGameManager.ballHitWall(rigidbody.velocity);
            AudioHelper.m_Instance.onBallHitWall(m_rigidbody.velocity);
            if (!hitWall)
            {
                GameManager.Rules.BallHitRail();
                hitWall = true;
            }
        }
        if (col.gameObject.CompareTag("Ball"))
        {
            //BaseGameManager.ballHitBall(rigidbody.velocity);
            if (AudioEnable) AudioHelper.m_Instance.onBallHitBall(m_rigidbody.velocity);
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

    public virtual void OnNewTurn(int turnIndex)
    {
        hitWall = false;
    }

    public float GetRadius()
    {
        return m_Radius; /* -ConstantData.BallRadiusAdjustment;//BallRadiusAdjustment is the collision adjustment, otherwise colliding can not be happen*/
    }

    public virtual void Update()
    {
        if (m_rigidbody.velocity.sqrMagnitude < .001f && m_rigidbody.angularVelocity.sqrMagnitude < .001f)
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
            if (m_state != State.ROLL)
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
        if (m_rigidbody.position.y > m_MaxYAxis && yConstrain)
            YValueDrag();
    }

    public virtual void Potted(PocketIndexes pocketIndex)
    {
        if (m_rigidbody)
        {
            m_state = State.POTTED;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            //m_state = State.IDLE;
            CloseDrag();
            CloseRenderer();
            enabled = false;
            RemovePhysicalMaterial();
            //gameObject.SetActive(false);
        }
    }

    public virtual void Reset()
    {
        if (m_state == State.HIDE) return;


        m_slowTime = 0;
        m_state = State.IDLE;
        //SupportTools.SetPosition(gameObject, m_initalPos, SupportTools.AxisIgnore.IgnoreX | SupportTools.AxisIgnore.IgnoreZ);
        Vector3 v = transform.position;
        m_rigidbody.isKinematic = false;
        v.y = m_MaxYAxis;
        transform.position = v;
        transform.rotation = m_initalRot;
        m_rigidbody.constraints = RigidbodyConstraints.None;
        m_rigidbody.angularVelocity = Vector3.zero;
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.useGravity = true;
        renderer.enabled = true;
        collider.enabled = true;
        LightRenderer.Open();
        OpenRenderer();
        ReversePhysicalMaterial();
        PhysicalSupportTools.Remove(gameObject, PhysicalSupportType.MaxSpeedLimit);
        RackBallCollision rb;
        if(rb = GetComponent<RackBallCollision>()) Destroy(rb);
        enabled = true;
        m_LastPosition = transform.position;
    }

    public bool IsDoneRolling()
    {
        return m_state != State.ROLL;
    }

    public void CloseRenderer()
    {
        if (m_ShadowRenderer) m_ShadowRenderer.Close();
    }

    public void OpenRenderer()
    {
        if (m_ShadowRenderer) m_ShadowRenderer.Open();
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

    [ContextMenu("Hide")]
    public void Hide()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        enabled = false;
        transform.FindChild("Shadow").gameObject.SetActive(false);
        GetComponent<BallShadowRenderer>().enabled = false;
        transform.FindChild("RefLight").gameObject.SetActive(false);
        transform.FindChild("Focus").gameObject.SetActive(false);
        m_state = State.HIDE;
    }

    [ContextMenu("Show")]
    public void Display()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        enabled = true;
        transform.FindChild("Shadow").gameObject.SetActive(true);
        GetComponent<BallShadowRenderer>().enabled = true;
        m_state = State.IDLE;
        transform.FindChild("RefLight").gameObject.SetActive(true);
        transform.FindChild("Focus").gameObject.SetActive(true);
    }

    public bool IsBallDisable()
    {
        return (m_state & (State.POTTED | State.HIDE)) != 0x0;
    }

}
