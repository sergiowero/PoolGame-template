using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class Testttt : MonoBehaviour
{
    void Update()
    {
        if(Physics.SphereCast(new Ray(transform.position, transform.forward), 1))
        {
            Debug.Log(1);
        }
    }
}