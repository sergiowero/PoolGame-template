using UnityEngine;
using System.Collections;

public class TemporarySlot : MonoBehaviour {

    private static TemporarySlot Instance = null;


    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }


    public static void Add(PoolBall ball)
    {
        ball.rigidbody.position = Instance.transform.position;
        //ball.CloseDrag();
        ball.AudioEnable = false;
        ball.CloseRenderer();

    }

}
