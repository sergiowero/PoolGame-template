using UnityEngine;
using System.Collections;

public class AnimationEventTools : MonoBehaviour {

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void ActiveObjectFalse(GameObject o)
    {
        o.SetActive(false);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void DestroyObject(GameObject o)
    {
        Destroy(o);
    }
}
