using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {

    public Cell[,,] cells;
    public Vector3Int gridSize;

    public List<Cell> allCells = new List<Cell>();

    public Grid(Vector3Int _gridSize)
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
                    allCells.Add(cells[x, y, z]);
                }
            }
        }
    }
}
