using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Ez;

namespace Ez.EzEditor
{
    public class EzEditorWindow : EditorWindow
    {
        public virtual string DatName { get; } = string.Empty;

        private  GUILayout.ScrollViewScope mainScrollView;
        private  Vector2 mainScrollPos;

        private  GUILayout.ScrollViewScope sideScrollView;
        private  Vector2 sideScrollPos;

        protected EzFileHandler ProjectDFileHandler { get; private set; } = new EzFileHandler();
        protected FileHandler FileHandler { get; private set; }

        public virtual string SideViewTitle { get; protected set; } = string.Empty;
        public virtual string MainViewTitle { get; protected set; } = string.Empty;

        protected virtual void OnEnable()
        {
            ProjectDFileHandler.DataName = DatName;
            FileHandler = new FileHandler(ProjectDFileHandler, ProjectDFileHandler);
            ImportData();
        }

        private void OnGUI()
        {
            DrawToolBar();
            using (new GUILayout.HorizontalScope())
            {
                DrawSideView();
                DrawMainView();
            }
        }

        /// <summary>
        /// 繪製工具列
        /// </summary>
        private void DrawToolBar()
        {
            using (new GUILayout.HorizontalScope())
            {
                DrawToolBarData();
            }
        }

        /// <summary>
        /// 繪製工具列資料
        /// </summary>
        protected virtual void DrawToolBarData()
        {
            if (GUILayout.Button("匯出"))
            {
                ExportData();
            }
        }

        /// <summary>
        /// 繪製側欄視窗
        /// </summary>
        private void DrawSideView()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(200f)))
            {
                GUILayout.Label(SideViewTitle, EditorExtension.HightlightTitle);
                GUILayout.Space(5);
                DrawSideToolBar();
                GUILayout.Space(5);
                using (sideScrollView = new GUILayout.ScrollViewScope(sideScrollPos))
                {
                    sideScrollPos = sideScrollView.scrollPosition;
                    DrawSideViewData();
                }
            }
        }


        /// <summary>
        /// 繪製排序工具列
        /// </summary>
        protected void DrawSortToolBar<T,TKey>(ref List<T> items,Func<T, TKey> keySelector)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("由小到大排序"))
                {
                    items = items.OrderBy(keySelector).ToList();
                }
                if (GUILayout.Button("由大到小排序"))
                {
                    items = items.OrderByDescending(keySelector).ToList();
                }
            }
        }

        /// <summary>
        /// 繪製側欄資料
        /// </summary>
        protected virtual void DrawSideViewData()
        {

        }

        /// <summary>
        /// 繪製側欄工具列
        /// </summary>
        protected virtual void DrawSideToolBar()
        {

        }

        /// <summary>
        /// 繪製側欄物件
        /// </summary>
        protected void DrawSideItem(string name, bool isCurrent, Action onClick)
        {
            Color showColor = isCurrent ? Color.blue : Color.white;
            DrawSideItem(name, isCurrent, showColor, onClick);
        }

        /// <summary>
        /// 繪製側欄物件
        /// </summary>
        protected void DrawSideItem(string name, bool isCurrent, Color showColor, Action onClick)
        {
            GUIStyle pressBtn = new GUIStyle(GUI.skin.button);
            pressBtn.normal.textColor = showColor;
            pressBtn.hover.textColor = showColor;
            string showTips = isCurrent ? "◆" : string.Empty;
            if (GUILayout.Button($"{showTips}{name}", pressBtn))
            {
                onClick?.Invoke();
            }
        }

        /// <summary>
        /// 繪製主視窗
        /// </summary>
        private void DrawMainView()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label(MainViewTitle, EditorExtension.HightlightTitle);
                GUILayout.Space(5);
                using (mainScrollView = new GUILayout.ScrollViewScope(mainScrollPos))
                {
                    mainScrollPos = mainScrollView.scrollPosition;
                    DrawMainViewData();
                }
            }
        }

        /// <summary>
        /// 繪製主視窗資料
        /// </summary>
        protected virtual void DrawMainViewData()
        {
           
        }

        /// <summary>
        /// 匯入資料
        /// </summary>
        protected virtual void ImportData()
        {

        }

        /// <summary>
        /// 匯出資料
        /// </summary>
        protected virtual void ExportData()
        {

        }
    }
}
