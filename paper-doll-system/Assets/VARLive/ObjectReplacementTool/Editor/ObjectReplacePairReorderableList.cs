using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class ObjectReplacePairReorderableList : CustomReorderableList
{
    public bool IsHovering;
    public Action OnClear;

    private const float DragHandleWidth = 15;

    private List<ObjectReplacePair> objectReplacePairs;
    private int currentRemoveIndex = -1;
    private bool isOrderBy = true;

    public ObjectReplacePairReorderableList(List<ObjectReplacePair> elements) : base(elements, typeof(ObjectReplacePair))
    {
        objectReplacePairs = elements;
    }

    protected override void DrawEmptyElementCallback(Rect rect)
    {
        GUIStyle textStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter
        };
        GUI.Label(rect, "請拖曳要置換的物件至此", textStyle);
    }

    GUIStyle headerStyle;
    protected override void DrawHeaderCallback(Rect rect)
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.toolbarButton);
            headerStyle.fontSize = 13;
        }

        float contentWidth = rect.width - DragHandleWidth;

        Rect sourceFileRect = new Rect(rect)
        {
            x = rect.x + DragHandleWidth,
            width = contentWidth * 0.4f
        };
        DrawHeaderSourceFile(sourceFileRect);

        Rect targetFileRect = new Rect(rect)
        {
            x = sourceFileRect.xMax,
            width = contentWidth * 0.4f
        };
        DrawHeaderTargetFile(targetFileRect);

        Rect removeButtonRect = new Rect(rect)
        {
            x = targetFileRect.xMax,
            width = contentWidth * 0.2f
        };
        DrawHeaderRemove(removeButtonRect);
    }

    private void DrawHeaderSourceFile(Rect rect)
    {
        if (GUI.Button(rect, "原始物件", headerStyle))
        {
            isOrderBy = !isOrderBy;

            List<ObjectReplacePair> orderedList;
            if (isOrderBy)
            {
                orderedList = objectReplacePairs.OrderBy(p => p.source.name).ToList();
            }
            else
            {
                orderedList = objectReplacePairs.OrderByDescending(p => p.source.name).ToList();
            }
            objectReplacePairs.Clear();
            objectReplacePairs.AddRange(orderedList);
            list = objectReplacePairs;
        }
    }

    private void DrawHeaderTargetFile(Rect rect)
    {
        if (GUI.Button(rect, "置換物件", headerStyle))
        {
            isOrderBy = !isOrderBy;

            List<ObjectReplacePair> orderedList;
            if (isOrderBy)
            {
                orderedList = objectReplacePairs.OrderBy(p => p.target.name).ToList();
            }
            else
            {
                orderedList = objectReplacePairs.OrderByDescending(p => p.target.name).ToList();
            }
            objectReplacePairs.Clear();
            objectReplacePairs.AddRange(orderedList);
            list = objectReplacePairs;
        }
    }

    private void DrawHeaderRemove(Rect rect)
    {
        if (GUI.Button(rect, "清除全部", headerStyle))
        {
            OnClear?.Invoke();
        }
    }

    GUIStyle contentStyle;
    protected override void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        IsHovering = false;

        if (contentStyle == null)
        {
            contentStyle = new GUIStyle("Button");
        }

        ObjectReplacePair fileAssetPair = objectReplacePairs[index];
        float contentWidth = rect.width;
        Rect sourceFileRect = new Rect(rect)
        {
            x = rect.x,
            width = contentWidth * 0.4f
        };
        DrawElementSourceFile(sourceFileRect, fileAssetPair);
        
        Rect targetFileRect = new Rect(rect)
        {
            x = sourceFileRect.xMax,
            width = contentWidth * 0.4f
        };
        DrawElementTargetFile(targetFileRect, fileAssetPair);

        Rect removeButtonRect = new Rect(rect)
        {
            x = targetFileRect.xMax,
            width = contentWidth * 0.2f
        };
        DrawElementRemove(removeButtonRect, index);
    }

    private void DrawElementSourceFile(Rect rect, ObjectReplacePair element)
    {
        //畫欄位
        var newObject = EditorGUI.ObjectField(rect, element.source, typeof(GameObject), true) as GameObject;

        if (newObject != null && newObject.scene.IsValid())
        {
            element.source = newObject;
        }

        if (rect.Contains(Event.current.mousePosition))
        {
            IsHovering = true;
        }
    }

    private void DrawElementTargetFile(Rect rect, ObjectReplacePair element)
    {
        // draw object field
        var newObject = EditorGUI.ObjectField(rect, element.target, typeof(GameObject), false) as GameObject;

        if (newObject != null && newObject.scene.IsValid() == false)
        {
            element.target = newObject;
        }

        if (rect.Contains(Event.current.mousePosition))
        {
            IsHovering = true;
        }
    }

    private void DrawElementRemove(Rect rect, int index)
    {
        if (GUI.Button(rect, "刪除"))
        {
            currentRemoveIndex = index;
        }
    }

    public ObjectReplacePair GetNeedRemoveAsset()
    {
        if (currentRemoveIndex >= 0)
        {
            ObjectReplacePair needRemoveAsset = objectReplacePairs[currentRemoveIndex];
            currentRemoveIndex = -1;
            return needRemoveAsset;
        }
        return null;
    }
}
