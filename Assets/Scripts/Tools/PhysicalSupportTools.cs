using UnityEngine;
using System.Collections;


interface IPhysicalSupport
{
    void Init(Hashtable table);

    void FixedCall();
}

public enum PhysicalSupportType
{
    MaxSpeedLimit,
    PhysicalDrag,
    None
}

public class PhysicalSupportTools : MonoBehaviour 
{
    private PhysicalSupportType m_SupportType;
    private IPhysicalSupport m_Support;

    void FixedUpdate()
    {
        m_Support.FixedCall();
    }

    public static PhysicalSupportTools MaxSpeedLimitTo(GameObject o, Hashtable table)
    {
        MaxSpeedLimit sl = new MaxSpeedLimit();
        sl.Init(table);
        return AddSupportTools(o, PhysicalSupportType.MaxSpeedLimit, sl);
    }

    public static PhysicalSupportTools MaxSpeedLimitTo(GameObject o, float maxSpeed)
    {
        if(o.rigidbody == null)
        {
            Debug.LogError("can not found the rigidbody at the gameobject : " + o);
            return null;
        }
        MaxSpeedLimit sl = new MaxSpeedLimit();
        sl.Init(Table("Rigidbody", o.rigidbody, "MaxSpeed", maxSpeed));
        return AddSupportTools(o, PhysicalSupportType.MaxSpeedLimit, sl);
    }

    public static PhysicalSupportTools PhysicalDragTo(GameObject o, Hashtable table)
    {
        PhysicalDrag pd = new PhysicalDrag();
        pd.Init(table);
        return AddSupportTools(o, PhysicalSupportType.PhysicalDrag, pd);
    }

    public static PhysicalSupportTools PhysicalDragTo(GameObject o, float velocityDrag, float angularVelocityDrag)
    {
        if(o.rigidbody == null)
        {
            Debug.LogError("can not found the rigidbody at the gameobject : " + o);
            return null;
        }

        PhysicalDrag pd = new PhysicalDrag();
        pd.Init(Table("Rigidbody", o.rigidbody, "VelocityDrag", velocityDrag, "AngularVelocityDrag", angularVelocityDrag));
        return AddSupportTools(o, PhysicalSupportType.PhysicalDrag, pd);
    }

    private static PhysicalSupportTools AddSupportTools(GameObject o, PhysicalSupportType supportType, IPhysicalSupport support)
    {
        PhysicalSupportTools ps = o.GetComponent<PhysicalSupportTools>();
        if (!ps || ps.m_SupportType != supportType)
        {
            ps = o.AddComponent<PhysicalSupportTools>();
            ps.m_SupportType = supportType;
        }
        ps.m_Support = support;
        return ps;
    }

    public static void Remove(GameObject o, PhysicalSupportType supportType)
    {
        PhysicalSupportTools[] pss = o.GetComponents<PhysicalSupportTools>();
        if(pss != null || pss.Length != 0)
        {
            for(int i = 0, length = pss.Length ; i < length; i++)
            {
                if (pss[i].m_SupportType == supportType)
                    Destroy(pss[i]);
            }
        }
    }

    public static Hashtable Table(params object[] values)
    {
        int length = values.Length;
        if(length % 2 != 0)
        {
            Debug.LogError("create hash table error. the number of params must be even number!");
            return null;
        }
        Hashtable t = new Hashtable();
        for(int i = 0; i < length; i+=2)
        {
            if(!(values[i] is string))
            {
                Debug.LogError("the key of the hashtable must be string");
                return null;
            }
            t.Add(values[i], values[i + 1]);
        }
        return t;
    }
}

#region MaxSpeedLimit-------------------------------------------------------------------------------------
class MaxSpeedLimit : IPhysicalSupport
{
    Rigidbody m_Rigidbody;
    float m_MaxSpeed;

    public void Init(Hashtable table)
    {
        m_Rigidbody = (Rigidbody)table["Rigidbody"];
        m_MaxSpeed = (float)table["MaxSpeed"];
    }

    public void FixedCall()
    {
        Vector3 v = m_Rigidbody.velocity;
        if (v.sqrMagnitude > (m_MaxSpeed * m_MaxSpeed))
        {
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_MaxSpeed;
        }
    }
}
#endregion

class PhysicalDrag : IPhysicalSupport
{
    Rigidbody m_Rigidbody;

    Vector3 m_Velocity;
    Vector3 m_AngularVelocity;

#if !UNITY_EDITOR
    float m_VelocityDrag;
    float m_AngularVelocityDrag;
#endif

    public void Init(Hashtable table)
    {
        m_Rigidbody = (Rigidbody)table["Rigidbody"];
#if !UNITY_EDITOR
        m_VelocityDrag = (float)table["VelocityDrag"];
        m_AngularVelocityDrag = (float)table["AngularVelocityDrag"];
#endif
    }

    public void FixedCall()
    {
        if (m_Rigidbody.velocity != Vector3.zero)
            VelocityDrag();
        if (m_Rigidbody.angularVelocity != Vector3.zero)
            AngularVelocityDrag();
    }

    private void VelocityDrag()
    {
        m_Velocity = m_Rigidbody.velocity;
#if UNITY_EDITOR
        float f = m_Velocity.magnitude - ConstantData.GetPoolDatas().BallDrag * Time.fixedDeltaTime;
#else
        float f = m_Velocity.magnitude - m_VelocityDrag * Time.fixedDeltaTime;
#endif
        if (f <= 0)
        {
            m_Rigidbody.velocity = Vector3.zero;
        }
        else
        {
            m_Rigidbody.velocity = m_Velocity.normalized * f;
        }
    }

    private void AngularVelocityDrag()
    {
        m_AngularVelocity = m_Rigidbody.angularVelocity;
#if UNITY_EDITOR
        float f = m_AngularVelocity.magnitude - ConstantData.GetPoolDatas().BallAngularDrag * (m_Velocity.sqrMagnitude < .1f ? 2 : 1) * Time.fixedDeltaTime;
#else
        float f = m_AngularVelocity.magnitude - m_AngularVelocityDrag * Time.fixedDeltaTime;
#endif
        if (f <= 0)
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            m_Rigidbody.angularVelocity = m_AngularVelocity.normalized * f;
        }
    }
}
