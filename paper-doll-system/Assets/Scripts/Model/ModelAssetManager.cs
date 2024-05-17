using System.Collections.Generic;
using UnityEngine;

public static class ModelAssetManager
{
    private class PreloadAssetData
    {
        public int id;
        public GameObject gameObject;
    }

    private static EzDataLoader projectDataLoader = new EzDataLoader("ModelAssetData");
    private static Dictionary<int, ModelAssetData> modelAssetDic = new Dictionary<int, ModelAssetData>();
    private static List<PreloadAssetData> preloadAssetDatas = new List<PreloadAssetData>();

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        InitData();
    }

    /// <summary>
    /// 初始化資料
    /// </summary>
    private static void InitData()
    {
        modelAssetDic.Clear();
        var datas = projectDataLoader.LoadData<ModelAssetDataTable>().datas;
        datas.ForEach(p =>
        {
            modelAssetDic.Add(p.assetID, p);
        });

        PreloadAsset();
    }

    /// <summary>
    /// 取得模型資料
    /// </summary>
    public static ModelAssetData GetModelAssetData(int id)
    {
        if (modelAssetDic.TryGetValue(id, out ModelAssetData modelAssetData))
        {
            return modelAssetData;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 取得模型資源
    /// </summary>
    public static GameObject LoadModelAsset(int id)
    {
        // 優先使用預載資源
        for (int i = 0; i < preloadAssetDatas.Count; i++)
        {
            if (preloadAssetDatas[i].id == id)
            {
                GameObject gameObject = preloadAssetDatas[i].gameObject;
                return gameObject;
            }
        }

        // 如果沒有預載資源，則載入資源
        GameObject obj = null;
        if (modelAssetDic.TryGetValue(id, out ModelAssetData modelAssetData))
        {
            switch (modelAssetData.provideType)
            {
                case ModelProvideType.AssetBundle:
                    //obj = AssetManager.Instance.LoadAsset<GameObject>(modelAssetData.bundletId);
                    break;
                case ModelProvideType.Resource:
                    obj = Resources.Load<GameObject>(modelAssetData.assetPath);
                    break;
            }
        }

        // 加入預載資源
        if (obj != null)
        {
            preloadAssetDatas.Add(new PreloadAssetData()
            {
                id = id,
                gameObject = obj
            });
        }

        return obj;
    }

    /// <summary>
    /// 預載資源
    /// </summary>
    public static void PreloadAsset()
    {
        foreach (var modelAssetData in modelAssetDic.Values)
        {
            LoadModelAsset(modelAssetData.assetID);
        }
    }
}