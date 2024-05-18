using Ez.MapID;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public interface DragDropEditor
{
    bool IsAcceptDrag(Object[] objects);

    void OnAcceptDrag(Object[] objects);
}

public class EditorExtension
{
    private static GUIStyle _titleStyle;
    public static GUIStyle TitleStyle
    {
        get
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(EditorStyles.boldLabel);
                _titleStyle.fontSize = 16;
                _titleStyle.alignment = TextAnchor.MiddleCenter;
                _titleStyle.padding = new RectOffset(5, 5, 5, 5);
            }
            return _titleStyle;
        }
    }

    private static GUIStyle _subtitleStyle;
    public static GUIStyle SubtitleStyle
    {
        get
        {
            if (_subtitleStyle == null)
            {
                _subtitleStyle = new GUIStyle(EditorStyles.boldLabel);
                _subtitleStyle.fontSize = 14;
                _subtitleStyle.alignment = TextAnchor.MiddleCenter;
                _subtitleStyle.padding = new RectOffset(5, 5, 5, 5);
            }
            return _subtitleStyle;
        }
    }

    private static GUIStyle _centeredLabel;
    public static GUIStyle CenteredLabel
    {
        get
        {
            if (_centeredLabel == null)
            {
                _centeredLabel = new GUIStyle(EditorStyles.label);
                _centeredLabel.alignment = TextAnchor.MiddleCenter;
            }
            return _centeredLabel;
        }
    }

    private static GUIStyle _leftedLabel;
    public static GUIStyle LeftedLabel
    {
        get
        {
            if (_leftedLabel == null)
            {
                _leftedLabel = new GUIStyle(EditorStyles.label);
                _leftedLabel.alignment = TextAnchor.MiddleLeft;
                _leftedLabel.padding = new RectOffset(5, 5, 5, 5);
            }
            return _leftedLabel;
        }
    }

    private static GUIStyle _rightedLabel;
    public static GUIStyle RightedLabel
    {
        get
        {
            if (_rightedLabel == null)
            {
                _rightedLabel = new GUIStyle(EditorStyles.label);
                _rightedLabel.alignment = TextAnchor.MiddleRight;
                _rightedLabel.padding = new RectOffset(5, 5, 5, 5);
            }
            return _rightedLabel;
        }
    }

    private static GUIStyle _richTextLabel;
    public static GUIStyle RichTextLabel
    {
        get
        {
            if (_richTextLabel == null)
            {
                _richTextLabel = new GUIStyle(EditorStyles.label);
                _richTextLabel.richText = true;
            }
            return _richTextLabel;
        }
    }

    private static GUIStyle _StandrandLabel;
    public static GUIStyle StandrandLabel
    {
        get
        {
            if (_StandrandLabel == null)
            {
                _StandrandLabel = new GUIStyle(EditorStyles.label);
                _StandrandLabel.fontSize = 14;
                _StandrandLabel.richText = true;
                _StandrandLabel.padding = new RectOffset(5, 5, 5, 5);
            }
            return _StandrandLabel;
        }
    }

    private static GUIStyle _StandrandCenteredLabel;
    public static GUIStyle StandrandCenteredLabel
    {
        get
        {
            if (_StandrandCenteredLabel == null)
            {
                _StandrandCenteredLabel = new GUIStyle(EditorStyles.label);
                _StandrandCenteredLabel.fontSize = 14;
                _StandrandCenteredLabel.alignment = TextAnchor.MiddleCenter;
                _StandrandCenteredLabel.richText = true;
                _StandrandCenteredLabel.padding = new RectOffset(5, 5, 5, 5);
            }
            return _StandrandCenteredLabel;
        }
    }

    private static GUIStyle _SubtitleButton;
    public static GUIStyle SubtitleButton
    {
        get
        {
            if (_SubtitleButton == null)
            {
                _SubtitleButton = new GUIStyle("button");
                _SubtitleButton.fontSize = 14;
                _SubtitleButton.alignment = TextAnchor.MiddleCenter;
                _SubtitleButton.margin = new RectOffset(0, 5, 5, 0);
            }
            return _SubtitleButton;
        }
    }

    private static GUIStyle _HightlightTitleStyle;
    public static GUIStyle HightlightTitle
    {
        get
        {
            if (_HightlightTitleStyle == null)
            {
                _HightlightTitleStyle = new GUIStyle();
                _HightlightTitleStyle.fontSize = 15;
                _HightlightTitleStyle.alignment = TextAnchor.MiddleCenter;
                _HightlightTitleStyle.normal.background = new Texture2D(1, 1);
                _HightlightTitleStyle.normal.textColor = Color.black;
            }
            return _HightlightTitleStyle;
        }
    }

    public static void HighlightObject(Object obj)
    {
        EditorGUIUtility.PingObject(obj);
        Selection.activeObject = obj;
    }

    public static List<Type> FindSubClassTypes<T>()
    {
        // 原功能轉移至 ReflectionHelper
        return ReflectionHelper.FindSubClassTypes<T>();
    }

    #region 拖曳物件API
    public static void DragDrop(DragDropEditor dragDropEditor)
    {
        switch (Event.current.type)
        {
            case EventType.DragUpdated:
                {
                    if (dragDropEditor.IsAcceptDrag(DragAndDrop.objectReferences))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    Event.current.Use();
                }
                break;
            case EventType.DragPerform:
                {
                    if (dragDropEditor.IsAcceptDrag(DragAndDrop.objectReferences))
                    {
                        DragAndDrop.AcceptDrag();
                        dragDropEditor.OnAcceptDrag(DragAndDrop.objectReferences);
                    }
                }
                break;
        }
    }
    #endregion

    public class ColorScope : GUI.Scope
    {
        private Color _oldColor;

        public ColorScope(Color color)
        {
            _oldColor = GUI.color;
            GUI.color = color;
        }

        protected override void CloseScope()
        {
            GUI.color = _oldColor;
        }
    }

    /// <summary>
    /// 繪製背景顏色
    /// </summary>
    public static void DrawBackGroundColor(Action body, Color color)
    {
        Color originColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        {
            body.Invoke();
        }
        GUI.backgroundColor = originColor;
    }

    /// <summary>
    /// 繪製緊貼的標題
    /// </summary>
    public static void DrawLabel(string label, Action action)
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(label, GUILayout.Width(label.Length * 25f));
            action?.Invoke();
        }
    }

    public static int FlexiblePopup(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(label, GUILayout.Width(label.Length * 25f));
            return EditorGUILayout.Popup(selectedIndex, displayedOptions, options);
        }
    }

    public static Enum FlexiableEnumPopup(string label, Enum selected, params GUILayoutOption[] options)
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(label, GUILayout.Width(label.Length * 25f));
            return EditorGUILayout.EnumPopup(selected, options);
        }
    }

    public static void DrawLine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    /// <summary>
    /// 繪製整數集合
    /// </summary>
    public static void DrawIntegerList(string title, List<int> datas)
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(title);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("新增", GUILayout.Width(80f)))
                {
                    datas.Add(0);
                }
            }
            GUILayout.Space(5f);
            for (int i = 0; i < datas.Count; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    datas[i] = EditorGUILayout.IntField(datas[i], EditorStyles.textField);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("刪除", GUILayout.Width(80f)))
                    {
                        datas.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }

    public static void DrawSwapButtons<T>(T list, int index) where T : IList
    {
        DrawSwapButtons(list, index, "上移", "下移");
    }

    public static void DrawSwapButtons<T>(T list, int index, string upLabel, string downLabel) where T : IList
    {
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button(upLabel, GUILayout.Width(80)))
            {
                if (index > 0)
                {
                    var temp = list[index - 1];
                    list[index - 1] = list[index];
                    list[index] = temp;
                }
            }
            if (GUILayout.Button(downLabel, GUILayout.Width(80)))
            {
                if (index < list.Count - 1)
                {
                    var temp = list[index + 1];
                    list[index + 1] = list[index];
                    list[index] = temp;
                }
            }
        }
    }

    public static int DrawToggleGroup<T>(string title, int currentIndex, bool containsAll) where T : Enum
    {
        List<string> names = Enum.GetNames(typeof(T)).ToList();
        names.Insert(0, "All");
        int index = 0;
        int select = currentIndex;
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {

            GUILayout.Label(title, HightlightTitle);
            using (new GUILayout.HorizontalScope())
            {
                for (int j = 0; j < names.Count; j++)
                {
                    if (index < names.Count)
                    {
                        if (GUILayout.Button(names[index]))
                        {
                            select = containsAll ? index - 1 : index;
                        }
                        index++;
                    }
                }
            }
        }
        return select;
    }

    /// <summary>
    /// 繪製枚舉集合
    /// </summary>
    public static void DrawEnumList<T>(string title, List<int> datas) where T : Enum
    {
        string[] options = Enum.GetNames(typeof(T));
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(title);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("新增", GUILayout.Width(80f)))
                {
                    datas.Add(0);
                }
            }
            GUILayout.Space(5f);
            for (int i = 0; i < datas.Count; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    datas[i] = FlexiblePopup(string.Empty, datas[i], options);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("刪除", GUILayout.Width(80f)))
                    {
                        datas.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 繪製展開
    /// </summary>
    public static bool Foldout(bool foldout, string title, bool hasChanged = false)
    {
        GUIStyle changeStyle = new GUIStyle(EditorStyles.foldout);
        if (hasChanged)
        {
            Color color = Color.cyan;
            changeStyle.normal.textColor = color;
            changeStyle.onNormal.textColor = color;
        }
        return EditorGUILayout.Foldout(foldout, title, changeStyle);
    }

    /// <summary>
    /// 繪製標題
    /// </summary>
    public static void LabelField(string label, bool hasChanged = false, params GUILayoutOption[] styles)
    {
        GUIStyle changeStyle = new GUIStyle(EditorStyles.label);
        if (hasChanged)
        {
            Color color = Color.cyan;
            changeStyle.normal.textColor = color;
            changeStyle.onNormal.textColor = color;
        }
        EditorGUILayout.LabelField(label, changeStyle, styles);
    }

    /// <summary>
    /// 繪製浮點輸入
    /// </summary>
    public static float FloatField(float value, bool hasChanged = false)
    {
        GUIStyle changeStyle = new GUIStyle(EditorStyles.numberField);
        if (hasChanged)
        {
            Color color = Color.cyan;
            changeStyle.normal.textColor = color;
            changeStyle.onNormal.textColor = color;
        }
        return EditorGUILayout.FloatField(value, changeStyle);
    }

    /// <summary>
    /// 取得類別顯示名稱
    /// </summary>
    /// <param name="type">掛載NameAttribute的類別</param>
    public static string ProcessTypeDisplayName(Type type)
    {
        var nameAttribute = type.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute;
        if (nameAttribute != null)
        {
            return nameAttribute.Name;
        }
        return type.Name;
    }

    public static int DrawMapIDPopup(string label, int conditionID, params Type[] type)
    {
        List<MapID> maps = new List<MapID>();
        List<string> mapStrings = new List<string>();
        foreach (var t in type)
        {
            maps.AddRange(MapID.GenerateMap(t));
        }
        foreach (var map in maps)
        {
            mapStrings.Add(map.ShowName);
        }
        int index = maps.FindIndex(p => p.ID == conditionID);
        if (index == -1)
        {
            index = 0;
        }
        return maps[EditorExtension.FlexiblePopup(label, index, mapStrings.ToArray())].ID;
    }
}