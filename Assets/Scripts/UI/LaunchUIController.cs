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
    private ScrollRectSnapElement m_ChapterSnapElement;

    [SerializeField]
    private GridLayoutGroup m_LevelRoot;

    private string m_MissionProgress;

    private Animator m_Animator;

    [SerializeField]
    private Image m_RiseMask;
    [SerializeField]
    private Image m_FadeMask;
    [SerializeField]
    private GameObject m_LoadingFlag;
    [SerializeField]
    private GameObject m_Blank;

    [SerializeField]
    private Text m_Physical;
    [SerializeField]
    private Image m_PhysicalBar;

    private ChapterLevel m_ChapterLevelPrefab;

    [SerializeField]
    private AudioSource m_AudioTest;

    public static void SetPhysical(int value) 
    {
        if (!m_Instance)
            return;
        value = Mathf.Max(0, value);
        m_Instance.m_Physical.text = value.ToString() + "/" + ConstantData.maxPhysical;
        m_Instance.m_PhysicalBar.fillAmount = (float)value / ConstantData.maxPhysical;
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        ConstantData.GType = GameType.None;
        m_Instance = this;
        m_Blank.SetActive(false);
        m_RiseMask.gameObject.SetActive(false);
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

        if(string.IsNullOrEmpty(ConstantData.BattleSceneType))
        {
            m_Animator.Play("Loading");
            Invoke("LoadingDone", 1f);
        }
        else
        {
            m_Animator.SetTrigger(ConstantData.BattleSceneType);
            LoadLevelWithChapter(ConstantData.LoadedChapter);
        }
    }

    void LoadingDone()
    {
        //m_Animator.SetTrigger("Default2Main");
        m_Animator.SetBool("GameStart", true);
        m_LoadingFlag.SetActive(false);
        m_Blank.SetActive(true);
    }

    void Start()
    {
        ChapterCities chapterCityPrefab = m_ChapterRoot.transform.GetChild(0).GetComponent<ChapterCities>();
        chapterCityPrefab.transform.SetParent(null);

        int i = 0;
        foreach(var v in ChapterConfiguration.chapterDictionary)
        {
            i++;
            ChapterInfo c = v.Value;
            ChapterCities chapterCity = SupportTools.AddChild<ChapterCities>(m_ChapterRoot.gameObject, chapterCityPrefab.gameObject);
            chapterCity.text.text = c.name;
            chapterCity.name = c.imageName.Substring(6); //imageName = Image_xxxxx  we just wanna get the real name
            chapterCity.chapter = c.chapter;
            Sprite sprite = Resources.Load<Sprite>("UI/LaunchScene/MissionCards/" + c.imageName);
            if (sprite)
                chapterCity.image.sprite = sprite;
            bool b = c.chapter > ConstantData.LevelDatas.GetChapter(m_MissionProgress);
            chapterCity.lockMask.SetActive(b);
        }
        Destroy(chapterCityPrefab.gameObject);
        m_ChapterRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(i * m_ChapterRoot.cellSize.x + i * m_ChapterRoot.spacing.x, m_ChapterRoot.cellSize.y);
        m_ChapterSnapElement.Initialize();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void LoadQuickFire()
    {
        ConstantData.GType = GameType.QuickFire;
        ConstantData.physical -= 1;
        ConstantData.BattleSceneType = ConstantData.ToPratice;
        LoadScene(1);
    }

    public void LoadStandard()
    {
        ConstantData.GType = GameType.Standard;
        ConstantData.physical -= 1;
        ConstantData.BattleSceneType = ConstantData.ToPratice;
        LoadScene(1);
    }

    public void LoadBattleWithLowAI()
    {
        LoadBattleWithAI(AIDifficulty.Low);
    }

    public void LoadBattleWithVeryLowAI()
    {
        LoadBattleWithAI(AIDifficulty.VeryLow);
    }

    public void LoadBattleWithMediumAI()
    {
        LoadBattleWithAI(AIDifficulty.Medium);
    }

    private void LoadBattleWithAI(AIDifficulty difficulty)
    {
        AIPlayer.difficulty = difficulty;
        ConstantData.GType = GameType.AI;
        ConstantData.physical -= 1;
        ConstantData.BattleSceneType = ConstantData.ToPratice;
        LoadScene(1);
    }

    private void LoadLevelWithChapter(int chapter)
    {
        ConstantData.LoadedChapter = chapter;
        int childCount = m_LevelRoot.transform.childCount;
        IList<LevelData> levels = ConstantData.LevelDatas.DumpLevelDatasWithChapter(chapter);
        int i;
        for (i = 0; i < levels.Count; i++)
        {
            ChapterLevel l;
            if (i < childCount)
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
            bool levelLock = ConstantData.LevelDatas.Comp(m_MissionProgress, levels[i].FileName) == -1;
            l.mask.SetActive(levelLock);
            if (levelLock)
                l.text.transform.localScale = Vector3.one * .72f;
            else
                l.text.transform.localScale = Vector3.one;
        }
        m_LevelRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(i * m_LevelRoot.cellSize.x + (i - 1) * m_LevelRoot.spacing.x, m_LevelRoot.cellSize.y);
        while (i < childCount)
        {
            m_LevelRoot.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void LoadLevel(GameObject go)
    {
        ChapterCities city = go.GetComponent<ChapterCities>();
        LoadLevelWithChapter(city.chapter);
    }

    public void GoMisstion(ChapterLevel l)
    {
        LevelDataIndex.CurrentLevel = l.levelData;
        ConstantData.GType = GameType.Mission;
        ConstantData.physical -= 1;
        ConstantData.BattleSceneType = ConstantData.ToLevel;
        LoadScene(1);
    }

    public void LoadScene(int sceneIndex)
    {
        //animation tools component attached at blackmask has a method named loadscene. we use this
        m_RiseMask.gameObject.SetActive(true);
        HOAudioManager.StopBGM();
        m_RiseMask.GetComponent<AnimationTools>().sceneIndex = 1;
        //Application.LoadLevel(sceneIndex);
    }
}
