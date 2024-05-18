using UnityEditor;
using UnityEngine;

public abstract class AssetProcesser
{
    public virtual string Name => GetType().Name;

    public abstract bool IsProcessable(Object asset);

    public abstract void Process(Object asset);

    public virtual void OnGUI()
    {
    }

    protected AssetImporter GetAssetImporter(Object asset)
    {
        if (asset == null)
        {
            return null;
        }

        string assetPath = AssetDatabase.GetAssetPath(asset);
        if (assetPath.IsNullOrEmpty())
        {
            return null;
        }

        return AssetImporter.GetAtPath(assetPath);
    }

    protected bool IsAssetOf<T>(Object asset) where T : AssetImporter
    {
        return GetAssetImporter(asset) is T;
    }
}