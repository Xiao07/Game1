using System.Collections.Generic;
using UnityEngine;

public class UIRegistry : MonoBehaviour
{
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();
    private Dictionary<string, List<GameObject>> categorizedElements = new Dictionary<string, List<GameObject>>();

    public void RegisterUI(string uiName, GameObject uiObject, string category = "default")
    {
        if (!uiElements.ContainsKey(uiName))
        {
            uiElements.Add(uiName, uiObject);

            if (!categorizedElements.ContainsKey(category))
            {
                categorizedElements.Add(category, new List<GameObject>());
            }
            categorizedElements[category].Add(uiObject);
        }
        else
        {
            Debug.LogWarning($"UI '{uiName}' is already registered.");
        }
    }
    public bool TryGetUIElement(string uiName, out GameObject uiObject)
    {
        return uiElements.TryGetValue(uiName, out uiObject);
    }

    public void UnregisterUI(string uiName, string category = "default")
    {
        if (uiElements.ContainsKey(uiName))
        {
            uiElements.Remove(uiName);
            categorizedElements[category]?.Remove(uiElements[uiName]);
        }
        else
        {
            Debug.LogWarning($"UI '{uiName}' not found during unregister operation.");
        }
    }

    // 可以添加获取特定分类UI元素的方法
}
