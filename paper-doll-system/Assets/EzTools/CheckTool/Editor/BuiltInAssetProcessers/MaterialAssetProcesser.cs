using UnityEngine;

public class MaterialAssetProcesser : AssetProcesser
{
    public override string Name => "材質處理器";

    public override bool IsProcessable(Object asset)
    {
        return true;
    }

    public override void Process(Object asset)
    {
        if(asset is Material)
        {
            Material material = asset as Material;
            material.enableInstancing = true;
        }
    }
}