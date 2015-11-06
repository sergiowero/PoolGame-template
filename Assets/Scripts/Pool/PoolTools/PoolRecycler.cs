//#define USE_GIZMOS

using UnityEngine;
using System.Collections;
//the pocket triggers 
public class PoolRecycler : MonoBehaviour
{
    [SerializeField]
    private Transform m_RefTrans;

    //the color of the gizmo
    public Color gizmoColor = new Color(1, 0, 0, 0.5f);

    //the box collider ref.
    private BoxCollider m_boxCollider;

    private PocketTrigger m_RefTrigger;

#if USE_GIZMOS
    [Range(0, 1)]
    [SerializeField]
    private float m_Radius = 1;
#endif
    
    public void SetTrigger(PocketTrigger _Trigger)
    {
        m_RefTrigger = _Trigger;
    }

    void Start()
    {
        m_boxCollider = gameObject.GetComponent<BoxCollider>();
    }

#if USE_GIZMOS
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = gizmoColor;
        //Gizmos.DrawCube(transform.position, transform.localScale);
        Gizmos.DrawSphere(transform.position, m_Radius);
        Gizmos.color = Color.white;
        if (m_RefTrans)
        {
            Gizmos.DrawCube(m_RefTrans.position, Vector3.one * .07f);
        }
    }
#endif
    public Vector3 getPosition()
    {
        return m_boxCollider.center;
    }

    //our ball has landed in the water, lets call the ball bowling pit
    void OnTriggerEnter(Collider col)
    {
        PoolBall pb = col.GetComponent<PoolBall>();
        if (col.CompareTag("Ball"))
        {
            GameManager.Rules.PotBall(pb, m_RefTrigger.PocketIndex);
            Pools.StorageRack.Add(pb);
            if (PocketTrigger.pocketTriggerBallList.Contains(col.GetInstanceID()))
                PocketTrigger.pocketTriggerBallList.Remove(col.GetInstanceID());
        }
    }
}
