using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LaunchUIController : MonoBehaviour
{
    private static LaunchUIController m_Instance = null;

    public GridLayoutGroup m_LayoutRoot;

    public GameObject m_ScrollView;
    public GameObject m_MissionNode;

    private string m_MissionProgress;

    public Text m_Physical;
    public static void SetPhysical(int value) 
    {
        m_Instance.m_Physical.text = value.ToString(); 
    }

    void Awake()
    {
        ConstantData.GType = GameType.None;
        m_Instance = this;
        SetPhysical(ConstantData.MPhysical);
        m_MissionNode.SetActive(false);
        if (PlayerPrefs.HasKey(ConstantData.MissionProgressKeyName))
        {
            m_MissionProgress = PlayerPrefs.GetString(ConstantData.MissionProgressKeyName);
        }
        else
        {
            m_MissionProgress = "1-1";
        }
    }

    void Start()
    {
        LevelDataIndex levelIndex = ConstantData.LevelDatas;
        int i = 0;
        for (int count = levelIndex.Count; i < count; i++)
        {
            //mission progress...
            if(LevelDataIndex.Comp(m_MissionProgress, levelIndex[i].FileName) == -1)
                break;

            LevelData data = levelIndex[i];
            GridImagesTest oo = SupportTools.AddChild<GridImagesTest>(m_LayoutRoot.gameObject, "TestRes/Image");
            oo.MLevelData = data;
            oo.m_Text.text = data.FileName;
        }
        m_LayoutRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(i * m_LayoutRoot.cellSize.x, m_LayoutRoot.cellSize.y);
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void LoadQuickFire()
    {
        ConstantData.GType = GameType.QuickFire;
        Application.LoadLevel(1);
        ConstantData.MPhysical -= 1;
    }

    public void LoadStandard()
    {
        ConstantData.GType = GameType.Standard;
        Application.LoadLevel(1);
        ConstantData.MPhysical -= 1;
    }

    public void LoadMission()
    {
        m_ScrollView.SetActive(false);
        m_MissionNode.SetActive(true);
    }

    public void HideMIssion()
    {
        m_ScrollView.SetActive(true);
        m_MissionNode.SetActive(false);
    }
}
