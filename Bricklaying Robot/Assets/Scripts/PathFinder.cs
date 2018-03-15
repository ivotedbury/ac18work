using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{

    public List<Cell> FindPath(CellGraph inputCellGraph, Cell _startCell, Cell _targetCell)
    {
        //BrickArrangement brickArrangement = _inputBrickArrangement;

        //List<Cell> availableCells = new List<Cell>();

        //foreach (Brick brick in inputAllBricks)
        //{
        //    foreach (Cell cell in brick.childCells)
        //    {
        //        availableCells.Add(cell);
        //    }
        //}

        Cell startCell = _startCell;
        Cell targetCell = _targetCell;


        List<Cell> wayPoints = new List<Cell>();
        bool pathSuccess = false;

        List<Cell> openSet = new List<Cell>(); // the set of cells to be evaluated
        HashSet<Cell> closedSet = new HashSet<Cell>(); // the set of cells ALREADY evaluated

        openSet.Add(startCell);
       

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

            foreach (Cell neighbour in inputCellGraph.GetPathFinderNeighbours(cell))
            {
                if (!neighbour.isAvailable || closedSet.Contains(neighbour))
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
            wayPoints = RetracePath(startCell, targetCell);
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

    int GetDistance(Cell cellA, Cell cellB)
    {
        int distX = Mathf.Abs(cellA.position.x - cellB.position.x);
        int distY = Mathf.Abs(cellA.position.y - cellB.position.y);
        int distZ = Mathf.Abs(cellA.position.z - cellB.position.z);

        if (distY != 0)
        {
            return ((20 * distY) + (1*distX) + (1*distZ));
        }
        else
        {
            return (distX + distZ);
        }
    }

}
