using UnityEditor;
using UnityEngine;

public class ModelAssetProcesser : AssetProcesser
{
    public override string Name => "模型處理器";

    public override bool IsProcessable(Object asset)
    {
        return true;
    }

    public override void Process(Object asset)
    {
        ModelImporter modelImporter = GetAssetImporter(asset) as ModelImporter;
        if (modelImporter.IsNotNull())
        {
            modelImporter.isReadable = true;
            modelImporter.importBlendShapes = false;
            modelImporter.importVisibility = false;
            modelImporter.importCameras = false;
            modelImporter.importLights = false;
        }
    }
}
