using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{

    int totalCostOfTrip;
    int totalCostOfBuild = 0;

    public List<Cell> FindPath(Grid _inputGrid, List<Cell> _availableCells, Cell _startCell, Cell _targetCell)
    {
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
                }
            }
        }

        openSet.Add(_startCell);

        while (openSet.Count > 0)
        {
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

            foreach (Cell neighbour in GetCellNeighbours(_inputGrid,cell))
            {
                if (!_availableCells.Contains(neighbour) || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
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
            totalCostOfTrip = waypoints[waypoints.Count - 1].gCost;
            totalCostOfBuild = totalCostOfBuild + totalCostOfTrip;

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
        int verticalCost = 10;

        distance = (distX * horizontalCost) + (distY * verticalCost) + (distZ * horizontalCost);

        return distance;
    }

    List<Cell> GetCellNeighbours(Grid _inputGrid, Cell _testCell)
    {
        List<Cell> neighbours = new List<Cell>();

        if (_testCell.position.x > 0 && _testCell.position.x < _inputGrid.gridSize.x - 1
         && _testCell.position.y > 0 && _testCell.position.y < _inputGrid.gridSize.y - 1
         && _testCell.position.z > 0 && _testCell.position.z < _inputGrid.gridSize.z - 1)
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

    //int GetDirection(Cell _startCell, Cell _testCell)
    //{
    //    int directionValue = 0;

    //    if (_testCell.position - _startCell.position == new Vector3Int(1, 0, 0))
    //    {
    //        directionValue = 0;
    //    }
    //    else if (_testCell.position - _startCell.position == new Vector3Int(0, 0, -1))
    //    {
    //        directionValue = 1;
    //    }
    //    else if (_testCell.position - _startCell.position == new Vector3Int(-1, 0, 0))
    //    {
    //        directionValue = 2;
    //    }
    //    else if (_testCell.position - _startCell.position == new Vector3Int(0, 0, 1))
    //    {
    //        directionValue = 3;
    //    }

    //    return directionValue;
    //}

    //Quaternion GetDirection(Cell _startCell, Cell _testCell)
    //{
    //    Quaternion currentDirection;

    //    if (_testCell.position - _startCell.position == )

    //    return currentDirection;
    //}

    //int GetDistance(Cell cellA, Cell cellB, int currentDirection, bool forNeighbourDistance)
    //{
    //    int distance;

    //    int distX = Mathf.Abs(cellA.position.x - cellB.position.x);
    //    int distY = Mathf.Abs(cellA.position.y - cellB.position.y);
    //    int distZ = Mathf.Abs(cellA.position.z - cellB.position.z);

    //    int xDirectionFactor;
    //    int yDirectionFactor;
    //    int zDirectionFactor;

    //    if (forNeighbourDistance)
    //    {
    //        if (currentDirection == 0 || currentDirection == 2)
    //        {
    //            xDirectionFactor = 1;
    //            yDirectionFactor = 20;
    //            zDirectionFactor = 20;
    //        }
    //        else
    //        {
    //            xDirectionFactor = 20;
    //            yDirectionFactor = 20;
    //            zDirectionFactor = 1;
    //        }
    //    }

    //    else
    //    {
    //        xDirectionFactor = 1;
    //        yDirectionFactor = 1;
    //        zDirectionFactor = 1;
    //    }

    //    if (GetDirection(cellA, cellB) != currentDirection)
    //    {
    //        distance = 1000 + ((yDirectionFactor * distY) + (xDirectionFactor * distX) + (zDirectionFactor * distZ)); // add a penalty for turning if test direction isn't the current direction
    //    }

    //    else
    //    {
    //        distance = ((yDirectionFactor * distY) + (xDirectionFactor * distX) + (zDirectionFactor * distZ));
    //    }

    //    return distance;
    //}

}