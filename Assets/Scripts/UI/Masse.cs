using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Masse : MonoBehaviour 
{
    [SerializeField]
    private int m_PrevValue;

    [SerializeField]
    private RectTransform m_Cue;



    public void OnCueDrag(BaseEventData data)
    {
        PointerEventData pd = data as PointerEventData;
        Vector3 worldPosition = pd.worldPosition;
        int angle = (int)Vector3.Angle(Vector3.right, worldPosition - m_Cue.position);
        angle = Mathf.Clamp(angle, 0, 90);
        if(m_PrevValue != angle)
        {
            m_PrevValue = angle;
            m_Cue.localEulerAngles = new Vector3(0, 0, angle);

        }
    }
}
