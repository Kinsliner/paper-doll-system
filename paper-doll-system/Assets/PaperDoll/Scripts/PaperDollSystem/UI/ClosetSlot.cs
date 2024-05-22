using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetSlot : MonoBehaviour
{
    public Action<ClosetSlot, PaperDollController.PaperDollCache> OnClickEvent;

    [System.Serializable]
    private struct SlotView
    {
        public BodyNode node;
        [UIKey("ClosetSlot")]
        public string rootKey;
        [UIKey("ClosetSlot")]
        public string iconKey;
    }

    public bool IsEmpty { get; private set; }

    public PaperDollController.PaperDollCache Cache => cache;

    [SerializeField]
    private UICollector uiCollector;

    [SerializeField]
    private List<SlotView> slotViews = new List<SlotView>();

    private PaperDollController.PaperDollCache cache;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        DeactiveSlot();

        Unselect();

        uiCollector.BindOnClick(UIKey.ClosetSlot_Button, OnCheck);
    }

    private void OnCheck()
    {
        OnClickEvent?.Invoke(this, cache);
    }

    /// <summary>
    /// 清除
    /// </summary>
    public void Clear()
    {
        DeactiveSlot();
        IsEmpty = true;
    }

    private void DeactiveSlot()
    {
        foreach (var slotView in slotViews)
        {
            uiCollector.SetActive(slotView.rootKey, false);
        }

        uiCollector.SetInteractable(UIKey.ClosetSlot_Button, false);
    }

    /// <summary>
    /// 顯示
    /// </summary>
    public void Display(PaperDollController.PaperDollCache paperDollCache)
    {
        cache = paperDollCache;
        SetActiveRoot(paperDollCache.node);
        SetIcon(paperDollCache.node, paperDollCache.icon);
        IsEmpty = false;
        uiCollector.SetInteractable(UIKey.ClosetSlot_Button, true);
    }

    private void SetActiveRoot(BodyNode node)
    {
        // active the root
        foreach (var slotView in slotViews)
        {
            if (slotView.node == node)
            {
                uiCollector.SetActive(slotView.rootKey, true);
            }
            else
            {
                uiCollector.SetActive(slotView.rootKey, false);
            }
        }
    }

    private void SetIcon(BodyNode node, Sprite icon)
    {
        // set icon
        foreach (var slotView in slotViews)
        {
            if (slotView.node == node)
            {
                uiCollector.SetImage(slotView.iconKey, icon);
                break;
            }
        }
    }

    /// <summary>
    /// 顯示選取效果
    /// </summary>
    public void Select()
    {
        uiCollector.SetActive(UIKey.ClosetSlot_Select, true);
    }

    /// <summary>
    /// 顯示未選取效果
    /// </summary>
    public void Unselect()
    {
        uiCollector.SetActive(UIKey.ClosetSlot_Select, false);
    }
}
