using System;
using System.Collections.Generic;
using UnityEngine;

public class PaperDollController
{
    public class PaperDollCache
    {
        public int id;
        public BodyNode node;
        public GameObject attachObject;
        public Sprite icon;
        public Dictionary<BodyDirection, int> sortOrders = new Dictionary<BodyDirection, int>();
        public bool isLocked = false;
    }

    public Action<PaperDoll> OnPaperDollSetEvent;
    public Action<PaperDollCache> OnLockEvent;
    public Action<PaperDollCache> OnUnlockEvent;

    private List<PaperDollCache> paperDollCaches = new List<PaperDollCache>();
    private PaperDoll currentPaperDoll;

    /// <summary>
    /// 初始化
    /// </summary>
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

    /// <summary>
    /// 取得該部位的所有Cache資料
    /// </summary>
    public List<PaperDollCache> GetPaperDollCaches(BodyNode node)
    {
        return paperDollCaches.FindAll(p => p.node == node);
    }

    /// <summary>
    /// 取得所有Cache資料
    /// </summary>
    public List<PaperDollCache> GetPaperDollCaches()
    {
        return new List<PaperDollCache>(paperDollCaches);
    }

    /// <summary>
    /// 從紙娃娃上取得部位Cache
    /// </summary>
    public List<PaperDollCache> GetCachesOnPaperDoll()
    {
        List<PaperDollCache> caches = new List<PaperDollCache>();
        var bodyNodes = Enum.GetValues(typeof(BodyNode));
        foreach (BodyNode node in bodyNodes)
        {
            PaperDollCache cache = currentPaperDoll.GetCache(node);
            if (cache != null)
            {
                caches.Add(cache);
            }
        }
        return caches;
    }

    /// <summary>
    /// 設置當前PaperDoll，並預設頭部和身體
    /// </summary>
    public void SetupPaperDoll(PaperDoll paperDoll)
    {
        SetupPaperDoll(paperDoll, BodyNode.Head, BodyNode.Body);
    }

    /// <summary>
    /// 設置當前PaperDoll
    /// </summary>
    public void SetupPaperDoll(PaperDoll paperDoll, params BodyNode[] defaultNodes)
    {
        currentPaperDoll = paperDoll;

        var bodyNodes = Enum.GetValues(typeof(BodyNode));
        var currentCaches = GetCachesOnPaperDoll();
        foreach (BodyNode node in bodyNodes)
        {
            var cache = currentCaches.Find(p => p.node == node);
            if (cache != null)
            {
                Attach(cache);
            }
            else
            {
                if (Array.Exists(defaultNodes, p => p == node))
                {
                    var defaultCache = paperDollCaches.Find(p => p.node == node);
                    Attach(defaultCache);
                }
            }
        }
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
            currentPaperDoll.SetDirectionAndAnim(currentPaperDoll.CurrentDirection);
        }
    }

    /// <summary>
    /// 旋轉至特定方向
    /// </summary>
    public void Turn(BodyDirection currentDirection)
    {
        if (currentPaperDoll != null)
        {
            currentPaperDoll.SetDirectionAndAnim(currentDirection);
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

    /// <summary>
    /// 反初始化
    /// </summary>
    public void Uninit()
    {
    }
}