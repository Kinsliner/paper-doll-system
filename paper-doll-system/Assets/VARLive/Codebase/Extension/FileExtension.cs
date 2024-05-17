using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileExtension
{
    /// <summary>
    /// 檔案改名
    /// </summary>
    public static void Rename(this FileInfo file, string newName)
    {
        if (file == null)
        {
            return;
        }

        if (file.Exists)
        {
            string newPath = Path.Combine(file.Directory.FullName, newName);
            file.MoveTo(newPath);
        }
    }

    
}
