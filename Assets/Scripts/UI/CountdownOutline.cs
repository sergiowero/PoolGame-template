﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownOutline : MonoBehaviour 
{
    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private ImageTwinkle m_Twinkle;

    [SerializeField]
    private Color m_NormalColor;
    [SerializeField]
    private Color m_WarningColor;

    void Awake()
    {
        if (m_Image == null)
            m_Image = GetComponent<Image>();
        m_Image.color = m_NormalColor;
        m_Twinkle = m_Image.gameObject.AddComponent<ImageTwinkle>();
        m_Twinkle.SetColor(Color.white, m_WarningColor);
        m_Twinkle.SetTime(.7f);
        m_Twinkle.enabled = false;
    }

    void OnEnable()
    {
        iTween.ColorTo(gameObject, m_NormalColor, .5f);
        PoolRulesBase.onFireBall += OnFireBall;
    }

    void OnDisable()
    {
        iTween.ColorTo(gameObject, Color.white * 0, .5f);
        PoolRulesBase.onFireBall -= OnFireBall;
        m_Twinkle.enabled = false;
    }

    public void SetValue(float fillAmount)
    {
        m_Image.fillAmount = fillAmount;
        if (m_Image.fillAmount < .25f)
        {
            m_Twinkle.enabled = true;
        }

        if(m_Image.fillAmount <= 0)
        {
            enabled = false;
        }
        else
        {
            enabled = true;
        }
        if(m_Twinkle.enabled)
        {
            HOAudioManager.PlayLoopClip("Clock");
        }
        if(!enabled || !m_Twinkle.enabled)
        {
            HOAudioManager.StopLoopClip();
        }
    }

    void OnFireBall()
    {
        m_Twinkle.enabled = false;
        HOAudioManager.StopLoopClip();
    }


    public float GetValue()
    {
        return m_Image.fillAmount;
    }

}
