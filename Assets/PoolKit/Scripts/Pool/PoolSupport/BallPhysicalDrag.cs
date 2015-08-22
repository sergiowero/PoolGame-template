using UnityEngine;
using System.Collections;

public class BallPhysicalDrag : MonoBehaviour {

    private Rigidbody m_Rigidbody;

    [SerializeField]
    private Vector3 m_Velocity;
    [SerializeField]
    private Vector3 m_AngularVelocity;

    [SerializeField]
    private float m_MaxYAxis;

    void Awake()
    {
        m_Rigidbody = rigidbody;
        StartCoroutine("RecordYValue");
    }

    IEnumerator RecordYValue()
    {
        yield return new WaitForSeconds(1);
        m_MaxYAxis = m_Rigidbody.position.y;
        yield return null;
    }

    public void FixedUpdate()
    {
        if (m_Rigidbody.position.y > m_MaxYAxis)
            YValueDrag();
        if(m_Rigidbody.velocity != Vector3.zero)
            VelocityDrag();
        if(m_Rigidbody.angularVelocity != Vector3.zero)
            AngularVelocityDrag();
    }

    private void YValueDrag()
    {
        Vector3 v = m_Rigidbody.position;
        v.y = m_MaxYAxis;
        m_Rigidbody.position = v;
    }

    private void VelocityDrag()
    {
        m_Velocity = m_Rigidbody.velocity;
        float f = m_Velocity.magnitude - ConstantData.GetPoolDatas().BallDrag * Time.fixedDeltaTime;
        if(f <= 0)
        {
            m_Rigidbody.velocity = Vector3.zero;
        }
        else
        {
            m_Rigidbody.velocity = m_Velocity.normalized * f;
        }
    }

    private void AngularVelocityDrag()
    {
        m_AngularVelocity = m_Rigidbody.angularVelocity;
        float f = m_AngularVelocity.magnitude - ConstantData.GetPoolDatas().BallAngularDrag * Time.fixedDeltaTime;
        if(f <= 0)
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            m_Rigidbody.angularVelocity = m_AngularVelocity.normalized * f;
        }
    }
}
