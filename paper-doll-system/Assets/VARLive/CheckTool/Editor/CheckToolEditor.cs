using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum CheckResult
{
    Uncheck,
    Pass,
    Fail
}

public class CheckAsset
{
    public bool IsSelected { get; set;}

    public Object asset;
    public CheckResult checkResult;
    public List<CheckReport> reports = new List<CheckReport>();

    public CheckAsset(Object asset)
    {
        this.asset = asset;
    }
}

public class CheckToolEditor : EditorWindowExtender
{
    private List<CheckAsset> currentCheckObjects = new List<CheckAsset>();
    private List<AssetChecker> checkers = new List<AssetChecker>();
    private List<AssetProcesser> processers = new List<AssetProcesser>();
    private CheckAssetReorderableList checkAssetReorderableList;
    private GUILayout.ScrollViewScope assetsScrollViewScope;
    private GUILayout.ScrollViewScope reportScrollViewScope;
    private GUILayout.ScrollViewScope checkersScrollViewScope;
    private GUILayout.ScrollViewScope processersScrollViewScope;
    private Vector2 assetsScrollPos;
    private Vector2 reportScrollPos;
    private Vector2 checkersScrollPos;
    private Vector2 processersScrollPos;

    [MenuItem("VAR Live/檢查小工具")]
    private static void ShowWindow()
    {
        var window = GetWindow<CheckToolEditor>("檢查小工具");
        window.Show();
    }

    private void OnEnable()
    {
        // 建立Config
        var config = new ConfiguredReorderableList<CheckAsset>.Configurator();
        config.RemoveColumnWidth = 100f;
        config.columns = new ()
        {
            new CheckResultField("檢查結果", 100f),
            new ObjectField("檔案", 300f),
        };

        // 創建ReorderableList
        checkAssetReorderableList = new CheckAssetReorderableList(currentCheckObjects, config);

        // 搜尋所有檢查器
        List<System.Type> types = EditorExtension.FindSubClassTypes<AssetChecker>();
        foreach (var type in types)
        {
            checkers.Add((AssetChecker)System.Activator.CreateInstance(type));
        }

        // 搜尋所有處理器
        types = EditorExtension.FindSubClassTypes<AssetProcesser>();
        foreach (var type in types)
        {
            processers.Add((AssetProcesser)System.Activator.CreateInstance(type));
        }
    }

    private void OnGUI()
    {
        if (checkAssetReorderableList.IsHovering == false)
        {
            DragDrop();
        }
        DrawToolBar();
        DrawPanel();
    }

    #region 工具列
    private const string ToolBarStyle = "Toolbar";
    private const string ToolBarButtonStyle = "toolbarbutton";
    private void DrawToolBar()
    {
        GUILayout.BeginHorizontal(ToolBarStyle, GUILayout.Width(position.width));
        {
            if (GUILayout.Button("清除檢查物件", ToolBarButtonStyle, GUILayout.Width(150)))
            {
                if (EditorUtility.DisplayDialog("警告", "確定清空所有資料?", "確定", "取消"))
                {
                    currentCheckObjects.Clear();
                }
            }

            GUILayout.FlexibleSpace();

            // 顯示搜尋框
            searchKeyword = GUILayout.TextField(searchKeyword, EditorStyles.toolbarSearchField, GUILayout.Width(300));

            // 顯示取消搜尋按鈕
            if (GUILayout.Button(string.Empty, "ToolbarSeachCancelButton"))
            {
                // Remove focus if cleared
                searchKeyword = string.Empty;
                PerformSearch();
            }

            // 執行搜尋
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
                PerformSearch();
            }
        }
        GUILayout.EndHorizontal();
    }

    private List<CheckAsset> filteredCheckObjects = new List<CheckAsset>();
    private string searchKeyword = ""; // 用於保存搜尋關鍵字
    private void PerformSearch()
    {
        if(string.IsNullOrEmpty(searchKeyword))
        {
            // 如果搜索關鍵字為空，則顯示所有檢查物件
            filteredCheckObjects.Clear();
            checkAssetReorderableList.UpdateList(currentCheckObjects);
            return;
        }

        // 執行搜尋
        filteredCheckObjects = currentCheckObjects.Where(x => x.asset.name.Contains(searchKeyword)).ToList();

        // 更新ReorderableList
        checkAssetReorderableList.UpdateList(filteredCheckObjects);

        GUI.FocusControl(null); // 這將清除焦點，並且將焦點設置為空，這樣可以清除搜索框的焦點

        // 重繪視窗
        Repaint();
    }
    #endregion

    #region 拖曳加入物件
    protected override void OnDragDropObjects(Object[] objectReferences)
    {
        base.OnDragDropObjects(objectReferences);

        foreach (var obj in objectReferences)
        {
            if (obj is DefaultAsset && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj)))
            {
                // 如果拖曳的是資料夾，搜尋資料夾內的物件
                string folderPath = AssetDatabase.GetAssetPath(obj);
                string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

                foreach (string filePath in files)
                {
                    string assetPath = filePath.Replace(Application.dataPath, "Assets");
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    AddCheckAsset(asset);
                }
            }
            else
            {
                AddCheckAsset(obj);
            }
        }
    }

    private void AddCheckAsset(Object asset)
    {
        if (asset == null)
            return;

        if (currentCheckObjects.Exists(x => x.asset == asset))
            return;

        currentCheckObjects.Add(new CheckAsset(asset));
    }
    #endregion

    private void DrawPanel()
    {
        using (new GUILayout.HorizontalScope())
        {
            DrawSidePanel();
            DrawMainPanel();
        }
    }

    private void DrawSidePanel()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(250)))
        {
            DrawSettings();

            DrawCheckList();

            DrawProcesserList();
        }
    }

    /// <summary>
    /// 繪製設定
    /// </summary>
    private void DrawSettings()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("設定", EditorExtension.SubtitleStyle);
            if (GUILayout.Button("開始檢查", GUILayout.Height(30)))
            {
                foreach (var checkAsset in currentCheckObjects)
                {
                    DoCheckAsset(checkAsset);
                }
            }
        }
    }

    private void DoCheckAsset(CheckAsset checkAsset)
    {
        // 清除舊的報告
        checkAsset.reports.Clear();

        // 物件必須通過所有啟動的檢測器
        foreach (var checker in checkers)
        {
            if (checker.IsEnable == false)
                continue;

            if (checker.IsCheckable(checkAsset.asset) == false)
                continue;

            CheckReport report = checker.Check(checkAsset.asset);

            // 加入檢測報告
            checkAsset.reports.Add(report);
        }

        // 檢查結果
        if (checkAsset.reports.Count > 0)
        {
            // 物件必須通過所有檢測器，才算通過
            checkAsset.checkResult = checkAsset.reports.All(x => x.IsPass) ? CheckResult.Pass : CheckResult.Fail;
        }
        else
        {
            // 沒有檢測報告，視為未檢查
            checkAsset.checkResult = CheckResult.Uncheck;
        }
    }

    /// <summary>
    /// 繪製檢查器清單
    /// </summary>
    private void DrawCheckList()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("檢查清單", EditorExtension.TitleStyle);

            using (checkersScrollViewScope = new GUILayout.ScrollViewScope(checkersScrollPos))
            {
                checkersScrollPos = checkersScrollViewScope.scrollPosition;
                foreach (var checker in checkers)
                {
                    DrawCheckers(checker);
                }
            }
        }
    }

    /// <summary>
    /// 繪製檢查器
    /// </summary>
    private void DrawCheckers(AssetChecker assetChecker)
    {
        // 準備GUI樣式
        GUIStyle style = new GUIStyle("button");
        style.alignment = TextAnchor.MiddleLeft;
        style.margin = new RectOffset(0, 10, 5, 5);
        
        GUIContent unuseIcon = EditorGUIUtility.IconContent("winbtn_mac_inact@2x");
        GUIContent usedIcon = EditorGUIUtility.IconContent("winbtn_mac_max@2x");
        GUIContent displayIcon = assetChecker.IsEnable ? usedIcon : unuseIcon;

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            Vector2 iconSize = EditorGUIUtility.GetIconSize();

            EditorGUIUtility.SetIconSize(new Vector2(12, 12));

            // 顯示檢查器名稱
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(displayIcon, EditorExtension.SubtitleStyle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
                assetChecker.IsEnable = GUILayout.Toggle(assetChecker.IsEnable, assetChecker.Name, style, GUILayout.Height(24));
            }

            // 客製化檢查器繪製
            assetChecker.OnGUI();

            EditorGUIUtility.SetIconSize(iconSize);
        }
    }

    /// <summary>
    /// 繪製處理器清單
    /// </summary>
    private void DrawProcesserList()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("處理器清單", EditorExtension.TitleStyle);

            using (processersScrollViewScope = new GUILayout.ScrollViewScope(processersScrollPos))
            {
                processersScrollPos = processersScrollViewScope.scrollPosition;
                foreach (var processer in processers)
                {
                    DrawProcesser(processer);
                }

                GUILayout.FlexibleSpace();
            }
        }
    }

    /// <summary>
    /// 繪製處理器
    /// </summary>
    private void DrawProcesser(AssetProcesser assetProcesser)
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new GUILayout.HorizontalScope())
            {
                GUIStyle style = new GUIStyle(EditorExtension.SubtitleStyle);
                style.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(assetProcesser.Name, style);

                GUIStyle buttonStyle = new GUIStyle("button");
                buttonStyle.alignment = TextAnchor.MiddleCenter;
                buttonStyle.margin = new RectOffset(0, 10, 5, 5);

                if (GUILayout.Button("執行", buttonStyle, GUILayout.Height(22)))
                {
                    foreach (var checkAsset in currentCheckObjects)
                    {
                        if (assetProcesser.IsProcessable(checkAsset.asset))
                        {
                            assetProcesser.Process(checkAsset.asset);
                        }

                        DoCheckAsset(checkAsset);
                    }
                }
            }
            

            // 客製化處理器繪製
            assetProcesser.OnGUI();
        }
    }

    private void DrawMainPanel()
    {
        using (new GUILayout.HorizontalScope())
        {
            DrawAssets();
            DrawReport();
        }
    }

    /// <summary>
    /// 列表顯示檢查物件
    /// </summary>
    private void DrawAssets()
    {
        using (new GUILayout.VerticalScope())
        {
            using (assetsScrollViewScope = new GUILayout.ScrollViewScope(assetsScrollPos))
            {
                assetsScrollPos = assetsScrollViewScope.scrollPosition;
                if (checkAssetReorderableList != null)
                {
                    checkAssetReorderableList.Draw();
                }
            }
        }
    }

    #region 檢查報告
    private void DrawReport()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(300), GUILayout.ExpandHeight(true)))
        {
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("檢查報告", EditorExtension.TitleStyle);
            }

            // 檢查是否有選擇的檢查物件
            CheckAsset checkAsset = checkAssetReorderableList.GetFocusingElement();
            if (checkAsset == null)
                return;

            // 顯示檢查物件Icon
            GUIContent content = EditorGUIUtility.ObjectContent(checkAsset.asset, checkAsset.asset.GetType());

            Vector2 iconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(20, 20));

            // 顯示檢查物件名稱
            GUILayout.Label(content, EditorExtension.SubtitleStyle);

            EditorGUIUtility.SetIconSize(iconSize);

            // 顯示檢查結果
            string reportResult = GetReportResult(checkAsset);
            GUILayout.Label(reportResult, EditorExtension.StandrandCenteredLabel);

            // 顯示檢查報告
            using (reportScrollViewScope = new GUILayout.ScrollViewScope(reportScrollPos))
            {
                reportScrollPos = reportScrollViewScope.scrollPosition;

                foreach (var report in checkAsset.reports)
                {
                    using (new GUILayout.VerticalScope("box"))
                    {
                        string label = GetReportLabel(report);
                        GUILayout.Label(label, EditorExtension.StandrandLabel);
                    }
                }
            }
        }
    }

    public const string PassColor = "#9cd023";
    public const string FailColor = "#ff0000";
    private string GetReportResult(CheckAsset checkAsset)
    {
        if (checkAsset.checkResult == CheckResult.Uncheck || checkAsset.reports.Count <= 0)
        {
            return "未檢查";
        }
        else if (checkAsset.checkResult == CheckResult.Pass)
        {
            return $"<color={PassColor}>通過</color>";
        }
        else if (checkAsset.checkResult == CheckResult.Fail)
        {
            int failCount = checkAsset.reports.Count(x => x.IsPass == false);
            return $"<color={FailColor}>失敗 - {failCount} 項未通過</color>";
        }
        return string.Empty;
    }

    private string GetReportLabel(CheckReport report)
    {
        string name = $"[{report.Checker.Name}]";
       
        string message = report.IsPass ? "通過" : report.Message;
        if (report.Message.IsNotNullOrEmpty())
        {
            message = report.Message;
        }
        string color = report.IsPass ? PassColor : FailColor;
        return $"<color={color}>{name}</color> : {message}";
    }
    #endregion
}
