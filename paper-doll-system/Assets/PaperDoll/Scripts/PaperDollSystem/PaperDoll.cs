using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部位節點
/// </summary>
public enum BodyNode
{
    Head,
    Body,
}

[System.Serializable]
public class BodyPart
{
    /// <summary>
    /// 部位節點
    /// </summary>
    public BodyNode node;

    /// <summary>
    /// 位置
    /// </summary>
    public Transform root;

    /// <summary>
    /// 部位物件
    /// </summary>
    [HideInInspector]
    public GameObject attachObject;

    /// <summary>
    /// 緩存資料
    /// </summary>
    public PaperDollController.PaperDollCache CacheData;
}

public class PaperDoll : MonoBehaviour
{
    [SerializeField]
    private List<BodyPart> parts = new List<BodyPart>(); // 部位列表

    void Start()
    {
    }

    /// <summary>
    /// 附加部位
    /// </summary>
    public void Attach(PaperDollController.PaperDollCache paperDollCache)
    {
        // 取得緩存資料
        var part = paperDollCache.attachObject;
        var node = paperDollCache.node;

        // 找到對應的部位
        var bodyPart = parts.Find(x => x.node == node);
        if (bodyPart == null)
        {
            Debug.LogError("找不到對應的部位");
            return;
        }

        // 更新緩存資料
        bodyPart.CacheData = paperDollCache;

        // 移除舊的物件
        if (bodyPart.attachObject != null)
        {
            Destroy(bodyPart.attachObject);
        }

        // 產生新的物件
        var clone = Instantiate(part, bodyPart.root);

        // 附加新的物件
        bodyPart.attachObject = clone;
    }

    /// <summary>
    /// 取得該部位的緩存資料
    /// </summary>
    public PaperDollController.PaperDollCache GetCache(BodyNode node)
    {
        var bodyPart = parts.Find(x => x.node == node);
        if (bodyPart == null)
        {
            return null;
        }

        return bodyPart.CacheData;
    }
}
