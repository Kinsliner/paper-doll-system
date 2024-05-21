using System;
using System.Collections.Generic;
using UnityEngine;

public class PaperDollController
{
    public Action<PaperDoll> OnPaperDollSetEvent;

    public class PaperDollCache
    {
        public int id;
        public BodyNode node;
        public GameObject attachObject;
        public Sprite icon;
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
            paperDollCaches.Add(paperDollCache);
        });
    }

    public List<PaperDollCache> GetPaperDollCaches(BodyNode node)
    {
        return paperDollCaches.FindAll(p => p.node == node);
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
        }
    }
}