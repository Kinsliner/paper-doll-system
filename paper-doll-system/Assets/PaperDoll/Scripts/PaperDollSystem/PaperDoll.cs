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
    Back
}

/// <summary>
/// 部位方向
/// </summary>
public enum BodyDirection
{
    Back,
    Left,
    Front,
    Right,
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
    public BodyDirection CurrentDirection => currentDirection;

    [SerializeField]
    private List<BodyPart> parts = new List<BodyPart>(); // 部位列表

    private BodyDirection currentDirection = BodyDirection.Front;

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

    /// <summary>
    /// 設定紙娃娃方向，會連動所有部位統一設定
    /// </summary>
    public void SetDirectionAndAnim(BodyDirection direction)
    {
        SetDirection(direction);

        switch (direction)
        {
            case BodyDirection.Front:
                ResetAnim("IdleFront");
                break;
            case BodyDirection.Back:
                ResetAnim("IdleBack");
                break;
            case BodyDirection.Left:
                ResetAnim("IdleLeft");
                break;
            case BodyDirection.Right:
                ResetAnim("IdleRight");
                break;
        }
    }

    /// <summary>
    /// 設定方向
    /// </summary>
    public void SetDirection(BodyDirection direction)
    {
        currentDirection = direction;
        SetOrderByDirection(direction);
    }

    /// <summary>
    /// 播放動畫，會連動所有部位統一播放
    /// </summary>
    public void PlayAnim(string animName)
    {
        foreach (var part in parts)
        {
            if (part.attachObject == null)
            {
                continue;
            }

            var animator = part.attachObject.GetComponent<Animator>();
            if (animator == null)
            {
                continue;
            }

            animator.Play(animName);
        }
    }

    /// <summary>
    /// 重置動畫
    /// </summary>
    public void ResetAnim(string animName)
    {
        foreach (var part in parts)
        {
            if (part.attachObject == null)
            {
                continue;
            }

            var animator = part.attachObject.GetComponent<Animator>();
            if (animator == null)
            {
                continue;
            }

            animator.Play(animName, 0, 0);
        }
    }

    /// <summary>
    /// 根據方向設定排序層級
    /// </summary>
    private void SetOrderByDirection(BodyDirection direction)
    {
        foreach (var part in parts)
        {
            if (part.attachObject == null)
            {
                continue;
            }

            var spriteRenderer = part.attachObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                continue;
            }

            if (part.CacheData == null)
            {
                continue;
            }

            if (part.CacheData.sortOrders.ContainsKey(direction) == false)
            {
                continue;
            }

            spriteRenderer.sortingOrder = part.CacheData.sortOrders[direction];
        }
    }
}
