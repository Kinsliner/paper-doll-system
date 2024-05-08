using System;
using System.IO;
using UnityEngine;

namespace VARLive.Build
{
	[Serializable]
    public class BuildToolSetting
    {
        [Serializable]
        public class PathSetting
        {
            public string CustomPath;
            public string CustomFileName;
            public bool Overwrite;
			public bool RecursiveCopy;

			public PathSetting()
            {
                CustomPath = string.Empty;
                CustomFileName = string.Empty;
                Overwrite = false;
				RecursiveCopy = true;
            }
        }

        [Serializable]
        public class CopySetting
        {
            public PathSetting PathSetting;
			public bool isNeedCopy;
			public string SelectPath;
            public string RelativePath;
			public int buildTypeFlag;

            public CopySetting()
            {
                PathSetting = new PathSetting();
                SelectPath = string.Empty;
                RelativePath = string.Empty;
            }
        }

		[Serializable]
		public class CustomSceneSetting
        {
			public bool enable;
			public string scenePath;
        }

		[Serializable]
		public class BuildTypeSetting
		{
			public string typeName;
			public string defineSymbol;
			public string buildPath;
			public string fileName;
			public bool supportVR;
			public bool allowBuildFromCI;
			public bool isCustomScenes;
			public CustomSceneSetting[] customSceneSettings;
		}

		public int buildTypeSelect;
		public bool copyAllFolder;
		public bool copyAllFile;
        public bool developmentMode;
        public bool copyDataOnBuild;
		public bool modifyProductName;
		public bool addBuildTypeLabel;
		public string additionProductName;
		public string outputPath;
        public string assetBundleManifestPath;
		public string[] ignoreFileExtension;

		public BuildTypeSetting[] buildTypeDatas;
        public CopySetting[] copyFolderDatas;
        public CopySetting[] copyFileDatas;

		public BuildToolSetting()
		{
			buildTypeSelect = -1;
			developmentMode = false;
			copyAllFolder = true;
			copyAllFile = true;
			copyDataOnBuild = true;
			assetBundleManifestPath = string.Empty;
            copyFolderDatas = new CopySetting[] { };
            copyFileDatas = new CopySetting[] { };
        }
    }

	public class BuildToolSettingLoader
	{
		public static BuildToolSettingLoader Instance
		{
			get
			{
				if (instance == null)
					instance = new BuildToolSettingLoader();

				return instance;
			}
		}
		private static BuildToolSettingLoader instance;

		private const string BuildSettingFileName = "BuildSetting.json";

		public BuildToolSetting Load(string path)
		{
			try
			{
				string readPath = Path.Combine(path, BuildSettingFileName);
				StreamReader OutReader = new StreamReader(readPath, System.Text.Encoding.UTF8);
				BuildToolSetting setting = JsonUtility.FromJson<BuildToolSetting>(OutReader.ReadToEnd());

				OutReader.Close();

				return setting;
			}
			catch (IOException e)
			{
				Debug.LogErrorFormat("無法讀取設定! 請檢查路徑或是檔案是否存在! Message: {0} ", e.Message);
				return null;
			}
			catch (ArgumentException e)
			{
				Debug.LogErrorFormat("無法讀取設定! 請檢查JSON檔案是否格式有誤! Message: {0}", e.Message);
				return null;
			}
		}

		public void Save(BuildToolSetting setting, string path)
		{
			string savePath = Path.Combine(path, BuildSettingFileName);
			using (FileStream fs = new FileStream(savePath, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8))
				{
					string allText = JsonUtility.ToJson(setting, true);
					writer.Write(allText);
				}
			}
		}
	}
}