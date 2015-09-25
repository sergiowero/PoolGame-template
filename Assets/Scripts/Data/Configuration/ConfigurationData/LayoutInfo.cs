using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutInfo
{
    public int Chapter { get; set; }
    public int Level { get; set; }
    public int Star1 { get; set; }
    public int Star2 { get; set; }
    public int Star3 { get; set; }
    public int OpenStar { get; set; }
    public int Cost { get; set; }

    public int GetStarWithScore(int score)
    {
        if (score >= Star3)
            return 3;
        if (score >= Star2)
            return 2;
        if (score >= Star1)
            return 1;
        return 0;
    }
}

public class LayoutInfoKey
{
    public int Chapter { get; set; }
    public int Level { get; set; }

    public override bool Equals(object obj)
    {
        if (!(obj is LayoutInfoKey) || obj == null)
            return false;

        LayoutInfoKey key = obj as LayoutInfoKey;
        return key.Chapter == Chapter && key.Level == Level;
    }

    public override int GetHashCode()
    {
        return Chapter.GetHashCode() + Level.GetHashCode();
    }

    public static LayoutInfoKey GenerateKey(int chapter, int level)
    {
        return new LayoutInfoKey() { Chapter = chapter, Level = level };
    }
}
