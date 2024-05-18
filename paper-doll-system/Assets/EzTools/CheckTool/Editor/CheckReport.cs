using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 檢查結果報告資料，可以使用CheckReport.Pass或CheckReport.Fail來快速建立報告
/// </summary>
public class CheckReport
{
    /// <summary>
    /// 檢查的物件
    /// </summary>
    public Object Asset { get; private set; }

    /// <summary>
    /// 檢查器
    /// </summary>
    public AssetChecker Checker { get; private set; }

    /// <summary>
    /// 是否通過檢查
    /// </summary>
    public bool IsPass { get; private set; }

    /// <summary>
    /// 回報訊息
    /// </summary>
    public string Message { get; private set; }

    public CheckReport()
    {
    }

    public CheckReport(AssetChecker checker, Object asset, bool isPass, string message)
    {
        Asset = asset;
        Checker = checker;
        IsPass = isPass;
        Message = message;
    }

    /// <summary>
    /// 建立通過檢查的報告
    /// </summary>
    public static CheckReport Pass(AssetChecker checker, Object asset)
    {
        return new CheckReport(checker, asset, true, string.Empty);
    }

    /// <summary>
    /// 建立未通過檢查的報告
    /// </summary>
    /// <param name="message">錯誤訊息</param>
    public static CheckReport Fail(AssetChecker checker, Object asset, string message)
    {
        return new CheckReport(checker, asset, false, message);
    }

    /// <summary>
    /// 建立未通過檢查的報告
    /// </summary>
    /// <param name="messages">多條錯誤訊息</param>
    public static CheckReport Fail(AssetChecker checker, Object asset, List<string> messages)
    {
        // join /n to each message
        string message = string.Join("\n", messages);

        // join /n in the start of message
        message = "\n" + message;

        return new CheckReport(checker, asset, false, message);
    }
}
