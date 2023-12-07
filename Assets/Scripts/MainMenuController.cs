using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // 实现开始游戏的逻辑
        Debug.Log("开始游戏");
        // 调用场景管理器加载新场景
        MySceneManager.Instance.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        // 实现打开设置界面的逻辑
        Debug.Log("打开设置");
    }

    public void ExitGame()
    {
        // 实现退出游戏的逻辑
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
