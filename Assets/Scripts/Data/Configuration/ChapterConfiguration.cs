using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChapterConfiguration : HOConfigurationReader
{
    public static ChapterConfiguration instance { get; private set; }
    public TextAsset table;
    public static Dictionary<int, ChapterInfo> chapterDictionary
    {
        get
        {
            return instance.mChapterDictionary; 
        }
    }
    protected Dictionary<int, ChapterInfo> mChapterDictionary = new Dictionary<int, ChapterInfo>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Read();
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }
    

    void Read()
    {
        Hashtable tb = new Hashtable();
        ParseData(table, tb);
        foreach (DictionaryEntry t in tb)
        {
            ChapterInfo info = new ChapterInfo();
            int key = new int();
            Hashtable value = (Hashtable)t.Value;
            info.chapter = ParseTableValueToInt(value["ID"]);
            info.name = ParseTableValueToString(value["Name"]);
            info.imageName = ParseTableValueToString(value["ImageName"]);
            key = info.chapter;
            mChapterDictionary.Add(key, info);
        }
    }
}
