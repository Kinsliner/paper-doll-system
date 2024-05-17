using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Ez;
using Ez.EzEditor;

public class ModelAssetDataEditor : EzEditorWindow
{
    public class EditModelAssetData
    {
        public ModelAssetData modelAssetData;
    }

    public override string DatName { get => "ModelAssetData"; }

    public override string SideViewTitle { get => "模型素材清單"; }

    private List<EditModelAssetData> editDatas = new List<EditModelAssetData>();
    private EditModelAssetData currentEdit = null;

    [MenuItem("Tools/模型資料編輯器")]
    public static void ShowWindow()
    {
        var window = GetWindow<ModelAssetDataEditor>("模型資料編輯器");
        window.Show();
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        currentEdit = null;
    }

    protected override void DrawSideViewData()
    {
        base.DrawSideViewData();
        DrawSortToolBar(ref editDatas, p => p.modelAssetData.assetID);
        DrawEditorModelAssetList();
    }

    /// <summary>
    /// 繪製物品資料清單
    /// </summary>
    private void DrawEditorModelAssetList()
    {
        if (GUILayout.Button("新增模型資料", GUILayout.Width(120f)))
        {
            var edit = new EditModelAssetData()
            {
                modelAssetData = new ModelAssetData()
                {
                    assetName = "新模型資料"
                }
            };
            editDatas.Add(edit);
            currentEdit = edit;
        }
        GUILayout.Space(10f);
        foreach (var edit in editDatas.ToArray())
        {
            bool isCurrent = currentEdit == edit;
            string name = $"{edit.modelAssetData.assetID}-{edit.modelAssetData.assetName}";
            DrawSideItem(name, isCurrent, delegate { currentEdit = edit; });
        }
    }

    protected override void DrawMainViewData()
    {
        base.DrawMainViewData();
        if (currentEdit != null)
        {
            string name = string.IsNullOrEmpty(currentEdit.modelAssetData.assetName) ? "未命名" : currentEdit.modelAssetData.assetName;
            MainViewTitle = $"{currentEdit.modelAssetData.assetID} - {name}";
        }
        else
        {
            MainViewTitle = "尚未選擇任何素材";
        }
        DrawEditItemData(currentEdit);
    }

    private void DrawEditItemData(EditModelAssetData edit)
    {
        if (edit == null)
        {
            return;
        }

        using (new GUILayout.HorizontalScope())
        {
            edit.modelAssetData.assetID = EditorGUILayout.IntField("ID", edit.modelAssetData.assetID);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("刪除", GUILayout.Width(80)))
            {
                editDatas.Remove(edit);
                return;
            }
        }

        edit.modelAssetData.assetName = EditorGUILayout.TextField("資源名稱", edit.modelAssetData.assetName);

        using (new GUILayout.VerticalScope("box"))
        {
            edit.modelAssetData.provideType = (ModelProvideType)EditorGUILayout.EnumPopup("提供方式", edit.modelAssetData.provideType);
            if (edit.modelAssetData.provideType == ModelProvideType.AssetBundle)
            {
                edit.modelAssetData.bundletId = EditorGUILayout.IntField("資源ID", edit.modelAssetData.bundletId);
            }
            else
            {
                edit.modelAssetData.assetPath = EditorGUILayout.TextField("資源路徑", edit.modelAssetData.assetPath);
            }
        }
    }

    /// <summary>
    /// 匯入資料
    /// </summary>
    protected override void ImportData()
    {
        base.ImportData();
        var table = FileHandler.Load<ModelAssetDataTable>();
        editDatas.Clear();
        foreach (var t in table.datas)
        {
            var editData = new EditModelAssetData();
            editData.modelAssetData = t;
            editDatas.Add(editData);
        }
    }

    /// <summary>
    /// 匯出資料
    /// </summary>
    protected override void ExportData()
    {
        base.ExportData();
        var table = new ModelAssetDataTable();
        table.datas = editDatas.Select(p => p.modelAssetData).ToList();
        FileHandler.Save(table);
    }
}
