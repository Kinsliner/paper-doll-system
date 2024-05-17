using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ez.Tool;

public class UICollectorHelper
{
    [MenuItem("VAR Live/UI Collector/GetAssetKey %#k")]
    private static void GetAssetKey()
    {
        GameObject selectObject = Selection.activeGameObject;
        if (selectObject == null)
            return;

        bool hasKey = CheckAssetKey(selectObject, selectObject);
        if (hasKey == false)
        {
            //確認是否自己有UICollector，又被綁在父物件的UICollector上
            UICollector uiCollector = selectObject.GetComponent<UICollector>();
            if (uiCollector != null && uiCollector.gameObject == selectObject)
            {
                if (uiCollector.transform.parent != null)
                    hasKey = CheckAssetKey(uiCollector.transform.parent.gameObject,
                                           uiCollector.gameObject);
            }
        }

        //找不到Key
        if (hasKey == false)
        {
            Debug.Log("[UICollector] can't find key");
        }
    }

    private static bool CheckAssetKey(GameObject uiCollectorTarget, GameObject compareTarget)
    {
        bool hasKey = false;
        UICollector uiCollector = uiCollectorTarget.GetComponentInParent<UICollector>();
        if (uiCollector != null)
        {
            foreach (var uiAsset in uiCollector.UIAsset)
            {
                if (uiAsset.asset is GameObject obj)
                {
                    if (obj == compareTarget)
                    {
                        Debug.Log($"[UICollector] GameObject: {obj.name} key is {uiAsset.realKey}");
                        hasKey = true;
                    }
                }
                if (uiAsset.asset is Component comp)
                {
                    if (comp.gameObject == compareTarget)
                    {
                        Debug.Log($"[UICollector] Component: {comp.GetType().Name} key is {uiAsset.realKey}");
                        hasKey = true;
                    }
                }
            }
        }
        return hasKey;
    }
}
