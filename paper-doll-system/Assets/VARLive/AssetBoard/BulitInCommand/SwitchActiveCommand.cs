using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActiveCommand : AssetBoardCommand
{
    public override string CommandName => "切換Active狀態";

    public override void Execute(List<Object> objects)
    {
        foreach (var obj in objects)
        {
            if (obj is GameObject go)
            {
                go.SetActive(!go.activeSelf);
            }
        }
    }
}
