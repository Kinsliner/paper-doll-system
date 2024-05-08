using UnityEditor;
using UnityEngine;

public abstract class AssetChecker
{
    public bool IsEnable { get; set; } = true;

    public virtual string Name => GetType().Name;

    public virtual string GetDescription()
    {
        return string.Empty;
    }

    public virtual bool IsCheckable(Object asset)
    {
        return true;
    }

    public abstract CheckReport Check(Object asset);

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
