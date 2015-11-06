using UnityEngine;
using System.Collections;

public class HOBGMPlayer : MonoBehaviour
{
    public int m_BGMKey;

    void Awake()
    {
        if (m_BGMKey == default(int))
            HOAudioManager.StopBGM(true);
        else
            HOAudioManager.PlayBGM(m_BGMKey,true);

    }
}
