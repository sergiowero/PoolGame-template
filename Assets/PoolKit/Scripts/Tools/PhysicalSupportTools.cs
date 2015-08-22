using UnityEngine;
using System.Collections;


interface IPhysicalSupport
{
    void Init(Hashtable table);

    void Update();
}

public enum PhysicalSupportType
{
    MaxSpeedLimit,
    None
}

public class PhysicalSupportTools : MonoBehaviour 
{
    private PhysicalSupportType m_SupportType;
    private IPhysicalSupport m_Support;

    void Update()
    {
        m_Support.Update();
    }

    public static void MaxSpeedLimitTo(GameObject o, Hashtable table)
    {
        MaxSpeedLimit sl = new MaxSpeedLimit();
        sl.Init(table);
        AddSupportTools(o, PhysicalSupportType.MaxSpeedLimit, sl);
    }

    public static void MaxSpeedLimitTo(GameObject o, float maxSpeed)
    {
        if(o.rigidbody == null)
        {
            Debug.LogError("can not found the rigidbody at the gameobject : " + o);
            return;
        }
        MaxSpeedLimit sl = new MaxSpeedLimit();
        sl.Init(Table("Rigidbody", o.rigidbody, "MaxSpeed", maxSpeed));
        AddSupportTools(o, PhysicalSupportType.MaxSpeedLimit, sl);
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
        if (table.ContainsKey("Rigidbody"))
        {
            m_Rigidbody = (Rigidbody)table["Rigidbody"];
        }
        else
        {
            Debug.LogError("there is no key named \"Rigidbody\" in the hashtable");
            return;
        }
        if (table.ContainsKey("MaxSpeed"))
        {
            m_MaxSpeed = (float)table["MaxSpeed"];
        }
        else
        {
            Debug.LogError("there is no key named \"MaxSpeed\" in the hashtable");
            return;
        }
    }

    public void Update()
    {
        Vector3 v = m_Rigidbody.velocity;
        if (v.sqrMagnitude > (m_MaxSpeed * m_MaxSpeed))
        {
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_MaxSpeed;
        }
    }
}
#endregion
