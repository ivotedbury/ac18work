using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // PathRequestManager requestManager;

    //CellGraph cellGraph;
   
       
    //public void StartFindPath(BrickArrangement inputBrickArrangement, Cell _startCell, Cell _targetCell)
    //{
    //    Cell[] waypoints = FindPath(_startCell, _targetCell));
    //}

   public Cell [] FindPath(CellGraph inputCellGraph, Cell _startCell, Cell _targetCell)
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


        Cell[] wayPoints = new Cell[0];
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
      //  yield return null;
        if (pathSuccess)
        {
            wayPoints = RetracePath(startCell, targetCell);
        }
        // requestManager.FinishedProcessingPath(wayPoints, pathSuccess);

        return wayPoints;
    }

    Cell[] RetracePath(Cell _startCell, Cell _endCell)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = _endCell;

        while (currentCell != _startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.parent;
        }
        path.Reverse();

        return path.ToArray();
    }

    int GetDistance(Cell cellA, Cell cellB)
    {
        int distX = cellA.position.x - cellB.position.x;
        int distY = cellA.position.y - cellB.position.y;
        int distZ = cellA.position.z - cellB.position.z;

        if (distY != 0)
        {
            return ((10 * distY) + distX + distZ);
        }
        else
        {
            return (distX + distZ);
        }
    }

}
