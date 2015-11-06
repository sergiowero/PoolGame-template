using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;



/// <summary>
/// 这个需要封装成通用的工具
/// </summary>
public class ScrollRectSnapElements : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    protected ScrollRect m_Rect;
    protected bool m_Dragging = false;
    protected RectTransform m_RectContent;
    protected bool m_MoveTo = false;

    [SerializeField]
    protected float m_SpeedThreshold;

    [SerializeField]
    [Range(0.01f, 0.2f)]
    protected float m_SnapTime;

    protected float m_Gap;

    void Awake()
    {
        m_Rect = GetComponent<ScrollRect>();
        m_RectContent = m_Rect.content;
        GridLayoutGroup group;
        if((group = m_RectContent.GetComponent<GridLayoutGroup>()))
        {
            m_Gap = group.cellSize.x + group.spacing.x;
        }
        else
        {
            Debug.LogError("No grid layout group component attach at the scroll rect content !");
        }
    }

    void Update()
    {
        if (m_Dragging) return;

        if (Mathf.Abs(m_Rect.velocity.x) <= m_SpeedThreshold && m_MoveTo)
        {
            m_MoveTo = false;
            float x = -MathTools.Round2Number(Mathf.Abs(m_RectContent.anchoredPosition.x), (int)m_Gap);
            m_Rect.velocity = Vector2.zero;
            iTween.ValueTo(m_RectContent.gameObject, 
                iTween.Hash("from", m_RectContent.anchoredPosition.x, 
                "to", x, 
                "time", m_SnapTime, 
                "onupdate", "MotionBlur", 
                "onupdatetarget", gameObject, 
                "easetype", iTween.EaseType.easeOutCubic));
        }
    }

    void MotionBlur(float value)
    {
        m_RectContent.anchoredPosition = new Vector2(value, m_RectContent.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_Dragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_Dragging = true;
        m_MoveTo = true;
    }
}
