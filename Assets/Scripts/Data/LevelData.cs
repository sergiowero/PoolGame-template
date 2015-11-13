using UnityEngine;
using System.Collections.Generic;

public class LevelData : ScriptableObject 
{
    public string FileName;

    [System.Serializable]
    public class BallData
    {
        public int ID;
        public Vector3 Position;
        public BallType Type;
        [HideInInspector]
        public Rect pRect;

        public BallData(int _ID, Vector3 _Position, Rect _pRect, BallType _type)
        {
            ID = _ID;
            Position = _Position;
            pRect = _pRect;
            Type = _type;
        }
    }

    [System.Serializable]
    public class OtherObjectDatas
    {
        public int ID;
        public Vector3 Position;
        public Vector3 Rotation;
        [HideInInspector]
        public Rect pRect;

        public OtherObjectDatas(int _ID, Vector3 _Position, Vector3 _Rotation, Rect _pRect)
        {
            ID = _ID;
            Position = _Position;
            Rotation = _Rotation;
            pRect = _pRect;
        }
    }

    public override bool Equals(object o)
    {
        if (!(o is LevelData) || o == null) 
            return false;
        LevelData d = (LevelData)o;
        if (d.FileName.CompareTo(FileName) == 0)
            return true;
        return false;
    }

    public override int GetHashCode()
    {
        return FileName.GetHashCode();
    }

    [SerializeField]
    private bool m_SpecificPocket;
    [SerializeField]
    private PocketIndexes m_BlockPocketList = PocketIndexes.None;
    [SerializeField]
    private PocketIndexes m_StartPunishmentPocket = PocketIndexes.None;
    [SerializeField]
    private PocketIndexes m_StartRewardPocket = PocketIndexes.None;
    [SerializeField]
    private int m_ShotCount;
    [SerializeField]
    private int m_DescriptionID;
    [SerializeField]
    private BallData m_CueballData;
    [SerializeField]
    private List<BallData> m_BallDatas = new List<BallData>();
    [SerializeField]
    private List<OtherObjectDatas> m_OtherObjectsPosition = new List<OtherObjectDatas>();
    public List<BallData> ballDatas { get { return m_BallDatas; } }
    public List<OtherObjectDatas> OtherObjectsPosition { get { return m_OtherObjectsPosition; } }
    public BallData cueBallData { get { return m_CueballData; } set { m_CueballData = value; } }
    public PocketIndexes BlockPockets { get { return m_BlockPocketList; } set { m_BlockPocketList = value; } }
    public PocketIndexes StartPunishmentPocket { get { return m_StartPunishmentPocket; } set { m_StartPunishmentPocket = value; } }
    public PocketIndexes StartRewardPocket { get { return m_StartRewardPocket; } set { m_StartRewardPocket = value; } }
    public int shotCount { get { return m_ShotCount; } set { m_ShotCount = value; } }
    public int DescriptionID { get { return m_DescriptionID; } set { m_DescriptionID = value; } }
    public bool SpecificPocket { get { return m_SpecificPocket; } set { m_SpecificPocket = value; } }
}


