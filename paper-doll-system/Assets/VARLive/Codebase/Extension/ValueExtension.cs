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
}