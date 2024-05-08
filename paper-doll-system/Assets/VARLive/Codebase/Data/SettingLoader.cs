using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VARLive;

public class SettingLoader : IPath, IDataParser
{
    private static FileHandler fileHandler = new FileHandler(new SettingLoader(), new SettingLoader());

    #region IPath, IDataParser
    public string GetName<T>()
    {
        return typeof(T).Name;
    }

    public string GetExtension()
    {
        return ".setting";
    }

    public string GetPath()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath);
        return Path.Combine(dirInfo.Parent.FullName, "Setting");
    }

    public T ParseFrom<T>(string data)
    {
        return JsonUtility.FromJson<T>(data);
    }

    public string ParseTo(object data)
    {
        return JsonUtility.ToJson(data, true);
    }
    #endregion

    public static T Load<T>() where T : new()
    {
        return fileHandler.Load<T>();
    }
}
