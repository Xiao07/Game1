using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}

public class BattleManager : MonoBehaviour
{
    private List<Character> playerCharacters;
    private List<Character> enemyCharacters;
    private int currentPlayerIndex;
    public BattleState state;

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }
    void Update()
    {
        if (state == BattleState.PlayerTurn)
        {
            // 检查玩家行动，例如按钮点击或其他输入
        }
    }
    IEnumerator SetupBattle()
    {
        // 设置战斗场景
        // ...
        yield return new WaitForSeconds(1.0f);  // 等待1秒
        state = BattleState.PlayerTurn;
        PlayerTurn();
    }
    void PlayerTurn()
    {
        // 玩家回合逻辑
    }
    public void OnPlayerAction()
    {
        // 处理玩家的行动
        // 然后切换到下一个状态，例如 EnemyTurn 或者检查战斗是否结束
    }
}
