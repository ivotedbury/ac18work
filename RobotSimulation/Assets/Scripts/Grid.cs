using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{

    public Cell[,,] cellsArray;
    public Vector3Int gridSize;
    public List<Cell> cellsList = new List<Cell>();

    public Grid(Vector3Int _gridSize)
    {
        gridSize = _gridSize;
        cellsArray = new Cell[gridSize.x, gridSize.y, gridSize.z]; // grid size determines the size of the cell array

        for (int z = 0; z < gridSize.z; z++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    cellsArray[x, y, z] = new Cell(new Vector3Int(x, y, z)); // creates a cell in each grid coordinate
                    cellsList.Add(cellsArray[x, y, z]);
                }
            }
        }
    }

    public List<Cell> GetChildren(Brick brick)
    {
        List<Cell> childCells = new List<Cell>();

        if (brick.brickType == 0)
        {

            if (brick.rotation == Quaternion.Euler(0, 0, 0) || brick.rotation == Quaternion.Euler(0, 180, 0))
            {
                childCells.Add(cellsArray[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z + 1]);
                childCells.Add(cellsArray[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z]);
                childCells.Add(cellsArray[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z - 1]);
            }

            if (brick.rotation == Quaternion.Euler(0, 90, 0) || brick.rotation == Quaternion.Euler(0, 270, 0))
            {
                childCells.Add(cellsArray[brick.originCell.position.x + 1, brick.originCell.position.y, brick.originCell.position.z]);
                childCells.Add(cellsArray[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z]);
                childCells.Add(cellsArray[brick.originCell.position.x - 1, brick.originCell.position.y, brick.originCell.position.z]);
            }
        }

        else if (brick.brickType == 1)
        {
            childCells.Add(cellsArray[brick.originCell.position.x, brick.originCell.position.y, brick.originCell.position.z]);
        }

        return childCells;
    }
}
