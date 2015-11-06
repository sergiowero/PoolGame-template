using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BallInHand : MonoBehaviour 
{
    [HideInInspector]
    public Color color;

    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private Animator m_Animator;

    public void Show()
    {
        m_Animator.SetTrigger("Show");
    }

    public void Hide()
    {
        m_Animator.SetTrigger("Hide");
    }

    public void Locate(Vector3 position)
    {
        transform.position = MathTools.World2UI(position);
    }

    void LateUpdate()
    {
        Locate(Pools.CueBall.transform.position);
    }


    /// <summary>
    /// 这个会改成动画，而不是颜色
    /// </summary>
    /// <param name="stateName"></param>
    public void ChangeState(int state)
    {
        if (state == 1)
        {
            iTween.ColorTo(m_Image.gameObject, Color.red, .2f);
        }
        else
        {
            iTween.ColorTo(m_Image.gameObject, Color.white, .2f);
        }
        //m_Animator.SetTrigger(state);
    }
}
