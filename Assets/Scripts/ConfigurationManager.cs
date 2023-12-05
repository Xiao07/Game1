using UnityEngine;
using System.Collections.Generic;


public class ConfigurationManager : MonoBehaviour
{
    public static ConfigurationManager Instance { get; private set; }

    private Dictionary<string, string> jsonFiles = new Dictionary<string, string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadJsonFile(string path)
    {
        // 加载逻辑...
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile != null)
        {
            string jsonContent = jsonFile.text;
            jsonFiles[path] = jsonContent; // 存储JSON内容以便以后检索
            Debug.Log($"Loaded JSON file '{path}'");
        }
        else
        {
            Debug.LogError($"Failed to load JSON file: {path}");
        }
    }

    public string GetJsonContent(string path)
    {
        // 获取内容逻辑...
        if (jsonFiles.TryGetValue(path, out string jsonContent))
        {
            return jsonContent;
        }
        else
        {
            Debug.LogError($"JSON file not loaded: {path}");
            return null;
        }
    }

    // 其他方法...
}
