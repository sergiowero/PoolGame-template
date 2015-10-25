using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchieveConfiguration : HOConfigurationReader
{
    public static AchieveConfiguration instance { get; private set; }
    public TextAsset table;
    public static Dictionary<int, AchieveInfo> achieveDictionary
    {
        get
        {
            return instance.mAchieveDictionary; 
        }
    }
    protected Dictionary<int, AchieveInfo> mAchieveDictionary = new Dictionary<int, AchieveInfo>();

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
            AchieveInfo info = new AchieveInfo();
            int key = new int();
            Hashtable value = (Hashtable)t.Value;
            info.id = ParseTableValueToInt(value["ID"]);
            info.name = ParseTableValueToString(value["Name"]);
            info.type = (AchieveType)ParseTableValueToInt(value["Type"]);
            info.level = ParseTableValueToString(value["Level"]);
            info.goal = ParseTableValueToInt(value["Goal"]);
            info.DescriptionID = ParseTableValueToInt(value["DescriptionID"]);
            info.Description = ParseTableValueToString(value["Description"]);
            key = info.id;
            mAchieveDictionary.Add(key, info);
        }
    }
}
