using UnityEngine;
using System.Collections;

	public class AudioVolume : MonoBehaviour {
		private float m_initalVol;
		void Awake () {
			if(audio)
			{
				m_initalVol=audio.volume;
			}
			onSetAudioVolume();
		}
		void OnEnable()
		{
			BaseGameManager.onToggleAudio += onSetAudioVolume;
		}
		void OnDisable()
		{
			BaseGameManager.onToggleAudio -= onSetAudioVolume;
		}

		void onSetAudioVolume () {
			audio.volume = PlayerPrefs.GetFloat("AudioVolume",1) * m_initalVol;
		}
	}
