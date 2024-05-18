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

    public interface IReadWrite
    {
        bool IsVaildPath(string path);
        void WriteFile(string path, string content);
        string ReadFile(string path);
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

    public class FileIOStrategy : IReadWrite
    {
        public void WriteFile(string path, string content)
        {
            string dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(content);
                }
            }
        }

        public string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    return reader.ReadToEnd();
                }
            }
            else
            {
                Debug.LogError("File not found: " + path);
                return null;
            }
        }

        public bool IsVaildPath(string path)
        {
            return File.Exists(path);
        }
    }

    public class FileHandler
    {
        private IPath path = new BuildInSettingPath();
        private IDataParser parser = new UnityJsonParser();
        private IReadWrite readWrite = new FileIOStrategy();

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
            string content = parser.ParseTo(file);
            readWrite.WriteFile(filePath, content);
        }

        public string LoadAsText<T>()
        {
            string filePath = GetFilePath<T>();
            if (readWrite.IsVaildPath(filePath))
            {
                return readWrite.ReadFile(filePath);
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
                return string.Empty;
            }
        }

        public T Load<T>() where T : new()
        {
            string filePath = GetFilePath<T>();

            if (readWrite.IsVaildPath(filePath))
            {
                string content = readWrite.ReadFile(filePath);
                if (string.IsNullOrEmpty(content))
                {
                    return new T();
                }
                else
                {
                    T setting = parser.ParseFrom<T>(content);
                    return setting;
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

        /// <summary>
        /// 讀取資料夾下所有檔案，僅支援IO讀寫
        /// </summary>
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
