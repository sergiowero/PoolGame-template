using UnityEngine;
using System.Collections;

public class BallShadowRenderer : MonoBehaviour {
    protected Transform m_Shadow;
    protected Transform m_ShadowRenderer;
    protected Vector3 m_ShadowRendererLocalPosition;

    void Awake()
    {
        m_Shadow = transform.FindChild("Shadow");
        if (m_Shadow)
        {
            m_ShadowRenderer = m_Shadow.FindChild("Renderer");
            m_ShadowRendererLocalPosition = m_ShadowRenderer.localPosition;
        }
    }

    public void Close()
    {
        if (m_Shadow)
        {
            m_Shadow.gameObject.SetActive(false);
            enabled = false;
        }
    }

    public void Open()
    {
        if (m_Shadow)
        {
            m_Shadow.gameObject.SetActive(true);
            enabled = true;
        }
    }

	void LateUpdate () 
    {
        if (m_Shadow)
        {
            RenderShadow();
        }
	}

    private void RenderShadow()
    {
        m_Shadow.rotation = Quaternion.identity;
        m_ShadowRendererLocalPosition.x = (transform.position.x - LightSource.Position.x) * .0015f;
        m_ShadowRendererLocalPosition.z = (transform.position.z - LightSource.Position.z) * .0015f;
        m_ShadowRenderer.localPosition = m_ShadowRendererLocalPosition;
    }
}
