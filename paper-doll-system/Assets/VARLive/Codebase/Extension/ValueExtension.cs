using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ValueExtension
{
    /// <summary>
    /// 取得文字的字元數, 中文當作兩個字元
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetCharLength(this string str)
    {
        int length = 0;

        for (int i = 0; i < str.Length; i++)
        {
            byte[] byte_len = Encoding.Default.GetBytes(str.Substring(i, 1));

            if (byte_len.Length > 1)
            {
                length += 2;
            }
            else
            {
                length += 1;
            }
        }

        return length;
    }

    public static string ToPercentString(this float value)
    {
        return string.Format("{0}%", value * 100);
    }

    public static string ToColorString(this string str, Color color)
    {
        return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), str);
    }

    public static string ToTimeString(this float time)
    {
        int hour = (int)(time / 3600);
        int minute = (int)((time - hour * 3600) / 60);
        int second = (int)(time - hour * 3600 - minute * 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
    }

    public static string MergeToString<T>(this List<T> src, string split = ",")
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < src.Count; i++)
        {
            sb.Append(src[i].ToString());

            if (i != src.Count - 1)
            {
                sb.Append(split);
            }
        }

        return sb.ToString();
    }

    public static List<string> ConvertToString<T>(this List<T> src)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < src.Count; i++)
        {
            result.Add(src[i].ToString());
        }

        return result;
    }

    public static string RemoveStart(this string src, string remove)
    {
        if (src.StartsWith(remove))
        {
            return src.Substring(remove.Length);
        }
        else
        {
            return src;
        }
    }

    public static string RemoveEnd(this string src, string remove)
    {
        if (src.EndsWith(remove))
        {
            return src.Substring(0, src.Length - remove.Length);
        }
        else
        {
            return src;
        }
    }

    /// <summary>
    /// 四捨五入至小數點第N位
    /// </summary>
    public static float Round(this float src, int digit)
    {
        return (float)Math.Round(src, digit);
    }

    public static string ToFullMessage(this Exception e)
    {
        return e.Message + "=>" + e.StackTrace;
    }

    public static LayerMask AddLayer(this LayerMask layerMask, string layerName)
    {
        int mask = layerMask.value;

        int layer = LayerMask.NameToLayer(layerName);

        if (layer == -1)
        {
            Debug.LogError("Layer不存在: " + layerName);
            return layerMask;
        }

        mask |= (1 << layer);
        layerMask.value = mask;
        return layerMask;
    }

    /// <summary>
    /// 轉換為Pose
    /// </summary>
    public static Pose ToPose(this Vector3 v3)
    {
        return new Pose(v3, Quaternion.identity);
    }

    /// <summary>
    /// 轉換為Pose
    /// </summary>
    public static Pose ToPose(this Quaternion quaternion)
    {
        return new Pose(Vector3.zero, quaternion);
    }

    /// <summary>
    /// 建立一個用於表示遮罩的Vector3，傳入的Vector3的數值會被正規化為0或1
    /// </summary>
    public static Vector3 ToMask(this Vector3 v3)
    {
        v3.x = v3.x == 0 ? 0 : 1;
        v3.y = v3.y == 0 ? 0 : 1;
        v3.z = v3.z == 0 ? 0 : 1;
        return v3;
    }

    /// <summary>
    /// 將數值轉換為Vector3
    /// </summary>
    public static Vector3 ToVector3(this float value)
    {
        return new Vector3(value, value, value);
    }

    /// <summary>
    /// 將數值轉換為Vector3
    /// </summary>
    /// <param name="mask">用於標示是否要使用數值遮罩</param>
    public static Vector3 ToVector3(this float value, Vector3 mask)
    {
        // 確定構成mask
        mask = mask.ToMask();

        return new Vector3(value * mask.x, value * mask.y, value * mask.z);
    }

    /// <summary>
    /// 最大值
    /// </summary>
    public static float Max(this float v, float max)
    {
        return v > max ? max : v;
    }

    /// <summary>
    /// 最小值
    /// </summary>
    public static float Min(this float v, float min)
    {
        return v < min ? min : v;
    }

    /// <summary>
    /// 限制在範圍內
    /// </summary>
    public static float Clamp(this float v, float min, float max)
    {
        return v.Max(min).Min(max);
    }

    /// <summary>
    /// 最大值
    /// </summary>
    public static int Max(this int v, int max)
    {
        return v > max ? max : v;
    }

    /// <summary>
    /// 最小值
    /// </summary>
    public static int Min(this int v, int min)
    {
        return v < min ? min : v;
    }

    /// <summary>
    /// 限制在範圍內
    /// </summary>
    public static int Clamp(this int v, int min, int max)
    {
        return v.Max(min).Min(max);
    }
}