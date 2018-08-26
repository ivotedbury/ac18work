using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{

    public float totalCostOfTrip;
    float totalCostOfBuild = 0;

    public List<Cell> FindPath(Grid _inputGrid, List<Cell> _availableCells, Cell _startCell, Cell _targetCell, int _startingDirection)
    {
        totalCostOfTrip = 0;
        List<Cell> waypoints = new List<Cell>();

        Cell startCell = _startCell;
        Cell targetCell = _targetCell;
        bool pathSuccess = false;

        List<Cell> openSet = new List<Cell>();
        List<Cell> closedSet = new List<Cell>();

        int currentDirection = _startingDirection;
        int proposedDirection;

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

            if (openSet.Count > 1)
            {
                currentDirection = GetDirection(cell.parent, cell);
            }

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
                if (!_availableCells.Contains(neighbour) || closedSet.Contains(neighbour))
                {
                    continue;
                }

                proposedDirection = GetDirection(cell, neighbour);

                float newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour, currentDirection, proposedDirection);

                List<Cell> potentialCompanionCells = FindListOfPotentialCompanionCells(_inputGrid, neighbour, proposedDirection, _availableCells);

                if (potentialCompanionCells.Count > 0)
                {
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetCell, currentDirection, proposedDirection);
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
            totalCostOfTrip = targetCell.gCost;

            totalCostOfBuild = totalCostOfBuild + totalCostOfTrip;

        }

        return waypoints;
    }

    int GetDirection(Cell _start, Cell _end)
    {
        int direction = 0;

        Vector3Int directionVector = _end.position - _start.position;

        if (directionVector.z > 0)
        {
            direction = 0;
        }
        else if (directionVector.x > 0)
        {
            direction = 1;
        }
        else if (directionVector.z < 0)
        {
            direction = 2;
        }
        else if (directionVector.x < 0)
        {
            direction = 3;
        }

        return direction;
    }

    public List<Cell> FindListOfPotentialCompanionCells(Grid _inputGrid, Cell _cell, int _stepOrientation, List<Cell> _availableCells)
    {
        List<Cell> potentialListOfCompanions = new List<Cell>();

        if (_stepOrientation == 0)
        {
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 4]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 3]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 2]);
        }
        if (_stepOrientation == 1)
        {
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x - 4, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x - 3, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x - 2, _cell.position.y, _cell.position.z]);
        }
        if (_stepOrientation == 2)
        {
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 4]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 3]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 2]);
        }
        if (_stepOrientation == 3)
        {
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x + 4, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x + 3, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(_inputGrid.cellsArray[_cell.position.x + 2, _cell.position.y, _cell.position.z]);
        }

        List<Cell> finalListOfCompanions = new List<Cell>();

        for (int i = 0; i < potentialListOfCompanions.Count; i++)
        {
            if (_availableCells.Contains(potentialListOfCompanions[i]))
            {
                finalListOfCompanions.Add(potentialListOfCompanions[i]);
            }
        }

        return finalListOfCompanions;
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

    int GetDistance(Cell _cellA, Cell _cellB, int _currentDirection, int _proposedDirection)
    {
        int distance;

        int distX = Mathf.Abs(_cellB.position.x - _cellA.position.x);
        int distY = Mathf.Abs(_cellB.position.y - _cellA.position.y);
        int distZ = Mathf.Abs(_cellB.position.z - _cellA.position.z);

        int horizontalCost = 1;
        int verticalCost = 10;
        int directionChangeCost = 1;

        if (_proposedDirection != _currentDirection)
        {
            directionChangeCost = 1000;
        }

        distance = (distX * horizontalCost) + (distY * verticalCost) + (distZ * horizontalCost) + directionChangeCost; ////////////////////////////////////////////////////////////////////////////////

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

}