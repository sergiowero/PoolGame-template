using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace PoolsEditor
{
    public class CustomLevelEditor : EditorWindow
    {
        #region Background
        private Rect m_LeftTop = new Rect(0, 0, 40, 40);
        private Rect m_RightTop = new Rect(988, 0, 40, 40);
        private Rect m_Top = new Rect(495, 0, 40, 40);
        private Rect m_Bottom = new Rect(495, 500, 40, 40);
        private Rect m_RightBottom = new Rect(988, 500, 40, 40);
        private Rect m_LeftBottom = new Rect(0, 500, 40, 40);
        private Rect m_OutlineRect = new Rect(0, 0, 1030, 541);
        private Rect m_BackgroundRect = new Rect(25, 25, 0, 0);
        #endregion // Background

        Vector2 backgroundHitPoint = new Vector2();

        int m_ShotCount = 0;

        int m_DescripID;

        GameObject[] standardBallCache;

        private float m_R;

        private Texture2D m_PocketTexture;

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

        //Texture2D m_BackImage;

        TouchObject m_CueBall;
        Dictionary<int, TouchObject> m_Balls = new Dictionary<int, TouchObject>();
       // List<TouchObject> m_OtherBalls = new List<TouchObject>();

        string m_LevelName;

        LevelData m_CurrentLevelData = null;
        LevelDataIndex m_LevelDataIndex = null;

        string m_PrevLevelDataFileName = null;

        Transform m_CustomBallRoot;

        Pocket[] m_Pockets = new Pocket[6];


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
            else
            {
                ((CustomLevelEditor)EditorWindow.GetWindow(typeof(CustomLevelEditor), false, "Level editor")).Close();
            }
        }

        [MenuItem("Window/关卡编辑/同步关卡数据")]
        static void SyncDataManual()
        {
            LevelDataIndex levelDataIndex = Resources.LoadAssetAtPath<LevelDataIndex>(StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataIndexPath);
            if (!levelDataIndex)
            {
                levelDataIndex = ScriptableObject.CreateInstance<LevelDataIndex>();
                AssetDatabase.CreateAsset(levelDataIndex, StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataIndexPath);
            }
            else
            {
                levelDataIndex.Clear();
            }

            System.IO.FileInfo[] fileInfos = StreamTools.GetAllFile(StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataPath, "*-*");
            foreach (var v in fileInfos)
            {
                LevelData data = Resources.LoadAssetAtPath<LevelData>(StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataPath + v.Name);
                data.SpecificPocket =
                    !(data.StartPunishmentPocket == PocketIndexes.None
                    && data.StartRewardPocket == PocketIndexes.None);
                levelDataIndex.Add(data);
            }
            Debug.Log("Synchronize data finished");
        }

        void SyncData()
        {
            System.IO.FileInfo[] fileInfos = StreamTools.GetAllFile(StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataPath, "*-*");

            if (fileInfos.Length != m_LevelDataIndex.Count)
            {
                Debug.Log("Level data indexes need synchronized, now synchronizing");
                m_LevelDataIndex.Clear();
                foreach (var v in fileInfos)
                {
                    LevelData data = Resources.LoadAssetAtPath<LevelData>(StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataPath + v.Name);
                    m_LevelDataIndex.Add(data);
                }
            }
        }

        void HideStandardBalls()
        {
            standardBallCache = new GameObject[15];
            for (int i = 1; i <= 15; i++)
            {
                //trans = GameObject.Find("Ball" + i.ToString()).transform;
                standardBallCache[i - 1] = GameObject.Find("Ball" + i.ToString()).gameObject;
                standardBallCache[i - 1].SetActive(false);
            }
        }

        void ShowStandardBalls()
        {
            for (int i = 0; i < 15; i++)
            {
                standardBallCache[i].SetActive(true);
                standardBallCache[i] = null;
            }
        }

        void Awake()
        {
            if (!EditorApplication.currentScene.Contains("PoolGame"))
                return;
            GetLevelDataIndex();
            m_CustomBallRoot = GameObject.Find("8Ball/OtherObjects").transform;
            Transform trans = GameObject.Find("WhiteBall").transform;
            Texture2D tex = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/EditorTextures/CUEBALL.png");
            m_PocketTexture = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/EditorTextures/Pocket.png");
            m_CueBall = new TouchObject() { transform = trans, texture = tex, id = 0, type = BallType.WHITE };
            HideStandardBalls();

            string ss = "Table/Colliders/pocket_";
            m_Pockets[0] = new Pocket(m_LeftTop, m_PocketTexture, GameObject.Find(ss + "TopLeft"), PocketIndexes.TopLeft);
            m_Pockets[1] = new Pocket(m_Top, m_PocketTexture, GameObject.Find(ss + "TopCenter"), PocketIndexes.TopCenter);
            m_Pockets[2] = new Pocket(m_RightTop, m_PocketTexture, GameObject.Find(ss + "TopRight"), PocketIndexes.TopRight);
            m_Pockets[3] = new Pocket(m_LeftBottom, m_PocketTexture, GameObject.Find(ss + "BottomLeft"), PocketIndexes.BottomLeft);
            m_Pockets[4] = new Pocket(m_Bottom, m_PocketTexture, GameObject.Find(ss + "BottomCenter"), PocketIndexes.BottomCenter);
            m_Pockets[5] = new Pocket(m_RightBottom, m_PocketTexture, GameObject.Find(ss + "BottomRight"), PocketIndexes.BottomRight);

            GameObject o1 = GameObject.FindObjectOfType<WhiteBall>().gameObject;
            Constraint c = o1.GetComponent<Constraint>();
            m_R = o1.GetComponent<SphereCollider>().radius * o1.transform.lossyScale.x;

            TableMin = new Vector2(c.min.x + m_R, c.min.z + m_R);
            TableMax = new Vector2(c.max.x - m_R, c.max.z - m_R);
            XSize = TableMax.x - TableMin.x;
            YSize = TableMax.y - TableMin.y;
            m_Aspect = YSize / XSize;
            m_GridSize = c.GetComponent<SphereCollider>().radius * c.transform.localScale.x - ConstantData.BallRadiusAdjustment;
            AreaWidth = 953;//constant value
            AreaHeight = AreaWidth * m_Aspect;
            m_xRadio = AreaWidth / XSize;
            m_yRadio = AreaHeight / YSize;
            m_GridSize = m_GridSize * AreaWidth / XSize;
            m_BackgroundRect.width = AreaWidth + 2 * m_GridSize;
            m_BackgroundRect.height = AreaHeight + 2 * m_GridSize;
            //m_BackImage = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/Table/bian.png");
            SyncData();
            GetBallObjectsRect();
        }

        void OnDestroy()
        {
            m_LevelEditorWindow = null;

            if (!EditorApplication.currentScene.Contains("PoolGame"))
                return;

            foreach (KeyValuePair<int, TouchObject> k in m_Balls)
            {
                if (k.Value.texture)
                {
                    Resources.UnloadAsset(k.Value.texture);
                }
            }

            foreach(var v in m_Pockets)
            {
                v.Clear();
            }

            RemoveObject();
            AreaWidth = 0;
            AreaHeight = 0;
            XSize = 0;
            YSize = 0;

            ShowStandardBalls();
        }

        private LevelDataIndex GetLevelDataIndex()
        {
            if (m_LevelDataIndex == null)
            {
                m_LevelDataIndex = Resources.LoadAssetAtPath<LevelDataIndex>(StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataIndexPath);
                if (!m_LevelDataIndex)
                {
                    m_LevelDataIndex = ScriptableObject.CreateInstance<LevelDataIndex>();
                    AssetDatabase.CreateAsset(m_LevelDataIndex, StreamTools.GetStreamingAssetsPathInEditor() + ConstantData.MissionLevelDataIndexPath);
                }
            }
            return m_LevelDataIndex;
        }

        public void Remove(object o)
        {
            int id = (int)o;
            if (m_Balls.ContainsKey(id))
            {
                DestroyImmediate(m_Balls[id].transform.gameObject);
                m_Balls.Remove(id);
            }
        }

        public void RemoveObject()
        {
            foreach (var v in m_Balls)
            {
                DestroyImmediate(v.Value.transform.gameObject);
            }
            m_Balls.Clear();
        }

        public void AddBall(object t)
        {
            BallType type = (BallType)t;
            string name = type.ToString();

            GameObject o = Instantiate((GameObject)Resources.Load(name)) as GameObject;

            o.transform.SetParent(m_CustomBallRoot.transform);
            o.transform.localRotation = Quaternion.identity;
            //o.transform.localPosition = new Vector3(-7.6f, -2.3f, 0);

            Texture2D tex = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/EditorTextures/" + name + ".png");
            Rect rect = new Rect(backgroundHitPoint.x, backgroundHitPoint.y, m_GridSize * 2, m_GridSize * 2);
            TouchObject to = new TouchObject() { transform = o.transform, id = o.GetInstanceID(), texture = tex, type = type, rect = rect};
            m_Balls.Add(to.id, to);

            //GetBallObjectsRect();
        }

        public void AddBall(LevelData.BallData data)
        {
            string name = data.Type.ToString();
            GameObject o = Instantiate((GameObject)Resources.Load(name)) as GameObject;

            o.transform.SetParent(m_CustomBallRoot.transform);
            o.transform.localRotation = Quaternion.identity;
            o.transform.localPosition = new Vector3(data.Position.x, -2.3f, data.Position.z);

            Texture2D tex = Resources.LoadAssetAtPath<Texture2D>("Assets/Images/EditorTextures/" + name + ".png");
            TouchObject to = new TouchObject() { transform = o.transform, id = data.ID, rect = data.pRect, texture = tex, type = data.Type };
            m_Balls.Add(to.id , to);
        }

        void ShowWarning(string text)
        {
            Color c = GUI.contentColor;
            GUI.contentColor = Color.red;
            GUILayout.Label(text);
            GUI.contentColor = c;
        }

        void OnGUI()
        {
            if (Application.isPlaying)
            {
                ShowWarning("编辑模式再打开这个窗口。");
                return;
            }

            if (!EditorApplication.currentScene.Contains("PoolGame"))
            {
                ShowWarning("请在PoolGame场景中打开此窗口。");
                return;
            }
            //GUILayout.Label(" count : " + m_Balls.Count);
            DrawBackground();
            DrawBallObjects();
            BackgroundInteractive();
            DrawOtherGUI();
            Repaint();
        }

        void BackgroundInteractive()
        {
            if(Event.current.type == EventType.ContextClick && m_BackgroundRect.Contains(Event.current.mousePosition))
            {
                backgroundHitPoint = Event.current.mousePosition;
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("添加一个红球",""), false, AddBall, BallType.REDCUSTOM);
                gm.AddItem(new GUIContent("添加一个蓝球", ""), false, AddBall, BallType.BLUECUSTOM);
                gm.AddItem(new GUIContent("添加一个黄球", ""), false, AddBall, BallType.YELLOWCUSTOM);
                gm.AddSeparator("---------------");
                gm.AddItem(new GUIContent("添加一个炸弹球", ""), false, AddBall, BallType.BOMB);
                gm.AddItem(new GUIContent("添加一个吸收球", ""), false, AddBall, BallType.ABSORB);
                gm.AddItem(new GUIContent("添加一个破碎球", ""), false, AddBall, BallType.SINGULARITY);
                gm.AddSeparator("----------------");
                gm.AddItem(new GUIContent("添加一个酱油球", ""), false, AddBall, BallType.JIANGYOU);
                gm.AddItem(new GUIContent("添加一个魔鬼球", ""), false, AddBall, BallType.DEMON);
                gm.ShowAsContext();
                Event.current.Use();
            }
        }

        void GetBallObjectsRect()
        {
            m_CueBall.rect = GetRect(m_CueBall.transform); 
            foreach (KeyValuePair<int, TouchObject> k in m_Balls)
            {
                k.Value.rect = GetRect(k.Value.transform);
            }
        }

        Rect GetRect(Transform t)
        {
            Vector3 v = t.position;
            return new Rect((v.x - TableMin.x) * m_xRadio + m_BackgroundRect.x, AreaHeight - (v.z - TableMin.y) * m_yRadio + m_BackgroundRect.y, m_GridSize * 2, m_GridSize * 2);
        }

        void DrawBackground()
        {
            Color c = GUI.color;
            GUI.color = Color.blue;
            GUI.Box(m_OutlineRect, "");
            GUI.color = Color.green;
            GUI.Box(m_BackgroundRect, "");
            GUI.color = c;
            foreach (Pocket v in m_Pockets)
            {
                v.Draw();
            }
        }

        void DrawBallObjects()
        {
            m_CueBall.Draw();
            foreach (KeyValuePair<int, TouchObject> k in m_Balls)
            {
                k.Value.Draw();
            }
        }

        void DrawOtherGUI()
        {
            GUI.BeginGroup(new Rect(0, AreaHeight + 2 * m_GridSize + m_OutlineRect.height - m_BackgroundRect.height, AreaWidth + 2 * m_GridSize + 100, 350));
            m_CurrentLevelData = EditorGUILayout.ObjectField("关卡数据文件： ", m_CurrentLevelData, typeof(LevelData), false) as LevelData;
            m_ShotCount = EditorGUILayout.IntField("击杆数：", m_ShotCount);
            m_DescripID = EditorGUILayout.IntField("描述ID : ", m_DescripID);
            m_LevelName = EditorGUILayout.TextField("关卡名称：", m_LevelName);
            CheckDataEquals();
            if (GUILayout.Button("保存") && !string.IsNullOrEmpty(m_LevelName))
            #region Save configuration
            {
                LevelData data = ScriptableObject.CreateInstance<LevelData>();
                data.cueBallData = new LevelData.BallData(m_CueBall.id, m_CueBall.transform.position, m_CueBall.rect, BallType.WHITE);
                foreach (KeyValuePair<int, TouchObject> k in m_Balls)
                {
                    LevelData.BallData d = new LevelData.BallData(k.Key, k.Value.transform.position, k.Value.rect, k.Value.type);
                    data.ballDatas.Add(d);
                }
                foreach(var v in m_Pockets)
                {
                    if (v.pocketType == PocketType.Punishment)
                        data.StartPunishmentPocket |= v.pocketIndexes;
                    else if (v.pocketType == PocketType.Reward)
                        data.StartRewardPocket |= v.pocketIndexes;
                    else if (v.pocketType == PocketType.BlockOff)
                        data.BlockPockets |= v.pocketIndexes;
                }
                data.shotCount = m_ShotCount;
                data.FileName = m_LevelName;
                data.DescriptionID = m_DescripID;
                AssetDatabase.CreateAsset(data, StreamTools.GetStreamingAssetsPathInEditor() + "LevelDatas/" + m_LevelName + ".asset");
                m_CurrentLevelData = data;
                m_LevelDataIndex.Add(data);
            }
            #endregion //Save configuration
            GUI.skin.label.fontSize = 18;
            GUILayout.Label("说明：右键点击台球桌添加球。 右键点击非白球删除球。右键点击袋口（黑色圆）设置球袋类型（提供色盲模式^-^）");
            GUI.skin.label.fontSize = 12;

            GUI.EndGroup();
        }

        void CheckDataEquals()
        {
            if (m_CurrentLevelData != null && m_CurrentLevelData.FileName != m_PrevLevelDataFileName)
            {
                RemoveObject();
                m_CueBall.transform.position = m_CurrentLevelData.cueBallData.Position;
                m_CueBall.rect = m_CurrentLevelData.cueBallData.pRect;
                List<LevelData.BallData> ballPositions = m_CurrentLevelData.ballDatas;
                for (int i = 0, count = ballPositions.Count; i < count; i++)
                {
                    AddBall(ballPositions[i]);
                }
                foreach(var v in m_Pockets)
                {
                    v.Clear();
                    if ((v.pocketIndexes & m_CurrentLevelData.StartPunishmentPocket) != 0x0)
                    {
                        v.SetAsPunishment();
                    }
                    if ((v.pocketIndexes & m_CurrentLevelData.StartRewardPocket) != 0x0)
                    {
                        v.SetAsReward();
                    }
                    if ((v.pocketIndexes & m_CurrentLevelData.BlockPockets) != 0x0)
                    {
                        v.SetAsBlock();
                    }
                }
                m_CurrentLevelData.SpecificPocket = 
                    !(m_CurrentLevelData.StartPunishmentPocket == PocketIndexes.None 
                    && m_CurrentLevelData.StartRewardPocket == PocketIndexes.None);
                m_ShotCount = m_CurrentLevelData.shotCount;
                m_DescripID = m_CurrentLevelData.DescriptionID;
                m_LevelName = m_CurrentLevelData.FileName;
                m_PrevLevelDataFileName = m_LevelName;
            }
            else if (m_CurrentLevelData == null && m_PrevLevelDataFileName != null)
            {
                m_PrevLevelDataFileName = null;
            }
        }
    }

    class Pocket
    {
        public Rect rect;
        public Texture2D texture;
        public GameObject pocketGameObject;

        private Color punishmentColor;
        private Color rewardColor;
        private Color blockColor;

        private GameObject punishmentObject;
        private GameObject rewardObject;
        private GameObject blockObject;

        private PocketType m_Type;
        public PocketType pocketType { get { return m_Type; } set { m_Type = value; } }

        private PocketIndexes m_Index;
        public PocketIndexes pocketIndexes { get { return m_Index; } set { m_Index = value; } }

        private bool colorBlindMode = false;

        public Pocket(Rect _rect, Texture2D _texture, GameObject _pocketGameObject, PocketIndexes _Index)
        {
            rect = _rect;
            texture = _texture;
            pocketGameObject = _pocketGameObject;
            m_Index = _Index;

            (punishmentObject = pocketGameObject.transform.FindChild("PunishmentSprite").gameObject).SetActive(false);
            (rewardObject = pocketGameObject.transform.FindChild("RewardSprite").gameObject).SetActive(false);
            (blockObject = pocketGameObject.transform.FindChild("BlockOff").gameObject).SetActive(false);

            punishmentColor = Color.red;
            rewardColor = Color.green;
            blockColor = Color.white;

            m_Type = PocketType.None;
        }

        public void Draw()
        {
            Color c = GUI.color;
            if (m_Type == PocketType.Punishment)
                GUI.color = punishmentColor;
            else if (m_Type == PocketType.Reward)
                GUI.color = rewardColor;
            else if (m_Type == PocketType.BlockOff)
                GUI.color = blockColor;
            else
                GUI.color = Color.black;
            GUI.DrawTexture(rect, texture);
            GUI.color = c;
            if(Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("惩罚袋"), false, SetAsPunishment);
                gm.AddItem(new GUIContent("奖励袋"), false, SetAsReward);
                gm.AddItem(new GUIContent("堵住"), false, SetAsBlock);
                gm.AddItem(new GUIContent("清除"), false, Clear);
                gm.AddItem(new GUIContent(colorBlindMode ? "取消色盲模式" : "色盲模式"), false, SetColorMode);
                gm.ShowAsContext();
                Event.current.Use();
            }
        }

        public void SetAsPunishment()
        {
            m_Type = PocketType.Punishment;
            punishmentObject.SetActive(true);
            rewardObject.SetActive(false);
            blockObject.SetActive(false);
        }

        public void SetAsReward()
        {
            m_Type = PocketType.Reward;
            punishmentObject.SetActive(false);
            rewardObject.SetActive(true);
            blockObject.SetActive(false);
        }

        public void SetAsBlock()
        {
            m_Type = PocketType.BlockOff;
            punishmentObject.SetActive(false);
            rewardObject.SetActive(false);
            blockObject.SetActive(true);
        }

        public void Clear()
        {
            m_Type = PocketType.None;
            punishmentObject.SetActive(false);
            rewardObject.SetActive(false);
            blockObject.SetActive(false);
        }

        public void SetColorMode()
        {
            colorBlindMode = !colorBlindMode;
            if(colorBlindMode)
            {
                punishmentColor = Color.yellow;
                rewardColor = Color.blue;
            }
            else
            {
                punishmentColor = Color.red;
                rewardColor = Color.green;
            }
        }

    }

    class TouchObject
    {
        public int id;

        public BallType type;

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

        public void Draw()
        {
            GUI.DrawTexture(rect, texture);
            GUI.Label(rect, id.ToString());
            if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
            {
                if (type == BallType.WHITE)
                    return;

                GenericMenu gMenu = new GenericMenu();
                gMenu.AddItem(new GUIContent("Delete", ""), false, CustomLevelEditor.m_LevelEditorWindow.Remove, id);

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

                v.x = Mathf.Clamp(v.x, 25, CustomLevelEditor.AreaWidth + 25);
                v.y = Mathf.Clamp(v.y, 25, CustomLevelEditor.AreaHeight + 25);

                rect.position = v;
            }
            //计算球实际的位置
            Vector3 vv = transform.position;
            vv.x = (rect.x - 25) * CustomLevelEditor.XSize / CustomLevelEditor.AreaWidth + CustomLevelEditor.TableMin.x;
            vv.z = (CustomLevelEditor.AreaHeight - (rect.y - 25)) * CustomLevelEditor.YSize / CustomLevelEditor.AreaHeight + CustomLevelEditor.TableMin.y;
            transform.position = vv;
        }
    }

}


