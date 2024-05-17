using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System.Reflection;

namespace Ez.Tool
{
    public class ConstKeysGeneratorWindow : EditorWindow
	{
		public const string NamespaceName = "VARLive.Tool";

        private static List<ConstKeysGenerator> generators = new List<ConstKeysGenerator>();

        private GUILayout.ScrollViewScope scrollViewScope;
        private Vector2 scrollPos;

        [MenuItem ("VAR Live/Const Keys Generator")]
		public static ConstKeysGeneratorWindow ShowWindow () 
		{
			var window = GetWindow<ConstKeysGeneratorWindow> ("常數表產生工具");
			return window;
		}

        private void OnEnable()
        {
			generators = GetGenerators();

            //set namespace
            generators.ForEach(generator =>
            {
                generator.SetNamespace(NamespaceName);
            });
        }

        private void OnDisable()
        {
            
        }

        private List<ConstKeysGenerator> GetGenerators()
        {
            List<ConstKeysGenerator> generators = new List<ConstKeysGenerator>();

            // 獲取所有已載入的組件
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 遍歷所有組件，尋找 ConstKeysGenerator 的子類別
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                Type[] possible = (from Type type in types where type.IsSubclassOf(typeof(ConstKeysGenerator)) select type).ToArray();

                foreach (var type in possible)
                {
                    var generator = Activator.CreateInstance(type) as ConstKeysGenerator;
                    generators.Add(generator);
                }
            }
            return generators;
        }

		void OnGUI () 
		{
			using (scrollViewScope = new GUILayout.ScrollViewScope(scrollPos))
			{
				scrollPos = scrollViewScope.scrollPosition;

				foreach (var generator in generators)
				{
					DrawGenerator(generator);
				}
            }
		}

        private void DrawGenerator(ConstKeysGenerator generator)
        {
            using (new GUILayout.HorizontalScope("box"))
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(generator.GetName(), GUILayout.Width(200), GUILayout.Height(60)))
                {
                    generator.Generate();

                    AssetDatabase.Refresh();
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(10f);
        }
	}
}
