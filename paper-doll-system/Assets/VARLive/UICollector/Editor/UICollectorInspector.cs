using UnityEditor;
using UnityEngine;

namespace VARLive.Tool
{
    [CustomEditor(typeof(UICollector))]
    public class UICollectorInspector : Editor
    {
        //private int currentID;
        //private UICollector currentCollector;

        public override void OnInspectorGUI()
        {
            UICollector collector = (UICollector)target;

            //紀錄在腳本被移除的時候會通知設定檔移除該ID的Keys
            //currentID = collector.collectorID;
            //currentCollector = collector;

            //顯示類似Monobehaviour的腳本欄位
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((UICollector)target), typeof(UICollector), false);
            GUI.enabled = true;

            //請出示你的身份
            string idLabel = "NaN";
            if(string.IsNullOrEmpty(collector.guid) == false)
            {
                idLabel = collector.guid;
            }
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("GUID:", idLabel);

                //TODO 產生新的GUID
                //var icon = EditorGUIUtility.IconContent("d_Refresh");
                //if (GUILayout.Button(icon, GUILayout.Width(30)))
                //{
                //    UICollectorEditor.CreateNewGUID(collector);
                //}
            }
            

            //群組只作為文字顯示，防止被不小心修改
            EditorGUILayout.LabelField("群組名稱:", collector.groupName);

            //偵測欄位是否有變動，有的話SetDirty
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                foreach (var asset in collector.UIAsset)
                {
                    DrawAsset(asset);
                }

                if (check.changed)
                {
                    EditorUtility.SetDirty(collector);
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("編輯器"))
            {
                //編輯器超連結
                var editor = EditorWindow.GetWindow<UICollectorEditor>("UI Collector");
                editor.Show();
                editor.ApplyCollector(collector);
            }

            if (collector == null)
            {
                Debug.LogError("OnInspectorGUI null");
            }
        }

        private void DrawAsset(UIAsset uiAsset)
        {
            EditorGUILayout.BeginHorizontal();
            {
                //Key作為名稱顯示，防止被不小心修改到。物件欄位開出來，想換物件就換
                var temp = EditorGUILayout.ObjectField(uiAsset.realKey, uiAsset.asset, typeof(Object), true);
                if (temp != uiAsset.asset)
                {
                    if (temp is GameObject obj && uiAsset.asset != null && uiAsset.asset is GameObject == false)
                    {
                        var comp = obj.GetComponent(uiAsset.asset.GetType());
                        if (comp != null)
                        {
                            uiAsset.asset = comp;
                        }
                        else
                        {
                            uiAsset.asset = temp;
                        }
                    }
                    else
                    {
                        uiAsset.asset = temp;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        //暫時棄用，Unity有Bug
        /*
        private void OnDisable()
        {
            //偵測是否有人移除UICollector的Component
            if (target == null && IsInEditMode())
            {
                //ID小於0代表是手動加Component，沒有被設定歸檔，不需要處理
                if (currentID >= 0)
                {
                    UICollectorSetting setting = UICollectorSettingLoader.LoadSetting();
                    setting.RemoveKeys(currentID);
                    EditorUtility.SetDirty(setting);
                }
            }
        }

        private bool IsInEditMode()
        {
            return Application.isEditor &&
                   Application.isPlaying == false &&
                   EditorApplication.isPlayingOrWillChangePlaymode == false;
        }
        */
    }
}
