using UnityEngine;
using System.Collections.Generic;

public class LevelDataIndex : ScriptableObject
{
    public static LevelData CurrentLevel;

    [SerializeField]
    private List<LevelData> m_Indexes;

    public int Count { get { return m_Indexes.Count; } }

    public LevelData this[int index]
    {
        get
        { 
            if(index < m_Indexes.Count)
            {
                return m_Indexes[index];
            }
            return null;
        }
    }

    public LevelData this[string index]
    {
        get
        {
            for (int i = 0; i < Count; i++ )
            {
                if (m_Indexes[i].FileName.CompareTo(index) == 0)
                    return m_Indexes[i];
            }
            return null;
        }
    }

    public void Add(LevelData data)
    {
        if (m_Indexes.Contains(data))
            return;

        m_Indexes.Add(data);
        m_Indexes.Sort(Comp);
    }

    public LevelData Next(LevelData data)
    {
        int index = m_Indexes.IndexOf(data);
        return this[index + 1];
    }

    public void Clear()
    {
        m_Indexes.Clear();
    }

    public LevelDataIndex()
        : base()
    {
        m_Indexes = new List<LevelData>();
    }

    public int Comp(string l, string r)
    {
        string[] leftNum = l.Split('-');
        string[] rightNum = r.Split('-');
        if (leftNum.Length != 2 || rightNum.Length != 2)
        {
            throw new System.Exception("Occur error . left name : " + l + " , right name : " + r);
        }
        int bigNumLeft = int.Parse(leftNum[0]);
        int bigNumRight = int.Parse(rightNum[0]);

        if (bigNumLeft < bigNumRight)
            return -1;
        if (bigNumLeft > bigNumRight)
            return 1;

        int smallNumLeft = int.Parse(leftNum[1]);
        int smallNumRight = int.Parse(rightNum[1]);

        if (smallNumLeft < smallNumRight)
            return -1;
        if (smallNumLeft > smallNumRight)
            return 1;

        return 0;
    }

    public int Comp(LevelData l, LevelData r)
    {
        string left = l.FileName;
        string right = r.FileName;
        return Comp(left, right);
    }

    public int GetChapter(string levelDataName)
    {
        return int.Parse(levelDataName.Split('-')[0]);
    }

    public IList<LevelData> DumpLevelDatasWithChapter(int chapter)
    {
        List<LevelData> list = new List<LevelData>();
        for (int i = 0; i < m_Indexes.Count; i++)
        {
            if(int.Parse(m_Indexes[i].FileName.Split('-')[0]) == chapter)
            {
                list.Add(m_Indexes[i]);
            }
        }
        return list;
    }
}


