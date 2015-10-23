using UnityEngine;
using System.Collections;

public class AnimationTools : MonoBehaviour
{
    private Animator m_Animator;

    public string defaultAnimation;

    public int sceneIndex;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        if(!string.IsNullOrEmpty(defaultAnimation) && m_Animator)
            m_Animator.Play(defaultAnimation);
    }

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void ActiveObjectFalse(GameObject o)
    {
        o.SetActive(false);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void DestroyObject(GameObject o)
    {
        Destroy(o);
    }

    public void FadeParam(string param)
    {
        m_Animator.SetBool(param, false);
    }

    public void ActiveParam(string param)
    {
        m_Animator.SetBool(param, true);
    }

    public void Trigger(string param)
    {
        m_Animator.SetTrigger(param);
    }

    public void LoadScene()
    {
        Application.LoadLevel(sceneIndex);
    }
}
