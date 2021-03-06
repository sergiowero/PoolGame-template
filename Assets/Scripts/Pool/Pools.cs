﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pools : MonoBehaviour
{
    private static Pools m_Instance = null;
    public static Pools Instance { get { return m_Instance; } }

    #region Fields ---------------------------------------------------------------
    private Dictionary<int, PoolBall> m_Balls;
    private Dictionary<int, PoolBall> m_CustomBalls;
    private PoolCue m_Cue;
    private Camera m_SceneCamera;
    private BallStorageRack m_BallStorageRack;
    private Sprite[] m_BallIcons;
    private List<PocketTrigger> m_PocketTriggers = new List<PocketTrigger>();
    [SerializeField]
    private Transform m_CueBallOrigin;
    [SerializeField]
    private Transform m_Black8Origin;
    [SerializeField]
    private Transform m_CenterOrigin;
    #endregion

    #region Static fields--------------------------------------------------------
    public static Dictionary<int, PoolBall> Balls { get { return m_Instance.m_Balls; } }
    public static Dictionary<int, PoolBall> CustomBalls { get { return m_Instance.m_CustomBalls; } }
    public static PoolBall[] BallsArray { get { return m_Instance._GetBallsArray(); } }
    public static PoolBall[] CustomBallsArray { get { return m_Instance._GetCustomBallArray(); } }
    public static PoolCue Cue { get { return m_Instance.m_Cue; } }
    public static WhiteBall CueBall { get { return m_Instance.m_Balls[0] as WhiteBall; } }
    public static Camera SceneCamera { get { return m_Instance.m_SceneCamera; } }
    public static BallStorageRack StorageRack { get { return m_Instance.m_BallStorageRack; } }
    public static Sprite[] BallIcons { get { return m_Instance.m_BallIcons; } }
    public static List<PocketTrigger> PocketTriggers { get { return m_Instance.m_PocketTriggers; } }
    public static Transform CueBallOrigin { get { return m_Instance.m_CueBallOrigin; } }
    public static Transform Black8Origin { get { return m_Instance.m_Black8Origin; } }
    public static Transform CenterOrigin { get { return m_Instance.m_CenterOrigin; } }
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
        m_CustomBalls = new Dictionary<int, PoolBall>();
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

        m_BallStorageRack = FindObjectOfType<BallStorageRack>();

        m_BallIcons = new Sprite[15];
        for (int i = 1; i <= 15; i++)
        {
            m_BallIcons[i - 1] = Resources.Load<Sprite>("BallsIcon/" + i);
        }

        m_PocketTriggers.AddRange(FindObjectsOfType<PocketTrigger>());
        m_PocketTriggers.Sort((PocketTrigger left, PocketTrigger right) =>
            {
                if (left.PocketIndex < right.PocketIndex)
                    return -1;
                else if (left.PocketIndex > right.PocketIndex)
                    return 1;
                else
                    return 0;
            });
    }

    private void _DisableStandardBalls()
    {
        for(int i = 1; i <= 15; i++)
        {
            m_Balls[i].gameObject.SetActive(false);
        }
    }

    private void _EnableStandardBalls()
    {
        for(int i = 1; i <= 15; i ++)
        {
            m_Balls[i].gameObject.SetActive(true);
        }
    }

    private PoolBall[] _GetBallsArray()
    {
        PoolBall[] balls = new PoolBall[m_Balls.Count];
        for (int i = 0, length = balls.Length; i < length; i++)
        {
            balls[i] = m_Balls[i];
        }
        return balls;
    }

    private PoolBall[] _GetCustomBallArray()
    {
        PoolBall[] cBalls = new PoolBall[m_CustomBalls.Count];
        int i = 0;
        foreach(var v in m_CustomBalls)
        {
            cBalls[i++] = v.Value;
        }
        return cBalls;
    }

    private void _AllBallsKinematic(bool cueBallKinematic)
    {
        for(int i = cueBallKinematic ? 0 : 1; i <= 15; i ++)
        {
            m_Balls[i].rigidbody.isKinematic = true;
        }
        foreach(var v in m_CustomBalls)
        {
            v.Value.rigidbody.isKinematic = true;
        }
    }

    private void _AllBallsNonKinematic()
    {
        for (int i = 0; i <= 15; i++)
        {
            m_Balls[i].rigidbody.isKinematic = false;
        }
        foreach (var v in m_CustomBalls)
        {
            v.Value.rigidbody.isKinematic = false;
        }
    }

    private List<PoolBall> _GetBalls(int min, int max)
    {
        min = Mathf.Max(0, min);
        max = Mathf.Min(15, max);
        List<PoolBall> list = new List<PoolBall>();
        for(int i = min; i <= max; i++)
        {
            if(m_Balls[i].BallState == PoolBall.State.IDLE)
                list.Add(m_Balls[i]);
        }
        return list;
    }

    public static List<PoolBall> GetSolidAndStripeBalls()
    {
        List<PoolBall> list = new List<PoolBall>();
        list.AddRange(GetSolidBalls());
        list.AddRange(GetStripeBalls());
        return list;
    }

    public static List<PoolBall> GetSolidBalls()
    {
        List<PoolBall> list = new List<PoolBall>();
        list.AddRange(m_Instance._GetBalls(1, 7));
        return list;
    }

    public static List<PoolBall> GetStripeBalls()
    {
        List<PoolBall> list = new List<PoolBall>();
        list.AddRange(m_Instance._GetBalls(9, 15));
        return list;
    }

    public static void AllBallsKinematic(bool cueBallKinematic = false)
    {
        m_Instance._AllBallsKinematic(cueBallKinematic);
    }

    public static void AllBallsNonKinematic()
    {
        m_Instance._AllBallsNonKinematic();
    }

    public static void ResetAllBalls(bool pottedOnly, bool black8Origin)
    {
        CheckAnyBallInTheResetBallArea();

        IDictionary<int, PoolBall> t = new Dictionary<int, PoolBall>();
        //whether put the black 8 to the origin ,1 means true, 0 means false
        int origin = 0;
        //square root of 3
        float squr3 = 1.732f;

        for (int i = 1; i <= 15; i++)
        {
            if(Balls[i].BallState != PoolBall.State.HIDE)
            {
                if ((pottedOnly && Balls[i].BallState == PoolBall.State.POTTED) || !pottedOnly)
                    t.Add(i, Balls[i]);
            }
        }

        if (t.Count == 0) return;

        if (black8Origin && t.ContainsKey(8))
        {
            t[8].Reset();
            SupportTools.SetPosition(t[8].gameObject, Black8Origin.position, SupportTools.AxisIgnore.IgnoreY);
            origin = 1;
            t.Remove(8);
        }
        //the x axis space can calculated with Pythagorean theorem, the z axis space is diameter of the ball
        float R = (CueBall.GetRadius() + 0.0001f) * 2, x = Black8Origin.position.x - squr3 * R, z;
        Vector3 p = Vector3.one;
        List<int> list = new List<int>(t.Keys);
        for (int i = 1, k = 1; i <= 5; i++)
        {
            z = Black8Origin.position.z - (i - 1) * .5f * R;
            for (int j = 1; j <= i; j++, k++)
            {
                if (t.Count == 0) break;
                if (k == 5 && origin == 1) //if we already put the black 8 ball , continue
                {
                    z += R;
                    continue;
                }
                if (k > 15 - t.Count - origin)
                {
                    int l = Random.Range(0, t.Count);
                    int key = list[l];
                    PoolBall b = t[key];
                    b.Reset();
                    p.z = z; p.x = x;
                    SupportTools.SetPosition(b.gameObject, p, SupportTools.AxisIgnore.IgnoreY);
                    t.Remove(key);
                    list.RemoveAt(l);
                }
                z += R;
            }
            x += .5f * R * squr3;
        }
    }

    private static void CheckAnyBallInTheResetBallArea()
    {
        float r = Pools.CueBall.GetRadius() * 7;
        Collider[] cols = Physics.OverlapSphere(Pools.Black8Origin.position, r, 1 << LayerMask.NameToLayer("Ball") | 1 << LayerMask.NameToLayer("WhiteBall"));
        Vector3 p1 = Pools.CenterOrigin.position;
        Vector3 p2 = Pools.CueBallOrigin.position;
        for (int i = 0, length = cols.Length; i < length; i++)
        {
            if(cols[i].name.Contains("WhiteBall"))
            {
                PutBallToThePoint(cols[i].GetComponent<WhiteBall>(), ref p2);
            }
            else
            {
                PutBallToThePoint(cols[i].GetComponent<PoolBall>(), ref p1);
            }
        }
    }

    public static void PutBallToThePoint(PoolBall ball, ref Vector3 p)
    {
        float r = ball.GetRadius();
        while (Physics.OverlapSphere(p, r, 1 << LayerMask.NameToLayer("Ball") | 1 << LayerMask.NameToLayer("WhiteBall")).Length != 0)
        {
            p.x -= r;
        }
        SupportTools.SetPosition(ball.gameObject, p, SupportTools.AxisIgnore.IgnoreY, true);
        p.x -= r;
    }

    public static void DisableStandardBalls()
    {
        m_Instance._DisableStandardBalls();
    }

    public static void EnableStandardBalls()
    {
        m_Instance._EnableStandardBalls();
    }

    public static Vector2 GetTableSize()
    {
        Constraint c = CueBall.GetComponent<Constraint>();
        Vector2 v = new Vector2(c.max.x - c.min.x, c.max.z - c.min.z);
        return v;
    }

    public static void GetTableMinAndMaxPoints(out Vector3 min, out Vector3 max)
    {
        Constraint c = CueBall.GetComponent<Constraint>();
        min = c.min;
        min.y = CueBall.transform.position.y;
        max = c.max;
        max.y = CueBall.transform.position.y;
    }
}
