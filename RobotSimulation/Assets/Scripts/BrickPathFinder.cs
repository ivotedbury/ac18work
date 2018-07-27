using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPathFinder
{

    public int totalCostOfTrip;

    public List<Cell> CalculatePathForSequencing(Grid _inputGrid, List<Cell> _availableCells, Cell _startCell, Cell _targetCell)
    {
        totalCostOfTrip = 0;
        List<Cell> waypoints = new List<Cell>();

        Cell startCell = _startCell;
        Cell targetCell = _targetCell;
        bool pathSuccess = false;

        List<Cell> openSet = new List<Cell>();
        List<Cell> closedSet = new List<Cell>();
        
        for (int z = 0; z < _inputGrid.gridSize.z; z++)
        {
            for (int y = 0; y < _inputGrid.gridSize.y; y++)
            {
                for (int x = 0; x < _inputGrid.gridSize.x; x++)
                {
                    _inputGrid.cellsArray[x, y, z].ResetCosts();

                    if (_availableCells.Contains(_inputGrid.cellsArray[x, y, z]))
                    {
                        _inputGrid.cellsArray[x, y, z].availableCost = 0;
                    }
                    else
                    {
                        _inputGrid.cellsArray[x, y, z].availableCost = 100000;
                    }
                }
            }
        }

        openSet.Add(_startCell);

        while (openSet.Count > 0)
        {
            Debug.Log(openSet.Count);

            Cell cell = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < cell.fCost || openSet[i].fCost == cell.fCost)
                {
                    if (openSet[i].hCost < cell.hCost)
                    {
                        cell = openSet[i];
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

            foreach (Cell neighbour in GetCellNeighbours(_inputGrid, cell))
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }
                                         
                int newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour); //cell.availableCost +

                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) //////////////////// PROBLEM HERE + neighbour.availableCost
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetCell);
                    neighbour.parent = cell;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startCell, targetCell);
            totalCostOfTrip = targetCell.gCost;
        }

        return waypoints;
    }

    List<Cell> RetracePath(Cell _startCell, Cell _endCell)
    {
        List<Cell> path = new List<Cell>();

        Cell currentCell = _endCell;

        while (currentCell != _startCell) // assemble path in reverse order
        {
            path.Add(currentCell);
            currentCell = currentCell.parent;
        }

        path.Add(_startCell);

        path.Reverse(); // reverse the path to the correct order
        return path;
    }

    int GetDistance(Cell _cellA, Cell _cellB)
    {
        int distance;

        int distX = Mathf.Abs(_cellB.position.x - _cellA.position.x);
        int distY = Mathf.Abs(_cellB.position.y - _cellA.position.y);
        int distZ = Mathf.Abs(_cellB.position.z - _cellA.position.z);

        int horizontalCost = 1;
        int verticalCost = 1000;
        
        distance = (distX * horizontalCost) + (distY * verticalCost) + (distZ * horizontalCost) + _cellB.availableCost;

        return distance;
    }

    List<Cell> GetCellNeighbours(Grid _inputGrid, Cell _testCell)
    {
        List<Cell> neighbours = new List<Cell>();

        if (_testCell.position.x > 3 && _testCell.position.x < _inputGrid.gridSize.x - 4
         && _testCell.position.y > 0 && _testCell.position.y < _inputGrid.gridSize.y - 4
         && _testCell.position.z > 3 && _testCell.position.z < _inputGrid.gridSize.z - 4)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -4; z <= 4; z++)
                {
                    if (z == 0)
                    {
                        continue;
                    }

                    neighbours.Add(_inputGrid.cellsArray[_testCell.position.x + 0, _testCell.position.y + y, _testCell.position.z + z]);
                }

                for (int x = -4; x <= 4; x++)
                {
                    if (x == 0)
                    {
                        continue;
                    }

                    neighbours.Add(_inputGrid.cellsArray[_testCell.position.x + x, _testCell.position.y + y, _testCell.position.z + 0]);
                }
            }
        }

        return neighbours;
    }

}
