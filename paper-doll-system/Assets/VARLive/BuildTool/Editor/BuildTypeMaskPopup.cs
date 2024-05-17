using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Ez.Build
{
	public class BuildTypeMaskPopup : PopupWindowContent
	{
		private int flagValue = -1;
		private string[] currentOptions;

		public override Vector2 GetWindowSize()
		{
			float height = currentOptions.Length * 20;

			return new Vector2(200, 45 + height);
		}

		public override void OnGUI(Rect rect)
		{
			GUILayout.Label("Build Target :", EditorStyles.boldLabel);

			BitArray flags = new BitArray(new int[] { flagValue });

			bool all = IsCheckAll(flags);
			all = EditorGUILayout.Toggle("All", all);
			if (all)
			{
				flags.SetAll(true);
			}
			else if (IsCheckAll(flags))
			{
				flags.SetAll(false);
			}

			for (int i = 0; i < currentOptions.Length; i++)
			{
				flags.Set(i, EditorGUILayout.Toggle(currentOptions[i], flags.Get(i)));
			}

			int[] array = new int[1];
			flags.CopyTo(array, 0);
			flagValue = array[0];
		}

		private void SetAll(BitArray flags, bool enable)
		{
			for (int i = 0; i < currentOptions.Length; i++)
			{
				flags[i] = enable;
			}
			int[] array = new int[1];
			flags.CopyTo(array, 0);
			flagValue = array[0];
		}

		public void SetDisplayOptions(string[] options)
		{
			currentOptions = options;
		}

		public string GetDisplayLabel()
		{
			BitArray flags = new BitArray(new int[] { flagValue });

			if (IsCheckAll(flags) && currentOptions.Length == 1)
			{
				return currentOptions[0];
			}else if(IsCheckAll(flags))
			{
				return "All";
			}

			int count = 0;
			string name = string.Empty;
			for (int i = 0; i < currentOptions.Length; i++)
			{
				if(flags[i])
				{
					count += 1;
					name = currentOptions[i];
				}
			}

			if(count > 1)
			{
				return "Mixed";
			}else if(count == 1)
			{
				return name;
			}

			return "None";
		}

		private bool IsCheckAll(BitArray bitArray)
		{
			for (int i = 0; i < currentOptions.Length; i++)
			{
				if(bitArray[i] == false)
				{
					return false;
				}
			}

			return true;
		}

		public int GetFlag()
		{
			return flagValue;
		}

		public void SetFlag(int flag)
		{
			flagValue = flag;
		}
	}
}