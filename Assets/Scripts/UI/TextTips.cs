using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTips : MonoBehaviour {

    [SerializeField]
    private Text m_Tips;
    [SerializeField]
    private Animator m_Animator;

    public Rect rect
    {
        get 
        {
            RectTransform rectT = transform as RectTransform;
            Rect r = new Rect();
            r.x = rectT.localPosition.x - rectT.rect.width * .5f;
            r.y = rectT.localPosition.y - rectT.rect.height * .5f;
            r.size = rectT.rect.size;
            return r;
        }
    }

    public void SetText(string text, Color c, bool stationary = false)
    {
        m_Tips.text = text;
        m_Tips.color = c;
        if (stationary)
            m_Animator.Play("Stationary");
        else
        {
            m_Animator.Play("Movement");
        }
    }
}
