using Ez;
using Ez.EzEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzDataLoader
{
    private FileHandler fileHandler;
    private EzFileHandler ezFileHandler = new EzFileHandler();

    public EzDataLoader(string dataName)
    {
        ezFileHandler.DataName = dataName;
        fileHandler = new FileHandler(ezFileHandler, ezFileHandler);
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
