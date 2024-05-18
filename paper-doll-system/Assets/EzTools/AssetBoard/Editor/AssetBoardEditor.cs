using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using Object = UnityEngine.Object;

public class AssetBoardEditor : EditorWindowExtender
{
    private List<Object> assets = new List<Object>();
    private GUIStyle assetContentStyle;
    private GUILayout.ScrollViewScope scrollViewScope;
    private Vector2 scrollPos;

    [MenuItem("Tools/Asset Board %Q")]
    public static void ShowWindow()
    {
        var window = CreateInstance<AssetBoardEditor>();
        window.titleContent = new GUIContent("Asset Board");
        window.Show();
    }

    private void OnEnable() => Selection.selectionChanged += Repaint;

    private void OnDisable() => Selection.selectionChanged -= Repaint;

    private void OnGUI()
    {
        DrawToolBar();
        DrawAssets();
        DragDrop();
    }

    private void DrawToolBar()
    {
        using (new GUILayout.HorizontalScope("toolbar"))
        {
            using (new EditorGUI.DisabledGroupScope(Selection.objects.Length == 0))
            {
                if (GUILayout.Button("Apply Selection", "toolbarbutton"))
                {
                    OnDragDropObjects(Selection.objects);
                }
            }
            if (GUILayout.Button("Command", "toolbarbutton", GUILayout.Width(80)))
            {
                ShowCommands();
            }
            if (GUILayout.Button("Clear", "toolbarbutton", GUILayout.Width(80)))
            {
                if (assets.Count <= 0) return;

                if (EditorUtility.DisplayDialog("注意", "將會清空面板，確定嗎?", "確認", "取消"))
                {
                    assets.Clear();
                }
            }
        }
    }

    protected override void OnDragDropObjects(Object[] objectReferences)
    {
        foreach (var obj in objectReferences)
        {
            if (assets.Contains(obj) == false)
            {
                assets.Add(obj);
            }
        }
    }

    private void DrawAssets()
    {
        assetContentStyle = new GUIStyle("Button")
        {
            alignment = TextAnchor.MiddleLeft,
        };
        using (scrollViewScope = new GUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = scrollViewScope.scrollPosition;
            using (new GUILayout.VerticalScope())
            {
                for (int i = 0; i < assets.Count; i++)
                {
                    Object asset = assets[i];

                    if (assets[i] == null)
                    {
                        assets.Remove(asset);
                        break;
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        var content = EditorGUIUtility.ObjectContent(asset, asset.GetType());
                        if (GUILayout.Button(content, assetContentStyle, GUILayout.MinWidth(100), GUILayout.Height(20)))
                        {
                            HighlightObject(asset);
                        }
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            assets.Remove(asset);
                            break;
                        }
                    }

                }
            }
        }
    }

    private void ShowCommands()
    {
        List<AssetBoardCommand> commands = GetCommands();
        GenericMenu commandMenu = new GenericMenu();
        foreach (var command in commands)
        {
            string commandName = command.CommandName;
            commandMenu.AddItem(new GUIContent(commandName), false, OnCommand, commandName);
        }
        commandMenu.ShowAsContext();
    }

    private List<AssetBoardCommand> GetCommands()
    {
        List<AssetBoardCommand> commands = new List<AssetBoardCommand>();

        // 獲取所有已載入的組件
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 遍歷所有組件，尋找 AssetBoardCommand 的子類別
        foreach (var assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            Type[] possible = (from Type type in types where type.IsSubclassOf(typeof(AssetBoardCommand)) select type).ToArray();

            foreach (var type in possible)
            {
                var command = Activator.CreateInstance(type) as AssetBoardCommand;
                commands.Add(command);
            }
        }
        return commands;
    }

    private void OnCommand(object commandName)
    {
        List<AssetBoardCommand> commands = GetCommands();
        foreach (var command in commands)
        {
            if (command.CommandName == commandName.ToString())
            {
                command.Execute(assets);
            }
        }
    }
}