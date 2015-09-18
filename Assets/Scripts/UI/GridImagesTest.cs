using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridImagesTest : MonoBehaviour 
{
    public LevelData MLevelData;

    public Text m_Text;

    public void OnClickButton()
    {
        LevelData.CurrentLevel = MLevelData;
        ConstantData.GType = GameType.Mission;
        Application.LoadLevel(1);
    }
}
