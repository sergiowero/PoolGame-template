using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonAudio : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private AudioClip m_AudioClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        //GetComponent<AudioSource>().PlayOneShot();
        HOAudioManager.PlayClip(m_AudioClip);
    }
}
