using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;
using System.Linq;

public class ObjectReplacePair
{
    public GameObject source;
    public GameObject target;

    public bool IsValid()
    {
        return source != null && target != null;
    }
}

public class ObjectReplacementTool : EditorWindow, DragDropEditor
{
    private const string ToolBarStyle = "toolbar";
    private const string ToolBarButtonStyle = "toolbarButton";

    private List<ObjectReplacePair> objectReplacePairs = new List<ObjectReplacePair>();
    private ObjectReplacePairReorderableList objectReplacePairList;
    private string projectPath;
    private int index = 0;
    private int maxCount = 0;


    [MenuItem("Tools/物件置換工具")]
    public static void ShowWindow()
    {
        GetWindow<ObjectReplacementTool>("物件置換工具");
    }

    private void OnEnable()
    {
        objectReplacePairList = new ObjectReplacePairReorderableList(objectReplacePairs);
        objectReplacePairList.OnClear = OnClear;
    }

    private void OnClear()
    {
        if (objectReplacePairs.Count <= 0)
            return;

        if (EditorUtility.DisplayDialog("警告", "確定清空所有資料?", "確定", "取消"))
        {
            objectReplacePairs.Clear();
        }
    }

    private void OnGUI()
    {
        DrawToolBar();
        DrawFileList();
        DrawProcessButtons();

        if (objectReplacePairList.IsHovering == false)
        {
            EditorExtension.DragDrop(this);
        }
    }

    private void DrawToolBar()
    {
        using (new GUILayout.HorizontalScope(ToolBarStyle))
        {
            if (GUILayout.Button("尋找置換物件", ToolBarButtonStyle, GUILayout.Width(150)))
            {
                FindAllReplacementObject();
            }
            if (GUILayout.Button("尋找置換變體", ToolBarButtonStyle, GUILayout.Width(150)))
            {
                FindAllReplacementVariant();
            }

            GUILayout.FlexibleSpace();
        }
    }

    #region DragDrop Add Object
    public bool IsAcceptDrag(UnityEngine.Object[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj is GameObject == false)
            {
                return false;
            }

            GameObject go = obj as GameObject;
            if (go.scene.IsValid() == false)
            {
                return false;
            }
        }
        return true;
    }

    public void OnAcceptDrag(UnityEngine.Object[] objects)
    {
        foreach (var obj in objects)
        {
            GameObject go = obj as GameObject;
            AddSource(go);
        }
    }

    private void AddSource(GameObject go)
    {
        if (go == null)
            return;

        if (go.scene.IsValid() == false)
        {
            return;
        }

        if (objectReplacePairs.Exists(x => x.source == go))
            return;
        
        ObjectReplacePair replacePair = new ObjectReplacePair()
        {
            source = go,
            target = null
        };
        objectReplacePairs.Add(replacePair);
    }
    #endregion

    #region Find Replacement Object
    private void FindAllReplacementObject()
    {
        foreach (var replacePair in objectReplacePairs)
        {
            if (replacePair.source == null)
                continue;

            FindReplacementObject(replacePair.source);
        }
    }
    
    private void FindReplacementObject(GameObject source)
    {
        // process name
        string sourceName = ProcessObjectName(source);

        // find same name prefab in project
        string[] guids = AssetDatabase.FindAssets(sourceName + " t:prefab");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                UpdatePair(source, prefab);
            }
        }
    }

    private string ProcessObjectName(GameObject source)
    {
        string sourceName = source.name;
        if (sourceName.EndsWith("(Clone)"))
        {
            sourceName = sourceName.Substring(0, sourceName.Length - 7);
        }
        if (sourceName.EndsWith(")"))
        {
            int index = sourceName.LastIndexOf('(');
            if (index > 0)
            {
                sourceName = sourceName.Substring(0, index);
            }
        }

        return sourceName;
    }

    private void UpdatePair(GameObject source, GameObject target)
    {
        ObjectReplacePair replacePair = objectReplacePairs.Find(x => x.source == source);
        if (replacePair != null)
        {
            replacePair.target = target;
        }
    }
    #endregion

    #region Find Replacement Variant
    private void FindAllReplacementVariant()
    {
        foreach (var replacePair in objectReplacePairs)
        {
            if (replacePair.source == null)
                continue;

            FindReplacementVariant(replacePair.source);
        }
    }

    private void FindReplacementVariant(GameObject source)
    {
        // process name
        string sourceName = ProcessObjectName(source);

        // find same name prefab variant in project
        string[] guids = AssetDatabase.FindAssets(sourceName + " t:prefab");
        if (guids.Length > 0)
        {
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    if (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Variant)
                    {
                        UpdatePair(source, prefab);
                        break;
                    }
                }
            }
        }
    }
    #endregion

    Vector2 scrollPosition;
    private void DrawFileList()
    {
        using (var scrollView = new GUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;

            if (objectReplacePairList != null)
            {
                objectReplacePairList.DoLayoutList();
                HandleRemoveFile();
            }
        }
    }

    private void HandleRemoveFile()
    {
        ObjectReplacePair needRemoveAsset = objectReplacePairList.GetNeedRemoveAsset();
        if (needRemoveAsset != null)
        {
            objectReplacePairs.Remove(needRemoveAsset);
        }
    }

    private void DrawProcessButtons()
    {
        // 計算無效的配對是否超過一個
        bool isDisabled = objectReplacePairs.Count(p => p.IsValid() == false) > 0;
        using (new EditorGUI.DisabledScope(isDisabled))
        {
            if (GUILayout.Button("開始處理", GUILayout.Height(30)))
            {
                ReplaceFiles();
            }
        }
    }

    private void ReplaceFiles()
    {
        if (EditorUtility.DisplayDialog("注意", "即將開始置換物件，此步驟可能會刪除原始物件", "Go") == false)
            return;

        index = 0;
        maxCount = objectReplacePairs.Count(p => p.IsValid());
        foreach (var pair in objectReplacePairs.ToArray())
        {
            if (pair.IsValid())
            {
                //取得檔案名稱
                GameObject source = pair.source;
                GameObject target = pair.target;

                //顯示進度條
                index++;
                string title = $"置換物件工具 ({index} / {maxCount})";
                string info = $"正在置換 {source.name} -> {target.name}";
                float progress = (float)index / maxCount;
                EditorUtility.DisplayProgressBar(title, info, progress);

                //置換物件
                ReplaceObject(source, target);

                //刪除原始物件
                objectReplacePairs.Remove(pair);
            }
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("置換物件工具", "處置完成", "確認");

        AssetDatabase.Refresh();
    }

    private void ReplaceObject(GameObject source, GameObject target)
    {
        // Replace the selected object with the replacement prefab
        GameObject newObject = PrefabUtility.InstantiatePrefab(target) as GameObject;
        newObject.transform.position = source.transform.position;
        newObject.transform.rotation = source.transform.rotation;
        newObject.transform.parent = source.transform.parent;

        // keep slibling index
        newObject.transform.SetSiblingIndex(source.transform.GetSiblingIndex());

        // check if the selectedObject object is a prefab instance, if not, destroy it
        if (PrefabUtility.GetPrefabInstanceStatus(source) == PrefabInstanceStatus.NotAPrefab)
        {
            // Destroy the old object
            Undo.DestroyObjectImmediate(source);
        }
    }
}
