using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSnapElement : MonoBehaviour ,IBeginDragHandler, IEndDragHandler
{
    protected enum SnappingStep
    {
        None,
        Drag,
        PrepareToSnapping,
        Snapping
    }
    protected SnappingStep m_Step;

    protected List<RectTransform> m_ElementList;
    protected List<Vector3> m_PositionList;
    protected ScrollRect m_ScrollRect;
    protected RectTransform m_ScrollRectTrans;
    protected RectTransform m_RectContentTrans;
    //protected bool m_Dragging;
    //protected bool m_Snapping;
    protected float m_FromHorizontalPosition;
    protected float m_ToHorizontalPosition;
    protected float m_Time;


    [SerializeField]
    protected float m_SpeedThreshold;
    [SerializeField]
    [Range(0.2f,1)]
    protected float m_SnapTime;
    [SerializeField]
    protected bool m_SnapAtAwake;

    void Awake()
    {
        if (!m_SnapAtAwake)
            return;

        Initialize();
    }

    public void Initialize()
    {
        m_ElementList = new List<RectTransform>();
        m_ScrollRect = GetComponent<ScrollRect>();
        m_ScrollRectTrans = m_ScrollRect.transform as RectTransform;
        m_RectContentTrans = m_ScrollRect.content;

        foreach (RectTransform t in m_RectContentTrans)
        {
            m_ElementList.Add(t);
        }
        m_Step = SnappingStep.PrepareToSnapping;
    }

	void Update () {
        if (m_Step == SnappingStep.Drag || m_Step == SnappingStep.None) return;

        if(m_Step == SnappingStep.PrepareToSnapping)
            FuncPrepareToSnapping();

        if(m_Step == SnappingStep.Snapping)
            FuncSnapping();
	}

    private void FuncSnapping()
    {
        float percentage = m_Time / m_SnapTime;
        m_Time += Time.deltaTime;
        float x = EaseOutCubic(m_FromHorizontalPosition, m_ToHorizontalPosition, percentage);

        Vector3 position = m_RectContentTrans.anchoredPosition;
        position.x = x;
        m_RectContentTrans.anchoredPosition = position;

        if(percentage >= 1)
        {
            m_Step = SnappingStep.None;
        }
    }

    private void FuncPrepareToSnapping()
    {
        if(Mathf.Abs(m_ScrollRect.velocity.x) <= m_SpeedThreshold)
        {
            m_FromHorizontalPosition = m_RectContentTrans.anchoredPosition.x;
            m_ToHorizontalPosition = m_FromHorizontalPosition + GetNearestX(0);
            m_Step = SnappingStep.Snapping;
            m_ScrollRect.velocity = Vector2.zero;
            m_Time = 0;
        }
    }

    private float GetNearestX(float value)
    {
        float x = float.MaxValue;
        for(int i = 0, count = m_ElementList.Count; i < count; i++)
        {
            Vector3 elementWorldPosition = m_RectContentTrans.localToWorldMatrix.MultiplyPoint(m_ElementList[i].anchoredPosition);
            Vector3 elementScrollRectPosition = m_ScrollRect.transform.worldToLocalMatrix.MultiplyPoint(elementWorldPosition);
            float a = value - elementScrollRectPosition.x;
            if (Mathf.Abs(a) <= x)
                x = a;
            else
                break;
        }
        return x;
    }

    private float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_Step = SnappingStep.Drag;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_Step = SnappingStep.PrepareToSnapping;
    }
}
