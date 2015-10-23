using UnityEngine;
using System.Collections;

public delegate void Delegate1Args<T>(T t);
public delegate void Delegate0Args();


public class SupportTools
{
    [System.Flags]
    public enum AxisIgnore
    {
        IgnoreX = 1 <<0,
        IgnoreY = 1 << 1,
        IgnoreZ = 1 << 2
    }

    public static GameObject AddChild(Transform parent, Transform child)
    {
        child.SetParent(parent);
        child.localRotation = Quaternion.identity;
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        return child.gameObject;
    }

    public static GameObject AddChild(GameObject parent, GameObject child)
    {
        GameObject o = (GameObject)GameObject.Instantiate(child);
        return AddChild(parent.transform, o.transform);
    }

    public static T AddChild<T>(GameObject parent, GameObject child) where T : Component
    {
        return AddChild(parent, child).GetComponent<T>();
    }

    public static T AddChild<T>(GameObject parent, string path) where T : Component
    {
        GameObject o = AddChild(parent, (GameObject)Resources.Load(path));
        return o.GetComponent<T>();
    }

    public static T AddChild<T>(GameObject parent) where T : Component
    {
        GameObject o = new GameObject();
        T t = o.AddComponent<T>();
        AddChild(parent.transform, o.transform);
        return t;
    }

    public static void SetPosition(GameObject o, Vector3 position, SupportTools.AxisIgnore ignore, bool useRigibody = false)
    {
        Vector3 v = o.transform.position;
        if((ignore & AxisIgnore.IgnoreX) == 0)
        {
            v.x = position.x;
        }
        if((ignore & AxisIgnore.IgnoreY) == 0)
        {
            v.y = position.y;
        }
        if((ignore & AxisIgnore.IgnoreZ) == 0)
        {
            v.z = position.z;
        }
        o.transform.position = v;
    }
}
