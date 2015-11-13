using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FixedTimeBalance : MonoBehaviour {

    List<Rigidbody> m_RigidList;
    int m_SleepCount;

    //[SerializeField]
    //InputField m_InputField;

    //[SerializeField]
    //InputField m_FixedDelta;

    void Start()
    {
        m_RigidList = new List<Rigidbody>();
        for (int i = 1; i <= 15; i++)
        {
            m_RigidList.Add(Pools.Balls[i].rigidbody);
        }
        foreach(KeyValuePair<int, PoolBall> v in Pools.CustomBalls)
        {
            m_RigidList.Add(v.Value.rigidbody);
        }
    }


	void Update () 
    {
        //do once update per 5 frames
        if(Time.frameCount % 2 == 0)
        {
            m_SleepCount = 0;
            for (int i = 0, count = m_RigidList.Count; i < count; i++)
            {
                if (m_RigidList[i].IsSleeping())
                    m_SleepCount++;
            }
            float time;
            if (m_SleepCount >= 7)
                time = .002f;
            else if (m_SleepCount >= 4)
                time = .003f;
            else
                time = .005f;
            Time.fixedDeltaTime = time;
        }
	}
}
