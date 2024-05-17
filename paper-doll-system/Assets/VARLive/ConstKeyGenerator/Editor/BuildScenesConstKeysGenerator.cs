using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System;

namespace Ez.Tool
{
    public class BuildScenesConstKeysGenerator : ConstKeysGenerator
    {
        public override string GetName()
        {
            return "更新BuildSetting的Scenes";
        }

        public override void Generate()
        {
            List<string> sceneNames = new List<string>();

            EditorBuildSettings.scenes.ToList().ForEach(scene =>
            {
                if (string.IsNullOrEmpty(scene.path) == false)
                {
                    var processPath = RemoveStart(scene.path, "Assets/");
                    processPath = RemoveEnd(processPath, ".unity");

                    if (sceneNames.Contains(processPath) == false)
                    {
                        sceneNames.Add(processPath);
                    }
                }
            });

            CreateConstSctipt("Scenes", sceneNames);
        }

        public string RemoveStart(string oldStr, string remove)
        {
            if (oldStr.StartsWith(remove))
            {
                string processStr = oldStr;
                //注意 第二個值是偏移值 不是結束的index
                processStr = processStr.Substring(remove.Length, oldStr.Length - remove.Length);
                return processStr;
            }
            else
            {
                throw new Exception("並非以此作為開頭");
            }
        }

        public string RemoveEnd(string srcStr, string endStr)
        {
            if (srcStr.EndsWith(endStr))
            {
                return srcStr.Substring(0, srcStr.Length - endStr.Length);
            }
            else
            {
                return srcStr;
            }
        }
    }
}
