using UnityEngine;
using System.Collections;

public class BallInPocket : MonoBehaviour {

    private Rigidbody m_Ball;

    private Vector3 m_RefPoint;

    private Transform m_Trans;

    Vector2 m_Tempa, m_Tempb;
    void Start()
    {
        m_Ball = rigidbody;
        m_Trans = transform;
    }

    public void FixedUpdate()
    {
        Vector3 force = (m_RefPoint - m_Trans.position).normalized;
        Vector3 velocity = m_Ball.velocity.normalized;
        //we abandon the y axis, calculate the 2d angle of two vector
        m_Tempa = new Vector2(force.x, force.z);
        m_Tempb = new Vector2(velocity.x, velocity.z);
        if (Vector2.Dot(m_Tempa, m_Tempb) > .9f) //means two vector's angle too small
            return;
        m_Ball.velocity = (force + velocity).normalized * m_Ball.velocity.magnitude;
    }

    void OnDisable()
    {
        Destroy(this);
    }

    public void SetRefTrans(Transform refTrans)
    {
        m_RefPoint = refTrans.position;
    }
}
