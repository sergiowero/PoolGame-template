using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchieveItem : MonoBehaviour 
{

    [SerializeField]
    private Image[] backgrounds;

    public Image icon;
    public Text achieveName;
    public Text description;
    public Text progress;
    public Color backGround
    {
        set 
        {
            for(int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].color = value;
            }
        }
    }
    public Image achieveIcon;
    public Image finishFlag;
}
