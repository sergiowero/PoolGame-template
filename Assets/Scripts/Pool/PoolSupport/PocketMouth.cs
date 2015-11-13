using UnityEngine;
using System.Collections;

public class PocketMouth : MonoBehaviour
{
    private PocketTrigger m_PocketTrigger;

    [SerializeField]
    private float m_SpeedThreshold;

    [SerializeField]
    private float m_SpeedLow;

    [SerializeField]
    private float m_SpeedHigh;

    void Awake()
    {
        m_PocketTrigger = GetComponentInParent<PocketTrigger>();
    }

    public void OnTriggerEnter(Collider other)
    {
        Rigidbody rid = other.GetComponent<Rigidbody>();
        if(rid && rid.velocity.sqrMagnitude <= m_SpeedThreshold)
        {
            rid.velocity = (transform.position - rid.transform.position).normalized * Mathf.Lerp(m_SpeedLow, m_SpeedHigh, rid.velocity.sqrMagnitude / 16);
        }
    }

    

}
