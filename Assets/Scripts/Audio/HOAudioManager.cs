using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class HOAudioManager : MonoBehaviour
{

    #region Fields
    [Range(3, 10)]
    public int m_ClipsPoolCount = 5;

    private static HOAudioManager m_Instance = null;
    public static HOAudioManager Instance { get { return m_Instance; } }

    private AudioSource m_OneShotSource;
    private AudioSource m_LoopSource;
    //public AudioSource Source { get { return m_OneShotSource; } }


    private System.Collections.Generic.Dictionary<string, AudioClip> m_ClipsPool;

    private AudioClip m_ClipCache;
    private string m_ClipNameCache;

    private BGMPlayer m_BGMPlayer;

    private float m_CacheTime;
    [SerializeField]
    private float m_PlayerCacheTimeInterval;
    private bool m_CacheReady = true;

    #endregion

    #region Unity messages
    void OnApplicationQuit ()
    {
        m_ClipsPool.Clear();
        m_Instance = null;
        m_ClipCache = null;
        m_BGMPlayer.DestroySelf();

    }

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);

            //Get music toggle
            if (!PlayerPrefs.HasKey(ConstantData.MusicVolumnKey))
                ConstantData.Music = true;
            else
                ConstantData.Music = PlayerPrefs.GetInt(ConstantData.MusicVolumnKey) == 1;

            //Get sound toggggggggggggggggggggggggle
            if (!PlayerPrefs.HasKey(ConstantData.SoundVolumnKey))
                ConstantData.Sound = false;
            else
                ConstantData.Sound = PlayerPrefs.GetInt(ConstantData.SoundVolumnKey) == 1;

            m_OneShotSource = GetComponent<AudioSource>();
            m_LoopSource = SupportTools.AddChild<AudioSource>(gameObject);
            m_LoopSource.loop = true;
            m_LoopSource.playOnAwake = false;
            m_LoopSource.name = "Loop source";
            m_ClipsPool = new System.Collections.Generic.Dictionary<string, AudioClip>(m_ClipsPoolCount);
            m_BGMPlayer = new BGMPlayer(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(m_CacheTime > 0)
        {
            m_CacheReady = false;
            m_CacheTime -= Time.deltaTime;
            if (m_CacheTime <= 0)
                m_CacheReady = true;
        }
    }
    #endregion

    #region Static methods
    public static void PlayLoopClip(string clipName)
    {
        if (m_Instance != null)
            m_Instance._PlayLoopClip(clipName);
    }

    public static void PlayLoopClip(int clipKey)
    {
        //if (m_Instance != null)
            //m_Instance._PlayLoopClip(HOAudioConfiguration.audioDictionary[clipKey].AudioName);
    }

    public static void StopLoopClip()
    {
        if (m_Instance != null)
            m_Instance._StopLoopClip();
    }

    public static void PlayClip(string clipName, float volumn, bool stopPrevousAudio = false)
    {
        if (m_Instance != null)
        {
            volumn = Mathf.Clamp01(volumn);
            m_Instance._PlayClip(clipName, volumn, stopPrevousAudio);
        }
    }

    public static void PlayClip(AudioClip clip)
    {
        if (m_Instance != null)
            m_Instance._PlayClip(clip);
    }

    public static void PlayClip(int clipKey, bool stopPrevousAudio = false)
    {
        //if (m_Instance != null)
        //    m_Instance._PlayClip(HOAudioConfiguration.audioDictionary[clipKey].AudioName, stopPrevousAudio);
    }

    public static void Stop()
    {
        if (m_Instance != null)
            m_Instance._Stop();
    }

    public static void SetMusic(bool value)
    {
        if (m_Instance != null)
            m_Instance._SetMusic(value);
    }

    public static void SetSound(bool value)
    {
        if (m_Instance != null)
            m_Instance._SetSound(value);
    }

    public static void PlayBGM(string bgmName, bool stopImmediate = false)
    {
        if (m_Instance != null)
            m_Instance._PlayBGM(bgmName, stopImmediate);
    }

    public static void PlayBGM(int bgmKey, bool stopImmediate = false)
    {
        //if (m_Instance != null)
        //    m_Instance._PlayBGM(HOAudioConfiguration.audioDictionary[bgmKey].AudioName, stopImmediate);
    }

    public static void StopBGM(bool stopImmediate = false)
    {
        if (m_Instance != null)
            m_Instance._MusicOff(stopImmediate);
    }
    #endregion

    #region Non-static methods
    private void _PlayLoopClip(string clipName)
    {
        if (!ConstantData.Sound)
            return;
        if (m_LoopSource.clip != null && m_LoopSource.clip.name.CompareTo(clipName) == 0) 
            return;

        AudioClip clip;
        if (m_ClipsPool.ContainsKey(clipName))
        {
            clip = m_ClipsPool[clipName];
        }
        else
        {
            clip = Resources.Load("Audio/" + clipName) as AudioClip;
            if (!clip)
            {
                //HODebug.LogError("can not find audio : <color=green>" + clipName + "</color>");
                return;
            }
            while (m_ClipsPool.Count >= m_ClipsPoolCount)
            {
                m_ClipsPool.Remove(m_ClipsPool.Keys.FirstOrDefault<string>());
            }
            m_ClipsPool.Add(clipName, clip);
        }
        m_LoopSource.clip = clip;
        m_LoopSource.Play();
    }

    private void _StopLoopClip()
    {
        m_LoopSource.Stop();
        m_LoopSource.clip = null;
    }

    private void _PlayClip(AudioClip clip)
    {
        if (!ConstantData.Sound)
            return;
        m_OneShotSource.PlayOneShot(clip);
    }

    private void _PlayClip(string clipName, float volumn, bool stopPrevousAudio)
    {
        if (!ConstantData.Sound)
            return;
        if(clipName.CompareTo(m_ClipNameCache) == 0)
        {
            if (!m_CacheReady) return;
        }

        AudioClip clip;
        if (clipName.CompareTo(m_ClipNameCache) == 0)
        {
            clip = m_ClipCache;
        }
        else if (m_ClipsPool.ContainsKey(clipName))
        {
            clip = m_ClipsPool[clipName];
        }
        else
        {
            clip = Resources.Load("Audio/" + clipName) as AudioClip;
            if (!clip)
            {
                //HODebug.LogError("can not find audio : <color=green>" + clipName + "</color>");
                return;
            }
            m_ClipCache = clip;
            m_ClipNameCache = clipName;
            while (m_ClipsPool.Count >= m_ClipsPoolCount)
            {
                m_ClipsPool.Remove(m_ClipsPool.Keys.FirstOrDefault<string>());
            }
            m_ClipsPool.Add(clipName, clip);
        }

        if (stopPrevousAudio) m_OneShotSource.Stop();
        m_OneShotSource.PlayOneShot(clip, volumn);
        m_CacheTime = m_PlayerCacheTimeInterval;
    }

    private void _Stop()
    {
        m_OneShotSource.Stop();
    }

    private void _SetMusic(bool value)
    {
        ConstantData.Music = value;
        if (value) _MusicOn();
        else _MusicOff();
    }

    private void _SetSound(bool value)
    {
        ConstantData.Sound = value;
    }

    private void _MusicOff(bool stopImmediate = false)
    {
        m_BGMPlayer.CloseBGM(stopImmediate);
    }

    private void _MusicOn()
    {
        m_BGMPlayer.OpenBGM();
    }

    private void _PlayBGM(string bgmName, bool stopImmediate = false)
    {
        m_BGMPlayer.PlayBGM(bgmName, stopImmediate);
    }
    #endregion

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(100, 190, 150, 30), "Sound on"))
    //    {
    //        SoundOn();
    //    }

    //    if (GUI.Button(new Rect(100, 220, 100, 30), "Sound off"))
    //    {
    //        SoundOff();
    //    }
    //}

    class BGMPlayer
    {

        private const float m_TransitTime = .5f;

        private enum BGMFocus
        {
            First = 0, Second
        }

        private AudioSource[] m_BGMs;

        private BGMFocus m_focus;
        private BGMFocus m_reverse
        {
            get { return m_focus == BGMFocus.First ? BGMFocus.Second : BGMFocus.First; }
        }

        public BGMPlayer(GameObject o)
        {
            m_BGMs = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                m_BGMs[i] = SupportTools.AddChild<AudioSource>(o);
                m_BGMs[i].name = "BGM_" + (i + 1);
                m_BGMs[i].loop = true;
                m_BGMs[i].playOnAwake = false;
                m_BGMs[i].volume = !ConstantData.Sound ? 0 : 1;
                m_BGMs[i].pitch = 1;
            }
            m_focus = BGMFocus.Second;
            m_BGMs[1].volume = 0;
        }

        public void DestroySelf()
        {
            for (int i = 0; i < 2; i++)
            {
                m_BGMs[i].clip = null;
            }
        }

        public void PlayBGM(string bgmName, bool stopImmediate)
        {
            int playDelay = 0;
            if(stopImmediate)
            {
                m_BGMs[(int)m_focus].Stop();
            }
            else
            {
                AudioTo(m_BGMs[(int)m_focus], 0);
                playDelay = 1;
            }
            if (m_BGMs[(int)m_reverse].clip == null || m_BGMs[(int)m_reverse].clip.name.CompareTo(bgmName) != 0)
            {
                AudioClip clip = Resources.Load("audio/" + bgmName) as AudioClip;
                m_BGMs[(int)m_reverse].clip = clip;
            }
            m_BGMs[(int)m_reverse].Play((ulong)playDelay);
            if (ConstantData.Sound)
            {
                AudioTo(m_BGMs[(int)m_reverse], 1);
            }
            m_focus = m_reverse;
        }

        public void CloseBGM(bool closeImmediate)
        {
            for(int i = 0; i < 2; i++)
            {
                iTween[] tweens = m_BGMs[i].GetComponents<iTween>();
                for (int j = 0; j < tweens.Length; j++ )
                {
                    Destroy(tweens[j]);
                }
                if (closeImmediate)
                    m_BGMs[i].volume = 0;
                else
                    AudioTo(m_BGMs[i], 0);
            }
        }

        public void OpenBGM()
        {
            iTween[] tweens = m_BGMs[(int)m_focus].GetComponents<iTween>();
            for(int i = 0; i < tweens.Length; i++)
            {
                Destroy(tweens[i]);
            }
            AudioTo(m_BGMs[(int)m_focus], 1);
        }

        private void AudioTo(AudioSource audioSource, float volume)
        {
            iTween.AudioTo(audioSource.gameObject, iTween.Hash("volume", volume, "time", m_TransitTime, "ignoretimescale", true));
        }
    }



    #region Extension
    public static void BallhitRail(Vector3 velocity)
    {
        if (!m_Instance)
            return;
        float v0 = Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y), Mathf.Abs(velocity.z));
        v0 = Mathf.Clamp(v0, 0, 5);
        v0 = Mathf.Lerp(0, 1, v0 * .2f);
        m_Instance._PlayClip("RailHit", v0, false);
    }

    public static void BallhitBall(Vector3 velocity)
    {
        if (!m_Instance)
            return;
        float v0 = Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y), Mathf.Abs(velocity.z));
        v0 = Mathf.Clamp(v0, 0, 5);
        v0 = Mathf.Lerp(0, 1, v0 * .2f);
        m_Instance._PlayClip("Ball", v0, false);
    }

    public static void FireBall()
    {
        if (m_Instance)
            m_Instance._PlayClip("Hitball", 1, false);
    }

    public static void PottedBall()
    {
        if (m_Instance)
            m_Instance._PlayClip("pocket", 1, false);
    }

    public static void Break()
    {
        if (m_Instance)
            m_Instance._PlayClip("Break", 1, false);
    }

    public static void Explosion()
    {
        if (m_Instance)
            m_Instance._PlayClip("Explosion", 1, false);
    }
    #endregion //Extension
}
