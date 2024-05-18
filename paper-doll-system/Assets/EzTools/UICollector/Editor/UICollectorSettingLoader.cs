using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ez.Tool
{
    public static class UICollectorSettingLoader
    {
        public static UICollectorSetting LoadSetting()
        {
            string folderPath = GetSettingFolderPath();
            string filePath = GetSettingPath();
            if (File.Exists(filePath) == false)
            {
                if (Directory.Exists(folderPath) == false)
                {
                    Directory.CreateDirectory(folderPath);
                }

                var setting = ScriptableObject.CreateInstance<UICollectorSetting>();
                AssetDatabase.CreateAsset(setting, filePath);
                AssetDatabase.SaveAssets();
            }

            return AssetDatabase.LoadAssetAtPath<UICollectorSetting>(filePath);
        }

        private static string GetSettingPath()
        {
            string path = GetSettingFolderPath();
            string assetPath = Path.Combine(path, "UICollectorSetting.asset");
            return assetPath;
        }

        private static string GetSettingFolderPath()
        {
            string editorRelativePath = GetEditorRelativePath();
            string editorFolderPath = Path.GetDirectoryName(editorRelativePath);

            DirectoryInfo dir = new DirectoryInfo(editorFolderPath);
            DirectoryInfo replaceDir = new DirectoryInfo(Application.dataPath);
            string dirPath = dir.Parent.ToString();
            string targetPath = dirPath.Replace(replaceDir.Parent.ToString(), "");
            targetPath = targetPath.TrimStart('\\');

            string result = Path.Combine(targetPath, "Setting");
            return result;
        }

        private static string GetEditorRelativePath()
        {
            var editor = EditorWindow.GetWindow<UICollectorEditor>();
            var script = MonoScript.FromScriptableObject(editor);
            return AssetDatabase.GetAssetPath(script.GetInstanceID());
        }
    }
}

