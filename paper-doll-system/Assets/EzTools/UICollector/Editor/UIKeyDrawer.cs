using Ez.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(UIKeyAttribute))]
public class UIKeyDrawer : PropertyDrawer
{
    private List<string> keys = new List<string>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (keys.Count == 0)
        {
            ProcessKeys();
        }

        if (property.propertyType == SerializedPropertyType.String)
        {
            if (keys.Count == 0)
            {
                EditorGUI.HelpBox(position, "No keys found", MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            {
                EditorGUI.BeginChangeCheck();

                // 取得目前的值
                string value = property.stringValue;
                int index = keys.IndexOf(value);
                if (index <= -1)
                {
                    index = 0;
                }

                // 顯示下拉式選單
                index = EditorGUI.Popup(position, label.text, index, keys.ToArray());
                if (index >= 0 && index < keys.Count)
                {
                    value = keys[index];
                }

                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = value;
                }
            }
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.HelpBox(position, "UIKeyAttribute can only be used on string fields", MessageType.Error);
        }
    }

    private void ProcessKeys()
    {
        // 取得所有的 key
        keys = GetKeys();

        string groupBy = (attribute as UIKeyAttribute).GroupBy;
        string startWith = groupBy + "_";

        // 篩選出符合 GroupBy 的 key
        if (!string.IsNullOrEmpty(groupBy))
        {
            // 篩選出符合 GroupBy 前綴的 key
            List<string> filterKeys = new List<string>();
            foreach (var key in keys)
            {
                if (key.StartsWith(startWith))
                {
                    filterKeys.Add(key);
                }
            }

            // 重新指定 keys
            keys = filterKeys;
        }
    }

    private List<string> GetKeys()
    {
        var fields = typeof(UIKey).GetFields();
        List<string> keys = new List<string>();
        foreach (var field in fields)
        {
            keys.Add(field.GetValue(null) as string);
        }
        return keys;
    }
}
