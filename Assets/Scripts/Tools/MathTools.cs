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
    /// value around min and max , include min and exclude max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int Roll(int min, int max, int value)
    {
        if (value >= min && value < max)
            return value;

        int f = (max - min);
        if (value >= max) value -= f;
        else if (value < min) value += f;
        return Roll(min, max, value);
    }

    /// <summary>
    /// switch coordinate form world space to ui space
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector3 World2UI(Vector3 v)
    {
        Vector2 sp = Pools.SceneCamera.WorldToScreenPoint(v);
        return BaseUIController.GetUICamera().ScreenToWorldPoint(sp);
    }

    //public static Vector3 GetWorldDeltaWithPixelDelta(Vector3 pixelDelta)
    //{
    //    BaseUIController.GetUICamera().
    //}

    /// <summary>
    /// Get Multiplicative of n nearest x
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int Round2Number(int x, int n)
    {
        if (x % n > n / 2)
            return x + (n - x % n);
        else
            return x - x % n;
    }

    /// <summary>
    /// Get Multiplicative of n nearest x
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int Round2Number(float x, int n)
    {
        int xx = Mathf.RoundToInt(x);
        return Round2Number(xx, n);
    }

    /// <summary>
    /// Clamp the vector 3
    /// </summary>
    /// <param name="v"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Vector3 Clamp3(Vector3 v, Vector3 min, Vector3 max)
    {
        v.x = Mathf.Clamp(v.x, min.x, max.x);
        v.y = Mathf.Clamp(v.y, min.y, max.y);
        v.z = Mathf.Clamp(v.z, min.z, max.z);
        return v;
    }

    /// <summary>
    /// the angle between v1 and v2 , not from v1 to v2
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static float AngleBetween(Vector3 v1, Vector3 v2)
    {
        float radian = Mathf.Acos(Vector3.Dot(v1.normalized, v2.normalized));
        return Mathf.Rad2Deg * radian;
    }
}
