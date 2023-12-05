using UnityEngine;

public enum GameState
{
    MainMenu,
    GamePlaying,
    GamePaused,
    GameOver
}


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                // 初始化主菜单逻辑
                break;
            case GameState.GamePlaying:
                ResumeGame();
                // 开始游戏逻辑
                break;
            case GameState.GamePaused:
                PauseGame();
                // 暂停游戏逻辑
                break;
            case GameState.GameOver:
                // 游戏结束逻辑
                break;
        }
    }

    // 在这里添加其他方法，比如用于暂停、继续、结束游戏的方法
    private void PauseGame()
    {
        Time.timeScale = 0; // 停止时间
        // 还可以在这里添加暂停游戏的其他逻辑，如显示暂停菜单
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // 恢复时间
        // 还可以在这里添加恢复游戏的其他逻辑
    }

    // 更新方法
    void Update()
    {
        switch (CurrentState)
        {
            case GameState.MainMenu:
                // 主菜单逻辑
                break;
            case GameState.GamePlaying:
                // 正常游戏更新逻辑
                break;
            case GameState.GamePaused:
                // 暂停时的更新逻辑，可能只是响应暂停菜单的操作
                break;
            // 其他状态的更新逻辑
            case GameState.GameOver:
                // 游戏结束逻辑
                break;
        }
    }
}
