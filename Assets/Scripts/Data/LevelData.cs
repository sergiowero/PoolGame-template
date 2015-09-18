using UnityEngine;
using System.Collections.Generic;

public class LevelData : ScriptableObject 
{
    public static LevelData CurrentLevel;

    public string FileName;

    [System.Serializable]
    public class PositionDatas
    {
        public int ID;
        public Vector3 Positon;
        public Rect pRect;

        public PositionDatas(int _ID, Vector3 _Position, Rect _pRect)
        {
            ID = _ID;
            Positon = _Position;
            pRect = _pRect;
        }
    }

    [System.Serializable]
    public class DisplayDatas
    {
        public int ID;
        public bool Draw;

        public DisplayDatas(int _ID, bool _Draw)
        {
            ID = _ID;
            Draw = _Draw;
        }
    }

    [System.Serializable]
    public class OtherObjectDatas
    {
        public int ID;
        public Vector3 Position;
        public Rect pRect;
        public BallType Type;

        public OtherObjectDatas(int _ID, Vector3 _Position, Rect _pRect, BallType _Type)
        {
            ID = _ID;
            Position = _Position;
            pRect = _pRect;
            Type = _Type;
        }
    }

    [SerializeField]
    private List<PositionDatas> m_BallsPosition = new List<PositionDatas>();
    [SerializeField]
    private List<DisplayDatas> m_BallsDrawList = new List<DisplayDatas>();
    [SerializeField]
    private List<OtherObjectDatas> m_OtherObjectsPosition = new List<OtherObjectDatas>();
    public List<PositionDatas> BallsPosition { get { return m_BallsPosition; } }
    public List<DisplayDatas> BallsDrawList { get { return m_BallsDrawList; } }
    public List<OtherObjectDatas> OtherObjectsPosition { get { return m_OtherObjectsPosition; } }
}

