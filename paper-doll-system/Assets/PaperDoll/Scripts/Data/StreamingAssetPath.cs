using Ez;
using System.IO;
using UnityEngine;

public class StreamingAssetPath : IPath
{
    public string DataName { get; set; }

    public StreamingAssetPath()
    {
    }

    public StreamingAssetPath(string dataName)
    {
        DataName = dataName;
    }

    public string GetName<T>()
    {
        if (string.IsNullOrEmpty(DataName) == false)
        {
            return DataName;
        }

        return typeof(T).Name;
    }

    public string GetPath()
    {
        return Path.Combine(Application.streamingAssetsPath, "GameData");
    }
}
