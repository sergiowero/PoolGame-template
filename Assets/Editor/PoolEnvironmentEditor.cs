using UnityEngine;
using UnityEditor;
using System.Collections;

public class PoolEnvironmentEditor : EditorWindow
{
    PoolDataAsset m_DataAsset, m_DataAssetTemp;

    string filePath;

    string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "", s6 = "", s7 = "";
    float f = 0;
    GUISkin skin;


    PhysicMaterial m_Railpm, m_Ballpm;

    static PoolEnvironmentEditor window;

    bool awakeAtPlaying = false;

    [MenuItem("Window/游戏环境编辑/打开")]
    static void Init()
    {
        window = (PoolEnvironmentEditor)EditorWindow.GetWindow(typeof(PoolEnvironmentEditor), false, "Pool Editor");
        window.Show();
    }

    [MenuItem("Window/游戏环境编辑/关闭")]
    static void CloseWindow()
    {
        if (window)
            window.Close();
    }

    void Awake()
    {
        if (!Application.isPlaying)
        {
            awakeAtPlaying = false;
            return;
        }

        awakeAtPlaying = true;
        skin = Resources.Load<GUISkin>("GUISkin/PoolGUISkin");
        m_DataAsset = ConstantData.GetPoolDatas();
        m_DataAssetTemp = StreamTools.Clone<PoolDataAsset>(m_DataAsset);

        m_Railpm = Resources.LoadAssetAtPath<PhysicMaterial>("Assets/PhysXMaterial/Wall.physicMaterial");
        m_Ballpm = Resources.LoadAssetAtPath<PhysicMaterial>("Assets/PhysXMaterial/Ball.physicMaterial");
        if (m_Railpm == null || m_Ballpm == null)
        {
            Debug.LogError("Physic material is null");
        }
        m_Railpm.bounciness = m_DataAsset.RailBounciness;
        m_Ballpm.bounciness = m_DataAsset.BallBounciness;
    }

    void OnDestroy()
    {
        if (m_Railpm) m_Railpm.bounciness = Mathf.Clamp01(m_DataAssetTemp.RailBounciness);
        if (m_Ballpm) m_Ballpm.bounciness = Mathf.Clamp01(m_DataAssetTemp.BallBounciness);
    }

    void OnGUI()
    {
        if (!awakeAtPlaying || !Application.isPlaying)
        {
            GUI.skin.label.fontSize = 16;
            GUI.color = Color.white;
            GUILayout.Label("请在运行游戏时打开此窗口");
            return;
        }

        GUI.skin = skin;
        /////////////////最大推力
        GUI.BeginGroup(new Rect(10, 10, 400, 80));
        SetValue("球杆的最大推力：",
            () =>
            {
                if(float.TryParse(s1,out f))
                {
                    m_DataAsset.MaxImpulse = f;
                }
            },
            () =>
            {
                m_DataAssetTemp.MaxImpulse = m_DataAsset.MaxImpulse;
                SerPool();
            },
            () =>
            {
                m_DataAsset.MaxImpulse = m_DataAssetTemp.MaxImpulse;
            });
        s1 = GUI.TextField(new Rect(180, 5, 30, 20), s1);
        GUI.Label(new Rect(10, 25, 200, 40), "当前应用的值：<color=darkblue><i>" + m_DataAsset.MaxImpulse + "</i></color>");
        GUI.Label(new Rect(10, 50, 200, 40), "上一个值：<color=blue><i>" + m_DataAssetTemp.MaxImpulse + "</i></color>");
        GUI.EndGroup();


        //////////////////球角度
        GUI.BeginGroup(new Rect(10, 95, 400, 80));
        SetValue("球的角度阻力：", () =>
        {
            if (float.TryParse(s2, out f))
            {
                m_DataAsset.BallAngularDrag = f;
            }
        },
            () =>
            {
                m_DataAssetTemp.BallAngularDrag = m_DataAsset.BallAngularDrag;
                SerPool();
            },
            () =>
            {
                m_DataAsset.BallAngularDrag = m_DataAssetTemp.BallAngularDrag;
            });
        s2 = GUI.TextField(new Rect(180, 5, 30, 20), s2);
        GUI.Label(new Rect(10, 25, 200, 40), "当前应用的值：<color=darkblue><i>" + m_DataAsset.BallAngularDrag + "</i></color>");
        GUI.Label(new Rect(10, 50, 200, 40), "上一个值：<color=blue>" + m_DataAssetTemp.BallAngularDrag + "</color>");
        GUI.EndGroup();


        //////////////////球阻力
        GUI.BeginGroup(new Rect(10, 180, 400, 80));
        SetValue("球的阻力：", () =>
        {
            if (float.TryParse(s3, out f))
            {
                m_DataAsset.BallDrag = f;
            }
        },
            () =>
            {
                m_DataAssetTemp.BallDrag = m_DataAsset.BallDrag;
                SerPool();
            },
            () =>
            {
                m_DataAsset.BallDrag = m_DataAssetTemp.BallDrag;
            });
        s3 = GUI.TextField(new Rect(180, 5, 30, 20), s3);
        GUI.Label(new Rect(10, 25, 200, 40), "当前应用的值：<color=darkblue><i>" + m_DataAsset.BallDrag + "</i></color>");
        GUI.Label(new Rect(10, 50, 200, 40), "上一个值：<color=blue>" + m_DataAssetTemp.BallDrag + "</color>");
        GUI.EndGroup();


        /////////////////球弹性
        GUI.BeginGroup(new Rect(10, 265, 400, 80));
        SetValue("球与球的弹性(0-1)：", () =>
        {
            if (float.TryParse(s4, out f) && f >= 0 && f <= 1)
            {
                m_DataAsset.BallBounciness = f;
                if (m_Ballpm) m_Ballpm.bounciness = m_DataAsset.BallBounciness;
            }
        },
            () =>
            {
                m_DataAssetTemp.BallBounciness = m_DataAsset.BallBounciness;
                SerPool();
            },
            () =>
            {
                m_DataAsset.BallBounciness = m_DataAssetTemp.BallBounciness;
                if (m_Ballpm)
                    m_Ballpm.bounciness = m_DataAssetTemp.BallBounciness;
            });
        s4 = GUI.TextField(new Rect(180, 5, 30, 20), s4);
        GUI.Label(new Rect(10, 25, 200, 40), "当前应用的值：<color=darkblue><i>" + m_DataAsset.BallBounciness + "</i></color>");
        GUI.Label(new Rect(10, 50, 200, 40), "上一个值：<color=blue>" + m_DataAssetTemp.BallBounciness + "</color>");
        GUI.EndGroup();


        /////////////////桌子边缘弹性
        GUI.BeginGroup(new Rect(10, 350, 400, 80));
        SetValue("球与桌子边缘的弹性\n(0-1)：", () =>
        {
            if (float.TryParse(s5, out f) && f >=0 && f <= 1)
            {
                m_DataAsset.RailBounciness = f;
                if (m_Railpm) m_Railpm.bounciness = m_DataAsset.RailBounciness;
            }
        },
            () =>
            {
                m_DataAssetTemp.RailBounciness = m_DataAsset.RailBounciness;
                SerPool();
            },
            () =>
            {
                m_DataAsset.RailBounciness = m_DataAssetTemp.RailBounciness;
                if (m_Railpm)
                    m_Railpm.bounciness = m_DataAssetTemp.RailBounciness;
            });
        s5 = GUI.TextField(new Rect(180, 5, 30, 20), s5);
        GUI.Label(new Rect(10, 25, 200, 40), "当前应用的值：<color=darkblue><i>" + m_DataAsset.RailBounciness + "</i></color>");
        GUI.Label(new Rect(10, 50, 200, 40), "上一个值：<color=blue>" + m_DataAssetTemp.RailBounciness + "</color>");
        GUI.EndGroup();

        /////////////////水平加塞
        GUI.BeginGroup(new Rect(10, 435, 400, 80));
        SetValue("水平加塞比值：", () =>
        {
            if (float.TryParse(s6, out f))
            {
                m_DataAsset.HorizontalSidingStrength = f;
            }
        },
            () =>
            {
                m_DataAssetTemp.HorizontalSidingStrength = m_DataAsset.HorizontalSidingStrength;
                SerPool();
            },
            () =>
            {
                m_DataAsset.HorizontalSidingStrength = m_DataAssetTemp.HorizontalSidingStrength;
            });
        s6 = GUI.TextField(new Rect(180, 5, 30, 20), s6);
        GUI.Label(new Rect(10, 25, 200, 40), "当前应用的值：<color=darkblue><i>" + m_DataAsset.HorizontalSidingStrength + "</i></color>");
        GUI.Label(new Rect(10, 50, 200, 40), "上一个值：<color=blue>" + m_DataAssetTemp.HorizontalSidingStrength + "</color>");
        GUI.EndGroup();

        /////////////////垂直加塞
        GUI.BeginGroup(new Rect(10, 520, 400, 80));
        SetValue("垂直加塞比值：", () =>
        {
            if (float.TryParse(s7, out f))
            {
                m_DataAsset.VerticalSidingStrength = f;
            }
        },
            () =>
            {
                m_DataAssetTemp.VerticalSidingStrength = m_DataAsset.VerticalSidingStrength;
                SerPool();
            },
            () =>
            {
                m_DataAsset.VerticalSidingStrength = m_DataAssetTemp.VerticalSidingStrength;
            });
        s7 = GUI.TextField(new Rect(180, 5, 30, 20), s7);
        GUI.Label(new Rect(10, 25, 200, 40), "当前应用的值：<color=darkblue><i>" + m_DataAsset.VerticalSidingStrength + "</i></color>");
        GUI.Label(new Rect(10, 50, 200, 40), "上一个值：<color=blue>" + m_DataAssetTemp.VerticalSidingStrength + "</color>");
        GUI.EndGroup();

        ////////////////结束
        GUI.BeginGroup(new Rect(10, 605, 400, 80));
        if (GUI.Button(new Rect(10, 10, 100, 20), "保存所有"))
        {
            m_DataAssetTemp = m_DataAsset;
            SerPool();
        }
        if (GUI.Button(new Rect(120, 10, 100, 20), "还原所有"))
        {
            m_DataAsset = m_DataAssetTemp;
            if (m_Railpm) m_Railpm.bounciness = m_DataAssetTemp.RailBounciness;
            if (m_Ballpm) m_Ballpm.bounciness = m_DataAssetTemp.BallBounciness;
        }
        GUI.EndGroup();
        GUI.skin = null;
    }

    private void SerPool()
    {
        StreamTools.SerializeObject(m_DataAssetTemp, ConstantData.PoolDataAssetsFile);
    }

    private void SetValue(string valueName, System.Action onApply, System.Action onSave, System.Action onReverse)
    {
        GUI.Box(new Rect(0, 0, 400, 80), valueName);
        if (GUI.Button(new Rect(185, 50, 60, 20), "应用"))
        {
            if (onApply != null)
                onApply();
        }
        if (GUI.Button(new Rect(250, 50, 60, 20), "保存"))
        {
            if (onSave != null)
                onSave();
        }
        if (GUI.Button(new Rect(315, 50, 60, 20), "还原"))
        {
            if (onReverse != null)
                onReverse();
        }
    }
}
