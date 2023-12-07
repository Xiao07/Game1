using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }

    void Awake()
    {
        // 确保场景管理器在场景切换时不被销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        // 异步加载新场景
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        // 等待新场景加载完成
        while (!asyncLoad.isDone)
        {
            // 可以在这里添加加载进度的代码
            yield return null;
        }
    }

    public void UnloadScene(string sceneName)
    {
        // 异步卸载场景
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
    }

    // 可以添加更多管理场景的方法，如重载当前场景等
}
