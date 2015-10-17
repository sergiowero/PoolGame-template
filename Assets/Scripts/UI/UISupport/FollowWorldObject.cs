using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FollowWorldObject : MonoBehaviour 
{
    [HideInInspector]
    public Transform followObject;

    [HideInInspector]
    public Color color;

    [SerializeField]
    private Image m_Image;

    void LateUpdate()
    {
        transform.position = MathTools.World2UI(followObject.position);
    }


    /// <summary>
    /// 这个会改成动画，而不是颜色
    /// </summary>
    /// <param name="stateName"></param>
    public void ChangeState(int state)
    {
        if(state == 1)
        {
            m_Image.color = Color.red;
        }
        else
        {
            m_Image.color = Color.white;
        }
    }
}
