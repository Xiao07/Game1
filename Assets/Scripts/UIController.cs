using UnityEngine;

public class UIController : MonoBehaviour
{
    private UIRegistry uiRegistry;

    void Start()
    {
        uiRegistry = FindObjectOfType<UIRegistry>();
    }

    public void ToggleUI(string uiName)
    {
        GameObject uiObject;
        if (uiRegistry.TryGetUIElement(uiName, out uiObject))
        {
            uiObject.SetActive(!uiObject.activeSelf);
        }
        else
        {
            Debug.LogError($"UI '{uiName}' not found.");
        }
    }

    // 可以添加其他UI控制方法
}
