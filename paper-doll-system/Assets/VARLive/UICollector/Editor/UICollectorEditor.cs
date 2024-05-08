using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System.IO;
using System.Linq;

namespace VARLive.Tool
{
    public class EditorUIAsset
    {
        public string displayKey;
        public string realKey;
        public string keyName;
        public string groupName;
        public Object asset;
        public bool isAssetConflict;
        public bool isObjectConflict;
        public string assetConflictMessage;
        public string objectConflictMessage;

        public bool IsVaild()
        {
            return string.IsNullOrEmpty(keyName) == false &&
                   string.IsNullOrEmpty(groupName) == false &&
                   isAssetConflict == false &&
                   isObjectConflict == false;
        }
    }

    public class UICollectorEditor : EditorWindow
    {
        private enum KeyBuildType
        {
            Manual,
            Auto,
            NotUse,
        }

        private enum ParseType
        {
            Children,
            Self,
        }

        private static string[] parseTypeOptions = new string[]
        {
            "包含子物件",
            "僅當前物件"
        };


        private static string[] keyBuildTypeOptions = new string[]
        {
            "手動建置",
            "自動建置",
            "不使用"
        };

        private const string KeyBuildTypePrefs = "UICollector_KeyBuildType";
        private const string ParseTypePrefs = "UICollector_ParseType";

        private UIAssetReorderableList reorderableList;
        private UICollectorSetting setting;
        private GameObject currentRootObj;
        private UICollector currentCollector;
        private GenericMenu groupMenu = new GenericMenu();
        private static KeyBuildType keyBuildType = KeyBuildType.Manual;
        private static ParseType parseType = ParseType.Children;
        private string groupName;
        private static string relativeEditorPath;


        [MenuItem("VAR Live/UI Collector/UI Collector Window %#U")]
        public static void ShowWindow()
        {
            var window = GetWindow<UICollectorEditor>("UI Collector");
            window.Show();
        }

        #region 初始化準備
        private void OnEnable()
        {
            keyBuildType = (KeyBuildType)PlayerPrefs.GetInt(KeyBuildTypePrefs, 0);
            parseType = (ParseType)PlayerPrefs.GetInt(ParseTypePrefs, 0);

            Init();
        }

        private void Init()
        {
            reorderableList = new UIAssetReorderableList(new List<EditorUIAsset>());
            reorderableList.OnRemoveAsset = OnRemoveElement;
            SetupRelativePath();
            SetupSetting();
        }

        private void OnRemoveElement(EditorUIAsset asset)
        {
            RefreshConflictOnRemove();
        }

        private void RefreshConflictOnRemove()
        {
            foreach (var item in reorderableList.UIAssets)
            {
                string error;
                bool isConflict = IsObjectConflict(item.asset, out error) || IsAssetConflict(item, out error);
                item.isAssetConflict = isConflict;
                item.assetConflictMessage = error;
            }
        }

        public static void SetupRelativePath()
        {
            var editor = EditorWindow.GetWindow<UICollectorEditor>();
            var script = MonoScript.FromScriptableObject(editor);
            relativeEditorPath = AssetDatabase.GetAssetPath(script.GetInstanceID());
        }

        private void SetupSetting()
        {
            setting = UICollectorSettingLoader.LoadSetting();
        }
        #endregion

        #region 編輯器介面
        private void OnGUI()
        {
            DragDrop();
            DrawSetting();
            DrawUIAssetList();
            DrawSave();
            DrawBuild();
            RefreshGroupName();
            RefreshConflict();
        }

        private void DrawBuild()
        {
            if (keyBuildType == KeyBuildType.Manual)
            {
                if (GUILayout.Button("建置設定檔", GUILayout.Height(25)))
                {
                    BuildScript(setting);
                }
            }
        }

        private void DrawSetting()
        {
            GUILayout.BeginVertical("Box");
            {
                var newRoot = EditorGUILayout.ObjectField("目前編輯物件:", currentRootObj, typeof(GameObject), true) as GameObject;
                if (newRoot != currentRootObj)
                {
                    ParseObject(newRoot, true);
                }

                GUILayout.BeginHorizontal();
                groupName = EditorGUILayout.TextField("群組名稱:", groupName);
                if (GUILayout.Button("選擇群組", GUILayout.Width(100)))
                {
                    RefreshGroupMenu();
                    groupMenu.ShowAsContext();
                }
                GUILayout.EndHorizontal();
                var newBuildType = (KeyBuildType)EditorGUILayout.Popup("建置方式:", (int)keyBuildType, keyBuildTypeOptions);
                if (keyBuildType != newBuildType)
                {
                    keyBuildType = newBuildType;
                    PlayerPrefs.SetInt(KeyBuildTypePrefs, (int)keyBuildType);
                }
                var newParseType = (ParseType)EditorGUILayout.Popup("分析模式:", (int)parseType, parseTypeOptions);
                if (parseType != newParseType)
                {
                    parseType = newParseType;
                    PlayerPrefs.SetInt(ParseTypePrefs, (int)parseType);
                }
            }
            GUILayout.EndVertical();
        }

        private void RefreshGroupMenu()
        {
            groupMenu = new GenericMenu();
            foreach (var group in setting.groupNames)
            {
                bool isOn = groupName == group;
                groupMenu.AddItem(new GUIContent(group), isOn, OnSelectGroup, group);
            }
        }

        private void OnSelectGroup(object group)
        {
            string selectGroup = group.ToString();
            if (string.IsNullOrEmpty(selectGroup) == false)
            {
                groupName = selectGroup;
            }
        }

        private void DrawUIAssetList()
        {
            reorderableList.OnDrawGUI();
            if (reorderableList.UIAssets.Count <= 0)
            {
                currentRootObj = null;
            }
        }

        private void RefreshGroupName()
        {
            foreach (var item in reorderableList.UIAssets)
            {
                item.groupName = groupName;
                item.displayKey = $"{item.groupName}.{item.keyName}";
                item.realKey = $"{ProcessGroupName(item.groupName)}_{item.keyName}";
            }
        }

        string[] prohibitKeys = new string[] { " ", "-", "(", ")", "." };
        private string ProcessGroupName(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return string.Empty;

            for (int j = 0; j < prohibitKeys.Length; j++)
            {
                groupName = groupName.Replace(prohibitKeys[j], "_");
            }
            return groupName;
        }

        private void RefreshConflict()
        {
            foreach (var item in reorderableList.UIAssets)
            {
                string error;
                bool isConflict = IsAssetConflict(item, out error);
                item.isAssetConflict = isConflict;
                item.assetConflictMessage = error;
            }
        }

        private void DrawSave()
        {
            GUILayout.BeginHorizontal();
            {
                GUI.enabled = currentRootObj != null && IsHasConflict() == false;
                if (GUILayout.Button("儲存", GUILayout.Height(30)))
                {
                    if (currentCollector == null)
                    {
                        CreateNewCollector();
                    }
                    if (string.IsNullOrEmpty(currentCollector.guid))
                    {
                        CreateNewGUID();
                    }
                    SaveUIAsset();
                    SaveSetting();
                    if (keyBuildType == KeyBuildType.Auto)
                    {
                        BuildScript(setting);
                    }
                    AssetDatabase.SaveAssets();
                }
                if (GUILayout.Button("另存", GUILayout.Height(30), GUILayout.Width(150)))
                {
                    CreateNewCollector();
                    if (string.IsNullOrEmpty(currentCollector.guid))
                    {
                        CreateNewGUID();
                    }
                    SaveUIAsset();
                    SaveSetting();
                    if (keyBuildType == KeyBuildType.Auto)
                    {
                        BuildScript(setting);
                    }
                    AssetDatabase.SaveAssets();
                }
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
        }

        private bool IsHasConflict()
        {
            if (reorderableList.UIAssets == null)
            {
                return false;
            }
            if (reorderableList.UIAssets.Count <= 0)
            {
                return false;
            }
            if (reorderableList.UIAssets.Any(p => p.IsVaild() == false))
            {
                return true;
            }
            if (reorderableList.UIAssets.Any(p => p.isAssetConflict))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 存檔
        private void CreateNewCollector()
        {
            currentCollector = currentRootObj.AddComponent<UICollector>();
        }

        private void CreateNewGUID()
        {
            CreateNewGUID(currentCollector);
        }

        public static void CreateNewGUID(UICollector uiCollector)
        {
            uiCollector.guid = GUID.Generate().ToString();
        }

        private void SaveSetting()
        {
            setting.AddGroup(groupName);
            List<string> keys = new List<string>();
            reorderableList.UIAssets.ForEach(p =>
            {
                keys.Add(p.realKey);
            });
            //如果遇到舊版本使用collectoriD，要更新成新版本使用GUID
            if (currentCollector.collectorID >= 0)
            {
                setting.UpdateKeys(currentCollector.collectorID, currentCollector.guid, keys);
            }
            else
            {
                setting.RefreshKeys(currentCollector.guid, keys);
            }
            setting.Refresh();
            EditorUtility.SetDirty(setting);
        }

        private void SaveUIAsset()
        {
            currentCollector.UIAsset.Clear();
            foreach (var editorAsset in reorderableList.UIAssets)
            {
                UIAsset uiAsset = ParseToUIAsset(editorAsset);
                currentCollector.UIAsset.Add(uiAsset);
            }
            currentCollector.groupName = groupName;
            EditorUtility.SetDirty(currentCollector);
        }

        private UIAsset ParseToUIAsset(EditorUIAsset editorUIAsset)
        {
            UIAsset asset = new UIAsset()
            {
                groupName = editorUIAsset.groupName,
                keyName = editorUIAsset.keyName,
                realKey = editorUIAsset.realKey,
                asset = editorUIAsset.asset
            };
            return asset;
        }

        private EditorUIAsset ParseToEditorUIAsset(UIAsset uiAsset)
        {
            EditorUIAsset editorUIAsset = new EditorUIAsset()
            {
                groupName = uiAsset.groupName,
                keyName = uiAsset.keyName,
                realKey = uiAsset.realKey,
                asset = uiAsset.asset,
                displayKey = $"{uiAsset.groupName}.{uiAsset.keyName}"
            };

            return editorUIAsset;
        }

        public static void BuildScript(UICollectorSetting setting)
        {
            CreateConstSctipt("UIKey", setting.keys);
            AssetDatabase.Refresh();
        }

        private const string NamespaceName = "VARLive.Tool";
        private static void CreateConstSctipt(string className, List<string> keys)
        {
            GeneratorClassSetting classSetting = new GeneratorClassSetting();

            classSetting.drawSerializableAttribute = false;
            classSetting.isStaticClass = true;
            classSetting.className = className;

            ConstKeyTableGenerator tableKeyData = new ConstKeyTableGenerator(keys, null);

            ClassGenerator classData = new ClassGenerator(classSetting, new List<ScriptGenerator>() { tableKeyData });

            NamespaceGenerator namespaceData = new NamespaceGenerator(new List<string>(), NamespaceName, new List<ScriptGenerator>() { classData });

            var lines = namespaceData.GetGeneratorLines();

            string path = GetSaveScriptPath(className);

            if (File.Exists(path) == false)
            {
                var file = File.Create(path);
                file.Close();
            }

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                lines.ForEach(line =>
                {
                    writer.WriteLine(line);
                });
            }
        }

        private static string GetSaveScriptPath(string className)
        {
            string path = GetSettingPath();
            string filePath = Path.Combine(path, $"{className}.cs");
            return filePath;
        }

        private static string GetSettingPath()
        {
            string editorPath = Path.GetDirectoryName(relativeEditorPath);
            DirectoryInfo relativeInfo = Directory.GetParent(editorPath);
            string path = Path.Combine(relativeInfo.ToString(), "Setting");
            return path;
        }
        #endregion

        #region 分析拖曳加入的物件
        private void DragDrop()
        {
            bool isHoldAlt = false;
            Event e = Event.current;
            if (e.alt)
            {
                isHoldAlt = true;
            }

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                    }
                    break;
                case EventType.DragPerform:
                    {
                        DragAndDrop.AcceptDrag();
                        if (isHoldAlt == false)
                        {
                            ParseDragObjects(DragAndDrop.objectReferences);
                        }
                        else
                        {
                            //如果是按住Alt拖入物件，直接加入物件
                            AddDragObjects(DragAndDrop.objectReferences);
                        }
                    }
                    break;
            }
        }

        private void AddDragObjects(Object[] objects)
        {
            foreach (var item in objects)
            {
                AddObject(item);
            }
        }

        private void ParseDragObjects(Object[] objects)
        {
            foreach (var item in objects)
            {
                if (item is UICollector)
                {
                    //如果拖入的物件中有複數的UICollector，只會處理第一個(理論上不可能發生，除非Unity支援複選Component)
                    ApplyCollector(item as UICollector);
                    break;
                }
                else if (item is Component)
                {
                    AddComponentAsset((item as Component));
                }
                else if (item is GameObject)
                {
                    ParseObject((item as GameObject));
                }
                else
                {
                    //你到底丟了甚麼進來
                    AddObject(item);
                }
            }
        }

        private void ParseObject(GameObject gameObject, bool forceRefresh = false)
        {
            //如果加入的物件身上有掛UICollector，不管怎樣都是當作加入腳本處理
            var collector = gameObject.GetComponent<UICollector>();
            if (collector != null)
            {
                ApplyCollector(collector);
                return;
            }
            //強制刷新根物件
            if (forceRefresh)
            {
                ApplyNewRoot(gameObject);
                return;
            }
            //沒有正在編輯的根物件，刷新根物件
            if (currentRootObj == null)
            {
                ApplyNewRoot(gameObject);
                return;
            }
            //重複拖入根物件，用加入物件處理
            if (currentRootObj == gameObject)
            {
                ApplyNewAsset(gameObject);
                return;
            }

            //拖曳加入的物件是子物件的話，用加入物件處理，不是的話刷新根物件
            bool isChild = IsChild(currentRootObj.transform, gameObject.transform);
            if (isChild)
            {
                ApplyNewAsset(gameObject);
            }
            else
            {
                ApplyNewRoot(gameObject);
            }
        }

        private void ApplyNewRoot(GameObject newRoot)
        {
            //一般情況下這裡都會回傳Null，只有少數狀況會抓的到東西，Ex 使用者自己掛了UICollector但又沒給他編資料
            currentCollector = newRoot.GetComponent<UICollector>();
            currentRootObj = newRoot;
            RefreshUIAssets(newRoot);
            AddObject(newRoot);
        }

        private void ApplyNewAsset(GameObject newAsset)
        {
            AddUIAssets(newAsset);
            AddObject(newAsset);
        }

        /// <summary>
        /// 編輯已經存在的UICollector
        /// </summary>
        public void ApplyCollector(UICollector collector)
        {
            LoadUIAsset(collector);
            currentRootObj = collector.gameObject;
            groupName = collector.groupName;
            currentCollector = collector;
        }

        private void LoadUIAsset(UICollector collector)
        {
            if (collector.UIAsset.Count <= 0)
            {
                Debug.LogWarning("偵測到空的UICollector，自動建置資料");
                ApplyNewRoot(collector.gameObject);
                return;
            }

            reorderableList.UIAssets.Clear();
            foreach (var uiAsset in collector.UIAsset)
            {
                EditorUIAsset editorUIAsset = ParseToEditorUIAsset(uiAsset);
                reorderableList.UIAssets.Add(editorUIAsset);
            }
        }

        private bool IsChild(Transform root, Transform target)
        {
            if (root.childCount <= 0)
                return false;

            foreach (Transform child in root)
            {
                if (child == target)
                {
                    return true;
                }
                if (child.childCount > 0)
                {
                    if (IsChild(child, target))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 根據類型加入物件
        private void RefreshUIAssets(GameObject gameObject)
        {
            reorderableList.UIAssets.Clear();
            GetAndAddComponentAssets<UIBehaviour>(gameObject);
        }

        private void AddUIAssets(GameObject gameObject)
        {
            GetAndAddComponentAssets<UIBehaviour>(gameObject);
        }

        private void GetAndAddComponentAssets<T>(GameObject gameObject) where T : Component
        {
            T[] assets = null;
            if (parseType == ParseType.Children)
            {
                assets = gameObject.GetComponentsInChildren<T>();
            }
            if (parseType == ParseType.Self)
            {
                assets = gameObject.GetComponents<T>();
            }
            if (assets != null)
            {
                foreach (var item in assets)
                {
                    AddComponentAsset(item);
                }
            }
        }

        private void AddComponentAsset(Component asset)
        {
            AddAsset(asset.gameObject.name, asset);
        }

        private void AddObject(Object asset)
        {
            AddAsset(asset.name, asset);
        }

        private void AddAsset(string key, Object asset)
        {
            if (reorderableList.UIAssets.Exists(p => p.asset == asset) == false)
            {
                EditorUIAsset uiAsset = new EditorUIAsset()
                {
                    groupName = groupName,
                    keyName = key,
                    asset = asset,
                };

                uiAsset.isObjectConflict = IsObjectConflict(asset, out string objError);
                uiAsset.objectConflictMessage = objError;
                uiAsset.isAssetConflict = IsAssetConflict(uiAsset, out string assetError);
                uiAsset.assetConflictMessage = assetError;

                reorderableList.UIAssets.Add(uiAsset);
            }
        }
        #endregion

        #region 物件衝突分析
        private bool IsObjectConflict(Object asset, out string error)
        {
            if (asset == null)
            {
                error = "偵測到物件不存在";
                return true;
            }

            GameObject testObj = null;
            if (asset is GameObject)
            {
                testObj = asset as GameObject;

            }
            else if (asset is Component)
            {
                testObj = (asset as Component).gameObject;
            }
            else
            {
                //有奇怪的東西混進來了，無法處理先讓你過吧
                error = string.Empty;
                return false;
            }

            if (currentRootObj != null && testObj != null)
            {
                if (currentRootObj.transform == testObj.transform)
                {
                    error = string.Empty;
                    return false;
                }

                bool isChild = IsChild(currentRootObj.transform, testObj.transform);
                bool isParent = IsChild(testObj.transform, currentRootObj.transform);
                if (isChild == false && isParent == false)
                {
                    error = "偵測到額外的物件";
                    return true;
                }
            }
            error = string.Empty;
            return false;
        }

        private bool IsAssetConflict(EditorUIAsset uiAsset, out string error)
        {
            if (uiAsset.asset == null)
            {
                error = "偵測到物件不存在";
                return true;
            }
            foreach (var asset in reorderableList.UIAssets)
            {
                //比對Key重複的時候，如果是自己就跳過
                if (asset == uiAsset)
                {
                    continue;
                }
                if (asset.keyName == uiAsset.keyName)
                {
                    error = "偵測到重複的Key";
                    return true;
                }
            }

            error = string.Empty;
            return false;
        }
        #endregion
    }
}