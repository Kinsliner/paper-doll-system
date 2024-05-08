using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace VARLive.Tool
{
    public class UIAssetReorderableList : CustomReorderableList
    {
        public Action<EditorUIAsset> OnRemoveAsset;
        public List<EditorUIAsset> UIAssets;

        private int currentRemoveIndex = -1;
        private float keyNameWidth = 130f;
        private float deleteBtnidth = 80f;
        private float copyBtnWidth = 30f;
        private float space = 5f; //每個欄位之間的間隔
        private float contentFactor = 0.5f; //物件欄位+物件ID欄位用的比例
        private Color proBlue = new Color(0.5f, 0.7f, 1, 1); //專業版Skin用的藍色
        private Color blue = new Color(0, 0.235f, 0.533f, 1);
        private Color red = new Color(1, 0.1f, 0.1f, 1);

        public UIAssetReorderableList(List<EditorUIAsset> elements) : base(elements, typeof(EditorUIAsset))
        {
            UIAssets = elements;
            elementHeight = 25f;
            footerHeight = 0;
        }

        //切換排序用的布林 true => 遞增, false => 遞減
        private bool orderByType = false;
        private bool orderByKey = false;
        private bool orderByID = false;
        protected override void DrawHeaderCallback(Rect rect)
        {
            GUIStyle headerStyle = new GUIStyle(EditorStyles.toolbarButton);
            headerStyle.fontSize = 13;
            float contentWidth = rect.width - keyNameWidth - space - deleteBtnidth - space - space;


            Rect textRect = new Rect(rect)
            {
                x = rect.x + 15f,
                width = keyNameWidth
            };
            if (GUI.Button(textRect, "Key", headerStyle) && UIAssets.Count > 0)
            {
                orderByKey = !orderByKey;
                if (orderByKey)
                    UIAssets = UIAssets.OrderBy(p => p.keyName).ToList();
                else
                    UIAssets = UIAssets.OrderByDescending(p => p.keyName).ToList();

                list = UIAssets;
            }


            Rect contentRect = new Rect(rect)
            {
                x = textRect.xMax,
                width = contentWidth * contentFactor
            };
            if (GUI.Button(contentRect, "物件", headerStyle) && UIAssets.Count > 0)
            {
                orderByType = !orderByType;
                if (orderByType)
                    UIAssets = UIAssets.OrderBy(p => p.asset.GetType().Name).ToList();
                else
                    UIAssets = UIAssets.OrderByDescending(p => p.asset.GetType().Name).ToList();

                list = UIAssets;
            }


            Rect keyRect = new Rect(rect)
            {
                x = contentRect.xMax,
                width = contentWidth * (1 - contentFactor)
            };
            if (GUI.Button(keyRect, "物件ID", headerStyle) && UIAssets.Count > 0)
            {
                orderByID = !orderByID;
                if (orderByID)
                    UIAssets = UIAssets.OrderBy(p => p.displayKey).ToList();
                else
                    UIAssets = UIAssets.OrderByDescending(p => p.displayKey).ToList();

                list = UIAssets;
            }


            Rect btnRect = new Rect(rect)
            {
                x = keyRect.xMax,
                width = deleteBtnidth
            };
            if (GUI.Button(btnRect, "清除全部", headerStyle) && UIAssets.Count > 0)
            {
                if (EditorUtility.DisplayDialog("清除全部", "即將清空資料，確定嗎?", "確定", "取消"))
                {
                    UIAssets.Clear();
                }
            }
        }

        protected override void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var item = UIAssets[index];
            float contentWidth = rect.width - keyNameWidth - space - deleteBtnidth - space - space;
            GUIStyle contentStyle = new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleLeft
            };
            GUIStyle conflictStyle = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleRight,
                contentOffset = new Vector2(-5f, 0f),
            };
            GUIStyle displayKeyStyle = new GUIStyle(EditorStyles.label)
            {
                clipping = TextClipping.Clip
            };


            //畫Key欄位
            Rect keyNameRect = new Rect(rect)
            {
                width = keyNameWidth
            };
            item.keyName = GUI.TextField(keyNameRect, item.keyName);


            //畫物件欄位
            Rect contentRect = new Rect(rect)
            {
                x = keyNameRect.xMax + space,
                width = contentWidth * 0.5f
            };
            GUI.Box(contentRect, EditorGUIUtility.ObjectContent(item.asset, item.asset.GetType()), contentStyle);
            if (Event.current.clickCount == 2 && contentRect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.PingObject(item.asset);
                Selection.activeObject = item.asset;
            }
            //顯示錯誤訊息
            if (string.IsNullOrEmpty(item.assetConflictMessage) == false)
                GUI.Label(contentRect, item.assetConflictMessage, conflictStyle);
            else if(string.IsNullOrEmpty(item.objectConflictMessage) == false)
                GUI.Label(contentRect, item.objectConflictMessage, conflictStyle);


            //畫物件ID欄位
            Rect keyRect = new Rect(rect)
            {
                x = contentRect.xMax + space,
                width = contentWidth * 0.5f
            };
            //如果物件ID不合法會顯示紅字，合法的話就顯示藍字
            if (item.IsVaild() == false)
            {
                displayKeyStyle.normal.textColor = red;
            }
            else if (EditorGUIUtility.isProSkin)
            {
                displayKeyStyle.normal.textColor = proBlue; //專業藍
            }
            else
            {
                displayKeyStyle.normal.textColor = blue; //草民藍
            }
            GUI.Label(keyRect, item.displayKey, displayKeyStyle);


            //畫跟在物件ID後面的複製按鈕
            Rect copyRect = new Rect(rect)
            {
                x = keyRect.xMax - copyBtnWidth,
                width = copyBtnWidth
            };
            GUIContent icon = EditorGUIUtility.IconContent("TreeEditor.Duplicate");
            if (GUI.Button(copyRect, icon))
            {
                EditorGUIUtility.systemCopyBuffer = item.realKey;
            }


            //畫刪除按鈕
            Rect btnRect = new Rect(rect)
            {
                x = keyRect.xMax + space,
                width = deleteBtnidth
            };
            if (GUI.Button(btnRect, "刪除"))
            {
                //在畫陣列的過程中直接刪除物件會導致錯誤
                //紀錄要刪除的物件的Index，等到OnGUI在刪掉
                currentRemoveIndex = index;
            }
        }

        protected override void DrawEmptyElementCallback(Rect rect)
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(rect, "拖曳物件至編輯器", textStyle);
        }

        private Vector2 scrollPosition;
        public void OnDrawGUI()
        {
            if (UIAssets == null)
                return;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            {
                DoLayoutList();

                if (currentRemoveIndex >= 0)
                {
                    OnRemoveAsset?.Invoke(UIAssets[currentRemoveIndex]);
                    UIAssets.RemoveAt(currentRemoveIndex);
                    currentRemoveIndex = -1;
                }
            }
            GUILayout.EndScrollView();
        }
    }
}

