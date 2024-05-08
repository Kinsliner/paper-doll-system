using System.Collections.Generic;
using UnityEngine;

public class PrefabAssetChecker : AssetChecker
{
    public override string Name => "物件檢查";

    public override bool IsCheckable(Object asset)
    {
        return asset is GameObject;
    }

    public override CheckReport Check(Object asset)
    {
        GameObject prefab = asset as GameObject;
        if (prefab.IsNotNull())
        {
            bool check = true;
            List<(bool, string)> message = new List<(bool, string)>();
            CheckScale(prefab, ref check, message);
            CheckStatic(prefab, ref check, message);

            List<string> reportMessage = ProcessMessage(message);

            if (check)
            {
                return new CheckReport(this, asset, true, "物件設定正確");
            }
            else
            {
                return CheckReport.Fail(this, asset, reportMessage);
            }
        }
        return new CheckReport(this, asset, true, "物件非Prefab");
    }

    private List<string> ProcessMessage(List<(bool, string)> message)
    {
        List<string> failMessage = new List<string>();
        foreach (var item in message)
        {
            if (item.Item1 == true)
            {
                ColorUtility.TryParseHtmlString(CheckToolEditor.PassColor, out Color color);

                failMessage.Add(item.Item2.ToColorString(color));
            }
            else
            {
                ColorUtility.TryParseHtmlString(CheckToolEditor.FailColor, out Color color);

                failMessage.Add(item.Item2.ToColorString(color));
            }
        }
        return failMessage;
    }

    private void CheckScale(GameObject prefab, ref bool check, List<(bool, string)> message)
    {
        if (prefab.transform.localScale.x < 0 || prefab.transform.localScale.y < 0 || prefab.transform.localScale.z < 0)
        {
            check = false;
            message.Add((false, "物件Scale有負值"));
        }
        else
        {
            message.Add((true, "物件Scale無負值"));
        }
    }

    private void CheckStatic(GameObject prefab, ref bool check, List<(bool, string)> message)
    {
        if (prefab.isStatic != false)
        {
            check = false;
            message.Add((false, "物件未取消Static"));
        }
        else
        {
            message.Add((true, "物件已取消Static"));
        }
    }
}
