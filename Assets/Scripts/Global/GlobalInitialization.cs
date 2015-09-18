using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class GlobalInitialization : MonoBehaviour 
{
    [SerializeField]
    private List<LevelData> m_LevelDatas = new List<LevelData>();

    void Awake()
    {
        ConstantData.LevelDatas = m_LevelDatas;
    }

    void Start()
    {
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
        if (string.IsNullOrEmpty(www.error))
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
}
