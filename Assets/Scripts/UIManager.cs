using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    instance = obj.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }

    private UIRegistry uiRegistry;
    private UIController uiController;

    void Awake()
    {
        uiRegistry = GetComponent<UIRegistry>();
        if (uiRegistry == null)
        {
            uiRegistry = gameObject.AddComponent<UIRegistry>();
        }

        uiController = GetComponent<UIController>();
        if (uiController == null)
        {
            uiController = gameObject.AddComponent<UIController>();
        }
    }

    public void RegisterUIElement(string uiName, GameObject uiObject, string category = "default")
    {
        uiRegistry.RegisterUI(uiName, uiObject, category);
    }

    public void UnregisterUIElement(string uiName, string category = "default")
    {
        uiRegistry.UnregisterUI(uiName, category);
    }

    public void ToggleUIElement(string uiName)
    {
        uiController.ToggleUI(uiName);
    }

    // 这里可以添加更多的方法来处理高层逻辑和UI的复杂交互

    // 例如，关闭所有UI元素的方法
    public void CloseAllUIElements()
    {
        // 这里的实现将依赖于UIRegistry和UIController
        // 比如，遍历所有注册的UI元素并调用UIController关闭它们
    }

    // 更多的方法和逻辑可以根据游戏的具体需求来添加
}
