﻿using UnityEngine;
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

    //the trigger id
    [SerializeField]
    private PocketIndexes m_PocketIndex;

    [Range(0, 1)]
    [SerializeField]
    private float m_Radius = 1;

    void Start()
    {
        m_boxCollider = gameObject.GetComponent<BoxCollider>();
    }

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

    public Vector3 getPosition()
    {
        return m_boxCollider.center;
    }

    //our ball has landed in the water, lets call the ball bowling pit
    void OnTriggerEnter(Collider col)
    {
        PoolBall pb = col.GetComponent<PoolBall>();
        if (col.name.Contains("Ball"))
        {
            GameManager.Rules.PotBall(pb, m_PocketIndex);
            Pools.StorageRack.Add(pb);
        }
    }
}
