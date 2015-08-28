using UnityEngine;
using System.Collections;

public class LightSource : MonoBehaviour 
{
    public static Vector3 Position;

    void Start()
    {
        Position = transform.position;
    }
}
