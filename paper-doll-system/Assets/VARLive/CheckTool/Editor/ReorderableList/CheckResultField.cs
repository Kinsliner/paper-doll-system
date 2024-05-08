using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CheckResultField : ConfiguredReorderableList<CheckAsset>.Column
{
    public CheckResultField(string label, float width) : base(label, width)
    {
    }

    public override List<CheckAsset> OrderBy(bool isAscending, List<CheckAsset> elements)
    {
        if(isAscending)
        {
            return elements.OrderBy(x => x.checkResult).ToList();
        }
        else
        {
            return elements.OrderByDescending(x => x.checkResult).ToList();
        }
    }

    public override void DrawColumn(Rect rect, CheckAsset element)
    {
        base.DrawColumn(rect, element);

        GUIContent passIcon = EditorGUIUtility.IconContent("TestPassed");
        GUIContent failIcon = EditorGUIUtility.IconContent("TestFailed");
        GUIContent uncheckIcon = EditorGUIUtility.IconContent("TestNormal");
        GUIContent icon = element.checkResult switch
        {
            CheckResult.Uncheck => uncheckIcon,
            CheckResult.Pass => passIcon,
            CheckResult.Fail => failIcon,
            _ => null
        };
        icon.text = element.checkResult switch
        {
            CheckResult.Uncheck => "未檢查",
            CheckResult.Pass => "通過",
            CheckResult.Fail => "失敗",
            _ => string.Empty
        };

        EditorGUI.LabelField(rect, icon, EditorExtension.CenteredLabel);
    }
}
