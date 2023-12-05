using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Cell
{
    public TerrainType Terrain { get; set; }
    public int PieceId { get; set; }

    public Cell(TerrainType terrain)
    {
        Terrain = terrain;
        PieceId = -1; // -1 表示没有棋子
    }
}
public enum TerrainType
{
    Plain,
    Mountain,
    Forest,
    Water
}
