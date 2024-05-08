using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections;
using System;

namespace VARLive.Build
{
    public enum CopyType
	{
		Folder,
		File
	}

	public enum ToolButton
	{
		BuildType,
		ScenesSet,
		CopySet,
		BuildSetting,
	}

	public class BuildToolWindow : EditorWindow
	{
		private GUIContent addIcon;
		private GUIContent removeIcon;
		private Vector2 scrollPosition;
		private int toolButtonSelect;
		private int buildTypePreviewSelect;
		private int buildTypeBuildSelect;
		private string addExtension;
		private string removeExtension;

		private const string ToolBarStyle = "Toolbar";
		private const string ToolBarButtonStyle = "toolbarbutton";
		
		private static BuildTypeMaskPopup buildTypeBuildSelectMaskPopup = new BuildTypeMaskPopup();

		[MenuItem("VAR Live/Build Tool %#T")]
		private static void ShowWindow()
		{
			LoadSetting();

			var window = GetWindow<BuildToolWindow>("Build Tool");
			window.Show();
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			LoadSetting();
		}

		private void OnGUI()
		{
			addIcon = EditorGUIUtility.IconContent("d_Toolbar Plus");
			removeIcon = EditorGUIUtility.IconContent("d_Toolbar Minus");

			DrawToolBar();

			GUILayout.BeginHorizontal("Box");
			{
				GUILayout.BeginVertical(GUILayout.Width(200));
				{
					DrawToolButtonPanel();
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical("ShurikenEffectBg");
				{
					DrawToolSettingPanel();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			UpdateBuildTypeIndex();
		}

		private void UpdateBuildTypeIndex()
		{
			for (int i = 0; i < BuildTool.buildTypeDatas.Count; i++)
			{
				BuildTool.buildTypeDatas[i].buildIndex = i;
			}
		}

		private void DrawToolBar()
		{
			GUILayout.BeginHorizontal(ToolBarStyle, GUILayout.Width(position.width));
			{
				string saveSettingLabel = BuildTool.IsNeedSave() ? "SaveSetting*" : "SaveSetting";
				if (GUILayout.Button(saveSettingLabel, ToolBarButtonStyle, GUILayout.Width(100)))
				{
					SaveSetting();
				}
				if (GUILayout.Button("Load Setting", ToolBarButtonStyle, GUILayout.Width(100)))
				{
					LoadSetting();
				}
				if (GUILayout.Button("Work Space : " + BuildTool.SettingPath, ToolBarButtonStyle))
				{
					SelectBuildToolWorkPath();
				}
			}
			GUILayout.EndHorizontal();
		}

		private static void LoadSetting()
        {
			BuildTool.LoadSetting();
			buildTypeBuildSelectMaskPopup.SetFlag(BuildTool.currentSetting.buildTypeSelect);
		}

		private static void SaveSetting()
        {
			BuildTool.SaveSetting(buildTypeBuildSelectMaskPopup.GetFlag());
		}

		private void SelectBuildToolWorkPath()
		{
			string newPath = SelectFolder();
			if (string.IsNullOrEmpty(newPath) == false)
			{
				BuildTool.ChangeBuildToolSettingPath(newPath);
				SaveSetting();
			}
		}


		private void DrawToolButtonPanel()
		{
			string[] buttonNames = System.Enum.GetNames(typeof(ToolButton));
			toolButtonSelect = ToggleList(toolButtonSelect, buttonNames);

			int prvSelect = buildTypeBuildSelect;

			GUILayout.FlexibleSpace();

			var buildTypes = BuildTool.buildTypeDatas.Select(d => d.name).ToArray();
			buildTypeBuildSelectMaskPopup.SetDisplayOptions(buildTypes);
			string displayLabel = buildTypeBuildSelectMaskPopup.GetDisplayLabel();
			Rect popupRect = GUILayoutUtility.GetLastRect();
			if (GUILayout.Button("Build Target : " + displayLabel, GUILayout.Height(30)))
			{
				popupRect.x += 4;
				popupRect.y += 36;
				PopupWindow.Show(popupRect, buildTypeBuildSelectMaskPopup);
			}

			buildTypeBuildSelect = buildTypeBuildSelectMaskPopup.GetFlag();

			bool buildable = IsBuildable();
			GUI.enabled = buildable;

			if(GUILayout.Button("Copy Data", GUILayout.Height(30)))
			{
				if (EditorUtility.DisplayDialog("Confirm", "確認輸出資料", "確認", "取消"))
				{
					string outputPath = GetOutputPath();
					int buildFlag = buildTypeBuildSelectMaskPopup.GetFlag();
					BuildTool.CopyGameData(outputPath, buildFlag, ProgressBar);
				}
			}

			if (GUILayout.Button("Build Game", GUILayout.Height(30)))
			{
				if (EditorUtility.DisplayDialog("Confirm", "確認輸出遊戲", "確認", "取消"))
				{
					string outputPath = GetOutputPath();
					int buildFlag = buildTypeBuildSelectMaskPopup.GetFlag();
					BuildTool.BuildGame(outputPath, buildFlag, ProgressBar);
				}
			}

			GUI.enabled = true;

			if (prvSelect != buildTypeBuildSelect)
				BuildTool.MarkNeedSave();
		}

		/// <summary>
		/// Displays a vertical list of toggles and returns the index of the selected item.
		/// </summary>
		private int ToggleList(int selected, string[] items)
		{
			// Keep the selected index within the bounds of the items array
			selected = selected < 0 ? 0 : selected >= items.Length ? items.Length - 1 : selected;

			for (int i = 0; i < items.Length; i++)
			{
				string name = ObjectNames.NicifyVariableName(items[i]);
				
				// Display toggle. Get if toggle changed.
				bool change = GUILayout.Toggle(selected == i, name, "Button", GUILayout.Height(30));

				// If changed, set selected to current index.
				if (change)
					selected = i;
			}

			// Return the currently selected item's index
			return selected;
		}

		private bool IsBuildable()
		{
			int flagValue = buildTypeBuildSelectMaskPopup.GetFlag();
			BitArray bitArray = new BitArray(new int[] { flagValue });
			for (int i = 0; i < bitArray.Length; i++)
			{
				if(bitArray[i] == true)
				{
					return true;
				}
			}

			return false;
		}

		private string GetOutputPath()
        {
			string outputPath = BuildTool.currentSetting.outputPath;
			if (string.IsNullOrEmpty(outputPath))
			{
				outputPath = SelectFolder();
			}
			return outputPath;
		}


		private void DrawToolSettingPanel()
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

			EditorGUI.BeginChangeCheck();

			ToolButton toolButton = (ToolButton)toolButtonSelect;
			if(toolButton == ToolButton.BuildType)
			{
				DrawBuildType();
			}
			if (toolButton == ToolButton.BuildSetting)
			{
				DrawBuildSetting();
			}
			if (toolButton == ToolButton.CopySet)
			{
				DrawCopySet();
			}
			if (toolButton == ToolButton.ScenesSet)
			{
				DrawScenesSet();
			}

			if (EditorGUI.EndChangeCheck())
				BuildTool.MarkNeedSave();

			GUILayout.EndScrollView();
		}

        private void DrawScenesSet()
        {
			GUILayout.Label("Scenes Set", PreLabelStyle);

			DrawUILine(Color.white);

            for (int i = 0; i < BuildTool.buildTypeDatas.Count; i++)
            {
				BuildTool.BuildTypeData currentBuildType = BuildTool.buildTypeDatas[i];

				GUILayout.BeginVertical("box");
                {
					GUILayout.BeginHorizontal();
                    {
						GUILayout.Label(currentBuildType.name, PreLabelStyle);

						GUILayout.FlexibleSpace();

						currentBuildType.isCustomScenes = GUILayout.Toggle(currentBuildType.isCustomScenes, "Custom", PreButtonStyle, GUILayout.Width(100));
					}
					GUILayout.EndHorizontal();

					if (currentBuildType.isCustomScenes)
					{
						if (GUILayout.Button("Apply Build Setting", PreButtonStyle, GUILayout.Width(150)))
						{
							ApplyBuildSettingToCustomScenes(currentBuildType);
						}

						DrawScenes(currentBuildType);
					}
				}
				GUILayout.EndVertical();
			}
		}

		private void DrawScenes(BuildTool.BuildTypeData buildTypeData)
		{
			if (buildTypeData.customScenes != null && buildTypeData.customScenes.Count > 0)
			{
				for (int i = 0; i < buildTypeData.customScenes.Count; i++)
				{
					buildTypeData.customScenes[i].enable = GUILayout.Toggle(buildTypeData.customScenes[i].enable, buildTypeData.customScenes[i].scenePath);
				}
			}
			else
			{
				ApplyBuildSettingToCustomScenes(buildTypeData);
			}
		}

		private void ApplyBuildSettingToCustomScenes(BuildTool.BuildTypeData buildTypeData)
        {
			buildTypeData.customScenes.Clear();
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				var scene = new BuildTool.CustomSceneData();
				scene.enable = EditorBuildSettings.scenes[i].enabled;
				scene.scenePath = EditorBuildSettings.scenes[i].path;

				buildTypeData.customScenes.Add(scene);
			}
		}

        private void DrawBuildType()
		{
			GUILayout.Label("Build Type", PreLabelStyle);

			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(addIcon, PreButtonStyle, GUILayout.Width(30)))
				{
					BuildTool.buildTypeDatas.Add(new BuildTool.BuildTypeData());
				}

				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

			DrawUILine(Color.white);

			GUILayout.Space(10);

			DrawBuildTypeDatas();
		}

		private void DrawBuildTypeDatas()
		{
			for (int i = 0; i < BuildTool.buildTypeDatas.Count; i++)
			{
				BuildTool.BuildTypeData currentBuildType = BuildTool.buildTypeDatas[i];
				string currentName = currentBuildType.name;

				GUILayout.BeginHorizontal();
                {
					string displayName = "*UnDefine*";
					if (string.IsNullOrEmpty(currentName) == false)
					{
						displayName = currentName;
					}
					GUILayout.Label(displayName, PreLabelStyle);

					if (GUILayout.Button(removeIcon, PreButtonStyle, GUILayout.Width(30)))
					{
						BuildTool.buildTypeDatas.Remove(currentBuildType);
						continue;
					}

					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				string showPath = string.IsNullOrEmpty(currentName) ? "[UnDefine]" : currentName;
				string showFile = string.IsNullOrEmpty(currentName) ? "[UnDefine]" : currentName;
				if(!string.IsNullOrEmpty(currentBuildType.customBuildPath))
				{
					showPath = currentBuildType.customBuildPath;
				}
				if(!string.IsNullOrEmpty(currentBuildType.customFileName))
				{
					showFile = currentBuildType.customFileName;
				}
				GUILayout.Label(string.Format("{0}/{1}.exe", showPath, showFile), EditorStyles.helpBox);

				currentBuildType.name = EditorGUILayout.TextField("Type Name", BuildTool.buildTypeDatas[i].name);
				currentBuildType.defines = EditorGUILayout.TextField("Define Symbol", BuildTool.buildTypeDatas[i].defines);
				currentBuildType.customBuildPath = EditorGUILayout.TextField("Custom Build Path", BuildTool.buildTypeDatas[i].customBuildPath);
				currentBuildType.customFileName = EditorGUILayout.TextField("Custom File Name", BuildTool.buildTypeDatas[i].customFileName);
				currentBuildType.supportVR = EditorGUILayout.Toggle("Support VR", BuildTool.buildTypeDatas[i].supportVR);
				currentBuildType.allowBuildFromCI = EditorGUILayout.Toggle("Allow Build From CI", BuildTool.buildTypeDatas[i].allowBuildFromCI);

				GUILayout.Space(5);
			}
		}

		
		private void DrawBuildSetting()
		{
			GUILayout.Label("Build Setting", PreLabelStyle);

			DrawUILine(Color.white);

			GenericSetting();

			DrawUILine(Color.white);

			AdvanceSetting();

			DrawUILine(Color.white);

			IgnoreSetting();

			DrawUILine(Color.white);

			PreviewBuildResult();
		}

		private void GenericSetting()
		{
			GUILayout.Label("Generic", PreLabelStyle);
			
			GUILayout.BeginHorizontal();
            {
				BuildTool.currentSetting.outputPath = EditorGUILayout.TextField("Output Path", BuildTool.currentSetting.outputPath);
				if (GUILayout.Button("Browse", PreButtonStyle, GUILayout.Width(80f)))
				{
					string path = SelectFolder();
					if (string.IsNullOrEmpty(path) == false)
					{
						BuildTool.currentSetting.outputPath = path;
					}
				}
			}
			GUILayout.EndHorizontal();

			BuildTool.currentSetting.developmentMode = EditorGUILayout.Toggle("Development Build", BuildTool.currentSetting.developmentMode);
			BuildTool.currentSetting.copyDataOnBuild = EditorGUILayout.Toggle("Copy Data", BuildTool.currentSetting.copyDataOnBuild);
		}

		private BuildTool.BuildTypeData GetBuildType(int index)
		{
			if(BuildTool.buildTypeDatas.Count > 0)
			{
				if (index >= BuildTool.buildTypeDatas.Count)
				{
					index = BuildTool.buildTypeDatas.Count - 1;
				}

				return BuildTool.buildTypeDatas[index];
			}

			return null;
		}

		private List<BuildTool.BuildTypeData> GetSelectBuildTypes()
		{
			List<BuildTool.BuildTypeData> result = new List<BuildTool.BuildTypeData>();
			int selectFlag = buildTypeBuildSelectMaskPopup.GetFlag();
			BitArray bitArray = new BitArray(new int[] { selectFlag });
			for (int i = 0; i < bitArray.Length; i++)
			{
				if (bitArray[i] == true)
				{
					var buildType = GetBuildType(i);
					if (buildType != null && result.Contains(buildType) == false)
						result.Add(buildType);
				}
			}

			return result;
		}

		private void AdvanceSetting()
		{
			GUILayout.Label("Advance", PreLabelStyle);

			GUILayout.BeginHorizontal();
			{
				BuildTool.currentSetting.assetBundleManifestPath = EditorGUILayout.TextField("AssetBundle Manifest", BuildTool.currentSetting.assetBundleManifestPath);
				if (GUILayout.Button("Select Path", PreButtonStyle, GUILayout.Width(100)))
				{
					string select = SelectFile();

					if (string.IsNullOrEmpty(select) == false)
						BuildTool.currentSetting.assetBundleManifestPath = select;
				}

				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(5);

			BuildTool.currentSetting.modifyProductName = EditorGUILayout.Toggle("Modify Product Name", BuildTool.currentSetting.modifyProductName);
			if(BuildTool.currentSetting.modifyProductName)
			{
				EditorGUILayout.LabelField("Product Name Preview", string.Format("{0}{1}", PlayerSettings.productName, BuildTool.currentSetting.additionProductName), EditorStyles.helpBox);

				BuildTool.currentSetting.addBuildTypeLabel = EditorGUILayout.Toggle("Add Build Type Label", BuildTool.currentSetting.addBuildTypeLabel);
				if(BuildTool.currentSetting.addBuildTypeLabel)
				{
					string buildTypeLabel = string.Empty;
					var buildType = GetBuildType(buildTypePreviewSelect);
					if(buildType != null)
					{
						buildTypeLabel = buildType.name;
					}
					BuildTool.currentSetting.additionProductName = string.Format("-{0}", buildTypeLabel);
				}
				else
				{
					BuildTool.currentSetting.additionProductName = EditorGUILayout.TextField("Custom Add Label", BuildTool.currentSetting.additionProductName);
				}
			}
		}

		private void IgnoreSetting()
		{
			GUILayout.Label("Ignore File Extension", PreLabelStyle);

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Add", PreButtonStyle, GUILayout.Width(50), GUILayout.Height(20)))
			{
				BuildTool.AddIgnoreExtension(addExtension);
				addExtension = string.Empty;
			}
			addExtension = GUILayout.TextField(addExtension, GUILayout.Width(100));

			if (GUILayout.Button("Remove", PreButtonStyle, GUILayout.Width(70), GUILayout.Height(20)))
			{
				BuildTool.RemoveIgnoreExtension(removeExtension);
				removeExtension = string.Empty;
			}
			removeExtension = GUILayout.TextField(removeExtension, GUILayout.Width(100));

			GUILayout.EndHorizontal();

			string extension = string.Empty;
			for (int i = 0; i < BuildTool.ignoreFileExtensions.Count; i++)
			{
				string txt = string.Format("*.{0}", BuildTool.ignoreFileExtensions[i]);
				extension += (txt + " ");
			}
			GUILayout.Label(extension, "PreLabel");
		}

		private void PreviewBuildResult()
        {
            GUILayout.Label("Preview", PreLabelStyle);
            
			string outputPath = string.IsNullOrEmpty(BuildTool.currentSetting.outputPath) ? "[Output Path]" : BuildTool.currentSetting.outputPath;
            var buildType = GetSelectBuildTypes();
			if(buildType.Count > 0)
            {
                for (int i = 0; i < buildType.Count; i++)
                {
					GUILayout.BeginVertical("Box");
					{
						EditorGUILayout.LabelField("Build Type :", buildType[i].name);
						string previewTypeLabel = GetPreviewTypeLabel(buildType[i]);
						string outputPathLabel = string.Format("{0}/{1}", outputPath, previewTypeLabel);
						EditorGUILayout.LabelField("Build Path :", outputPathLabel, EditorStyles.helpBox);
					}
					GUILayout.EndVertical();
				}
			}
        }

		private static string GetPreviewTypeLabel(BuildTool.BuildTypeData buildType)
        {
			string showPath = GetDefaultOrTargetName("[BuildType]", buildType.name);
			string showFile = GetDefaultOrTargetName("[BuildType]", buildType.name);
			if(buildType != null)
            {
				showPath = GetDefaultOrTargetName(showPath, buildType.customBuildPath);
				showFile = GetDefaultOrTargetName(showFile, buildType.customFileName);
			}

			string previewLabel = string.Format("{0}/{1}.exe", showPath, showFile);
			return previewLabel;
		}

		private static string GetDefaultOrTargetName(string defaultName, string targetName)
		{
			string resultName = string.IsNullOrEmpty(targetName) ? defaultName : targetName;
			return resultName;
		}

		private void DrawCopySet()
		{
			GUILayout.Label("Copy Set", "PreLabel");

			DrawUILine(Color.white);

			GUILayout.BeginHorizontal();
			{
				bool all = DrawCopyAllToggle(BuildTool.copyFolderDatas);
				BuildTool.currentSetting.copyAllFolder = all;
				ShowAddButton("Copy Folder List", BuildTool.copyFolderDatas);
			}
			GUILayout.EndHorizontal();

			ShowCopyList(BuildTool.copyFolderDatas, CopyType.Folder);

			DrawUILine(Color.white);

			GUILayout.BeginHorizontal();
			{
				bool all = DrawCopyAllToggle(BuildTool.copyFileDatas);
				BuildTool.currentSetting.copyAllFile = all;
				ShowAddButton("Copy File List", BuildTool.copyFileDatas);
			}
			GUILayout.EndHorizontal();

			ShowCopyList(BuildTool.copyFileDatas, CopyType.File);
		}

		private bool DrawCopyAllToggle(List<BuildTool.CopyData> copyDatas)
        {
			bool isCopyAll = BuildTool.IsCopyAll(copyDatas);
			isCopyAll = GUILayout.Toggle(isCopyAll, "", GUILayout.Width(20));
			if (isCopyAll)
			{
				BuildTool.SetCopyAll(copyDatas, true);
			}
			else if (BuildTool.IsCopyAll(copyDatas) == true)
			{
				BuildTool.SetCopyAll(copyDatas, false);
			}
			return isCopyAll;
		}

		string PreLabelStyle = "PreLabel";
		string PreButtonStyle = "PreButton";
		private void ShowAddButton(string title, List<BuildTool.CopyData> copyDatas)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(title, PreLabelStyle);

				if (GUILayout.Button(addIcon, PreButtonStyle, GUILayout.Width(30), GUILayout.Height(20)))
				{
					copyDatas.Add(new BuildTool.CopyData());
				}
			}
			GUILayout.EndHorizontal();
		}

		private void ShowCopyList(List<BuildTool.CopyData> copyDatas, CopyType copyType)
		{
			for (int i = 0; i < copyDatas.Count; i++)
			{
				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(5);

					copyDatas[i].isNeedCopy = GUILayout.Toggle(copyDatas[i].isNeedCopy, "");

					if (GUILayout.Button("Select Path", PreButtonStyle, GUILayout.Width(100)))
					{
						string path = string.Empty;
						if (copyType == CopyType.Folder) path = SelectFolder();
						if (copyType == CopyType.File) path = SelectFile();

						if (string.IsNullOrEmpty(path) == false)
						{
							copyDatas[i].selectPath = path;
							copyDatas[i].settingPopup.SetSourcePath(path);
						}
					}

					GUILayout.Space(5);

					Rect pathPopupRect = GUILayoutUtility.GetLastRect();
					if (GUILayout.Button("Path Setting", PreButtonStyle, GUILayout.Width(100)))
					{
						pathPopupRect.x += 4;
						pathPopupRect.y += 16;
						copyDatas[i].settingPopup.copyType = copyType;
						PopupWindow.Show(pathPopupRect, copyDatas[i].settingPopup);
					}

					GUILayout.Space(5);

					var buildTypes = BuildTool.buildTypeDatas.Select(d => d.name).ToArray();
					copyDatas[i].buildTypeMaskPopup.SetDisplayOptions(buildTypes);
					string displayLabel = copyDatas[i].buildTypeMaskPopup.GetDisplayLabel();
					Rect buildTypePopupRect = GUILayoutUtility.GetLastRect();
					if (GUILayout.Button("Build Type : " + displayLabel, PreButtonStyle, GUILayout.Width(200)))
					{
						buildTypePopupRect.x += 4;
						buildTypePopupRect.y += 16;

						PopupWindow.Show(buildTypePopupRect, copyDatas[i].buildTypeMaskPopup);
					}

					GUILayout.FlexibleSpace();

					if (GUILayout.Button(removeIcon, PreButtonStyle, GUILayout.Width(30)))
					{
						copyDatas.Remove(copyDatas[i]);
						continue;
					}
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				{
					GUILayout.Label("Path :", GUILayout.Width(40));
					copyDatas[i].selectPath = GUILayout.TextField(copyDatas[i].selectPath, GUILayout.Width(position.width - 300));
				}
				GUILayout.EndHorizontal();
			}
		}

		private string SelectFolder(string title = "選擇資料夾")
		{
			string path = EditorUtility.SaveFolderPanel(title, Application.dataPath, string.Empty);
			return path;
		}

		private string SelectFile(string title = "選擇檔案")
		{
			string path = EditorUtility.OpenFilePanel(title, Application.dataPath, string.Empty);
			return path;
		}

		public static void DrawUILine(Color color, int thickness = 1, int padding = 5)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
			r.height = thickness;
			r.y += padding / 2;
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}

		public void ProgressBar(string title, int current, int all)
		{
			float Progress = (float)current / (float)all;
			if (current < all)
			{
				string info = string.Format("{0}/{1}", current.ToString(), all.ToString());
				EditorUtility.DisplayProgressBar(title, info, Progress);
			}
			else
				EditorUtility.ClearProgressBar();

		}
	}
}

