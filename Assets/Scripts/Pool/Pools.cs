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
    private BallStorageRack m_BallStorageRack;
    private Sprite[] m_BallIcons;
    #endregion

    #region Static fields--------------------------------------------------------
    public static Dictionary<int, PoolBall> Balls { get { return m_Instance.m_Balls; } }
    public static PoolBall[] BallsArray { get { return m_Instance._GetBallsArray(); } }
    public static PoolCue Cue { get { return m_Instance.m_Cue; } }
    public static WhiteBall CueBall { get { return m_Instance.m_Balls[0] as WhiteBall; } }
    public static Camera SceneCamera { get { return m_Instance.m_SceneCamera; } }
    public static BallStorageRack StorageRack { get { return m_Instance.m_BallStorageRack; } }
    public static Sprite[] BallIcons { get { return m_Instance.m_BallIcons; } }
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

        m_BallStorageRack = FindObjectOfType<BallStorageRack>();

        m_BallIcons = new Sprite[15];
        for (int i = 1; i <= 15; i++)
        {
            m_BallIcons[i - 1] = Resources.Load<Sprite>("BallsIcon/" + i);
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(LoadPoolAsset(OnPoolAssetLoadedAtAndroidPlatform));
#endif
    }

    #region IEnumerator
    IEnumerator LoadPoolAsset(System.Action<PoolDataAsset> onloaded)
    {
        WWW www = new WWW(StreamTools.GetStreamingAssetsPath() + ConstantData.PoolDataAssetsFile);
        Debug.Log("load file : " + StreamTools.GetStreamingAssetsPath() + ConstantData.PoolDataAssetsFile);
        yield return www;
        if(string.IsNullOrEmpty(www.error))
        {
            Debug.Log("load file success");
            onloaded(StreamTools.DeserializeObject<PoolDataAsset>(www.bytes));
        }
        else
        {
            Debug.LogError(www.error);
        }
    }
    #endregion

    #region Callback
#if UNITY_ANDROID
    private void OnPoolAssetLoadedAtAndroidPlatform(PoolDataAsset dataAsset)
    {
        ConstantData.SetPoolDatas(dataAsset);
    }
#endif
    #endregion

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

    public static void ResetAllBalls()
    {

    }
}
