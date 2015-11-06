using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchieveSubject : MonoBehaviour {

    [SerializeField]
    private AchieveItem m_AchieveItemPrefab;

    [SerializeField]
    private GridLayoutGroup m_Root;

    [SerializeField]
    private Text m_Progress;

	void Start () 
    {
        int progress = 0, i = 0;
        foreach(var v in AchieveConfiguration.achieveDictionary)
        {
            i++;
            AchieveItem item = SupportTools.AddChild<AchieveItem>(m_Root.gameObject, m_AchieveItemPrefab.gameObject);
            int id = v.Key;
            item.achieveName.text = v.Value.name;
            item.description.text = v.Value.DescriptionID.ToString();
            item.achieveIcon.sprite = Resources.Load<Sprite>("UI/LaunchScene/AchieveIcon/" + v.Value.IconName);
            if(ConstantData.achieveRecords[id] < v.Value.goal)
            {
                item.progress.gameObject.SetActive(true);
                item.finishFlag.gameObject.SetActive(false);
                item.progress.text = ConstantData.achieveRecords[id] + "/" + v.Value.goal;
                item.backGround.color = Color.gray;
            }
            else
            {
                item.progress.gameObject.SetActive(false);
                item.finishFlag.gameObject.SetActive(true);
                item.backGround.color = Color.white;
                progress++;
            }
        }
        m_Progress.text = progress + "/" + AchieveConfiguration.achieveDictionary.Count;
        m_Root.GetComponent<RectTransform>().sizeDelta = new Vector2(m_Root.cellSize.x, i * m_Root.cellSize.y + (i - 1) * m_Root.spacing.y);
	}
}
