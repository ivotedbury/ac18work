using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{

    public int totalCostofTrip;
    public int totalCostofBuild = 0;

    public List<Cell> FindPath(Grid inputGrid, Cell _startCell, Cell _targetCell)
    {
        List<Cell> waypoints = new List<Cell>();

        Cell startCell = _startCell;
        Cell targetCell = _targetCell;
        bool pathSuccess = false;

        List<Cell> openSet = new List<Cell>();
      List<Cell> closedSet = new List<Cell>();

        // reset costs from previous searches
        for (int z = 0; z < inputGrid.gridSize.z; z++)
        {
            for (int y = 0; y < inputGrid.gridSize.y; y++)
            {
                for (int x = 0; x < inputGrid.gridSize.x; x++)
                {
                   inputGrid.cells[x,y,z].resetCosts();
                    FilterCoveredCell(inputGrid.cells, inputGrid.cells[x, y, z]);
                    FilterSolidCell(inputGrid.cells, inputGrid.cells[x, y, z]);
                }
            }
        }

        //////////////////////////////







        return waypoints;
    }
}
