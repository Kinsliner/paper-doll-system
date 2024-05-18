using Ez.Tool;
using System.Collections;
using System.Collections.Generic;

public static class PaperDollManager
{
    public static PaperDollController Controller { get; private set; }

    private static StreamingAssetPath streamingAssetPath = new StreamingAssetPath("PaperDollData");
    private static EzDataLoader ezDataLoader = new EzDataLoader(streamingAssetPath);
    private static Dictionary<int, PaperDollData> paperDollDic = new Dictionary<int, PaperDollData>();

    public static void Init()
    {
        InitData();

        Controller = new PaperDollController();
        Controller.Init();
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

    public static PaperDollData GetPaperDollData(int id)
    {
        if (paperDollDic.ContainsKey(id))
        {
            return paperDollDic[id];
        }
        return null;
    }

    public static List<PaperDollData> GetPaperDollDatas()
    {
        return new List<PaperDollData>(paperDollDic.Values);
    }
}
