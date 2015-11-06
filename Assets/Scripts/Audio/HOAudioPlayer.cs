using UnityEngine;
using System.Collections;

public class HOAudioPlayer : MonoBehaviour
{
    //private bool m_Played;

    public string m_AudioName;

    public bool m_PlayWithKey;

    public string m_LoopIDs;

    void OnEnable()
    {
        //m_Played = false;
        Play();
    }

    void OnDisable()
    {
        StopLoopClip();
    }

    public void PlayS(string key)
    {
        HOAudioManager.PlayClip(key);
    }

    public void PlayI(int key)
    {
        HOAudioManager.PlayClip(key);
    }

    public void PlayLoopS(string key)
    {
        HOAudioManager.PlayLoopClip(key);
    }

    public void PlayLoopI(int key)
    {
        HOAudioManager.PlayLoopClip(key);
    }

    public void StopLoopClip()
    {
        HOAudioManager.StopLoopClip();
    }
    //public void PlayOnceS(string key)
    //{
    //    if (m_Played) return;

    //    PlayS(key);
    //    m_Played = true;
    //}

    //public void PlayOnceI(int key)
    //{
    //    if (m_Played) return;

    //    PlayI(key);
    //    m_Played = true;
    //}

    private void Play()
    {
        if (string.IsNullOrEmpty(m_AudioName))
            return;

        string[] name = m_AudioName.Split(',');
        int length = name.Length;

        if (!m_PlayWithKey)
        {
            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrEmpty(m_LoopIDs) && m_LoopIDs.Contains(name[i]))
                    PlayLoopS(name[i]);
                else
                    PlayS(name[i]);
            }
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrEmpty(m_LoopIDs) && m_LoopIDs.Contains(name[i]))
                    PlayLoopI(int.Parse(name[i]));
                else
                    PlayI(int.Parse(name[i]));
            }
        }
    }
}
