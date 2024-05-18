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
}