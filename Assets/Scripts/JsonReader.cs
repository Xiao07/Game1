using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class JsonReader
{
    public static T LoadJsonFile<T>(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        else
        {
            Debug.LogError($"文件路径 '{path}' 不存在");
            return default(T);
        }
    }
}