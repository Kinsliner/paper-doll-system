using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;

public static class PaperDollManager
{
    public static PaperDollController Controller { get; private set; }

    private static StreamingAssetPath streamingAssetPath = new StreamingAssetPath("PaperDollData");
    private static EzDataLoader ezDataLoader = new EzDataLoader(streamingAssetPath);
    private static Dictionary<int, PaperDollData> paperDollDic = new Dictionary<int, PaperDollData>();

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        InitData();

        Controller = new PaperDollController();
        Controller.Init();
    }

    /// <summary>
    /// 初始化資料
    /// </summary>
    public static void InitData()
    {
        paperDollDic.Clear();
        var datas = ezDataLoader.LoadData<PaperDollDataTable>().datas;
        datas.ForEach(p =>
        {
            paperDollDic.Add(p.id, p);
        });
    }

    /// <summary>
    /// 取得紙娃娃資料
    /// </summary>
    public static PaperDollData GetPaperDollData(int id)
    {
        if (paperDollDic.ContainsKey(id))
        {
            return paperDollDic[id];
        }
        return null;
    }

    /// <summary>
    /// 取得所有紙娃娃資料
    /// </summary>
    /// <returns></returns>
    public static List<PaperDollData> GetPaperDollDatas()
    {
        return new List<PaperDollData>(paperDollDic.Values);
    }

    /// <summary>
    /// 反初始化
    /// </summary>
    public static void Uninit()
    {
        Controller = null;
    }
}
