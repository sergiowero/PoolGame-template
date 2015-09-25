using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutConfiguration : HOConfigurationReader {
    public static LayoutConfiguration instance { get; private set; }
    public TextAsset table;
    public static Dictionary<LayoutInfoKey, LayoutInfo> cardDictionary
    {
        get
        {
            return instance.mCardDictionary;
        }
    }
    protected Dictionary<LayoutInfoKey, LayoutInfo> mCardDictionary = new Dictionary<LayoutInfoKey, LayoutInfo>();

    public LayoutInfo this[string index]
    {
        get 
        {
            LayoutInfoKey s = GetKeyFromString(index);
            if(mCardDictionary.ContainsKey(s))
                return mCardDictionary[s];
            return null;
        }
    }

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
            DestroyObject(this.gameObject);
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
            LayoutInfo info = new LayoutInfo();
            LayoutInfoKey key = new LayoutInfoKey();
            Hashtable value = (Hashtable)t.Value;
            info.Chapter = ParseTableValueToInt(value["Chapter"]);
            key.Chapter = info.Chapter;
            info.Level = ParseTableValueToInt(value["Level"]);
            key.Level = info.Level;
            info.Star1 = ParseTableValueToInt(value["Star1"]);
            info.Star2 = ParseTableValueToInt(value["Star2"]);
            info.Star3 = ParseTableValueToInt(value["Star3"]);
            info.OpenStar = ParseTableValueToInt(value["OpenStar"]);
            info.Cost = ParseTableValueToInt(value["Cost"]);
            mCardDictionary.Add(key, info);
        }
    }

    public static LayoutInfoKey GetKeyFromString(string s)
    {
        string[] ss = s.Split('-');
        if(ss.Length == 2)
        {
            LayoutInfoKey key = LayoutInfoKey.GenerateKey(int.Parse(ss[0]), int.Parse(ss[1]));
            return key;
        }
        return null;
    }
}
