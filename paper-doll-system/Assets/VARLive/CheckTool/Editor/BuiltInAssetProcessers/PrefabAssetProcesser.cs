using UnityEditor;
using UnityEngine;

public class PrefabAssetProcesser : AssetProcesser
{
    public override string Name => "物件處理器";

    public override bool IsProcessable(Object asset)
    {
        return asset is GameObject;
    }

    public override void Process(Object asset)
    {
        GameObject prefab = asset as GameObject;
        if (prefab.IsNotNull())
        {
            // 處理Static旗標
            GameObjectUtility.SetStaticEditorFlags(prefab, staticflags);

            // 處理Scale
            prefab.transform.localScale = new Vector3(Mathf.Abs(prefab.transform.localScale.x), Mathf.Abs(prefab.transform.localScale.y), Mathf.Abs(prefab.transform.localScale.z));
        }
    }

    private bool foldout = false;
    private StaticEditorFlags staticflags = 0;
    public override void OnGUI()
    {
        base.OnGUI();

        foldout = EditorGUILayout.Foldout(foldout, "Static設定");
        if (foldout)
        {
            using (new EditorGUI.IndentLevelScope(1))
            {
                EditorGUIUtility.labelWidth = 100;
                staticflags = (StaticEditorFlags)EditorGUILayout.EnumFlagsField("Static Flags", staticflags);
            }
        }
    }
}