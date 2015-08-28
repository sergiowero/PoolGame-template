using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageTwinkle : MonoBehaviour
{
    [SerializeField]
    private Image m_Image = null;

    [SerializeField]
    private Color c1;
    [SerializeField]
    private Color c2;

    [SerializeField]
    private float m_Time;
    private float m_T;
    private float m_percentage;

    private bool m_Inverse;

    public void SetColor(Color _c1, Color _c2)
    {
        c1 = _c1;
        c2 = _c2;
    }

    public void SetTarget(Image i)
    {
        m_Image = i;
    }

    public void SetTime(float _time)
    {
        m_Time = _time;
    }

    void Start()
    {
        m_T = 0;
        if (m_Image == null)
            m_Image = GetComponent<Image>();
    }

	void Update () 
    {
        m_percentage = m_T / m_Time;
        if(!m_Inverse)
        {
            m_Image.color = Color.Lerp(c1, c2, m_percentage);
        }
        else
        {
            m_Image.color = Color.Lerp(c2, c1, m_percentage);
        }
        m_T += Time.deltaTime;
        if (m_T >= m_Time)
        {
            m_T = 0;
            m_Inverse = !m_Inverse;
        }
    }
}
