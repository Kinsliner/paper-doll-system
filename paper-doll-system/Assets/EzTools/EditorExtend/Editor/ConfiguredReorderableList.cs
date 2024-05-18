using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class ConfiguredReorderableList<T> : CustomReorderableList
{
    public class Configurator
    {
        /// <summary>
        /// 欄位數量，由欄的數量加上移除用欄位(Columns.Count + 1)
        /// </summary>
        public int FieldCount => columns.Count + (IsDeletable ? 1 : 0);

        /// <summary>
        /// 加總所有欄位寬度後的總寬度
        /// </summary>
        public float FullFieldWidth => columns.Sum(c => c.desiredWidth) + (IsDeletable ? RemoveColumnWidth : 0);

        /// <summary>
        /// 是否有移除欄位
        /// </summary>
        public bool IsDeletable { get; set; } = true;

        /// <summary>
        /// 移除欄位寬度
        /// </summary>
        public float RemoveColumnWidth { get; set; } = 150;

        /// <summary>
        /// 欄位列表
        /// </summary>
        public List<Column> columns = new List<Column>();

        /// <summary>
        /// 自訂欄位樣式
        /// </summary>
        public GUIStyle customStyle = null;
    }

    public class Column
    {
        /// <summary>
        /// 欄位文字
        /// </summary>
        public string Label;

        /// <summary>
        /// 是否可排序
        /// </summary>
        public bool Sortable = true;

        /// <summary>
        /// 期望欄位寬度，會自動器算所有欄位寬度的平均值後和按比例分配
        /// </summary>
        public float desiredWidth;

        public Column(string label, float width)
        {
            Label = label;
            desiredWidth = width;
        }

        /// <summary>
        /// 自訂排序方法
        /// </summary>
        /// <param name="isAscending">是否升序</param>
        /// <param name="elements">要排序的元素</param>
        /// <returns></returns>
        public virtual List<T> OrderBy(bool isAscending, List<T> elements)
        {
            return elements;
        }

        /// <summary>
        /// 自訂繪製方法
        /// </summary>
        /// <param name="rect">欄位的Rect</param>
        /// <param name="element">當前繪製元素</param>
        public virtual void DrawColumn(Rect rect, T element)
        {
        }
    }

    public bool IsHovering { get; private set; }

    protected const float BeginIndent = 15;

    protected List<T> elements;
    protected Configurator configurator;
    private Column currentHoveringColumn;
    private int currentRemoveIndex = -1;
    private bool isOrderByAscending = true;

    public ConfiguredReorderableList(List<T> elements, Configurator configurator) : base(elements, typeof(T))
    {
        this.elements = elements;
        this.configurator = configurator;
    }

    protected override void DrawEmptyElementCallback(Rect rect)
    {
        GUIStyle textStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter
        };
        GUI.Label(rect, "請拖曳物件至此", textStyle);
    }

    GUIStyle headerStyle;
    protected override void DrawHeaderCallback(Rect rect)
    {
        if (configurator.customStyle != null)
        {
            headerStyle = configurator.customStyle;
        }
        else
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.toolbarButton);
                headerStyle.fontSize = 13;
            }
        }

        float fullWidth = rect.width - BeginIndent;

        int columnCount = configurator.FieldCount;

        // 計算欄位寬度
        float GetColumnWidth(int index)
        {
            float factor;

            // 如果有加入移除欄位，當計算最後一格欄位時，則使用移除欄位寬度
            if (configurator.IsDeletable && index == columnCount - 1)
            {
                factor = configurator.RemoveColumnWidth / configurator.FullFieldWidth;
            }
            else
            {
                // 計算一般欄位寬度比例
                factor = configurator.columns[index].desiredWidth / configurator.FullFieldWidth;
            }
            return factor * fullWidth;
        }

        float prevColumnWidth = 0;
        for (int i = 0; i < columnCount; i++)
        {
            // 取得欄位寬度
            float columnWidth = GetColumnWidth(i);

            Rect columnRect = new Rect(rect)
            {
                x = rect.x + BeginIndent + prevColumnWidth,
                width = columnWidth
            };

            prevColumnWidth += columnWidth;

            //如果是最後一格欄位，則畫移除按鈕
            if (i == columnCount - 1)
            {
                DrawHeaderRemove(columnRect);
            }
            else
            {
                DrawHeaderColumn(columnRect, configurator.columns[i]);
            }
        }
    }

    private void DrawHeaderColumn(Rect columnRect, Column column)
    {
        if(column == null)
        {
            return;
        }

        if (column.Sortable)
        {
            DrawHeaderSortableColumn(columnRect, column);
        }
        else
        {
            DrawHeaderUnsortableColumn(columnRect, column);
        }
    }

    private void DrawHeaderUnsortableColumn(Rect columnRect, Column column)
    {
        GUI.Label(columnRect, column.Label, headerStyle);
    }

    private void DrawHeaderSortableColumn(Rect columnRect, Column column)
    {
        if (GUI.Button(columnRect, column.Label, headerStyle))
        {
            isOrderByAscending = !isOrderByAscending;

            List<T> orderedList;
            if (isOrderByAscending)
            {
                orderedList = column.OrderBy(true, elements);
            }
            else
            {
                orderedList = column.OrderBy(false, elements);
            }
            if (orderedList == null)
            {
                return;
            }
            elements.Clear();
            elements.AddRange(orderedList);
            list = elements;
        }
    }

    private void DrawHeaderRemove(Rect rect)
    {
        if (GUI.Button(rect, "清除全部", headerStyle))
        {
            if (OnClearElement())
            {
                elements.Clear();
            }
        }
    }

    protected virtual bool OnClearElement()
    {
        if (elements.Count <= 0)
            return false;

        return EditorUtility.DisplayDialog("警告", "確定清空所有資料?", "確定", "取消");
    }

    protected override void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        IsHovering = false;

        T element = elements[index];

        float fullWidth = rect.width;

        int columnCount = configurator.FieldCount;

        // 計算欄位寬度
        float GetColumnWidth(int index)
        {
            float factor;

            // 如果有加入移除欄位，當計算最後一格欄位時，則使用移除欄位寬度
            if (configurator.IsDeletable && index == columnCount - 1)
            {
                factor = configurator.RemoveColumnWidth / configurator.FullFieldWidth;
            }
            else
            {
                // 計算一般欄位寬度比例
                factor = configurator.columns[index].desiredWidth / configurator.FullFieldWidth;
            }
            return factor * fullWidth;
        }

        float prevColumnWidth = 0;
        for (int i = 0; i < columnCount; i++)
        {
            // 取得欄位寬度
            float columnWidth = GetColumnWidth(i);

            Rect columnRect = new Rect(rect)
            {
                x = rect.x + prevColumnWidth,
                width = columnWidth,
            };

            prevColumnWidth += columnWidth;

            //如果是最後一格欄位，則畫移除按鈕
            if (i == columnCount - 1)
            {
                DrawElementRemove(columnRect, index);
            }
            else
            {
                //畫一般欄位
                DrawElementColumn(columnRect, configurator.columns[i], element);
            }
        }
    }

    private void DrawElementColumn(Rect columnRect, Column column, T element)
    {
        if (column == null)
        {
            return;
        }

        if (columnRect.Contains(Event.current.mousePosition))
        {
            IsHovering = true;
            currentHoveringColumn = column;
        }

        column.DrawColumn(columnRect, element);
    }

    private void DrawElementRemove(Rect rect, int index)
    {
        if (GUI.Button(rect, "刪除"))
        {
            currentRemoveIndex = index;
        }
    }

    public void Draw()
    {
        DoLayoutList();
        HandleRemoveFile();
    }

    private void HandleRemoveFile()
    {
        T needRemoveAsset = GetNeedRemoveAsset();
        if (needRemoveAsset != null)
        {
            if (OnRemoveElement(needRemoveAsset))
            {
                elements.Remove(needRemoveAsset);
            }
        }
    }

    public T GetNeedRemoveAsset()
    {
        if (currentRemoveIndex >= 0)
        {
            T needRemoveAsset = elements[currentRemoveIndex];
            currentRemoveIndex = -1;
            return needRemoveAsset;
        }
        return default;
    }

    /// <summary>
    /// ReorderableList 嘗試移除元素時的回呼，返回 true 表示允許移除，返回 false 表示不允許移除
    /// </summary>
    protected virtual bool OnRemoveElement(T element)
    {
        return true;
    }

    public Column GetHoveringColumn()
    {
        if (IsHovering)
        {
            return currentHoveringColumn;
        }
        return null;
    }

    public void UpdateList(List<T> newList)
    {
        elements = newList;
        list = elements;
    }
}
