using UnityEngine;
using System.Collections;

public class GlobalInitialization : MonoBehaviour 
{
    void Awake()
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
