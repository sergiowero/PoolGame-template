using UnityEngine;
using System.Collections;

public class BallInPocket : MonoBehaviour {

    private Rigidbody m_Ball;
    private Vector3 m_Force;
    void Start()
    {
        m_Ball = rigidbody;
        m_Force = rigidbody.velocity.magnitude * .5f * Vector3.down ;
    }

    public void FixedUpdate()
    {

        m_Ball.AddForce(m_Force, ForceMode.Acceleration);
    }

    void OnDisable()
    {
        Destroy(this);
    }
}
