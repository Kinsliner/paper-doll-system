using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Ez.Tool;

[CustomEditor(typeof(UICollectorSetting))]
public class UICollectorSettingEditor : Editor
{
    private UICollectorSetting mergeTarget;

    public override void OnInspectorGUI()
    {
        var setting = target as UICollectorSetting;
        using (new GUILayout.VerticalScope("Box"))
        {
            mergeTarget = (UICollectorSetting)EditorGUILayout.ObjectField("合併目標: ", mergeTarget, typeof(UICollectorSetting), false);

            GUI.enabled = mergeTarget != null && mergeTarget != setting;
            if (GUILayout.Button("合併設定"))
            {
                setting.Merge(mergeTarget);
                EditorUtility.SetDirty(setting);
                AssetDatabase.SaveAssets();
            }
            GUI.enabled = true;
        }

        if (GUILayout.Button("建置設定"))
        {
            UICollectorEditor.SetupRelativePath();
            UICollectorEditor.BuildScript(setting);
        }

        base.OnInspectorGUI();
    }
}
