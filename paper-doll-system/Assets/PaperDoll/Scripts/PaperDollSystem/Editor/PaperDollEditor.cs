using Ez;
using Ez.EzEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditPaperDollData
{
    public PaperDollData data;
}

public class PaperDollEditor : EzEditorWindow
{
    public override string DatName => "PaperDollData";

    public override string SideViewTitle => "紙娃娃資料清單";

    private List<EditPaperDollData> editDatas = new List<EditPaperDollData>();

    private EditPaperDollData currentEdit = null;

    private List<int> modelAssetsID = new List<int>();

    [MenuItem("Tools/紙娃娃編輯器", priority = 2001)]
    public static void OpenWindow()
    {
        GetWindow<PaperDollEditor>("紙娃娃編輯器");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        currentEdit = null;
        RefreshModelAssetsID();
    }

    protected override FileHandler GetFileHandler()
    {
        var streamingAssetPath = new StreamingAssetPath(DatName);
        return new FileHandler(streamingAssetPath, EzFileHandler);
    }

    private void RefreshModelAssetsID()
    {
        ModelAssetManager.InitData();
        modelAssetsID = ModelAssetManager.GetModelAssetDatas().Select(p => p.assetID).ToList();
    }

    protected override void ImportData()
    {
        base.ImportData();
        var table = FileHandler.Load<PaperDollDataTable>();
        editDatas.Clear();
        foreach (var t in table.datas)
        {
            var editData = new EditPaperDollData();
            editData.data = t;
            editDatas.Add(editData);
        }
    }

    protected override void ExportData()
    {
        base.ExportData();
        var table = new PaperDollDataTable();
        table.datas = editDatas.Select(p => p.data).ToList();
        FileHandler.Save(table);

        AssetDatabase.Refresh();
    }

    protected override void DrawSideViewData()
    {
        base.DrawSideViewData();
        DrawSortToolBar(ref editDatas, p => p.data.id);
        DrawEditDataList();
    }

    private void DrawEditDataList()
    {
        if (GUILayout.Button("新增紙娃娃資料", GUILayout.Width(120f)))
        {
            var edit = new EditPaperDollData()
            {
                data = new PaperDollData()
                {
                    name = "未命名",
                    assetID = modelAssetsID.FirstOrDefault(),
                }
            };
            editDatas.Add(edit);
            currentEdit = edit;
        }
        GUILayout.Space(10f);
        foreach (var edit in editDatas.ToArray())
        {
            bool isCurrent = currentEdit == edit;
            string name = $"{edit.data.id}-{edit.data.name}";

            DrawSideItem(name, isCurrent, delegate { currentEdit = edit; });
        }
    }

    protected override void DrawMainViewData()
    {
        base.DrawMainViewData();
        if (currentEdit != null)
        {
            string name = string.IsNullOrEmpty(currentEdit.data.name) ? "未命名" : currentEdit.data.name;
            MainViewTitle = $"{currentEdit.data.id} - {name}";
        }
        else
        {
            MainViewTitle = "尚未選擇任何資料";
        }
        DrawEditItemData(currentEdit);
    }

    private void DrawEditItemData(EditPaperDollData currentEdit)
    {
        if (currentEdit == null)
        {
            return;
        }

        using (new GUILayout.HorizontalScope())
        {
            currentEdit.data.id = EditorGUILayout.IntField("ID", currentEdit.data.id);

            // draw delete button
            if (GUILayout.Button("刪除", GUILayout.Width(120f)))
            {
                editDatas.Remove(currentEdit);
                currentEdit = null;
                return;
            }
        }

        currentEdit.data.name = EditorGUILayout.TextField("名稱", currentEdit.data.name);
        currentEdit.data.node = (BodyNode)EditorGUILayout.EnumPopup("節點", currentEdit.data.node);

        DrawModelAssetID(currentEdit);

        DrawIconPath(currentEdit);
    }

    private void DrawModelAssetID(EditPaperDollData currentEdit)
    {
        using (new GUILayout.HorizontalScope())
        {
            // label fixed width is 150
            EditorGUILayout.LabelField("素材ID", GUILayout.Width(150));

            // drop down list from modelAssetsID
            currentEdit.data.assetID = EditorGUILayout.IntPopup(currentEdit.data.assetID, modelAssetsID.Select(p => new GUIContent(p.ToString())).ToArray(), modelAssetsID.ToArray());

            if (GUILayout.Button("刷新", GUILayout.Width(120f)))
            {
                RefreshModelAssetsID();
            }
        }

        if (modelAssetsID.Contains(currentEdit.data.assetID) == false)
        {
            EditorGUILayout.HelpBox("素材ID不存在", MessageType.Error);
        }
    }

    private void DrawIconPath(EditPaperDollData currentEdit)
    {
        currentEdit.data.iconPath = EditorGUILayout.TextField("圖示路徑", currentEdit.data.iconPath);
    }
}
