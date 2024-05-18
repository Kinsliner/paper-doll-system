using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PaperDollManager
{
    private static StreamingAssetPath streamingAssetPath = new StreamingAssetPath("PaperDollData");
    private static EzDataLoader ezDataLoader = new EzDataLoader(streamingAssetPath);
    private static Dictionary<int, PaperDollData> paperDollDic = new Dictionary<int, PaperDollData>();

    public static void Init()
    {
        InitData();
    }

    public static void InitData()
    {
        paperDollDic.Clear();
        var datas = ezDataLoader.LoadData<PaperDollDataTable>().datas;
        datas.ForEach(p =>
        {
            paperDollDic.Add(p.id, p);
        });
    }
}
