using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LaunchUIController : MonoBehaviour
{
    private static LaunchUIController m_Instance = null;

    [SerializeField]
    private GridLayoutGroup m_ChapterRoot;

    [SerializeField]
    private GridLayoutGroup m_LevelRoot;

    private string m_MissionProgress;

    private Animator m_Animator;

    [SerializeField]
    private Image m_BlackMask;
    [SerializeField]
    private GameObject m_LoadingFlag;
    [SerializeField]
    private GameObject m_Blank;

    [SerializeField]
    private Text m_Physical;
    [SerializeField]
    private Image m_PhysicalBar;

    private ChapterLevel m_ChapterLevelPrefab;



    public static void SetPhysical(int value) 
    {
        value = Mathf.Max(0, value);
        m_Instance.m_Physical.text = value.ToString() + "/" + ConstantData.maxPhysical;
        m_Instance.m_PhysicalBar.fillAmount = value / ConstantData.maxPhysical;
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        ConstantData.GType = GameType.None;
        m_Instance = this;
        m_Blank.SetActive(false);
        m_LoadingFlag.SetActive(true);
        //Get chapter level prefab
        m_ChapterLevelPrefab = m_LevelRoot.transform.GetChild(0).GetComponent<ChapterLevel>();
        m_ChapterLevelPrefab.transform.SetParent(transform);
        m_ChapterLevelPrefab.gameObject.SetActive(false);
        if (PlayerPrefs.HasKey(ConstantData.MissionProgressKeyName))
        {
            m_MissionProgress = PlayerPrefs.GetString(ConstantData.MissionProgressKeyName);
        }
        else
        {
            m_MissionProgress = "1-1";
            PlayerPrefs.SetString(ConstantData.MissionProgressKeyName, m_MissionProgress);
        }
        Invoke("LoadingDone", 1f);
    }

    void LoadingDone()
    {
        m_Animator.SetTrigger("Default2Main");
        m_LoadingFlag.SetActive(false);
        m_Blank.SetActive(true);
    }

    void Start()
    {
        ChapterCities chapterCityPrefab = m_ChapterRoot.transform.GetChild(0).GetComponent<ChapterCities>();

        int i = 0;
        foreach(var v in ChapterConfiguration.chapterDictionary)
        {
            i++;
            ChapterInfo c = v.Value;
            ChapterCities chapterCity = SupportTools.AddChild<ChapterCities>(m_ChapterRoot.gameObject, chapterCityPrefab.gameObject);
            chapterCity.text.text = c.name;
            chapterCity.name = c.imageName.Substring(6); //imageName = Image_xxxxx  we just wanna get the real name
            chapterCity.chapter = c.chapter;
            //chapterCity.image = Resources.Load() //load city texture... no textures
            bool b = c.chapter > ConstantData.LevelDatas.GetChapter(m_MissionProgress);
            chapterCity.lockMask.SetActive(b);
        }
        Destroy(chapterCityPrefab.gameObject);
        m_ChapterRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(i * m_ChapterRoot.cellSize.x + (i-1) * m_ChapterRoot.spacing.x, m_ChapterRoot.cellSize.y);
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void LoadQuickFire()
    {
        ConstantData.GType = GameType.QuickFire;
        ConstantData.MPhysical -= 1;
        SetPhysical(ConstantData.MPhysical);
        LoadScene(1);
    }

    public void LoadStandard()
    {
        ConstantData.GType = GameType.Standard;
        ConstantData.MPhysical -= 1;
        SetPhysical(ConstantData.MPhysical);
        LoadScene(1);
    }

    public void LoadLevel(ChapterCities city)
    {
        int childCount = m_LevelRoot.transform.childCount;
        IList<LevelData> levels = ConstantData.LevelDatas.DumpLevelDatasWithChapter(city.chapter);
        int i;
        for (i = 0; i < levels.Count; i++)
        {
            ChapterLevel l;
            if(i < childCount)
            {
                l = m_LevelRoot.transform.GetChild(i).GetComponent<ChapterLevel>();
            }
            else
            {
                l = SupportTools.AddChild<ChapterLevel>(m_LevelRoot.gameObject, m_ChapterLevelPrefab.gameObject);
            }
            l.gameObject.SetActive(true);
            l.levelData = levels[i];
            l.text.text = (i + 1).ToString();
            l.name = "Level-" + l.text.text;
            l.SetStar(ConstantData.missionRecords.GetStar(levels[i].FileName));
            l.mask.SetActive(ConstantData.LevelDatas.Comp(m_MissionProgress, levels[i].FileName)  == -1);
        }
        m_LevelRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(i * m_LevelRoot.cellSize.x + (i - 1) * m_LevelRoot.spacing.x, m_LevelRoot.cellSize.y);
        while(i < childCount)
        {
            m_LevelRoot.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void GoMisstion(ChapterLevel l)
    {
        LevelDataIndex.CurrentLevel = l.levelData;
        ConstantData.GType = GameType.Mission;
        ConstantData.MPhysical -= 1;
        SetPhysical(ConstantData.MPhysical);
        LoadScene(1);
    }

    public void LoadScene(int sceneIndex)
    {
        //animation tools component attached at blackmask has a method named loadscene. we use this
        //m_BlackMask.gameObject.SetActive(true);
        //m_BlackMask.GetComponent<AnimationTools>().sceneIndex = 1;
        Application.LoadLevel(sceneIndex);
    }
}
