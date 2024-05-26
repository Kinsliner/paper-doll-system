using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ClosetPanelUI : MonoBehaviour
{
    [SerializeField]
    private UICollector uiCollector;

    [SerializeField]
    private GameObject bodyNodeButtonPrefab;

    [SerializeField]
    private GameObject closetSlotPrefab;

    [SerializeField]
    private int slotPerPage = 10;

    private PaperDollController paperDollController;
    private List<BodyNodeButton> bodyNodeButtons = new List<BodyNodeButton>();
    private List<ClosetSlot> closetSlots = new List<ClosetSlot>();
    private List<PaperDollController.PaperDollCache> currentCaches = new List<PaperDollController.PaperDollCache>();
    private PaperDoll currentPaperDoll;
    private BodyNode currentDisplayNode = BodyNode.Head;
    private int currentPage = 0;
    private int maxPage = 0;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        paperDollController = PaperDollManager.Controller;
        paperDollController.OnPaperDollSetEvent += OnPaperDollSet;

        BuildBodyNodeButtons();
        BuildClosetSlots();
        SetupPageButtons();
    }

    private void OnPaperDollSet(PaperDoll doll)
    {
        currentPaperDoll = doll;

        RefreshClosetByPaperDoll();
    }

    private void BuildBodyNodeButtons()
    {
        var root = uiCollector.GetRectTransform(UIKey.Closet_BodyNodeButtonRoot);

        var options = Enum.GetValues(typeof(BodyNode));

        // 清除舊的按鈕
        foreach (BodyNodeButton button in bodyNodeButtons)
        {
            button.OnClickEvent -= OnBodyNodeButtonClicked;
            Destroy(button.gameObject);
        }
        bodyNodeButtons.Clear();

        // 建立新的按鈕
        foreach (BodyNode option in options)
        {
            var button = Instantiate(bodyNodeButtonPrefab, root);
            var bodyNodeButton = button.GetComponent<BodyNodeButton>();
            bodyNodeButton.Init(option);
            bodyNodeButton.OnClickEvent += OnBodyNodeButtonClicked;
            bodyNodeButtons.Add(bodyNodeButton);
        }
    }

    private void OnBodyNodeButtonClicked(BodyNodeButton sender, BodyNode node)
    {
        currentDisplayNode = node;

        RefreshClosetByPaperDoll();

        // 通知按鈕選取
        sender.SetSelected(true);

        // 通知其他按鈕取消選取
        foreach (BodyNodeButton otherButton in bodyNodeButtons)
        {
            if (otherButton != sender)
            {
                otherButton.SetSelected(false);
            }
        }
    }

    private void BuildClosetSlots()
    {
        var root = uiCollector.GetRectTransform(UIKey.Closet_SlotRoot);

        // 清除舊的 Slot
        foreach (ClosetSlot slot in closetSlots)
        {
            slot.OnClickEvent -= OnClosetSlotClicked;
            Destroy(slot.gameObject);
        }
        closetSlots.Clear();

        // 建立新的 Slot
        for (int i = 0; i < slotPerPage; i++)
        {
            var slot = Instantiate(closetSlotPrefab, root);
            var closetSlot = slot.GetComponent<ClosetSlot>();
            closetSlot.Init();
            closetSlot.OnClickEvent += OnClosetSlotClicked;
            closetSlots.Add(closetSlot);
        }
    }

    private void OnClosetSlotClicked(ClosetSlot slot, PaperDollController.PaperDollCache cache)
    {
        // 附加物件
        paperDollController.Attach(cache);

        // 通知Slot選取
        slot.Select();

        // 通知其他Slot取消選取
        foreach (ClosetSlot otherSlot in closetSlots)
        {
            if (otherSlot != slot)
            {
                otherSlot.Unselect();
            }
        }
    }


    private void SetupPageButtons()
    {
        uiCollector.BindOnClick(UIKey.Closet_PrevButton, OnPreviousButtonClicked);
        uiCollector.BindOnClick(UIKey.Closet_NextButton, OnNextButtonClicked);
    }

    private void OnNextButtonClicked()
    {
        ToPage(currentPage + 1);
    }

    private void OnPreviousButtonClicked()
    {
        ToPage(currentPage - 1);
    }

    /// <summary>
    /// 切換到指定頁數
    /// </summary>
    /// <param name="page">頁數</param>
    public void ToPage(int page)
    {
        currentPage = Mathf.Clamp(page, 1, maxPage);
        uiCollector.SetText(UIKey.Closet_PageText, $"{currentPage}/{maxPage}");
        DisplayBodyItems(currentPage);
    }

    /// <summary>
    /// 重新整理目前部位的衣櫃，並顯示在第一頁
    /// </summary>
    public void RefreshCloset()
    {
        // 取得部位的所有項目
        currentCaches = paperDollController.GetPaperDollCaches(currentDisplayNode);

        // 重置頁數
        ResetPages();

        // 顯示項目在Slot上
        DisplayBodyItems(currentPage);
    }

    /// <summary>
    /// 重新整理指定部位的衣櫃，並顯示在第一頁
    /// </summary>
    /// <param name="node"></param>
    public void RefreshCloset(BodyNode node)
    {
        // 設定目前部位
        currentDisplayNode = node;

        // 取得部位的所有項目
        currentCaches = paperDollController.GetPaperDollCaches(currentDisplayNode);

        // 重置頁數
        ResetPages();

        // 顯示項目在Slot上
        DisplayBodyItems(currentPage);
    }

    /// <summary>
    /// 重新整理指定部位的衣櫃，並指定要跳到的頁數
    /// </summary>
    public void RefeshCloset(BodyNode node, int page)
    {
        // 設定目前部位
        currentDisplayNode = node;

        // 取得部位的所有項目
        currentCaches = paperDollController.GetPaperDollCaches(currentDisplayNode);

        // 重置並跳到指定頁數
        ResetToPage(page);

        // 顯示項目在Slot上
        DisplayBodyItems(currentPage);
    }

    /// <summary>
    /// 重置頁面資料
    /// </summary>
    private void ResetPages()
    {
        // 計算總頁數
        maxPage = Mathf.CeilToInt((float)currentCaches.Count / slotPerPage);
        currentPage = 1;

        // 顯示頁數
        uiCollector.SetText(UIKey.Closet_PageText, $"{currentPage}/{maxPage}");
    }

    /// <summary>
    /// 重置頁面資料，並指定要跳到的頁數
    /// </summary>
    private void ResetToPage(int toPage)
    {
        // 計算總頁數
        maxPage = Mathf.CeilToInt((float)currentCaches.Count / slotPerPage);
        currentPage = Mathf.Clamp(toPage, 1, maxPage);

        // 顯示頁數
        uiCollector.SetText(UIKey.Closet_PageText, $"{currentPage}/{maxPage}");
    }

    /// <summary>
    /// 顯示指定頁數的項目在Slot上
    /// </summary>
    private void DisplayBodyItems(int page)
    {
        // 清除Slot
        foreach (ClosetSlot slot in closetSlots)
        {
            slot.Clear();
        }

        // 計算要顯示的範圍
        int startIndex = (page - 1).Min(0) * slotPerPage;
        int endIndex = Mathf.Min(startIndex + slotPerPage, currentCaches.Count);
        
        // 防呆
        int displayCount = endIndex - startIndex;
        if (displayCount == 0)
        {
            return;
        }

        // 顯示項目
        for (int i = startIndex; i < endIndex; i++)
        {
            closetSlots[i - startIndex].Display(currentCaches[i]);
        }
    }

    /// <summary>
    /// 根據PaperDoll更新衣櫃
    /// </summary>
    public void RefreshClosetByPaperDoll()
    {
        if (currentPaperDoll != null)
        {
            var cache = currentPaperDoll.GetCache(currentDisplayNode);
            if (cache != null)
            {
                int page = FindPageByCache(cache);
                RefeshCloset(currentDisplayNode, page);
                Select(cache);
            }
            else
            {
                RefreshCloset(currentDisplayNode);
            }
        }
    }

    /// <summary>
    /// 尋找指定Cache所在的頁數
    /// </summary>
    public int FindPageByCache(PaperDollController.PaperDollCache cache)
    {
        int index = currentCaches.FindIndex(p => p == cache);
        if (index < 0)
        {
            return -1;
        }

        return Mathf.CeilToInt((float)(index + 1) / slotPerPage);
    }

    /// <summary>
    /// 選取指定Cache
    /// </summary>
    private void Select(PaperDollController.PaperDollCache cache)
    {
        // 選取指定Slot
        foreach (ClosetSlot slot in closetSlots)
        {
            if (slot.Cache == cache)
            {
                slot.Select();
            }
            else
            {
                slot.Unselect();
            }
        }

        // 選取指定BodyNodeButton
        foreach (BodyNodeButton button in bodyNodeButtons)
        {
            if (button.Node == cache.node)
            {
                button.SetSelected(true);
            }
            else
            {
                button.SetSelected(false);
            }
        }

    }
}
