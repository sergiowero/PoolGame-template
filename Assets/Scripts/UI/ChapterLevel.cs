using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChapterLevel : MonoBehaviour
{
    [SerializeField]
    private Color m_EnableColor;
    [SerializeField]
    private Color m_DisableColor;

    public Text text;

    [SerializeField]
    private Image[] stars;

    public GameObject mask;

    public LevelData levelData;

    public void SetStar(int star)
    {
        for(int i = 0; i < stars.Length; i++)
        {
            stars[i].color = star > i ? m_EnableColor : m_DisableColor;
        }
    }
}
