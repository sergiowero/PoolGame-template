using UnityEngine;
using System.Collections;

public class StorageBottom : MonoBehaviour 
{
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ball"))
        {
            Rigidbody rig = collision.rigidbody;
            rig.velocity = Vector3.zero;
            rig.angularVelocity = Vector3.zero;
        }
    }
}
