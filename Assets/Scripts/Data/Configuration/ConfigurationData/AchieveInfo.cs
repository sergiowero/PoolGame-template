using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AchieveType
{
    Finish = 0,
    FinishWithTriangularPole = 1
}

public class AchieveInfo
{
    public int id { get; set; }
    public string name { get; set; }
    public AchieveType type { get; set; }
    public string level { get; set; }
    public int goal { get; set; }
    public int DescriptionID { get; set; }
    public string Description { get; set; }
    public string IconName { get; set; }
}

