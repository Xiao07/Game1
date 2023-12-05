using System.Collections.Generic;
using UnityEngine;

public class UILoader : MonoBehaviour
{
    [System.Serializable]
    public struct UIPrefabEntry
    {
        public string name;
        public GameObject prefab;
    }

    public List<UIPrefabEntry> uiPrefabs;

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    void Awake()
    {
        // 填充字典
        foreach (var entry in uiPrefabs)
        {
            prefabDictionary[entry.name] = entry.prefab;
        }
    }

    public void LoadUI(string uiName)
    {
        if (prefabDictionary.TryGetValue(uiName, out GameObject prefab))
        {
            GameObject uiInstance = Instantiate(prefab, transform);
            UIManager.Instance.RegisterUIElement(uiName, uiInstance);
            // 根据需要设置其他UI属性
        }
        else
        {
            Debug.LogError($"UI prefab with name '{uiName}' not found.");
        }
    }

    // ...其他方法...
}
