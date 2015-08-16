using UnityEngine;
using System.Collections;

public class PocketTrigger : MonoBehaviour 
{

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Contains("Ball"))
        {
            if(collision.gameObject.GetComponent<BallInPocket>() == null)
                collision.gameObject.AddComponent<BallInPocket>(); 
        }
    }
}
