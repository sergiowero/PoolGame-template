﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MissionRecords
{
    public Dictionary<string, MRecord> Records = new Dictionary<string, MRecord>();

    public void Record(string name, int star, int score)
    {
        MRecord r;
        if(!Records.ContainsKey(name))
        {
            r = new MRecord() { Star = star, HighScore = score };
            Records.Add(name, r);
        }
        else
        {
            r = Records[name];
            r.HighScore = score;
            r.Star = star;
        }
    }

    public int GetTotleStar()
    {
        int stars = 0;
        foreach (KeyValuePair<string, MRecord> record in Records)
        {
            stars += record.Value.Star;
        }
        return stars;
    }

    public int GetHighScore(string name)
    {
        if(Records.ContainsKey(name))
        {
            return Records[name].HighScore;
        }
        return 0;
    }

    public int GetStar(string name)
    {
        if(Records.ContainsKey(name))
        {
            return Records[name].Star;
        }
        return 0;
    }

    public int GetChapterStars(int chapter)
    {
        int star = 0;
        string missionProcess = PlayerPrefs.GetString(ConstantData.MissionProgressKeyName);
        for(int i = 0; i < 20; i++)
        {
            string name = chapter + "-" + i;
            if(ConstantData.LevelDatas.Comp(name, missionProcess) > 0)
                break;
            star += GetStar(name);
        }
        return star;
    }

    [System.Serializable]
    public class MRecord
    {
        public int HighScore;
        public int Star;
    }
}


