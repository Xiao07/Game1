using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChessboardLogic
{
    public Vector2Int gridSize = new Vector2Int(8, 8);
    private Cell[,] grid;

    public ChessboardLogic()
    {
        grid = new Cell[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = new Cell(TerrainType.Plain); // 初始化为平原地形
            }
        }
    }

    // 添加棋子
    public void AddPiece(int pieceId, Vector2Int position)
    {
        if (IsPositionValid(position) && grid[position.x, position.y].PieceId == -1)
        {
            grid[position.x, position.y].PieceId = pieceId;
        }
    }

    // 移动棋子
    public void MovePiece(Vector2Int from, Vector2Int to)
    {
        if (IsPositionValid(from) && IsPositionValid(to) && grid[from.x, from.y].PieceId != -1)
        {
            int pieceId = grid[from.x, from.y].PieceId;
            grid[to.x, to.y].PieceId = pieceId;
            grid[from.x, from.y].PieceId = -1;
        }
    }

    // 删除棋子
    public void RemovePiece(Vector2Int position)
    {
        if (IsPositionValid(position))
        {
            grid[position.x, position.y].PieceId = -1;
        }
    }

    // 检查特定位置是否有棋子
    public bool HasPiece(Vector2Int position)
    {
        return IsPositionValid(position) && grid[position.x, position.y].PieceId != -1;
    }

    private bool IsPositionValid(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSize.x && position.y >= 0 && position.y < gridSize.y;
    }
}
