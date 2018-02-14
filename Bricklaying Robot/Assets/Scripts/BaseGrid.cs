using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BaseGrid
{

    Cell[,,] _cells;
    Vector3Int _gridSize;

    public BaseGrid(Vector3Int gridSize)
    {
        _gridSize = gridSize;
        _cells = new Cell[gridSize.x, gridSize.y, gridSize.z];
    }

    public List<Cell> GetChildren(Cell originCell, Quaternion rotation)
    {
        var childCells = new List<Cell>();

        if (rotation == Quaternion.Euler(0, 0, 0) || rotation == Quaternion.Euler(0, 180, 0))
        {
            childCells.Add(_cells[originCell.position.x, originCell.position.y, originCell.position.z + 1]);
            childCells.Add(_cells[originCell.position.x, originCell.position.y, originCell.position.z]);
            childCells.Add(_cells[originCell.position.x, originCell.position.y, originCell.position.z - 1]);
        }

        if (rotation == Quaternion.Euler(0, 90, 0) || rotation == Quaternion.Euler(0, 270, 0))
        {
            childCells.Add(_cells[originCell.position.x + 1, originCell.position.y, originCell.position.z]);
            childCells.Add(_cells[originCell.position.x, originCell.position.y, originCell.position.z]);
            childCells.Add(_cells[originCell.position.x - 1, originCell.position.y, originCell.position.z]);
        }

        return childCells;

    }

}

