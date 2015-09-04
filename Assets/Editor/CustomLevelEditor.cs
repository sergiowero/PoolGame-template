using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class CustomLevelEditor : EditorWindow
{
    public static Vector2 TableMin;
    public static Vector2 TableMax;

    private float m_GridSize;

    public static float XSize;
    public static float YSize;

    private float m_xRadio;
    private float m_yRadio;


    private float m_Aspect;
    public static float AreaWidth;
    public static float AreaHeight;

    static CustomLevelEditor m_LevelEditorWindow;

    Texture2D m_BackImage;

    Dictionary<int, TouchObject> m_touchObjects = new Dictionary<int, TouchObject>();

    string m_LevelName;

    LevelData m_CurrentLevelData = null;

    string m_PrevLevelDataFileName = null;

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
        TableMin = new Vector2(c.xAxis.x, c.zAxis.x);
        TableMax = new Vector2(c.xAxis.y, c.zAxis.y);
        XSize = TableMax.x - TableMin.x;
        YSize = TableMax.y - TableMin.y;
        m_Aspect = YSize / XSize;
        m_GridSize = c.GetComponent<SphereCollider>().radius * c.transform.localScale.x - ConstantData.BallRadiusAdjustment;
        AreaWidth = Screen.width / 2;
        AreaHeight = AreaWidth * m_Aspect;
        m_xRadio = AreaWidth / XSize;
        m_yRadio = AreaHeight / YSize;
        m_GridSize = m_GridSize * AreaWidth / XSize;

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
        AreaWidth = 0;
        AreaHeight = 0;
        XSize = 0;
        YSize = 0;
    }

    void OnGUI()
    {
        //GUI.BeginGroup(new Rect(0, 0, m_AreaWidth + 100, m_AreaHeight + 100));

        DrawBackground();

        DrawBallObjects();

        DrawOtherGUI();

        Repaint();
        //GUI.EndGroup();
    }

    void GetBallObjectsRect()
    {
        foreach (KeyValuePair<int, TouchObject> k in m_touchObjects)
        {
            Vector3 v = k.Value.transform.position;
            Rect rect = new Rect((v.x - TableMin.x) * m_xRadio, AreaHeight - (v.z - TableMin.y) * m_yRadio, m_GridSize * 2, m_GridSize * 2);
            k.Value.rect = rect;
            k.Value.multiplicative = (int)m_GridSize;
        }
    }


    void DrawBackground()
    {
        Color c = GUI.color;
        GUI.color = Color.green;
        GUI.Box(new Rect(0, 0, AreaWidth + 2 * m_GridSize, AreaHeight + 2 * m_GridSize), "");
        GUI.color = c;
    }

    void DrawBallObjects()
    {
        foreach (KeyValuePair<int, TouchObject> k in m_touchObjects)
        {
            k.Value.Update();
        }
    }

    void DrawOtherGUI()
    {
        GUI.BeginGroup(new Rect(0, AreaHeight + 2 * m_GridSize, AreaWidth + 2 * m_GridSize + 100, 100));
        m_CurrentLevelData = EditorGUILayout.ObjectField("关卡数据文件： ", m_CurrentLevelData, typeof(LevelData), false) as LevelData;
        CheckDataEquals();
        GUI.skin.label.fontSize = 20;
        GUI.skin.textField.fontSize = 20;
        GUI.Label(new Rect(0, 50, 120, 30), "关卡名称：");
        m_LevelName = EditorGUI.TextField(new Rect(120, 50, 80, 30), m_LevelName);
        if (GUI.Button(new Rect(255, 50, 100, 30), "保存") && !string.IsNullOrEmpty(m_LevelName))
        {
            LevelData data = ScriptableObject.CreateInstance<LevelData>();
            foreach (KeyValuePair<int, TouchObject> k in m_touchObjects)
            {
                LevelData.PositionDatas d = new LevelData.PositionDatas(k.Key, k.Value.transform.position, k.Value.rect);
                data.BallsPosition.Add(d);
            }
            data.FileName = m_LevelName;
            AssetDatabase.CreateAsset(data, StreamTools.GetStreamingAssetsPathInEditor() + "LevelDatas/" + m_LevelName + ".asset");
            m_CurrentLevelData = data;
        }
        GUI.skin.label.fontSize = 12;
        GUI.skin.textField.fontSize = 12;

        GUI.EndGroup();
    }

    void CheckDataEquals()
    {
        if(m_CurrentLevelData != null && m_CurrentLevelData.FileName != m_PrevLevelDataFileName)
        {
            List<LevelData.PositionDatas> ballPositions = m_CurrentLevelData.BallsPosition;
           for(int i = 0, count = ballPositions.Count; i < count; i++)
           {
               int key = ballPositions[i].ID;
               m_touchObjects[key].transform.position = ballPositions[i].Positon;
               m_touchObjects[key].rect = ballPositions[i].pRect;
           }
           m_LevelName = m_CurrentLevelData.FileName;
           m_PrevLevelDataFileName = m_LevelName;
        }
    }
}

class TouchObject
{
    public Transform transform;
    public Texture2D texture;

    public Rect rect;

    private Vector2 m_LastPosition;
    private Vector2 m_PositionDelta;

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
            m_PositionDelta = Event.current.mousePosition - rect.position;
            m_Dragging = true;
            Event.current.Use();
        }
        else if (m_Dragging)
        {
            Vector2 v = Event.current.mousePosition - m_PositionDelta;
            //v.x = MathTools.Round2Number(v.x, multiplicative);
            //v.y = MathTools.Round2Number(v.y, multiplicative);

            v.x = Mathf.Clamp(v.x, 0, CustomLevelEditor.AreaWidth);
            v.y = Mathf.Clamp(v.y, 0, CustomLevelEditor.AreaHeight);

            rect.position = v;
        }

        //计算球实际的位置
        Vector3 vv = transform.position;
        vv.x = rect.x * CustomLevelEditor.XSize / CustomLevelEditor.AreaWidth + CustomLevelEditor.TableMin.x;
        vv.z = (CustomLevelEditor.AreaHeight - rect.y) * CustomLevelEditor.YSize / CustomLevelEditor.AreaHeight + CustomLevelEditor.TableMin.y;
        transform.position = vv;
    }



}
