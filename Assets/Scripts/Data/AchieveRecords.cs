using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AchieveRecords 
{
    private Dictionary<int, int> achieveRecords = new Dictionary<int, int>();

    public int this[int id]
    {
        get 
        {
            if (achieveRecords.ContainsKey(id))
                return achieveRecords[id];
            else
                return 0;
        }
    }

    public void Mark(string levelName, AchieveType type, int num = 1)
    {
        foreach(var v in AchieveConfiguration.achieveDictionary)
        {
            if(v.Value.level.CompareTo(levelName) == 0 && v.Value.type == type)
            {
                Add(v.Key, num);
                break;
            }
        }
    }

    private void Add(int id, int num)
    {
        if(!achieveRecords.ContainsKey(id))
        {
            achieveRecords.Add(id, num);
        }
        else
        {
            achieveRecords[id] += num;
        }
        if (achieveRecords[id] > AchieveConfiguration.achieveDictionary[id].goal)
            achieveRecords[id] = AchieveConfiguration.achieveDictionary[id].goal;
        ConstantData.achieveRecords = this;
    }
}
