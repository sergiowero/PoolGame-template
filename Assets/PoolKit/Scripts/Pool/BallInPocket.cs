using UnityEngine;
using System.Collections;

public class BallInPocket : MonoBehaviour {

    private Vector3 m_RefPoint;
    private Rigidbody m_Ball;

    private float m_VeloMag;

    [SerializeField]
    private float m_MinVelocity;

    void Awake()
    {
        m_Ball = rigidbody;
    }

    void Start () 
    {
        m_VeloMag = Mathf.Max(m_MinVelocity, m_Ball.velocity.magnitude);
	}

    public void FixedUpdate()
    {
        Vector3 refv = (m_RefPoint - m_Ball.transform.position).normalized * m_VeloMag * .5f;
        m_Ball.velocity = (m_Ball.velocity + refv).normalized * m_VeloMag;
        m_Ball.AddTorque(Vector3.Cross(Vector3.up, m_Ball.velocity) * m_VeloMag);
    }
	
    
    public void SetRefPoint(Vector3 refPoint)
    {
        m_RefPoint = refPoint;
    }
}
