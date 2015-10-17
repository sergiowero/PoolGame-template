using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class BallRefLightRenderer : MonoBehaviour {
    protected Transform m_RefLight;

    protected Quaternion m_Rotation;
    protected Vector3 m_Position;

    [SerializeField]
    protected Vector3 m_Offset;

    void Awake()
    {
        m_RefLight = transform.FindChild("RefLight");
        m_Rotation = m_RefLight.rotation;
        m_Position = m_RefLight.position;
    }

    void LateUpdate()
    {
        if (m_RefLight)
        {
            Render();
        }
    }

    private void Render()
    {
        m_RefLight.rotation = m_Rotation;
        m_Position = transform.position + m_Offset;
        m_RefLight.position = m_Position;
    }

    public void Close()
    {
        if(m_RefLight)
        {
            m_RefLight.gameObject.SetActive(false);
            enabled = false;
        }
    }

    public void Open()
    {
        if(m_RefLight)
        {
            m_RefLight.gameObject.SetActive(true);
            enabled = true;
        }
    }
}
