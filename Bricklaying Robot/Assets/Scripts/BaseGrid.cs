using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGrid
{

    public Cell[,,] _cells;
    public Vector3Int _gridSize;

    public BaseGrid(Vector3Int gridSize) // creates a base Grid and populates with Cells in the grid coordinate locations
    {
        _gridSize = gridSize;
        _cells = new Cell[gridSize.x, gridSize.y, gridSize.z]; // grid size determines the size of the cell array

        for (int z = 0; z < gridSize.z; z++) 
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    _cells[x, y, z] = new Cell(new Vector3Int(x, y, z)); // creates a cell in each grid coordinate
                }
            }
        }
    }

    public List<Cell> GetChildren(Brick brick) 
    {
        var childCells = new List<Cell>();

        if (brick.rotation == Quaternion.Euler(0, 0, 0) || brick.rotation == Quaternion.Euler(0, 180, 0))
        {
            childCells.Add(_cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z + 1]);
            childCells.Add(_cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z]);
            childCells.Add(_cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z - 1]);
        }

        if (brick.rotation == Quaternion.Euler(0, 90, 0) || brick.rotation == Quaternion.Euler(0, 270, 0))
        {
            childCells.Add(_cells[brick.originCell.position.x + 1, brick.originCell.position.y, brick.originCell.position.z]);
            childCells.Add(_cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z]);
            childCells.Add(_cells[brick.originCell.position.x - 1, brick.originCell.position.y, brick.originCell.position.z]);
        }

        return childCells;

    }


}

