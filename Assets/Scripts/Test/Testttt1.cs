using UnityEngine;
using System.Collections;

public class Testttt1 : MonoBehaviour
{
    [SerializeField]
    Vector2 size;

    [SerializeField]
    Vector3 position;

    void LateUpdate()
    {
        //GetComponent<RectTransform>().sizeDelta = size;
        GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
    }

}