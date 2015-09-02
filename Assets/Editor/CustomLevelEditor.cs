using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class CustomLevelEditor : EditorWindow
{
    Vector2 m_TableMin;
    Vector2 m_TableMax;

    private float m_GridSize;

    private float m_XSize;
    private float m_YSize;

    private float m_xRadio;
    private float m_yRadio;


    private float m_Aspect;
    private float m_AreaWidth;
    private float m_AreaHeight;

    static CustomLevelEditor m_LevelEditorWindow;

    Texture2D m_BackImage;

    Dictionary<int, TouchObject> m_touchObjects = new Dictionary<int, TouchObject>();

    [MenuItem("Window/关卡编辑")]
    static void Init()
    {
        if(m_LevelEditorWindow == null)
            m_LevelEditorWindow = (CustomLevelEditor)EditorWindow.GetWindow(typeof(CustomLevelEditor), false, "Level editor");
        m_LevelEditorWindow.Show();
    }

    void Awake()
    {
        Transform trans = GameObject.Find("WhiteBall").transform;
        Texture2D tex = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/Side/Siding.png");
        m_touchObjects.Add(0, new TouchObject() { transform = trans, texture = tex });
        for(int i = 1; i <= 15; i++)
        {
            trans = GameObject.Find("Ball" + i.ToString()).transform;
            tex = Resources.Load<Texture2D>("BallsIcon/" + i.ToString());
            TouchObject to = new TouchObject();
            to.transform = trans;
            to.texture = tex;
            try
            {
                m_touchObjects.Add(i, to);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        // 暂时留空，  没有其他物品
        //trans = GameObject.Find("OtherObjects").transform;
        //int j = 0;
        //foreach(Transform t in trans)
        //{
        //    j++;
        //}


        Constraint c = GameObject.FindObjectOfType<WhiteBall>().GetComponent<Constraint>();
        m_TableMin = new Vector2(c.xAxis.x, c.zAxis.x);
        m_TableMax = new Vector2(c.xAxis.y, c.zAxis.y);
        m_XSize = m_TableMax.x - m_TableMin.x;
        m_YSize = m_TableMax.y - m_TableMin.y;
        m_Aspect = m_YSize / m_XSize;
        m_GridSize = c.GetComponent<SphereCollider>().radius * c.transform.localScale.x - ConstantData.BallRadiusAdjustment;
        m_AreaWidth = Screen.width / 2;
        m_AreaHeight = m_AreaWidth * m_Aspect;
        m_xRadio = m_AreaWidth / m_XSize;
        m_yRadio = m_AreaHeight / m_YSize;
        m_GridSize = m_GridSize * m_AreaWidth / m_XSize;

        m_BackImage = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/MatchScene/QuickFire/Box.png");

        GetBallObjectsRect();
    }

    void OnDestroy()
    {
        m_LevelEditorWindow = null;
        if (m_BackImage) Resources.UnloadAsset(m_BackImage);
        foreach (KeyValuePair<int, TouchObject> k in m_touchObjects)
        {
            if(k.Value.texture)
            {
                Resources.UnloadAsset(k.Value.texture);
            }
        }
        m_touchObjects.Clear();
    }

    void OnGUI()
    {
        //GUI.BeginGroup(new Rect(0, 0, m_AreaWidth + 100, m_AreaHeight + 100));

        DrawBackground();

        DrawBallObjects();

        Repaint();
        //GUI.EndGroup();
    }

    void GetBallObjectsRect()
    {
        foreach (KeyValuePair<int, TouchObject> k in m_touchObjects)
        {
            Vector3 v = k.Value.transform.position;
            int x = MathTools.Round2Number((int)((v.x - m_TableMin.x) * m_xRadio), (int)m_GridSize);
            int y = MathTools.Round2Number((int)m_AreaHeight - (int)((v.z - m_TableMin.y) * m_yRadio), (int)m_GridSize);
            Rect rect = new Rect(x, y, m_GridSize * 2, m_GridSize * 2);
            k.Value.rect = rect;
            k.Value.multiplicative = (int)m_GridSize;
        }
    }


    void DrawBackground()
    {
        for (float x = 0; x <= m_AreaWidth; x += m_GridSize)
        {
            for (float y = 0; y <= m_AreaHeight; y += m_GridSize)
            {
                GUI.DrawTexture(new Rect(x, y, m_GridSize, m_GridSize), m_BackImage);
            }
        }
    }

    void DrawBallObjects()
    {
        foreach (KeyValuePair<int, TouchObject> k in m_touchObjects)
        {
            k.Value.Update();
            break;
        }
    }
}

class TouchObject
{
    public Transform transform;
    public Texture2D texture;

    public Rect rect;

    private Vector2 m_LastPosition;
    private Vector2 m_StartPosition;

    private bool m_Dragging;

    public int multiplicative;

    public TouchObject()
    {
        m_Dragging = false;
    }


    public void Update()
    {
        GUI.DrawTexture(rect, texture);
        if(Event.current.type == EventType.MouseUp)
        {
            //m_LastPosition = Vector3.zero;
            m_Dragging = false;
        }
        else if(Event.current.type == EventType.MouseDown
            && rect.Contains(Event.current.mousePosition))
        {
            m_LastPosition = Event.current.mousePosition;
            m_Dragging = true;
            Event.current.Use();
            Debug.Log("Hit!");
        }
        else if (m_Dragging)
        {
            Vector2 v = Event.current.mousePosition - m_LastPosition;
            rect.x += v.x;
            rect.y += v.y;
            m_LastPosition = Event.current.mousePosition;
            rect.x = MathTools.Round2Number((int)rect.x, multiplicative);
            rect.y = MathTools.Round2Number((int)rect.y, multiplicative);
        }
    }



}
