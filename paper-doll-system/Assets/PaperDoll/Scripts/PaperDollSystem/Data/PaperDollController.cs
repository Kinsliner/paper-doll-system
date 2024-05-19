using System.Collections.Generic;
using UnityEngine;

public class PaperDollController
{
    public class PaperDollCache
    {
        public int id;
        public BodyNode node;
        public GameObject attachObject;
    }

    public List<PaperDollCache> PaperDollCaches => paperDollCaches;

    private List<PaperDollCache> paperDollCaches = new List<PaperDollCache>();

    public void Init()
    {
        List<PaperDollData> paperDollDatas = PaperDollManager.GetPaperDollDatas();
        paperDollDatas.ForEach(p =>
        {
            var paperDollCache = new PaperDollCache();
            paperDollCache.id = p.id;
            paperDollCache.node = p.node;
            paperDollCache.attachObject = ModelAssetManager.LoadModelAsset(p.assetID);
            paperDollCaches.Add(paperDollCache);
        });
    }

    public List<PaperDollCache> GetPaperDollCaches(BodyNode node)
    {
        return paperDollCaches.FindAll(p => p.node == node);
    }
}