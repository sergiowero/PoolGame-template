using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChapterCities : MonoBehaviour 
{
    public Image image;
    public Text text;
    public Text process;
    public GameObject lockMask;
    public int chapter;

    private RectTransform m_Trans;
    [SerializeField]
    private float m_Scale;

    private Vector3 m_ScaleVector;

    [SerializeField]
    [Range(0.001f, 7)]
    private float m_Distance;

    [SerializeField]
    private ScrollRect m_ScrollRect;


    void Awake()
    {
        m_Trans = transform as RectTransform;
        m_ScaleVector = Vector3.one * m_Scale;
        m_ScaleVector.z = 1;
        m_ScrollRect.onValueChanged.AddListener(OnScrollValueChange);
    }

    void OnDestroy()
    {
        m_ScrollRect.onValueChanged.RemoveListener(OnScrollValueChange);
    }

    public void OnScrollValueChange(Vector2 value)
    {
        m_Trans.localScale = Vector3.Lerp(m_ScaleVector, Vector3.one, Mathf.Abs(m_Trans.position.x / m_Distance));
    }
}
