using UnityEngine;
using System.Collections;
using DTime = System.DateTime;
using DSpan = System.TimeSpan;

public class EternalNode : MonoBehaviour
{
    private static EternalNode m_Instance = null;
    public static EternalNode Instance { get { return m_Instance; } }

    private float m_TimeDelta;
    public float TimeDelta { get { return m_TimeDelta; } }

    private float m_LastTime;

    private float m_PhysicalRcoverTime = 0;
    private string m_QuitTimeKey = "QuitTime";
    private string m_LastPhysicalKey = "LastPhysical";


    void Awake()
    {
        if (m_Instance)
            Destroy(gameObject);
        else
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
            if (PlayerPrefs.HasKey(m_LastPhysicalKey))
            {
                ConstantData.MPhysical = PlayerPrefs.GetInt(m_LastPhysicalKey);
            }
            if (PlayerPrefs.HasKey(m_QuitTimeKey))
            {
                DTime time = DTime.Parse(PlayerPrefs.GetString(m_QuitTimeKey));
                DTime now = DTime.Now;
                DSpan span = now - time;
                int ap = span.Minutes / 30;
                ConstantData.MPhysical += ap;
                if (ConstantData.MPhysical > 20)
                {
                    ConstantData.MPhysical = 20;
                }
            }
        }
    }

    void Update()
    {
        float rt = Time.realtimeSinceStartup;
        m_TimeDelta = rt - m_LastTime;
        m_LastTime = rt;
        PhysicalRecover();
    }

    private void PhysicalRecover()
    {
        m_PhysicalRcoverTime += m_TimeDelta;
        if(m_PhysicalRcoverTime > ConstantData.PhysicalRecoverInternal)
        {
            ConstantData.MPhysical++;
            if (ConstantData.MPhysical > 20)
                ConstantData.MPhysical = 20;
            m_PhysicalRcoverTime = 0;
            LaunchUIController.SetPhysical(ConstantData.MPhysical);
        }
    }

    void OnApplicationQuit()
    {
        m_Instance = null;
        PlayerPrefs.SetString(m_QuitTimeKey, System.DateTime.Now.ToString());
        PlayerPrefs.SetInt(m_LastPhysicalKey, ConstantData.MPhysical);
    }
}
