using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

namespace Ez.Build
{
	public class BuildProcess
	{
		private static BuildToolSetting currentSetting;

		#region Build

		internal static void Build(BuildToolSetting buildToolSetting, string outputPath, BuildTool.BuildTypeData buildType)
		{
			currentSetting = buildToolSetting;
			string outputFolderPath = GetBuildFolderPath(outputPath, buildType);

			CreateOuputPath(outputFolderPath);

			BuildGame(outputFolderPath, buildType);
		}

		private static void BuildGame(string outputPath, BuildTool.BuildTypeData buildType)
		{
			string _oldDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, buildType.defines);

			bool vrSupport = PlayerSettings.virtualRealitySupported;
			if (buildType.supportVR)
				SetVREnable();
			else
				SetVRDisable();

			PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
			if (buildType.isCustomScenes)
				buildPlayerOptions.scenes = GetAllScenePath(buildType);
			else
				buildPlayerOptions.scenes = GetAllScenePath();
			buildPlayerOptions.locationPathName = GetBuildPath(outputPath, buildType);
			buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
			buildPlayerOptions.assetBundleManifestPath = currentSetting.assetBundleManifestPath;

			if (currentSetting.developmentMode)
				buildPlayerOptions.options = BuildOptions.Development | BuildOptions.ShowBuiltPlayer | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
			else
				buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer;

#if UNITY_2018_1_OR_NEWER
			UnityEditor.Build.Reporting.BuildReport buildResult = BuildPipeline.BuildPlayer(buildPlayerOptions);
			if (buildResult.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
			{
				Debug.LogFormat("Build Successfully!! Build Time:{0}", buildResult.summary.totalTime.TotalSeconds);
			}
			else
			{
				Debug.LogErrorFormat("Build Failed!! Build Time:{0}", buildResult.summary.totalTime.TotalSeconds);
			}
#else
			string error = BuildPipeline.BuildPlayer(buildPlayerOptions);
			if(string.IsNullOrEmpty(error) == false)
            {
				Debug.LogErrorFormat("Build Failed!! Build Error:{0}", error);
			}
#endif

			PlayerSettings.virtualRealitySupported = vrSupport;
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _oldDefine);
			AssetDatabase.SaveAssets();
		}

		public static string[] GetAllScenePath()
		{
			List<string> scenesPath = new List<string>();

			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				if (EditorBuildSettings.scenes[i].enabled)
				{
					scenesPath.Add(EditorBuildSettings.scenes[i].path);
				}
			}

			return scenesPath.ToArray();
		}

		public static string[] GetAllScenePath(BuildTool.BuildTypeData buildType)
		{
			List<string> scenesPath = new List<string>();
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				if (EditorBuildSettings.scenes[i].enabled)
				{
					scenesPath.Add(EditorBuildSettings.scenes[i].path);
				}
			}

			List<string> validPath = new List<string>();
			for (int i = 0; i < buildType.customScenes.Count; i++)
			{
				if (buildType.customScenes[i].enable == false) continue;
				string path = buildType.customScenes[i].scenePath;
				if (scenesPath.Contains(path))
				{
					validPath.Add(path);
				}
			}

			return validPath.ToArray();
		}

#endregion

#region Build Path

		public static void CreateOuputPath(string outputPath)
		{
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}
		}

		private static string GetBuildPath(string outputPath, BuildTool.BuildTypeData buildType)
		{
			string filePath = buildType.name;
			if(string.IsNullOrEmpty(buildType.customFileName) == false)
			{
				filePath = buildType.customFileName;
			}

			return Path.Combine(outputPath, string.Format("{0}.exe", filePath));
		}

		private static string GetBuildFolderPath(string outputPath, BuildTool.BuildTypeData buildType)
		{
			string buildPath = buildType.name;
			if (string.IsNullOrEmpty(buildType.customBuildPath) == false)
			{
				buildPath = buildType.customBuildPath;
			}

			return Path.Combine(outputPath, buildPath);
		}

		private static string GetFilePathFromProject(string fileName)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath);

			string filePath = Path.Combine(dirInfo.Parent.FullName, fileName);
			return filePath;
		}

#endregion

#region VR Setting

		private static void SetVREnable()
		{
			PlayerSettings.virtualRealitySupported = true;
		}

		private static void SetVRDisable()
		{
			PlayerSettings.virtualRealitySupported = false;
		}

#endregion

#region Copy File

		internal static void CopyData(List<BuildTool.CopyData> copyFiles, List<BuildTool.CopyData> copyFolders, BuildToolSetting buildToolSetting, string outputPath, BuildTool.BuildTypeData buildType, Action<string, int, int> Progress = null)
		{
			currentSetting = buildToolSetting;
			string outputFolderPath = GetBuildFolderPath(outputPath, buildType);

			CreateOuputPath(outputFolderPath);

			ProcessCopyFolders(copyFolders, outputFolderPath, buildType.buildIndex, Progress);
			ProcessCopyFiles(copyFiles, outputFolderPath, buildType.buildIndex, Progress);
		}

		private static void ProcessCopyFolders(List<BuildTool.CopyData> copyDatas, string savePath, int buildTypeIndex, Action<string, int, int> Progress = null)
		{
			if (!Directory.Exists(savePath))
				Directory.CreateDirectory(savePath);

			for (int i = 0; i < copyDatas.Count; i++)
			{
				if (string.IsNullOrEmpty(copyDatas[i].selectPath))
					continue;

				if (IsNeedCopy(copyDatas[i], buildTypeIndex))
				{
					CopyFolder(copyDatas[i], savePath, Progress);
				}
			}
		}

		private static void ProcessCopyFiles(List<BuildTool.CopyData> copyDatas, string savePath, int buildTypeIndex, Action<string, int, int> Progress = null)
		{
			if (!Directory.Exists(savePath))
				Directory.CreateDirectory(savePath);

			List<BuildTool.CopyData> needCopyDatas = new List<BuildTool.CopyData>();
			for (int i = 0; i < copyDatas.Count; i++)
			{
				if (string.IsNullOrEmpty(copyDatas[i].selectPath))
					continue;

				if (IsNeedCopy(copyDatas[i], buildTypeIndex))
				{
					needCopyDatas.Add(copyDatas[i]);
				}
			}

			for (int i = 0; i < needCopyDatas.Count; i++)
			{
				string fileName = needCopyDatas[i].settingPopup.GetSourceFileName();
				string title = string.Format("Copying {0}", fileName);
				if (Progress != null) Progress.Invoke(title, i + 1, needCopyDatas.Count);

				CopyFile(needCopyDatas[i], savePath, Progress);
			}
		}

		private static bool IsNeedCopy(BuildTool.CopyData copyData, int checkIndex)
		{
			if (copyData.isNeedCopy == false) return false;

			int flagValue = copyData.buildTypeMaskPopup.GetFlag();
			BitArray flags = new BitArray(new int[] { flagValue });

			return flags[checkIndex];
		}

		private static void CopyFolder(BuildTool.CopyData copyData, string savePath, Action<string, int, int> Progress = null)
		{
			bool overwrite = copyData.settingPopup.overwrite;
			bool isRecursive = copyData.settingPopup.recursiveCopy;
			string sourcePath = copyData.selectPath;
			string outputPath = GetOutputPath(savePath, copyData, CopyType.Folder);

			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);

			if (Directory.Exists(sourcePath))
			{
				DirectoryInfo dir = new DirectoryInfo(sourcePath);

				if (isRecursive)
					RecursiveCopyFolder(dir, outputPath, overwrite, Progress);
				else
					CopyOneFolder(dir, outputPath, overwrite, Progress);
			}
			else
			{
				Debug.LogErrorFormat("複製的資料夾路徑錯誤，請檢察{0}路徑是否正確", copyData.selectPath);
			}
		}

		private static void RecursiveCopyFolder(DirectoryInfo dir, string outputPath, bool overwrite, Action<string, int, int> Progress = null)
        {
			DirectoryInfo[] dirs = dir.GetDirectories();
			CopyOneFolder(dir, outputPath, overwrite, Progress);

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(outputPath, subdir.Name);
				DirectoryInfo nextDir = new DirectoryInfo(subdir.FullName);
				RecursiveCopyFolder(nextDir, tempPath, overwrite, Progress);
            }
        }

        private static void CopyOneFolder(DirectoryInfo dir, string outputPath, bool overwrite, Action<string, int, int> Progress)
        {
            string fileName = string.Empty;
            string destFile = string.Empty;
            string extension = string.Empty;
            List<string> ignoreExtension = new List<string>();
            ignoreExtension.AddRange(currentSetting.ignoreFileExtension);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            FileInfo[] files = dir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                extension = file.Extension.TrimStart('.');
                if (ignoreExtension.Exists(e => e == extension)) continue;

                string tempPath = Path.Combine(outputPath, file.Name);

                string title = string.Format("Copying {0}", fileName);
                if (Progress != null) Progress.Invoke(title, i + 1, files.Length);

                file.CopyTo(tempPath, overwrite);
            }
        }

        private static void CopyFile(BuildTool.CopyData copyData, string savePath, Action<string, int, int> Progress = null)
		{
			string sourcePath = copyData.selectPath;
			string extension = Path.GetExtension(sourcePath).TrimStart('.');
			string outputPath = GetOutputPath(savePath, copyData, CopyType.File);
			string fileName = copyData.settingPopup.GetSourceFileName();
			List<string> ignoreExtension = new List<string>();
			ignoreExtension.AddRange(currentSetting.ignoreFileExtension);

			if (ignoreExtension.Exists(e => e == extension)) return;

			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);

			string destPath = Path.Combine(outputPath, fileName);

			if (File.Exists(sourcePath))
			{
				FileUtil.ReplaceFile(sourcePath, destPath);
			}
			else
			{
				Debug.LogErrorFormat("複製的檔案路徑錯誤，請檢查{0}路徑是否正確", copyData.selectPath);
			}
		}

		private static string GetOutputPath(string savePath, BuildTool.CopyData copyData, CopyType copyType)
		{
			string filePath = copyData.settingPopup.GetSaveFilePath();
			string folderPath = string.Empty;

			if (copyType == CopyType.File)
				folderPath = copyData.settingPopup.RemovePathNode(filePath, 1);
			else
				folderPath = copyData.settingPopup.RemovePathNode(filePath, 0);

			return Path.Combine(savePath, folderPath);
		}

#endregion
	}
}

