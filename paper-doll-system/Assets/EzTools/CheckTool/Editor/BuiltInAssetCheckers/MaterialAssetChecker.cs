using UnityEditor;
using UnityEngine;

public class MaterialAssetChecker : AssetChecker
{
    public override string Name => "材質檢查";

    public override bool IsCheckable(Object asset)
    {
        return asset is Material;
    }

    public override CheckReport Check(Object asset)
    {
        Material material = asset as Material;
        if (material.IsNotNull())
        {
            if(material.enableInstancing == false)
            {
                return new CheckReport(this, asset, false, "材質未開啟Instancing");
            }
            else
            {
                return new CheckReport(this, asset, true, "材質已開啟Instancing");
            }
        }
        return new CheckReport(this, asset, true, "物件非材質");
    }

    private bool foldout = false;
    public override void OnGUI()
    {
        base.OnGUI();

        using (new EditorGUI.IndentLevelScope(1))
        {
            foldout = EditorGUILayout.Foldout(foldout, "材質設定");
            if (foldout)
            {
                using (new EditorGUI.IndentLevelScope(1))
                {
                    GUIContent checkIcon = EditorGUIUtility.IconContent("Check");

                    checkIcon.text = "Enable GPU Instancing".ToColorString(Color.white);
                    EditorGUILayout.LabelField(checkIcon, EditorExtension.RichTextLabel);
                }
            }
        }
    }
}
