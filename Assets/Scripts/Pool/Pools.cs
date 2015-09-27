using UnityEngine;
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

    private PoolBall[] _GetBallsArray()
    {
        PoolBall[] balls = new PoolBall[m_Balls.Count];
        for (int i = 0, length = balls.Length; i < length; i++)
        {
            balls[i] = m_Balls[i];
        }
        return balls;
    }

    public static void ResetAllBalls(bool pottedOnly, bool black8Origin)
    {
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
        float R = (CueBall.GetRadius() + 0.001f) * 2, x = Black8Origin.position.x - squr3 * R, z;
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
}
