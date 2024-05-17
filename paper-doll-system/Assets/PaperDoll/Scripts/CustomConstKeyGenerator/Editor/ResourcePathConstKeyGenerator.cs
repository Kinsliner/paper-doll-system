using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ez.Tool;

public class ResourcePathConstKeyGenerator : ConstKeysGenerator
{
    [MenuItem("Assets/Get Resource Path")]
    private static void GetResourcePath()
    {
        // get asset path
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        // 去掉開頭的 Assets/Resources/
        string processPath = RemoveStart(path, "Assets/Resources/");

        // 去掉副檔名
        processPath = RemoveEnd(processPath, ".prefab");

        // 複製路徑
        GUIUtility.systemCopyBuffer = processPath;

        // Log
        Debug.Log("已複製路徑到剪貼簿: " + processPath);
    }

    [MenuItem("Assets/Get Resource Path", true)]
    private static bool GetResourcePathValidation()
    {
        // get asset path
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        // 只有選擇的路徑在Resource底下和是prefab才會顯示
        return path.StartsWith("Assets/Resources/") && path.EndsWith(".prefab");
    }

    public override string GetName()
    {
        return "更新Resource的路徑";
    }

    public override void Generate()
    {
        List<string> resourcePaths = new List<string>();
        string[] paths = GetAllResourcePaths();
        foreach (var path in paths)
        {
            // 只取得有檔案的路徑
            if (File.Exists(path))
            {
                // 去掉開頭的 Assets/Resources/
                string processPath = RemoveStart(path, "Assets/Resources/");

                // 去掉副檔名
                processPath = RemoveEnd(processPath, ".prefab");

                resourcePaths.Add(processPath);

                Debug.Log(processPath);
            }
        }

        CreateConstSctipt("ResourcePaths", resourcePaths);
    }

    private static string RemoveEnd(string processPath, string v)
    {
        if (processPath.EndsWith(v))
        {
            return processPath.Substring(0, processPath.Length - v.Length);
        }
        else
        {
            return processPath;
        }
    }

    private static string RemoveStart(string path, string startPath)
    {
        return path.Substring(startPath.Length, path.Length - startPath.Length);
    }

    // 取得所有 "Resources" 資料夾下的資源路徑
    private string[] GetAllResourcePaths()
    {
        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/Resources" });
        string[] paths = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }

        return paths;
    }
}
