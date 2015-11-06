using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LaunchMenu : MonoBehaviour 
{
    [SerializeField]
    private Text m_MusicText;
    [SerializeField]
    private Text m_SoundText;

    void Start()
    {
        SetMusic(); 
        SetSound();
    }

    private void SetMusic()
    {
        if (ConstantData.Music)
        {
            m_MusicText.text = string.Format(HOLocalizationConfiguration.GetValue(160), HOLocalizationConfiguration.GetValue(421));
        }
        else
        { 
            m_MusicText.text = string.Format(HOLocalizationConfiguration.GetValue(160), HOLocalizationConfiguration.GetValue(422));
        }
        HOAudioManager.SetMusic(ConstantData.Music);
    }

    private void SetSound()
    {
        if (ConstantData.Sound)
            m_SoundText.text = string.Format(HOLocalizationConfiguration.GetValue(161), HOLocalizationConfiguration.GetValue(421));
        else
            m_SoundText.text = string.Format(HOLocalizationConfiguration.GetValue(161), HOLocalizationConfiguration.GetValue(422));
        HOAudioManager.SetSound(ConstantData.Sound);
    }

    public void MusicButtonClick()
    {
        ConstantData.Music = !ConstantData.Music;
        SetMusic();
    }

    public void SoundButtonClick()
    {
        ConstantData.Sound = !ConstantData.Sound;
        SetSound();
    }
}
