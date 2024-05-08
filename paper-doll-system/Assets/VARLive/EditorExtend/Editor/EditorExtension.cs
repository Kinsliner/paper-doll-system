using System;
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
}