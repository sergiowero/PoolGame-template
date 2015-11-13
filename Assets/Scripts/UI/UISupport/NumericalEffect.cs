using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class NumericalEffect : MonoBehaviour 
{
    private Text m_Text;
    private string m_TextValue;
    private float m_Value;

    public int value;

    [SerializeField]
    private string m_Pattern;

    [SerializeField]
    private float m_RollSpeed;

    void Awake()
    {
        m_Text = GetComponent<Text>();
        Pattern();
    }

    void Update()
    {
        if (m_Value == value)
            return;

        if (m_Value < value)
        {
            m_Value += Time.deltaTime * m_RollSpeed;
            HOAudioManager.PlayLoopClip("Scoreup");
            if (m_Value > value)
            {
                m_Value = value;
                HOAudioManager.StopLoopClip();
            }
        }
        else if (m_Value > value)
        {
            m_Value -= Time.deltaTime * m_RollSpeed;
            HOAudioManager.PlayLoopClip("Scoreup");
            if (m_Value < value)
            {
                m_Value = value;
                HOAudioManager.StopLoopClip();
            }
        }
        Pattern();
    }

    void LateUpdate()
    {
        if (m_Text.text.CompareTo(m_TextValue) != 0)
        {
            m_Text.text = m_TextValue;
        }
    }

    private void Pattern()
    {
        if(string.IsNullOrEmpty(m_Pattern))
        {
            m_TextValue = ((int)m_Value).ToString();
        }
        else
        {
            m_TextValue = string.Format(m_Pattern, ((int)m_Value));
        }
        m_Text.text = m_TextValue;
    }
}
