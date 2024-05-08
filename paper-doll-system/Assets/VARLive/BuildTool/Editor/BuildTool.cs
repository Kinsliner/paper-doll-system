using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VARLive.Build
{
    public static class BuildTool
    {
		public class CopyData
		{
			public PathSettingPopup settingPopup = new PathSettingPopup();
			public BuildTypeMaskPopup buildTypeMaskPopup = new BuildTypeMaskPopup();
			public bool isNeedCopy = true;
			public string selectPath;
		}

		public class CustomSceneData
        {
			public bool enable;
			public string scenePath;
        }

		public class BuildTypeData
		{
			public int buildIndex;
			public string name;
			public string defines;
			public string customBuildPath;
			public string customFileName;
			public bool supportVR;
			public bool allowBuildFromCI;
			public bool isCustomScenes;
			public List<CustomSceneData> customScenes = new List<CustomSceneData>();
		}

		public static string SettingPath { get { return currentSettingPath; } }

        public static BuildToolSetting currentSetting = new BuildToolSetting();
		public static List<BuildTypeData> buildTypeDatas = new List<BuildTypeData>();
		public static List<CopyData> copyFolderDatas = new List<CopyData>();
		public static List<CopyData> copyFileDatas = new List<CopyData>();
		public static List<string> ignoreFileExtensions = new List<string>() { "meta", "keep" };

		private const string BuildToolSettingKey = "BuildSettingPath";

        #region NeedSaveSetting
        private static bool isNeedSaveSetting;
		public static void MarkNeedSave()
		{
			isNeedSaveSetting = true;
		}

		public static bool IsNeedSave()
        {
			return isNeedSaveSetting;
        }
        #endregion

        #region Load & Save Setting
        private static string defaultPath;
		private static string currentSettingPath;
		public static void ChangeBuildToolSettingPath(string newPath)
		{
			currentSettingPath = newPath;
			PlayerPrefs.SetString(BuildToolSettingKey, currentSettingPath);
		}

		public static void LoadSetting()
        {
			DirectoryInfo projectDir = new DirectoryInfo(Application.dataPath);
			defaultPath = projectDir.Parent.FullName;
			currentSettingPath = PlayerPrefs.GetString(BuildToolSettingKey, defaultPath);

			BuildToolSetting setting = BuildToolSettingLoader.Instance.Load(currentSettingPath);
			if (setting == null)
			{
				InitAndSaveSetting();
			}
			else
			{
				currentSetting = setting;
			}

			SetBuildTypeSettingToData(currentSetting.buildTypeDatas);
			copyFolderDatas = GetCopyDatasFromSetting(currentSetting.copyFolderDatas);
			copyFileDatas = GetCopyDatasFromSetting(currentSetting.copyFileDatas);
			ignoreFileExtensions = currentSetting.ignoreFileExtension.ToList();

			isNeedSaveSetting = false;
		}

		private static void InitAndSaveSetting()
		{
			currentSetting = new BuildToolSetting();
			ChangeBuildToolSettingPath(defaultPath);
			SaveSetting(-1);

			Debug.LogWarningFormat("Can't find BuildToolSetting file therefore create the new one. Path:{0}", currentSettingPath);
		}

		private static void SetBuildTypeSettingToData(BuildToolSetting.BuildTypeSetting[] buildTypeSettings)
		{
			buildTypeDatas = new List<BuildTypeData>();

			for (int i = 0; i < buildTypeSettings.Length; i++)
			{
				BuildTypeData data = new BuildTypeData();
				data.buildIndex = i;
				data.name = buildTypeSettings[i].typeName;
				data.defines = buildTypeSettings[i].defineSymbol;
				data.customBuildPath = buildTypeSettings[i].buildPath;
				data.customFileName = buildTypeSettings[i].fileName;
				data.supportVR = buildTypeSettings[i].supportVR;
				data.allowBuildFromCI = buildTypeSettings[i].allowBuildFromCI;
				data.isCustomScenes = buildTypeSettings[i].isCustomScenes;
				data.customScenes = GetCustomScenes(buildTypeSettings[i].customSceneSettings);

				buildTypeDatas.Add(data);
			}
		}

		private static List<CustomSceneData> GetCustomScenes(BuildToolSetting.CustomSceneSetting[] customScenes)
		{
			List<CustomSceneData> sceneDatas = new List<CustomSceneData>();
			if (customScenes != null && customScenes.Length > 0)
			{
				for (int i = 0; i < customScenes.Length; i++)
				{
					var data = new CustomSceneData();
					data.enable = customScenes[i].enable;
					data.scenePath = customScenes[i].scenePath;

					sceneDatas.Add(data);
				}
			}

			return sceneDatas;
		}

		private static List<CopyData> GetCopyDatasFromSetting(BuildToolSetting.CopySetting[] copySettings)
		{
			List<CopyData> copyDatas = new List<CopyData>();

			for (int i = 0; i < copySettings.Length; i++)
			{
				CopyData copyData = new CopyData();

				copyData.settingPopup.customPath = copySettings[i].PathSetting.CustomPath;
				copyData.settingPopup.customFileName = copySettings[i].PathSetting.CustomFileName;
				copyData.settingPopup.overwrite = copySettings[i].PathSetting.Overwrite;
				copyData.settingPopup.overwrite = copySettings[i].PathSetting.RecursiveCopy;

				string sourcePath = string.Empty;
				if (string.IsNullOrEmpty(copySettings[i].RelativePath) == false)
				{
					sourcePath = GetSourcePath(copySettings[i].RelativePath);
				}
				else
				{
					sourcePath = copySettings[i].SelectPath;
				}
				copyData.selectPath = sourcePath;
				copyData.settingPopup.SetSourcePath(sourcePath);
				copyData.buildTypeMaskPopup.SetFlag(copySettings[i].buildTypeFlag);
				copyData.isNeedCopy = copySettings[i].isNeedCopy;

				copyDatas.Add(copyData);

			}

			return copyDatas;
		}

		public static string GetSourcePath(string relativePath)
		{
			var directoryInfo = new DirectoryInfo(Application.dataPath);
			string sourcePath = string.Empty;

			if (relativePath.Contains(":/"))
			{
				sourcePath = relativePath;
			}
			else
			{
				if (relativePath.StartsWith("/"))
				{
					relativePath = relativePath.Remove(0, 1);
				}
				sourcePath = Path.Combine(directoryInfo.Parent.FullName, relativePath);
				sourcePath = sourcePath.Replace("\\", "/");
			}

			return sourcePath;
		}

		public static void SaveSetting(int buildFlag)
        {
            currentSetting.buildTypeDatas = GetBuildTypeSettingsFromData();
            currentSetting.copyFolderDatas = GetCopySettingsFromData(copyFolderDatas);
            currentSetting.copyFileDatas = GetCopySettingsFromData(copyFileDatas);
            currentSetting.ignoreFileExtension = ignoreFileExtensions.ToArray();
            currentSetting.buildTypeSelect = buildFlag;

            BuildToolSettingLoader.Instance.Save(currentSetting, currentSettingPath);

            isNeedSaveSetting = false;
        }

		private static BuildToolSetting.BuildTypeSetting[] GetBuildTypeSettingsFromData()
		{
			List<BuildToolSetting.BuildTypeSetting> buildTypeSettings = new List<BuildToolSetting.BuildTypeSetting>();
			for (int i = 0; i < buildTypeDatas.Count; i++)
			{
				BuildToolSetting.BuildTypeSetting setting = new BuildToolSetting.BuildTypeSetting();

				if (string.IsNullOrEmpty(buildTypeDatas[i].name)) continue;

				setting.typeName = buildTypeDatas[i].name;
				setting.defineSymbol = buildTypeDatas[i].defines;
				setting.buildPath = buildTypeDatas[i].customBuildPath;
				setting.fileName = buildTypeDatas[i].customFileName;
				setting.supportVR = buildTypeDatas[i].supportVR;
				setting.allowBuildFromCI = buildTypeDatas[i].allowBuildFromCI;
				setting.isCustomScenes = buildTypeDatas[i].isCustomScenes;
				setting.customSceneSettings = GetCustomSceneSettingsFromData(buildTypeDatas[i].customScenes);

				buildTypeSettings.Add(setting);
			}

			return buildTypeSettings.ToArray();
		}

		private static BuildToolSetting.CustomSceneSetting[] GetCustomSceneSettingsFromData(List<CustomSceneData> customScenes)
        {
			List<BuildToolSetting.CustomSceneSetting> sceneSettings = new List<BuildToolSetting.CustomSceneSetting>();
			if(customScenes != null && customScenes.Count > 0)
            {
                for (int i = 0; i < customScenes.Count; i++)
                {
					var setting = new BuildToolSetting.CustomSceneSetting();
					setting.enable = customScenes[i].enable;
					setting.scenePath = customScenes[i].scenePath;

					sceneSettings.Add(setting);
				}
            }

			return sceneSettings.ToArray();
        }

		private static BuildToolSetting.CopySetting[] GetCopySettingsFromData(List<CopyData> copyDatas)
		{
			List<BuildToolSetting.CopySetting> copySettings = new List<BuildToolSetting.CopySetting>();

			for (int i = 0; i < copyDatas.Count; i++)
			{
				BuildToolSetting.CopySetting copySetting = new BuildToolSetting.CopySetting();

				copySetting.PathSetting.CustomPath = copyDatas[i].settingPopup.customPath;
				copySetting.PathSetting.CustomFileName = copyDatas[i].settingPopup.customFileName;
				copySetting.PathSetting.Overwrite = copyDatas[i].settingPopup.overwrite;
				copySetting.PathSetting.RecursiveCopy = copyDatas[i].settingPopup.overwrite;
				copySetting.buildTypeFlag = copyDatas[i].buildTypeMaskPopup.GetFlag();
				copySetting.isNeedCopy = copyDatas[i].isNeedCopy;

				copySetting.SelectPath = copyDatas[i].selectPath;
				string relative = GetRelativePath(copyDatas[i].selectPath);
				copySetting.RelativePath = relative;

				copySettings.Add(copySetting);
			}

			return copySettings.ToArray();
		}

		public static string GetRelativePath(string sourcePath)
		{
			var directoryInfo = new DirectoryInfo(Application.dataPath);
			string relativePath = string.Empty;
			string testPath = directoryInfo.Parent.FullName.Replace("\\", "/");

			if (sourcePath.StartsWith(testPath))
			{
				relativePath = sourcePath.Replace(testPath, "");
			}
			else
			{
				relativePath = sourcePath;
			}

			return relativePath;
		}
        #endregion

        #region Build Process
        public static void CopyGameData(string outputPath, int buildFlag, Action<string, int, int> ProgressBar = null)
		{
			var buildTypes = GetBuildTypes(buildFlag);
			for (int i = 0; i < buildTypes.Count; i++)
			{
				BuildProcess.CopyData(copyFileDatas, copyFolderDatas, currentSetting, outputPath, buildTypes[i], ProgressBar);
			}
		}

		public static void BuildGame(string outputPath, int buildFlag, Action<string, int, int> ProgressBar = null)
		{
			var buildTypes = GetBuildTypes(buildFlag);
			for (int i = 0; i < buildTypes.Count; i++)
			{
				if (currentSetting.copyDataOnBuild)
				{
					BuildProcess.CopyData(copyFileDatas, copyFolderDatas, currentSetting, outputPath, buildTypes[i], ProgressBar);
				}

				BuildProcess.Build(currentSetting, outputPath, buildTypes[i]);
			}
		}

		private static List<BuildTypeData> GetBuildTypes(int flagValue)
		{
			List<BuildTypeData> result = new List<BuildTypeData>();
			BitArray flags = new BitArray(new int[] { flagValue });
			for (int i = 0; i < buildTypeDatas.Count; i++)
			{
				if (flags[i] == true)
				{
					result.Add(buildTypeDatas[i]);
				}
			}

			return result;
		}
        #endregion

        #region Copy All
        public static bool IsCopyAll(List<CopyData> copyDatas)
		{
			for (int i = 0; i < copyDatas.Count; i++)
			{
				if (copyDatas[i].isNeedCopy == false)
				{
					return false;
				}
			}

			return true;
		}

		public static void SetCopyAll(List<CopyData> copyDatas, bool enable)
		{
			for (int i = 0; i < copyDatas.Count; i++)
			{
				copyDatas[i].isNeedCopy = enable;
			}
		}
        #endregion

        #region IgnoreExtension
        public static void AddIgnoreExtension(string extension)
        {
			if (string.IsNullOrEmpty(extension)) return;

			if (ignoreFileExtensions.Exists(e => e == extension) == false)
			{
				ignoreFileExtensions.Add(extension);
			}
		}

		public static void RemoveIgnoreExtension(string extension)
        {
			if (string.IsNullOrEmpty(extension)) return;

			if (ignoreFileExtensions.Exists(e => e == extension) == true)
			{
				var target = ignoreFileExtensions.Find(e => e == extension);
				ignoreFileExtensions.Remove(target);
			}
		}
		#endregion

		#region Build From CI
		static string defaultName = "SGB";
		static string destinationPathParameter = "-destinationPath";
		public static void BuildProjectFromCI()
		{
			string outputPath = FindCustomCommandLineDestinationPath();

			var buildSetting = GetCIBuildSetting();
			SetBuildTypeSettingToData(buildSetting.buildTypeDatas);
			var copyFileDatas = GetCopyDatasFromSetting(buildSetting.copyFileDatas);
			var copyFolderDatas = GetCopyDatasFromSetting(buildSetting.copyFolderDatas);

			for (int i = 0; i < BuildTool.buildTypeDatas.Count; i++)
			{
				if (BuildTool.buildTypeDatas[i].allowBuildFromCI)
				{
					BuildProcess.CopyData(copyFileDatas, copyFolderDatas, buildSetting, outputPath, BuildTool.buildTypeDatas[i], null);
					BuildProcess.Build(buildSetting, outputPath, BuildTool.buildTypeDatas[i]);
				}
			}

			//Build(defaultName, outputPath, BuildType.Box, Role.Host, false, true, true, true, true);
		}
		private static string FindCustomCommandLineDestinationPath()
		{
			string _path = string.Empty;

			Dictionary<string, System.Action<string>> commandLineActions = new Dictionary<string, System.Action<string>> {
				{ destinationPathParameter, delegate( string arg ) { _path = arg; } }
			};

			string[] commandLineArgs = System.Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineActions.ContainsKey(commandLineArgs[i]))
				{
					System.Action<string> action = commandLineActions[commandLineArgs[i]];
					action.Invoke(commandLineArgs[i + 1]);
				}
			}

			return _path;
		}
		private static BuildToolSetting GetCIBuildSetting()
		{
			string settingPath = GetSettingPath();
			BuildToolSetting setting = BuildToolSettingLoader.Instance.Load(settingPath);
			setting.copyAllFile = true;
			setting.copyAllFolder = true;
			setting.developmentMode = false;
			return setting;
		}
		private static string GetSettingPath()
		{
			DirectoryInfo projectFolder = new DirectoryInfo(Application.dataPath);
			string settingPath = projectFolder.Parent.FullName;
			return settingPath;
		}
		#endregion


	}
}

