﻿using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {

    protected Quaternion m_Rotation;
    protected Vector3 m_Position;
    [SerializeField]
    protected Transform m_RefObject;
    [SerializeField]
    protected Vector3 m_Offset;

    private Transform m_Trans;

    [SerializeField]
    private bool m_DisableAtAwake;

    void Awake()
    {
        m_Trans = transform;
        m_Rotation = m_Trans.rotation;
        m_Position = m_Trans.position;
        if (m_DisableAtAwake)
            Close();
    }

    void LateUpdate()
    {
        if (m_RefObject)
        {
            Render();
        }
    }

    private void Render()
    {
        m_Trans.rotation = m_Rotation;
        m_Position = m_RefObject.position + m_Offset;
        m_Trans.position = m_Position;
    }

    public void Close()
    {
        transform.gameObject.SetActive(false);
        enabled = false;
    }

    public void Open()
    {
        transform.gameObject.SetActive(true);
        enabled = true;
    }

    public void SetRefObject(Transform refObject)
    {
        m_RefObject = refObject;
    }

    public void SetOffset(Vector3 offset)
    {
        m_Offset = offset;
    }
}
