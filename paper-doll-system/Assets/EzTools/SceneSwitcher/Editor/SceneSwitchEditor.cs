using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using UnityEngine.SceneManagement;

namespace Ez.Tool
{
    public class SceneSwitchEditor : EditorWindow
    {
        private List<string> scenes = new List<string>();

        [MenuItem("Tools/Scene Switcher %W")]
        public static void ShowWindow()
        {
            GetWindow<SceneSwitchEditor>("Scenewitcher");
        }

        private void OnEnable()
        {
            ApplyBuildSetting();
        }

        private void OnGUI()
        {
            DrawToolBar();
            DrawSceneButtons();
            DragDrop();
        }

        #region 工具列
        private const string ToolBarStyle = "Toolbar";
        private const string ToolBarButtonStyle = "toolbarbutton";
        private void DrawToolBar()
        {
            GUILayout.BeginHorizontal(ToolBarStyle, GUILayout.Width(position.width));
            {
                if (GUILayout.Button("Apply Build Setting", ToolBarButtonStyle, GUILayout.Width(200)))
                {
                    ApplyBuildSetting();
                }
                if (GUILayout.Button("Join Open Scene", ToolBarButtonStyle, GUILayout.Width(150)))
                {
                    JoinOpenScene();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void JoinOpenScene()
        {
            var openScenes = GetOpenScenes();
            foreach (var sc in openScenes)
            {
                AddScene(sc.path);
            }
        }

        private List<Scene> GetOpenScenes()
        {
            List<Scene> openScenes = new List<Scene>();
            for (int i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                openScenes.Add(scene);
            }
            return openScenes;
        }

        private void ApplyBuildSetting()
        {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var sceneSetting = EditorBuildSettings.scenes[i];
                AddScene(sceneSetting.path);
            }
        }

        private void AddScene(string scenePath)
        {
            if (scenes.Contains(scenePath) == false)
            {
                scenes.Add(scenePath);
            }
        }
        #endregion

        #region 場景切換
        private Vector2 scrollPos;
        private void DrawSceneButtons()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleLeft;

            using (var scrollView = new GUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scrollView.scrollPosition;
                for (int i = 0; i < scenes.Count; i++)
                {
                    string scene = scenes[i];
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(scene, style))
                        {
                            if (IsNeedSave(out List<Scene> dirtyScenes))
                            {
                                AskOpenScene(dirtyScenes, scene);
                            }
                            else
                            {
                                OpenScene(scene);
                            }
                        }
                        if (GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            EditorSceneManager.OpenScene(scene, OpenSceneMode.Additive);
                        }
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            scenes.Remove(scene);
                            break;
                        }
                    }
                }
            }
        }

        private bool IsNeedSave(out List<Scene> dirtyScenes)
        {
            dirtyScenes = GetOpenScenes();
            dirtyScenes.RemoveAll(s => s.isDirty == false);
            return dirtyScenes.Count > 0;
        }

        private void AskOpenScene(List<Scene> dirtyScenes, string openScene)
        {
            //0 - ok, 1 - cancel, 2 - alt
            int choose = EditorUtility.DisplayDialogComplex("即將切換場景", GetAskText(dirtyScenes), "儲存", "取消", "不儲存");
            switch (choose)
            {
                case 0:
                    SvaeScenes();
                    OpenScene(openScene);
                    break;
                case 2:
                    OpenScene(openScene);
                    break;
            }
        }

        private string GetAskText(List<Scene> dirtyScenes)
        {
            if (dirtyScenes.Count == 1)
            {
                return $"偵測到 {dirtyScenes[0].path} 有變更，要儲存嗎?";
            }
            else
            {
                string sceneListText = string.Empty;
                foreach (var sc in dirtyScenes)
                {
                    sceneListText += sc.path + "\n";
                }
                return $"偵測到以下場景有變更\n{sceneListText}\n要儲存嗎?";
            }
        }

        private void OpenScene(string scene)
        {
            EditorSceneManager.OpenScene(scene);
        }

        private void SvaeScenes()
        {
            EditorSceneManager.SaveOpenScenes();
        }
        #endregion

        #region 拖曳加入場景
        private void DragDrop()
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:

                    if (IsAcceptDrag(DragAndDrop.objectReferences))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    Event.current.Use();

                    break;
                case EventType.DragPerform:

                    if (IsAcceptDrag(DragAndDrop.objectReferences))
                    {
                        DragAndDrop.AcceptDrag();
                        ParseDargInObjects(DragAndDrop.objectReferences);
                    }

                    break;
            }
        }

        private bool IsAcceptDrag(UnityEngine.Object[] objectReferences)
        {
            foreach (var obj in objectReferences)
            {
                if (obj is SceneAsset)
                {
                    return true;
                }
            }
            return false;
        }

        private void ParseDargInObjects(UnityEngine.Object[] objectReferences)
        {
            foreach (var obj in objectReferences)
            {
                if (obj is SceneAsset sceneAsset)
                {
                    string path = AssetDatabase.GetAssetPath(sceneAsset);
                    AddScene(path);
                }
            }
        }
        #endregion
    }
}