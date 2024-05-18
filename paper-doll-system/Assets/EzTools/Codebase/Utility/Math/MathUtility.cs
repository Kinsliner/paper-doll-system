using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NumberCompare
{
    Less,
    LessOrEqual,
    Equal,
    More,
    MoreOrEqual
}

public static class MathUtility
{
    public static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Mathf.Lerp(oMin, oMax, t);
    }

    public static Color Remap(float iMin, float iMax, Color oMin, Color oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Color.Lerp(oMin, oMax, t);
    }

    public static float MinProgress(float min, float max, float value)
    {
        if (value <= min)
        {
            return 0;
        }
        else if (value >= max)
        {
            return 1;
        }
        else
        {
            return (value - min) / (max - min);
        }
    }

    /// <summary>
    /// 把Euler轉成0~360
    /// </summary>
    public static Vector3 ClampEuler(float x, float y, float z)
    {
        return new Vector3(ClampAngle(x), ClampAngle(y), ClampAngle(z));
    }

    /// <summary>
    /// 把Angle轉成0~360
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float ClampAngle(float x)
    {
        if (x < 0f)
        {
            x = 360f + x;
        }
        else if (x > 360f)
        {
            x = x - 360f;
        }
        return x;
    }

    /// <summary>
    /// 計算拋物線
    /// </summary>
    public static Vector3 Parabolic(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1; // 將t轉換為-1到1之間
        Vector3 travelDirection = end - start; // 計算拋物線的方向向量
        Vector3 result = start + t * travelDirection; // 計算拋物線的線性運動
        result.y += ((-parabolicT * parabolicT + 1) * height); // 計算拋物線的高度
        return result;
    }

    public static bool Compare(float a, float b, NumberCompare compare)
    {
        switch (compare)
        {
            case NumberCompare.Less:
                return a < b;
            case NumberCompare.LessOrEqual:
                return a <= b;
            case NumberCompare.Equal:
                return a == b;
            case NumberCompare.More:
                return a > b;
            case NumberCompare.MoreOrEqual:
                return a >= b;
            default:
                return false;
        }
    }
}
