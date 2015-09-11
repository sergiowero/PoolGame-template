using UnityEngine;
using System.Collections;

public class RackBallCollision : MonoBehaviour 
{

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Contains("Destination") || collision.transform.name.Contains("Ball"))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
