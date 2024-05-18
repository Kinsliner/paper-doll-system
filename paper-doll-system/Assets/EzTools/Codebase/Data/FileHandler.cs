using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ez
{
    public interface IPath
    {
        string GetName<T>();

        string GetPath();
    }

    public interface IDataParser
    {
        string GetExtension();

        T ParseFrom<T>(string data);

        string ParseTo(object data);
    }

    public class BuildInSettingPath : IPath
    {
        public string GetName<T>()
        {
            return typeof(T).Name;
        }

        public string GetPath()
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            string dataPath = Application.persistentDataPath;
            return dataPath;
#else
            DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath);
            return dirInfo.Parent.FullName;
#endif
        }
    }

    public class UnityJsonParser : IDataParser
    {
        public string GetExtension()
        {
            return ".json";
        }

        public T ParseFrom<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }

        public string ParseTo(object data)
        {
            return JsonUtility.ToJson(data, true);
        }
    }

    public class FileHandler
    {
        private IPath path = new BuildInSettingPath();
        private IDataParser parser = new UnityJsonParser();

        public FileHandler()
        {
        }

        public FileHandler(IPath path)
        {
            SetPath(path);
        }

        public FileHandler(IDataParser parser)
        {
            SetParser(parser);
        }

        public FileHandler(IPath path, IDataParser parser)
        {
            SetPath(path);
            SetParser(parser);
        }

        public void SetPath(IPath path)
        {
            this.path = path;
        }

        public void SetParser(IDataParser parser)
        {
            this.parser = parser;
        }

        public void Save<T>(T file)
        {
            string filePath = GetFilePath<T>();
            Save(filePath, file);
        }

        public void Save(string filePath, object file)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    string fileText = parser.ParseTo(file);
                    writer.Write(fileText);
                }
            }
        }

        public string LoadAsText<T>()
        {
            string filePath = GetFilePath<T>();
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return reader.ReadToEnd();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public T Load<T>() where T : new()
        {
            string filePath = GetFilePath<T>();

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string file = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(file))
                    {
                        return new T();
                    }
                    else
                    {
                        T setting = parser.ParseFrom<T>(file);
                        return setting;
                    }
                }
            }
            else
            {
                //找不到檔案，建立一個新檔案
                T fileClass = new T();
                Save(filePath, fileClass);
                return fileClass;
            }
        }

        public List<T> LoadAll<T>() where T : new()
        {
            List<T> list = new List<T>();
            string dir = path.GetPath();
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, $"*{parser.GetExtension()}");
                foreach (string file in files)
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        string fileText = reader.ReadToEnd();
                        T setting = parser.ParseFrom<T>(fileText);
                        list.Add(setting);
                    }
                }
            }
            return list;
        }

        private string GetFilePath<T>()
        {
            string fileName = $"{path.GetName<T>()}{parser.GetExtension()}";// default is xxx.json
            string filePath = Path.Combine(path.GetPath(), fileName);
            return filePath;
        }
    }
}
