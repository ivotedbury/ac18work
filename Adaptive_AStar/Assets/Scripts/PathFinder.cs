using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{

    public int totalCostofTrip;
    public int totalCostofBuild = 0;

    //Grid _grid;

    //public PathFinder(Grid grid)
    //{
    //    _grid = grid;
    //}

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
                    inputGrid.cells[x, y, z].resetCosts();
                    FilterCoveredCell(inputGrid, inputGrid.cells[x, y, z]);
                    FilterSolidCell(inputGrid.cells, inputGrid.cells[x, y, z]);
                }
            }
        }

        openSet.Add(startCell);

        while (openSet.Count > 0)
        {
            Cell cell = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].coveredCost == cell.coveredCost)
                {
                    if (openSet[i].fCost() < cell.fCost() || openSet[i].fCost() == cell.fCost())
                    {
                        if (openSet[i].hCost + openSet[i].mCost < cell.hCost + cell.mCost)
                        {
                            cell = openSet[i];
                        }
                    }
                }
            }

            openSet.Remove(cell);
            closedSet.Add(cell);

            if (cell == targetCell)
            {
                pathSuccess = true;
                break;
            }

            Cell neighbour = null;
            bool continueToNextNeighbour;


            for (int i = 0; i < GetCellNeighbours(inputGrid, cell).Count; i++)
            {
                continueToNextNeighbour = false;

                neighbour = GetCellNeighbours(inputGrid, cell)[i];

                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                List<Cell> pathToNeighbour = RetracePath(startCell, cell);

                for (int j = 0; j < pathToNeighbour.Count - 1; j++)
                {
                    if (neighbour.position.x == pathToNeighbour[j].position.x && neighbour.position.z == pathToNeighbour[j].position.z)
                    {
                        continueToNextNeighbour = true;
                    }
                }

                if (continueToNextNeighbour)
                {
                    continue;
                }

                int newCostToNeighbour = cell.bCost + GetDistance(cell, neighbour);
                if (neighbour.coveredCost == cell.coveredCost)
                {

                    if (newCostToNeighbour < neighbour.bCost + cell.mCost || !openSet.Contains(neighbour))
                    {
                        neighbour.bCost = cell.bCost + GetDistance(cell, neighbour);
                        neighbour.hCost = GetDistance(neighbour, targetCell);

                        neighbour.parent = cell;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }


        if (pathSuccess)
        {
            waypoints = RetracePath(startCell, targetCell);
        }

        return waypoints;
    }

    List<Cell> RetracePath(Cell startCell, Cell endCell)
    {
        List<Cell> path = new List<Cell>();

        Cell currentCell = endCell;

        while (currentCell != startCell) // assemble path in reverse order
        {
            path.Add(currentCell);
            currentCell = currentCell.parent;
        }

        path.Reverse(); // reverse the path to the correct order
        return path;
    }

    int GetDistance(Cell cellA, Cell cellB)
    {
        int distance;

        int distX = Mathf.Abs(cellB.position.x - cellA.position.x);
        int distY = Mathf.Abs(cellB.position.y - cellA.position.y);
        int distZ = Mathf.Abs(cellB.position.z - cellA.position.z);

        int horizontalCost = 1;
        int verticalCost = 10;

        if (cellB.isSolid == false)
        {
            distance = (distX * horizontalCost) + (distY * verticalCost) + (distZ * horizontalCost) + cellB.mCost;
        }
        else
        {
            distance = (distX * horizontalCost) + (distY * verticalCost) + (distZ * horizontalCost);
        }

        return distance;
    }

    void FilterCoveredCell(Grid inputGrid, Cell testCell)
    {
        int coveredCellCost = 1000000000;

        if (testCell.position.y != 0 && testCell.position.y < inputGrid.gridSize.y - 1)
        {
            if (inputGrid.cells[testCell.position.x, testCell.position.y + 1, testCell.position.z].isSolid == true)
            {
                testCell.coveredCost = coveredCellCost;
            }
        }
    }

    void FilterSolidCell(Cell[,,] allCells, Cell testCell)
    {
        int mCost = 100000;
        int startCellCost = 10000000;
        if (testCell.position.y != 0)
        {
            if (allCells[testCell.position.x, testCell.position.y - 1, testCell.position.z].isSolid != true && allCells[testCell.position.x, testCell.position.y, testCell.position.z].isSolid != true)
            {
                //if (allCells[testCell.position.x, testCell.position.y - 1, testCell.position.z].isGround != true)
                //{
                    testCell.mCost = testCell.position.y * mCost;
                //}
            }
            for (int i = 0; i < testCell.position.y; i++)
            {
                if (allCells[testCell.position.x, testCell.position.y - i, testCell.position.z].isSeed == true)
                {
                    testCell.mCost = startCellCost;
                }
            }
        }
    }

    List<Cell> GetCellNeighbours(Grid inputGrid, Cell testCell)
    {
        List<Cell> neighbours = new List<Cell>();

        if (testCell.position.x > 0 && testCell.position.x < inputGrid.gridSize.x - 1
         && testCell.position.y > 0 && testCell.position.y < inputGrid.gridSize.y - 1
         && testCell.position.z > 0 && testCell.position.z < inputGrid.gridSize.z - 1)
        {

            neighbours.Add(inputGrid.cells[testCell.position.x + 0, testCell.position.y + -1, testCell.position.z + 1]);
            neighbours.Add(inputGrid.cells[testCell.position.x + 1, testCell.position.y + -1, testCell.position.z + 0]);
            neighbours.Add(inputGrid.cells[testCell.position.x + 0, testCell.position.y + -1, testCell.position.z + -1]);
            neighbours.Add(inputGrid.cells[testCell.position.x + -1, testCell.position.y + -1, testCell.position.z + 0]);

            neighbours.Add(inputGrid.cells[testCell.position.x + 0, testCell.position.y + 0, testCell.position.z + 1]);
            neighbours.Add(inputGrid.cells[testCell.position.x + 1, testCell.position.y + 0, testCell.position.z + 0]);
            neighbours.Add(inputGrid.cells[testCell.position.x + 0, testCell.position.y + 0, testCell.position.z + -1]);
            neighbours.Add(inputGrid.cells[testCell.position.x + -1, testCell.position.y + 0, testCell.position.z + 0]);

            neighbours.Add(inputGrid.cells[testCell.position.x + 0, testCell.position.y + 1, testCell.position.z + 1]);
            neighbours.Add(inputGrid.cells[testCell.position.x + 1, testCell.position.y + 1, testCell.position.z + 0]);
            neighbours.Add(inputGrid.cells[testCell.position.x + 0, testCell.position.y + 1, testCell.position.z + -1]);
            neighbours.Add(inputGrid.cells[testCell.position.x + -1, testCell.position.y + 1, testCell.position.z + 0]);
        }
        return neighbours;
    }
}







