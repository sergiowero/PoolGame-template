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

    public static CustomLevelEditor m_LevelEditorWindow;

    Texture2D m_BackImage;

    Dictionary<int, TouchObject> m_StardardBalls = new Dictionary<int, TouchObject>();
    List<TouchObject> m_OtherBalls = new List<TouchObject>();

    string m_LevelName;

    LevelData m_CurrentLevelData = null;

    string m_PrevLevelDataFileName = null;

    Transform m_CustomBallRoot;

    [MenuItem("Window/关卡编辑/打开")]
    static void Init()
    {
        if (m_LevelEditorWindow == null)
            m_LevelEditorWindow = (CustomLevelEditor)EditorWindow.GetWindow(typeof(CustomLevelEditor), false, "Level editor");
        m_LevelEditorWindow.Show();
    }

    [MenuItem("Window/关卡编辑/关闭")]
    static void CloseWindow()
    {
        if (m_LevelEditorWindow)
            m_LevelEditorWindow.Close();
    }

    void Awake()
    {
        m_CustomBallRoot = GameObject.Find("8Ball/OtherObjects").transform;
        Transform trans = GameObject.Find("WhiteBall").transform;
        Texture2D tex = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/Side/Siding.png");
        m_StardardBalls.Add(0, new TouchObject() { transform = trans, texture = tex, id = 0, type = BallType.WHITE });
        for (int i = 1; i <= 15; i++)
        {
            trans = GameObject.Find("Ball" + i.ToString()).transform;
            tex = Resources.Load<Texture2D>("BallsIcon/" + i.ToString());
            TouchObject to = new TouchObject();
            to.transform = trans;
            to.texture = tex;
            to.id = i;
            to.type = trans.GetComponent<PoolBall>().ballType;
            try
            {
                m_StardardBalls.Add(i, to);
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
        AreaWidth = 953;//constant value
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
        foreach (KeyValuePair<int, TouchObject> k in m_StardardBalls)
        {
            if (k.Value.texture)
            {
                Resources.UnloadAsset(k.Value.texture);
            }
        }

        while(m_OtherBalls.Count != 0)
        {
            Remove(m_OtherBalls[0]);
        }
        
        m_StardardBalls.Clear();
        AreaWidth = 0;
        AreaHeight = 0;
        XSize = 0;
        YSize = 0;
    }

    public void Remove(object o)
    {
        TouchObject to = (TouchObject)o;
        if(m_OtherBalls.Contains(to))
        {
            DestroyImmediate(to.transform.gameObject);
            m_OtherBalls.Remove(to);
        }
        //if (m_StardardBalls.ContainsKey(id))
        //{
        //    TouchObject to = m_StardardBalls[id];
        //    DestroyImmediate(to.transform.gameObject);
        //    m_StardardBalls.Remove(id);
        //    I--;
        //}
    }

    public void Hide(object o)
    {
        int id = (int)o;
        if (m_StardardBalls.ContainsKey(id))
        {
            m_StardardBalls[id].Hide();
        }
    }

    public void AddBall(BallType type)
    {
        string name = type.ToString();

        GameObject o = Instantiate((GameObject)Resources.Load(name)) as GameObject;

        o.transform.SetParent(m_CustomBallRoot.transform);
        o.transform.localRotation = Quaternion.identity;
        o.transform.localPosition = new Vector3(-7.6f, -2.3f, 0);

        Texture2D tex = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/EditorTextures/" + name + ".png");
        TouchObject to = new TouchObject() { transform = o.transform, id = o.GetInstanceID(), texture = tex, draw = true, type = type };
        m_OtherBalls.Add(to);

        GetBallObjectsRect();
    }

    public void AddBall(LevelData.OtherObjectDatas data)
    {
        string name = data.Type.ToString();
        GameObject o = Instantiate((GameObject)Resources.Load(name)) as GameObject;

        o.transform.SetParent(m_CustomBallRoot.transform);
        o.transform.localRotation = Quaternion.identity;
        o.transform.localPosition = new Vector3(data.Position.x, -2.3f, data.Position.z);

        Texture2D tex = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/EditorTextures/" + name + ".png");
        TouchObject to = new TouchObject() { transform = o.transform, id = data.ID, rect = data.pRect, texture = tex, type = data.Type, draw = true };
        m_OtherBalls.Add(to);
    }

    void OnGUI()
    {
        //GUI.BeginGroup(new Rect(0, 0, m_AreaWidth + 100, m_AreaHeight + 100));
        if (Application.isPlaying)
        {
            Color c = GUI.contentColor;
            GUI.contentColor = Color.red;
            GUILayout.Label("编辑模式再打开这个窗口。");
            GUI.contentColor = c;
            return;
        }

        GUILayout.Label(" count : " + m_OtherBalls.Count)
            ;
        DrawBackground();

        DrawBallObjects();

        DrawOtherGUI();

        DrawBallListCheckBox();

        Repaint();
        //GUI.EndGroup();
    }

    void GetBallObjectsRect()
    {
        foreach (KeyValuePair<int, TouchObject> k in m_StardardBalls)
        {
            Vector3 v = k.Value.transform.position;
            Rect rect = new Rect((v.x - TableMin.x) * m_xRadio, AreaHeight - (v.z - TableMin.y) * m_yRadio, m_GridSize * 2, m_GridSize * 2);
            k.Value.rect = rect;
            k.Value.draw = k.Value.transform.GetComponent<Renderer>().enabled;
        }
        foreach(var v in m_OtherBalls)
        {
            Vector3 vv = v.transform.position;
            Rect rect = new Rect((vv.x - TableMin.x) * m_xRadio, AreaHeight - (vv.z - TableMin.y) * m_yRadio, m_GridSize * 2, m_GridSize * 2);
            v.rect = rect;
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
        foreach (KeyValuePair<int, TouchObject> k in m_StardardBalls)
        {
            k.Value.Draw();
        }
        foreach(var v in m_OtherBalls)
        {
            v.Draw();
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
            foreach (KeyValuePair<int, TouchObject> k in m_StardardBalls)
            {
                    LevelData.PositionDatas d = new LevelData.PositionDatas(k.Key, k.Value.transform.position, k.Value.rect);
                    data.BallsPosition.Add(d);
                    LevelData.DisplayDatas dd = new LevelData.DisplayDatas(k.Key, k.Value.draw);
                    data.BallsDrawList.Add(dd);
            }
            foreach(var v in m_OtherBalls)
            {
                Vector3 vv = v.transform.position;
                vv.y = -2.3f;
                v.transform.position = vv;
                LevelData.OtherObjectDatas d = new LevelData.OtherObjectDatas(v.id, v.transform.position, v.rect, v.type);
                data.OtherObjectsPosition.Add(d);
            }
            data.FileName = m_LevelName;
            AssetDatabase.CreateAsset(data, StreamTools.GetStreamingAssetsPathInEditor() + "LevelDatas/" + m_LevelName + ".asset");
            m_CurrentLevelData = data;
        }
        GUI.skin.label.fontSize = 12;
        GUI.skin.textField.fontSize = 12;

        if (GUI.Button(new Rect(AreaWidth + 2 * m_GridSize - 500, 50, 150, 30), "添加一个红球"))
        {
            AddBall(BallType.REDCUSTOM);
        }
        if (GUI.Button(new Rect(AreaWidth + 2 * m_GridSize - 350, 50, 150, 30), "添加一个蓝球"))
        {
            AddBall(BallType.BLUECUSTOM);
        }
        if (GUI.Button(new Rect(AreaWidth + 2 * m_GridSize - 200, 50, 150, 30), "添加一个黄球"))
        {
            AddBall(BallType.YELLOWCUSTOM);
        }


        GUI.EndGroup();
    }

    void DrawBallListCheckBox()
    {
        float height = AreaHeight + 2 * m_GridSize;
        float heightDivide16 = height * 0.0625f;
        GUI.BeginGroup(new Rect(AreaWidth + 2 * m_GridSize, 0, 100, height));
        GUI.Box(new Rect(0, 0, 100, height), "");

        int i = 0;
        foreach (KeyValuePair<int, TouchObject> k in m_StardardBalls)
        {
            Color c = GUI.color;
            GUI.color = Color.blue;
            GUI.Box(new Rect(0, i * heightDivide16, 100, heightDivide16 - 1), "");
            GUI.color = c;
            k.Value.draw = GUI.Toggle(new Rect(0, i * heightDivide16, 100, heightDivide16 - 5), k.Value.draw, "Ball " + k.Key);
            k.Value.CheckValueChanged();
            i++;
        }

        GUI.EndGroup();
    }

    void CheckDataEquals()
    {
        if (m_CurrentLevelData != null && m_CurrentLevelData.FileName != m_PrevLevelDataFileName)
        {
            while(m_OtherBalls.Count != 0)
            {
                Remove(m_OtherBalls[0]);
            }
            List<LevelData.PositionDatas> ballPositions = m_CurrentLevelData.BallsPosition;
            for (int i = 0, count = ballPositions.Count; i < count; i++)
            {
                int key = ballPositions[i].ID;
                m_StardardBalls[key].transform.position = ballPositions[i].Positon;
                m_StardardBalls[key].rect = ballPositions[i].pRect;
            }
            List<LevelData.DisplayDatas> ballDisplays = m_CurrentLevelData.BallsDrawList;
            for (int i = 0, count = ballDisplays.Count; i < count; i++)
            {
                m_StardardBalls[ballDisplays[i].ID].draw = ballDisplays[i].Draw;
            }
            List<LevelData.OtherObjectDatas> otherData = m_CurrentLevelData.OtherObjectsPosition;
            for (int i = 0, count = otherData.Count; i < count; i++)
            {
                AddBall(otherData[i]);
            }
            m_LevelName = m_CurrentLevelData.FileName;
            m_PrevLevelDataFileName = m_LevelName;
        }
        else if (m_CurrentLevelData == null && m_PrevLevelDataFileName != null)
        {
            m_PrevLevelDataFileName = null;
        }
    }
}

class TouchObject
{
    public int id;

    public BallType type;

    public bool draw = true;

    private bool prevDraw = true;

    public Transform transform;
    public Texture2D texture;

    public Rect rect;

    private Vector2 m_LastPosition;
    private Vector2 m_PositionDelta;

    private bool m_Dragging;

    public TouchObject()
    {
        m_Dragging = false;
    }

    public void CheckValueChanged()
    {
        if (prevDraw != draw)
        {
            if (draw)
            {
                transform.GetComponent<PoolBall>().Display();
            }
            else
            {
                transform.GetComponent<PoolBall>().Hide();
            }
            prevDraw = draw;
        }
    }

    public void Hide()
    {
        if (!draw) return;

        transform.GetComponent<Renderer>().enabled = false;
        transform.GetComponent<Collider>().enabled = false;
        transform.FindChild("Shadow").gameObject.SetActive(false);
        transform.GetComponent<BallShadowRenderer>().enabled = false;
        draw = false;
        prevDraw = draw;
    }

    public void Draw()
    {
        if (!draw) return;
        GUI.DrawTexture(rect, texture);
        GUI.Label(rect, id.ToString());
        if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
        {
            if (type == BallType.WHITE)
                return;

            GenericMenu gMenu = new GenericMenu();

            if (type <= BallType.STRIPE)
                gMenu.AddItem(new GUIContent("Hide", ""), false, CustomLevelEditor.m_LevelEditorWindow.Hide, id);
            else
                gMenu.AddItem(new GUIContent("Delete", ""), false, CustomLevelEditor.m_LevelEditorWindow.Remove, this);

            gMenu.ShowAsContext();
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            //m_LastPosition = Vector3.zero;
            m_Dragging = false;
        }
        else if (Event.current.type == EventType.MouseDown
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
