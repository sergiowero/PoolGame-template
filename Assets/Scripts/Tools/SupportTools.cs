using UnityEngine;
using System.Collections;

public class SupportTools
{
    public static GameObject AddChild(Transform parent, Transform child)
    {
        child.SetParent(parent);
        child.localRotation = Quaternion.identity;
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        return child.gameObject;
    }

    public static GameObject AddChild(GameObject parent, GameObject Child)
    {
        GameObject o = (GameObject)GameObject.Instantiate(Child);
        return AddChild(parent.transform, o.transform);
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
}
