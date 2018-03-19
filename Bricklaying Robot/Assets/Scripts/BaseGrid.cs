using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGrid
{

    public Cell[,,] cells;
    public Vector3Int gridSize;

    public BaseGrid(Vector3Int _gridSize) // creates a base Grid and populates with Cells in the grid coordinate locations
    {
        gridSize = _gridSize;
        cells = new Cell[gridSize.x, gridSize.y, gridSize.z]; // grid size determines the size of the cell array

        for (int z = 0; z < gridSize.z; z++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    cells[x, y, z] = new Cell(new Vector3Int(x, y, z)); // creates a cell in each grid coordinate
                }
            }
        }
    }

    public List<Cell> GetChildren(Brick brick)
    {
        var childCells = new List<Cell>();

        if (brick.rotation == Quaternion.Euler(0, 0, 0) || brick.rotation == Quaternion.Euler(0, 180, 0))
        {
            childCells.Add(cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z + 1]);
            childCells.Add(cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z]);
            childCells.Add(cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z - 1]);
        }

        if (brick.rotation == Quaternion.Euler(0, 90, 0) || brick.rotation == Quaternion.Euler(0, 270, 0))
        {
            childCells.Add(cells[brick.originCell.position.x + 1, brick.originCell.position.y, brick.originCell.position.z]);
            childCells.Add(cells[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z]);
            childCells.Add(cells[brick.originCell.position.x - 1, brick.originCell.position.y, brick.originCell.position.z]);
        }

        return childCells;
    }

 

}

