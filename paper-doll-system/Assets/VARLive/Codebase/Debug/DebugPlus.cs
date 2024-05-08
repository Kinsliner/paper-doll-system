using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebugPlus
{
    /// <summary>
    /// 除錯與額外堆疊追蹤功能 (執行順序 9.8.7...1)
    /// </summary>
    /// <param name="message">訊息</param>
    /// <param name="skipFrames">追蹤數量</param>
    public static void Log(string message, int skipFrames = 20)
    {
        message = $"<color=#f6cee8>{message}</color>\n";
        int index = 1;
        for (int i = 0; i < skipFrames; i++)
        {
            try
            {
                StackTrace stackTrace = new(i + 1, true);
                StackFrame stack = stackTrace.GetFrame(0);
                message += $"\n______________________________【({index}).  <color=#f6cee8>{stack.GetMethod().DeclaringType.Name}</color>】______________________________";
                message += $"\n追蹤行數 : {stack.GetFileLineNumber()}";
                message += $"\n方法 : <a href=\"{stack.GetFileName()}\" line=\"{stack.GetFileLineNumber()}\">{MethodColorSwitch(stack.GetMethod().ToString())}</a>\n";
                index++;
            }
            catch (System.Exception)
            {

            }

        }
        Debug.Log(message);
    }

    static string MethodColorSwitch(string msg)
    {
        var type = msg.Split('.').FirstOrDefault();
        
        if (type != null)
        {
            msg = type switch
            {
                "UnityEngine" => $"<color=#9cd023>{msg}</color>",

                "System"=> $"<color=#8E8E8E>{msg}</color>",

                _=> $"<color=#f7a4c0>{msg}</color>"
            };
        }

        return msg;
    }

    public static void Log(object obj, int skipFrames = 3) =>
        Log(obj.ToString(), skipFrames);
}
