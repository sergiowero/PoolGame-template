using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pools : MonoBehaviour
{
    private static Pools m_Instance = null;
    public static Pools Instance { get { return m_Instance; } }

    #region Fields ---------------------------------------------------------------
    private Dictionary<int, PoolBall> m_Balls;
    private PoolCue m_Cue;
    private Camera m_SceneCamera;
    #endregion

    #region Static fields--------------------------------------------------------
    public static Dictionary<int, PoolBall> Balls { get { return m_Instance.m_Balls; } }
    public static PoolBall[] BallsArray { get { return m_Instance._GetBallsArray(); } }
    public static PoolCue Cue { get { return m_Instance.m_Cue; } }
    public static WhiteBall CueBall { get { return m_Instance.m_Balls[0] as WhiteBall; } }
    public static Camera SceneCamera { get { return m_Instance.m_SceneCamera; } }
    #endregion

    void Awake()
    {
        if (m_Instance)
        {
            Debug.LogError("There id two " + gameObject.name + " in the scene");
            Debug.Break();
        }
        m_Instance = this;
        PoolBall[] balls = GameObject.FindObjectsOfType<PoolBall>();
        if (balls != null)
        {
            m_Balls = new Dictionary<int, PoolBall>();
            for (int i = 0, length = balls.Length; i < length; i++)
            {
                m_Balls.Add(balls[i].GetBallID(), balls[i]);
            }
        }
        else
        {
            Debug.LogError("the balls number is not explicit in the scene, please debug the code");
        }

        PoolCue[] cues = GameObject.FindObjectsOfType<PoolCue>();
        if (cues == null || cues.Length != 1)
        {
            Debug.LogError("the number of the cue is not explicit in the scene, please debug the code");
        }
        else
        {
            m_Cue = cues[0];
        }

        m_SceneCamera = Camera.main;
    }

    #region Methods-------------------------------------------------------------
    private PoolBall[] _GetBallsArray()
    {
        PoolBall[] balls = new PoolBall[m_Balls.Count];
        for (int i = 0, length = balls.Length; i < length; i++)
        {
            balls[i] = m_Balls[i];
        }
        return balls;
    }
    private PoolBall _GetBall(int id)
    {
        return m_Balls[id];
    }
    private WhiteBall _GetWhiteBall()
    {
        return m_Balls[0] as WhiteBall;
    }
    #endregion
}
