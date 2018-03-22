using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public bool pathIsIncomplete;
    public int totalCostofTrip;
    public int totalCostofBuild = 0;
    int counterLimit = 100000;

    public Cell bestCurrentCell;

    public bool CheckPath(CellGraph inputCellGraph, Cell _startCell, Cell _targetCell)
    {
        // pathIsIncomplete = false;



        Cell startCell = _startCell;
        Cell targetCell = _targetCell;

        List<Cell> wayPoints = new List<Cell>();
        bool pathSuccess = false;
        int counter = 0;
        bestCurrentCell = startCell;

        List<Cell> openSet = new List<Cell>(); // the set of cells to be evaluated
        List<Cell> closedSet = new List<Cell>(); // the set of cells ALREADY evaluated (used to be a hashset)

        //if (!inputCellGraph.availableCells.Contains(_targetCell))
        //{
        //    pathSuccess = true;
        //}

        foreach (Cell cell in inputCellGraph.availableCells) // reset the costs from previous searches
        {
            cell.gCost = 0;
            cell.hCost = 0;
        }

        openSet.Add(startCell);

        while (openSet.Count > 0 && counter < counterLimit)
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
            }

            foreach (Cell neighbour in inputCellGraph.GetPathFinderNeighbours(cell))
            {
                if (!neighbour.isAvailable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour, GetDirection(cell, neighbour), true);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetCell, GetDirection(cell, neighbour), true);
                    neighbour.parent = cell;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
            counter++;
        }

        if (pathSuccess)
        {
            //  wayPoints = RetracePath(startCell, targetCell);
            // bestCurrentCell = wayPoints[wayPoints.Count - 1];
            return pathSuccess;
            // totalCostofTrip = wayPoints[wayPoints.Count - 1].gCost;
        }

        else // if the counter limit has expired - suggesting there is no complete path
        {
            int bestFCost = 10000000; //bench mark large number
            for (int i = 0; i < closedSet.Count; i++)
            {
                if (closedSet[i].fCost < bestFCost)
                {
                    bestCurrentCell = closedSet[i]; // find the cell with the best fcost explored so far in the search
                }
            }
            //wayPoints = RetracePath(startCell, bestCurrentCell); // assemble the waypoints to the best current cell - will try adding more bricks around the last brick and try again
            return pathSuccess;
        }
    }


    public List<Cell> FindPath(CellGraph inputCellGraph, Cell _startCell, Cell _targetCell, int _currentDirection)
    {
        pathIsIncomplete = false;
        totalCostofTrip = 0;

        Cell startCell = _startCell;
        Cell targetCell = _targetCell;

        List<Cell> wayPoints = new List<Cell>();
        bool pathSuccess = false;
        int counter = 0;
        Cell bestCurrentCell = startCell;

        List<Cell> openSet = new List<Cell>(); // the set of cells to be evaluated
        List<Cell> closedSet = new List<Cell>(); // the set of cells ALREADY evaluated (used to be a hashset)

        foreach (Cell cell in inputCellGraph.availableCells) // reset the costs from previous searches
        {
            cell.gCost = 0;
            cell.hCost = 0;
        }

        openSet.Add(startCell);

        while (openSet.Count > 0 && counter < counterLimit)
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

            foreach (Cell neighbour in inputCellGraph.GetPathFinderNeighbours(cell))
            {
                if (!neighbour.isAvailable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour, GetDirection(cell, neighbour), true);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetCell, GetDirection(cell, neighbour), true);
                    neighbour.parent = cell;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
            counter++;
        }

        if (pathSuccess)
        {
            wayPoints = RetracePath(startCell, targetCell);
            totalCostofTrip = wayPoints[wayPoints.Count - 1].gCost;
            totalCostofBuild = totalCostofBuild + totalCostofTrip;

        }

        else // if the counter limit has expired - suggesting there is no complete path
        {
            int bestFCost = 10000000; //bench mark large number
            for (int i = 0; i < closedSet.Count; i++)
            {
                if (closedSet[i].fCost < bestFCost)
                {
                    bestCurrentCell = closedSet[i]; // find the cell with the best fcost explored so far in the search
                }
            }

            pathIsIncomplete = true;
            wayPoints = RetracePath(startCell, bestCurrentCell); // assemble the waypoints to the best current cell - will try adding more bricks around the last brick and try again
        }

        return wayPoints;
    }

    List<Cell> RetracePath(Cell _startCell, Cell _endCell)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = _endCell;

        while (currentCell != _startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.parent;
        }
        path.Add(_startCell);

        path.Reverse();

        return path;
    }

    int GetDirection(Cell _startCell, Cell _testCell)
    {
        int directionValue = 0;

        if (_testCell.position - _startCell.position == new Vector3Int(1, 0, 0))
        {
            directionValue = 0;
        }
        else if (_testCell.position - _startCell.position == new Vector3Int(0, 0, -1))
        {
            directionValue = 1;
        }
        else if (_testCell.position - _startCell.position == new Vector3Int(-1, 0, 0))
        {
            directionValue = 2;
        }
        else if (_testCell.position - _startCell.position == new Vector3Int(0, 0, 1))
        {
            directionValue = 3;
        }

        return directionValue;
    }

    int GetDistance(Cell cellA, Cell cellB, int currentDirection, bool forNeighbourDistance)
    {
        int distance;

        int distX = Mathf.Abs(cellA.position.x - cellB.position.x);
        int distY = Mathf.Abs(cellA.position.y - cellB.position.y);
        int distZ = Mathf.Abs(cellA.position.z - cellB.position.z);

        int xDirectionFactor;
        int yDirectionFactor;
        int zDirectionFactor;

        if (forNeighbourDistance)
        {
            if (currentDirection == 0 || currentDirection == 2)
            {
                xDirectionFactor = 1;
                yDirectionFactor = 20;
                zDirectionFactor = 20;
            }
            else
            {
                xDirectionFactor = 20;
                yDirectionFactor = 20;
                zDirectionFactor = 1;
            }
        }

        else
        {
            xDirectionFactor = 1;
            yDirectionFactor = 1;
            zDirectionFactor = 1;
        }

        if (GetDirection(cellA, cellB) != currentDirection)
        {
            distance = 1000 + ((yDirectionFactor * distY) + (xDirectionFactor * distX) + (zDirectionFactor * distZ)); // add a penalty for turning if test direction isn't the current direction
        }

        else
        {
            distance = ((yDirectionFactor * distY) + (xDirectionFactor * distX) + (zDirectionFactor * distZ));
        }

        return distance;
    }

}
