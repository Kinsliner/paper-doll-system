using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace Ez.SystemModule
{
    [CustomEditor(typeof(SystemRunner))]
    public class SystemRunnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SystemRunner runner = (SystemRunner)target;

            EditorGUILayout.LabelField("System Type Names", EditorStyles.boldLabel);

            for (int i = 0; i < runner.systemTypes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(runner.systemTypes[i]);
                if (GUILayout.Button("Remove"))
                {
                    runner.systemTypes.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add System Type"))
            {
                ShowAddSystemTypeMenu(runner);
            }

            serializedObject.ApplyModifiedProperties();

        }

        private List<Type> GetSystemTypes()
        {
            return TypeCache.GetTypesDerivedFrom<ISystem>().Where(type => !type.IsInterface && !type.IsAbstract).ToList();
        }

        private void ShowAddSystemTypeMenu(SystemRunner runner)
        {
            var menu = new GenericMenu();

            var types = GetSystemTypes();

            foreach (var type in types)
            {
                if (!runner.systemTypes.Contains(type.Name))
                {
                    menu.AddItem(new GUIContent(type.Name), false, () => AddSystemType(runner, type.Name));
                }
            }

            menu.ShowAsContext();
        }

        private void AddSystemType(SystemRunner runner, string typeName)
        {
            runner.systemTypes.Add(typeName);
            EditorUtility.SetDirty(runner);
        }
    }
}
