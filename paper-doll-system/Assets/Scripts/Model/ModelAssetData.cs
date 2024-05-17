using System.Collections;
using System.Collections.Generic;

public enum ModelProvideType
{
    AssetBundle,
    Resource
}

[System.Serializable]
public class ModelAssetDataTable
{
    public List<ModelAssetData> datas = new List<ModelAssetData>();
}

[System.Serializable]
public class ModelAssetData
{
    public ModelProvideType provideType;
    public int assetID;
    public string assetName;
    public int bundletId;
    public string assetPath;
}