﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class GlobalInitialization : MonoBehaviour 
{
    [SerializeField]
    private LevelDataIndex m_LevelDatas;

    void Awake()
    {
        ConstantData.LevelDatas = m_LevelDatas;
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(LoadPoolAsset(OnPoolAssetLoadedAtAndroidPlatform));
#endif
        ConstantData.missionRecords = LoadRecords<MissionRecords>(ConstantData.MissionLevelDataRecordPath);
        ConstantData.quickFireRecords = LoadRecords<QuickFirePlayer.PlayerData>(ConstantData.QuickFireGameRecordPath);
    }

    #region IEnumerator
    IEnumerator LoadPoolAsset(Delegate1Args<PoolDataAsset> onloaded)
    {
        WWW www = new WWW(ConstantData.PoolDataAssetsFile);
        Debug.Log("load file : " + ConstantData.PoolDataAssetsFile);
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

    private T LoadRecords<T>(string path)  where T : new()
    {
        T t = StreamTools.DeserializeObject<T>(path);
        if(t == null)
        {
            Debug.Log("File : " + path + " does not exist, now create.");
            t = new T();
        }
        return t;
    }
}
