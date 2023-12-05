using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int CurrentLevel { get; private set; }
    public GameObject[] levelUIPrefabs;

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

    public void LoadLevel(int levelNumber)
    {
        CurrentLevel = levelNumber;
        // 加载关卡逻辑
        // 可以调用 SceneManager 来加载相应的场景

        StartCoroutine(SetupLevel(levelNumber));
    }

    IEnumerator SetupLevel(int levelNumber)
    {
        // 等待场景加载完毕的逻辑
        // 初始化关卡数据，如角色位置、关卡状态等

        yield return null; // 等待场景加载完毕

        // 初始化战斗管理器和UI管理器
        InitializeBattleManager();
        InitializeUIManager();

        // 设置游戏状态到关卡开始
        GameStateManager.Instance.ChangeState(GameState.GamePlaying);

        // 其他初始化代码
    }

    void InitializeBattleManager()
    {
        // 初始化战斗管理器
    }

    void InitializeUIManager()
    {
        // 初始化UI
        LoadLevelUI(0); // 示例：加载第一个关卡的UI
    }
     public void LoadLevelUI(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelUIPrefabs.Length)
        {
            GameObject uiPrefab = levelUIPrefabs[levelIndex];
            if (uiPrefab != null)
            {
                Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogError("UI Prefab for level " + levelIndex + " is not set.");
            }
        }
    }

    // 其他方法，如结束关卡、暂停等
}
