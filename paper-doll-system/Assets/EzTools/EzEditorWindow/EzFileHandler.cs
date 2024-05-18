using System.IO;
using UnityEngine;
using Ez;

namespace Ez.EzEditor
{
    public class EzFileHandler : IPath, IDataParser
    {
        public const string DataFolder = "GameData";

        public string SubFolderPath { get; set; }

        public string DataName { get; set; }

        public EzFileHandler()
        {
        }

        public EzFileHandler(string subFolderPath)
        {
            SubFolderPath = subFolderPath;
        }

        public virtual string GetExtension()
        {
            return ".ez";
        }

        public virtual string GetName<T>()
        {
            if (string.IsNullOrEmpty(DataName) == false)
            {
                return DataName;
            }

            return typeof(T).Name;
        }

        public virtual string GetPath()
        {
            var appDir = new DirectoryInfo(Application.dataPath);
            string gameDataDir = Path.Combine(appDir.Parent.FullName, DataFolder);
            if (string.IsNullOrEmpty(SubFolderPath) == false)
            {
                gameDataDir = Path.Combine(gameDataDir, SubFolderPath);
            }

            return gameDataDir;
        }

        public virtual T ParseFrom<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }

        public virtual string ParseTo(object data)
        {
            return JsonUtility.ToJson(data, true);
        }
    }
}
