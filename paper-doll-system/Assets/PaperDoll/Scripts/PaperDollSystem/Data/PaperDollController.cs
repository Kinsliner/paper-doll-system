using System;
using System.Collections.Generic;
using UnityEngine;

public class PaperDollController
{
    public Action<PaperDoll> OnPaperDollSetEvent;
    public Action<PaperDollCache> OnLockEvent;
    public Action<PaperDollCache> OnUnlockEvent;

    public class PaperDollCache
    {
        public int id;
        public BodyNode node;
        public GameObject attachObject;
        public Sprite icon;
        public Dictionary<BodyDirection, int> sortOrders = new Dictionary<BodyDirection, int>();
        public bool isLocked = false;
    }

    private List<PaperDollCache> paperDollCaches = new List<PaperDollCache>();
    private PaperDoll currentPaperDoll;

    public void Init()
    {
        List<PaperDollData> paperDollDatas = PaperDollManager.GetPaperDollDatas();
        paperDollDatas.ForEach(p =>
        {
            var paperDollCache = new PaperDollCache();
            paperDollCache.id = p.id;
            paperDollCache.node = p.node;
            paperDollCache.attachObject = ModelAssetManager.LoadModelAsset(p.assetID);
            paperDollCache.icon = Resources.Load<Sprite>(p.iconPath);
            p.sideDatas.ForEach(s => paperDollCache.sortOrders.Add(s.direction, s.overrideSortOrder));
            paperDollCaches.Add(paperDollCache);
        });
    }

    public List<PaperDollCache> GetPaperDollCaches(BodyNode node)
    {
        return paperDollCaches.FindAll(p => p.node == node);
    }

    /// <summary>
    /// 初始化PaperDoll
    /// </summary>
    /// <param name="paperDoll"></param>
    public void InitPaperDoll(PaperDoll paperDoll)
    {
        currentPaperDoll = paperDoll;

        var bodyNodes = Enum.GetValues(typeof(BodyNode));
        foreach (BodyNode node in bodyNodes)
        {
            var paperDollCache = paperDollCaches.Find(p => p.node == node);
            Attach(paperDollCache);
        }
    }

    /// <summary>
    /// 設置當前PaperDoll
    /// </summary>
    /// <param name="paperDoll"></param>
    /// <param name="bodyNodes"></param>
    public void SetupPaperDoll(PaperDoll paperDoll, params BodyNode[] bodyNodes)
    {
        currentPaperDoll = paperDoll;

        foreach (BodyNode node in bodyNodes)
        {
            var paperDollCache = paperDollCaches.Find(p => p.node == node);
            Attach(paperDollCache);
        }

        OnPaperDollSetEvent?.Invoke(currentPaperDoll);
    }

    /// <summary>
    /// 設置當前PaperDoll
    /// </summary>
    public void SetupPaperDoll(PaperDoll paperDoll)
    {
        currentPaperDoll = paperDoll;

        OnPaperDollSetEvent?.Invoke(currentPaperDoll);
    }

    /// <summary>
    /// 附加物件
    /// </summary>
    public void Attach(int id)
    {
        var paperDollCache = paperDollCaches.Find(p => p.id == id);
        Attach(paperDollCache);
    }

    /// <summary>
    /// 附加物件
    /// </summary>
    public void Attach(PaperDollCache paperDollCache)
    {
        if (paperDollCache != null && currentPaperDoll != null)
        {
            currentPaperDoll.Attach(paperDollCache);
            currentPaperDoll.SetDirection(currentPaperDoll.CurrentDirection);
        }
    }

    /// <summary>
    /// 旋轉至特定方向
    /// </summary>
    public void Turn(BodyDirection currentDirection)
    {
        if (currentPaperDoll != null)
        {
            currentPaperDoll.SetDirection(currentDirection);
        }
    }

    /// <summary>
    /// 鎖定
    /// </summary>
    /// <param name="id">紙娃娃資料ID</param>
    public void Lock(int id)
    {
        var find = paperDollCaches.Find(p => p.id == id);
        if (find != null)
        {
            find.isLocked = true;
            OnLockEvent?.Invoke(find);
        }
    }

    /// <summary>
    /// 解鎖
    /// </summary>
    /// <param name="id">紙娃娃資料ID</param>
    public void Unlock(int id)
    {
        var find = paperDollCaches.Find(p => p.id == id);
        if (find != null)
        {
            find.isLocked = false;
            OnUnlockEvent?.Invoke(find);
        }
    }
}