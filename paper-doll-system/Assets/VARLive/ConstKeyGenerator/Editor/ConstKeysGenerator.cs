using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace VARLive.Tool
{
    public abstract class ConstKeysGenerator
	{
        public string NamespaceName { get; private set; }

        public virtual string GetName()
		{
			return GetType().Name;
		}

        public void SetNamespace(string namespaceName)
        {
            NamespaceName = namespaceName;
        }

		public abstract void Generate();

        public void CreateConstSctipt(string className, List<string> keys)
        {
            CreateConstSctipt(className, "Scripts/Consts", keys);
        }

        public void CreateConstSctipt(string className, string scriptPath, List<string> keys)
        {
            GeneratorData keyGeneratorData = new TableKeyGeneratorData_String(keys, null);

            CreateConstSctipt(className, scriptPath, keyGeneratorData);
        }

        public void CreateConstSctipt(string className, string scriptPath, List<(string key, int value)> keyPairs)
        {
            GeneratorData keyGeneratorData = new TableKeyGeneratorData_Int(keyPairs, null);

            CreateConstSctipt(className, scriptPath, keyGeneratorData);
        }

        public void CreateConstSctipt(string className, string scriptPath, List<(string key, float value)> keyPairs)
        {
            GeneratorData keyGeneratorData = new TableKeyGeneratorData_Float(keyPairs, null);

            CreateConstSctipt(className, scriptPath, keyGeneratorData);
        }

        private void CreateConstSctipt(string className, string scriptPath, GeneratorData tableKeyGeneratorData)
        {
            GeneratorClassSetting classSetting = new GeneratorClassSetting();

            classSetting.drawSerializableAttribute = false;
            classSetting.isStaticClass = true;
            classSetting.className = className;

            ClassGeneratorData classData = new ClassGeneratorData(classSetting, new List<GeneratorData>() { tableKeyGeneratorData });

            NamespaceGeneratorData namespaceData = new NamespaceGeneratorData(new List<string>(), NamespaceName, new List<GeneratorData>() { classData });

            var lines = namespaceData.GetGeneratorLines();


            string path = $"{Application.dataPath}/{scriptPath}/{className}.cs";

            if (Directory.Exists(Path.GetDirectoryName(path)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            if (File.Exists(path) == false)
            {
                var file = File.Create(path);
                file.Close();
            }

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                lines.ForEach(line =>
                {
                    writer.WriteLine(line);
                });
            }
        }
    }
}
