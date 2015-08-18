using UnityEngine;
using System.Collections;

public class MathTools
{
    /// <summary>
    /// value around min and max , include min and exclude max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float Roll(float min, float max, float value)
    {
        if (value >= min && value < max)
            return value;

        float f = (max - min);
        if (value >= max) value -= f;
        else if (value < min) value += f;
        return Roll(min, max, value);
    }

    /// <summary>
    /// siwtch coordinate form world space to ui space
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector3 World2UI(Vector3 v)
    {
        Vector2 sp = Pools.SceneCamera.WorldToScreenPoint(v);
        return BaseUIController.GetUICamera().ScreenToWorldPoint(sp);
    }

}
