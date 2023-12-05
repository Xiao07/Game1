using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public GridCell[,] grid;  // 二维数组表示地图格子
    public List<EnemySpawnData> enemySpawnData;  // 怪物分布数据
    public LevelRules levelRules;  // 关卡规则
}

[System.Serializable]
public class EnemySpawnData
{
    public Vector2Int position;  // 怪物位置
    public int enemyType;  // 怪物类型
}

[System.Serializable]
public class LevelRules
{
    // 具体的关卡规则属性
}
[System.Serializable]
public class GridCell
{
    // 这里可以定义格子的属性
}
