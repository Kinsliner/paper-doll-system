using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace VARLive.Build
{
	public class PathSettingPopup : PopupWindowContent
    {
        public CopyType copyType;
        public string customPath;
        public string customFileName;
        public bool overwrite = true;
        public bool recursiveCopy = true;

        private string sourcePath;
        private string relativePath;

        public override Vector2 GetWindowSize()
        {
            int addHeight = GetHeight();

            return new Vector2(280, 110 + addHeight);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Copy to :", EditorStyles.boldLabel);

            string showPath = GetShowPath();

            GUILayout.Label(string.Format(showPath), EditorStyles.helpBox);
            customPath = EditorGUILayout.TextField("Custom Path", customPath);
            customFileName = EditorGUILayout.TextField("Custom Name", customFileName);
            overwrite = EditorGUILayout.Toggle("Overwrite", overwrite);
            if (copyType == CopyType.Folder)
                recursiveCopy = EditorGUILayout.Toggle("Recursive", recursiveCopy);
        }

        private int GetHeight()
        {
            int result = 0;
            string showPath = GetShowPath();

            int length = showPath.Length;
            float add = (float)length / 40;
            if (add > 1.0f)
            {
                result = Mathf.FloorToInt(add) * 15;
            }
            if(copyType == CopyType.Folder)
            {
                result += 20;
            }
            return result;
        }

        private string GetShowPath()
        {
            string showPath = "[destination]/";
            string addPath = string.Empty;

            addPath = GetSaveFilePath();

            return showPath + addPath;
        }

        public string GetSaveFilePath()
        {
            string savePath;

            if (string.IsNullOrEmpty(sourcePath))
            {
                savePath = "[Un select]";
            }

            string fileName = GetSourceFileName();
            savePath = fileName;

            if (string.IsNullOrEmpty(customPath) == false)
            {
                if (string.IsNullOrEmpty(fileName) == false)
                {
                    savePath = string.Format("{0}/{1}", customPath, fileName);
                }
                else
                {
                    savePath = customPath;
                }
            }

            return savePath;
        }

        public string GetSourceFileName()
        {
            string fileName = string.Empty;
            string extension = string.Empty;

            if (string.IsNullOrEmpty(sourcePath))
            {
                fileName = "[Un select]";
            }
            else
            {
                string fullPath = Path.GetFullPath(sourcePath).TrimEnd(Path.DirectorySeparatorChar);
                fileName = fullPath.Split(Path.DirectorySeparatorChar).Last();
                extension = Path.GetExtension(fileName);
            }

            if (string.IsNullOrEmpty(customFileName) == false)
            {
                fileName = customFileName + extension;
            }

            return fileName;
        }

        public string RemovePathNode(string path, int nodeAmount)
        {
            List<string> filePaths = path.Split('/').ToList();

            for (int i = 0; i < nodeAmount; i++)
            {
                filePaths.RemoveAt(filePaths.Count - 1);
            }

            string resultPath = string.Empty;

            for (int i = 0; i < filePaths.Count; i++)
            {
                resultPath += filePaths[i] + "/";
            }

            return resultPath;
        }

        public void SetSourcePath(string path)
        {
            sourcePath = path;
        }
    }
}