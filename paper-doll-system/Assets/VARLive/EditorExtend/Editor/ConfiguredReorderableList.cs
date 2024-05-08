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
        /// ���ƶq�A���檺�ƶq�[�W���������(Columns.Count + 1)
        /// </summary>
        public int FieldCount => columns.Count + (IsDeletable ? 1 : 0);

        /// <summary>
        /// �[�`�Ҧ����e�׫᪺�`�e��
        /// </summary>
        public float FullFieldWidth => columns.Sum(c => c.desiredWidth) + (IsDeletable ? RemoveColumnWidth : 0);

        /// <summary>
        /// �O�_���������
        /// </summary>
        public bool IsDeletable { get; set; } = true;

        /// <summary>
        /// �������e��
        /// </summary>
        public float RemoveColumnWidth { get; set; } = 150;

        /// <summary>
        /// ���C��
        /// </summary>
        public List<Column> columns = new List<Column>();

        /// <summary>
        /// �ۭq���˦�
        /// </summary>
        public GUIStyle customStyle = null;
    }

    public class Column
    {
        /// <summary>
        /// ����r
        /// </summary>
        public string Label;

        /// <summary>
        /// �O�_�i�Ƨ�
        /// </summary>
        public bool Sortable = true;

        /// <summary>
        /// �������e�סA�|�۰ʾ���Ҧ����e�ת������ȫ�M����Ҥ��t
        /// </summary>
        public float desiredWidth;

        public Column(string label, float width)
        {
            Label = label;
            desiredWidth = width;
        }

        /// <summary>
        /// �ۭq�ƧǤ�k
        /// </summary>
        /// <param name="isAscending">�O�_�ɧ�</param>
        /// <param name="elements">�n�ƧǪ�����</param>
        /// <returns></returns>
        public virtual List<T> OrderBy(bool isAscending, List<T> elements)
        {
            return elements;
        }

        /// <summary>
        /// �ۭqø�s��k
        /// </summary>
        /// <param name="rect">��쪺Rect</param>
        /// <param name="element">��eø�s����</param>
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
        GUI.Label(rect, "�Щ즲����ܦ�", textStyle);
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

        // �p�����e��
        float GetColumnWidth(int index)
        {
            float factor;

            // �p�G���[�J�������A��p��̫�@�����ɡA�h�ϥβ������e��
            if (configurator.IsDeletable && index == columnCount - 1)
            {
                factor = configurator.RemoveColumnWidth / configurator.FullFieldWidth;
            }
            else
            {
                // �p��@�����e�פ��
                factor = configurator.columns[index].desiredWidth / configurator.FullFieldWidth;
            }
            return factor * fullWidth;
        }

        float prevColumnWidth = 0;
        for (int i = 0; i < columnCount; i++)
        {
            // ���o���e��
            float columnWidth = GetColumnWidth(i);

            Rect columnRect = new Rect(rect)
            {
                x = rect.x + BeginIndent + prevColumnWidth,
                width = columnWidth
            };

            prevColumnWidth += columnWidth;

            //�p�G�O�̫�@�����A�h�e�������s
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
        if (GUI.Button(rect, "�M������", headerStyle))
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

        return EditorUtility.DisplayDialog("ĵ�i", "�T�w�M�ũҦ����?", "�T�w", "����");
    }

    protected override void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        IsHovering = false;

        T element = elements[index];

        float fullWidth = rect.width;

        int columnCount = configurator.FieldCount;

        // �p�����e��
        float GetColumnWidth(int index)
        {
            float factor;

            // �p�G���[�J�������A��p��̫�@�����ɡA�h�ϥβ������e��
            if (configurator.IsDeletable && index == columnCount - 1)
            {
                factor = configurator.RemoveColumnWidth / configurator.FullFieldWidth;
            }
            else
            {
                // �p��@�����e�פ��
                factor = configurator.columns[index].desiredWidth / configurator.FullFieldWidth;
            }
            return factor * fullWidth;
        }

        float prevColumnWidth = 0;
        for (int i = 0; i < columnCount; i++)
        {
            // ���o���e��
            float columnWidth = GetColumnWidth(i);

            Rect columnRect = new Rect(rect)
            {
                x = rect.x + prevColumnWidth,
                width = columnWidth,
            };

            prevColumnWidth += columnWidth;

            //�p�G�O�̫�@�����A�h�e�������s
            if (i == columnCount - 1)
            {
                DrawElementRemove(columnRect, index);
            }
            else
            {
                //�e�@�����
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
        if (GUI.Button(rect, "�R��"))
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
    /// ReorderableList ���ղ��������ɪ��^�I�A��^ true ��ܤ��\�����A��^ false ��ܤ����\����
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
