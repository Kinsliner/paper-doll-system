using Ez;
using Ez.EzEditor;
using System.Collections;
using System.Collections.Generic;
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

public class EzDataLoader
{
    private FileHandler fileHandler;
    private EzFileHandler ezFileHandler = new EzFileHandler();

    public EzDataLoader(string dataName)
    {
        ezFileHandler.DataName = dataName;
        fileHandler = new FileHandler(ezFileHandler, ezFileHandler);
    }

    public EzDataLoader(string dataName, string subFolderPath)
    {
        ezFileHandler.DataName = dataName;
        ezFileHandler.SubFolderPath = subFolderPath;
        fileHandler = new FileHandler(ezFileHandler, ezFileHandler);
    }

    public EzDataLoader(IPath path, IDataParser parser)
    {
        fileHandler = new FileHandler(path, parser);
    }

    public EzDataLoader(IPath path)
    {
        fileHandler = new FileHandler(path, ezFileHandler);
    }

    public EzDataLoader(IDataParser parser)
    {
        fileHandler = new FileHandler(ezFileHandler, parser);
    }

    public T LoadData<T>() where T : new()
    {
        fileHandler.SetParser(ezFileHandler);
        return fileHandler.Load<T>();
    }

    public string LoadAsText<T>()
    {
        return fileHandler.LoadAsText<T>();
    }
}
