using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPathFinder
{

    public int totalCostOfTrip;

    public List<Cell> CalculatePathForSequencing(Grid _inputGrid, List<Cell> _availableCells, List<Cell> _forbiddenCells, Cell _startCell, Cell _targetCell)
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
                        _inputGrid.cellsArray[x, y, z].availableCost = 100000 * y; ///////////////////////////////make this y squared (reduce costs to floats to give more scale)
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

            Cell neighbour = null;
            bool continueToNextNeighbour;
            List<Cell> pathToNeighbour = new List<Cell>();
            //  List<Cell> cellToNeighbour = new List<Cell>();

            List<Cell> cellNeighbours = new List<Cell>();
            cellNeighbours = GetCellNeighbours(_inputGrid, cell);

            for (int i = 0; i < cellNeighbours.Count; i++)
            {
                continueToNextNeighbour = false;
                neighbour = cellNeighbours[i];

                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                pathToNeighbour.Clear();

                //   if (cell != _startCell && RetracePath(startCell, cell).Count > 1)
                //   {
                pathToNeighbour = RetracePath(_startCell, cell);
                //pathToNeighbour = RetraceFullCellPath(startCell, cell,  _inputGrid); //2 was previously "cell"
                //  }
                //else
                //{
                //    pathToNeighbour = GetCellToNeighbour(cell, neighbour, _inputGrid);
                //}

                //    cellToNeighbour.Clear();
                //   cellToNeighbour = GetCellToNeighbour(cell, neighbour, _inputGrid);

                if (_forbiddenCells.Contains(neighbour))
                {
                    continue;
                }

                for (int j = 0; j < pathToNeighbour.Count-1; j++)
                {
                    if ((neighbour.position.x == pathToNeighbour[j].position.x && neighbour.position.z == pathToNeighbour[j].position.z) ||

                     (neighbour.position.x == pathToNeighbour[j].position.x + 0 && neighbour.position.z == pathToNeighbour[j].position.z + 1) ||
                     (neighbour.position.x == pathToNeighbour[j].position.x + 1 && neighbour.position.z == pathToNeighbour[j].position.z + 1) ||
                     (neighbour.position.x == pathToNeighbour[j].position.x + 1 && neighbour.position.z == pathToNeighbour[j].position.z + 0) ||
                     (neighbour.position.x == pathToNeighbour[j].position.x + 1 && neighbour.position.z == pathToNeighbour[j].position.z - 1) ||
                     (neighbour.position.x == pathToNeighbour[j].position.x + 0 && neighbour.position.z == pathToNeighbour[j].position.z - 1) ||
                     (neighbour.position.x == pathToNeighbour[j].position.x - 1 && neighbour.position.z == pathToNeighbour[j].position.z - 1) ||
                     (neighbour.position.x == pathToNeighbour[j].position.x - 1 && neighbour.position.z == pathToNeighbour[j].position.z + 0) ||
                     (neighbour.position.x == pathToNeighbour[j].position.x - 1 && neighbour.position.z == pathToNeighbour[j].position.z + 1))
                    {
                        continueToNextNeighbour = true;
                        break;
                    }


                    //    for (int k = 0; k < _forbiddenCells.Count; k++)
                    //    {
                    //        if ((_forbiddenCells[k].position.x == pathToNeighbour[j].position.x && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z) ||

                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x + 0 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z + 1) ||
                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x + 1 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z + 1) ||
                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x + 1 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z + 0) ||
                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x + 1 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z - 1) ||
                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x + 0 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z - 1) ||
                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x - 1 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z - 1) ||
                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x - 1 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z + 0) ||
                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x - 1 && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z + 1) ||

                    //          (_forbiddenCells[k].position.x == pathToNeighbour[j].position.x && _forbiddenCells[k].position.z == pathToNeighbour[j].position.z) ||

                    //          (_forbiddenCells[k].position.x == neighbour.position.x + 0 && _forbiddenCells[k].position.z == neighbour.position.z + 1) ||
                    //          (_forbiddenCells[k].position.x == neighbour.position.x + 1 && _forbiddenCells[k].position.z == neighbour.position.z + 1) ||
                    //          (_forbiddenCells[k].position.x == neighbour.position.x + 1 && _forbiddenCells[k].position.z == neighbour.position.z + 0) ||
                    //          (_forbiddenCells[k].position.x == neighbour.position.x + 1 && _forbiddenCells[k].position.z == neighbour.position.z - 1) ||
                    //          (_forbiddenCells[k].position.x == neighbour.position.x + 0 && _forbiddenCells[k].position.z == neighbour.position.z - 1) ||
                    //          (_forbiddenCells[k].position.x == neighbour.position.x - 1 && _forbiddenCells[k].position.z == neighbour.position.z - 1) ||
                    //          (_forbiddenCells[k].position.x == neighbour.position.x - 1 && _forbiddenCells[k].position.z == neighbour.position.z + 0) ||
                    //          (_forbiddenCells[k].position.x == neighbour.position.x - 1 && _forbiddenCells[k].position.z == neighbour.position.z + 1))
                    //        {
                    //            continueToNextNeighbour = true;
                    //            break;
                    //        }
                }

                if (continueToNextNeighbour)
                {
                    break;
                }

                //for (int m = 0; m < cellToNeighbour.Count; m++)
                //{
                //    if ((cellToNeighbour[m].position.x == pathToNeighbour[j].position.x && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z) ||

                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x + 0 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z + 1) ||
                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x + 1 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z + 1) ||
                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x + 1 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z + 0) ||
                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x + 1 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z - 1) ||
                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x + 0 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z - 1) ||
                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x - 1 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z - 1) ||
                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x - 1 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z + 0) ||
                //        (cellToNeighbour[m].position.x == pathToNeighbour[j].position.x - 1 && cellToNeighbour[m].position.z == pathToNeighbour[j].position.z + 1))
                //    {
                //        continueToNextNeighbour = true;
                //    }
                //}

                //for (int m = 0; m < pathToNeighbour.Count; m++)
                //{
                //    if (m == j)
                //    {
                //        continue;
                //    }

                //    if ((pathToNeighbour[m].position.x == pathToNeighbour[j].position.x && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z)

                //     //   ||

                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x + 0 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z + 1) ||
                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x + 1 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z + 1) ||
                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x + 1 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z + 0) ||
                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x + 1 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z - 1) ||
                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x + 0 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z - 1) ||
                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x - 1 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z - 1) ||
                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x - 1 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z + 0) ||
                //     //(pathToNeighbour[m].position.x == pathToNeighbour[j].position.x - 1 && pathToNeighbour[m].position.z == pathToNeighbour[j].position.z + 1)
                //     )
                //    {
                //        continueToNextNeighbour = true;
                //    }
                //}


                //if (continueToNextNeighbour)
                //{
                //    continue;
                //}

                int newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour) + cell.availableCost; //cell.availableCost +

                if (newCostToNeighbour < neighbour.gCost + neighbour.availableCost || !openSet.Contains(neighbour)) //////////////////// PROBLEM HERE + neighbour.availableCost
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
            // waypoints = RetracePath(startCell, targetCell); ///////////////////////////////////////////////////////////////////////////////////////////////
            waypoints = RetraceFullCellPath(startCell, targetCell, targetCell, _inputGrid);

            for (int i = 0; i < waypoints.Count; i++)
            {
                Debug.Log(waypoints[i].gCost);
            }

            totalCostOfTrip = targetCell.gCost;
        }

        return waypoints;
    }

    List<Cell> GetCellToNeighbour(Cell _startCell, Cell _neighbourCell, Grid _inputGrid)
    {
        List<Cell> cellToNeighbour = new List<Cell>();

        Vector3Int toNextCell = _neighbourCell.position - _startCell.position;

        for (int x = 1; x < Mathf.Abs(toNextCell.x); x++)
        {
            if (toNextCell.x > 0)
            {
                cellToNeighbour.Add(_inputGrid.GetANeighbour(_startCell, new Vector3Int(x, 0, 0)));
            }
            else
            {
                cellToNeighbour.Add(_inputGrid.GetANeighbour(_startCell, new Vector3Int(-x, 0, 0)));
            }
        }

        for (int z = 1; z < Mathf.Abs(toNextCell.z); z++)
        {
            if (toNextCell.z > 0)
            {
                cellToNeighbour.Add(_inputGrid.GetANeighbour(_startCell, new Vector3Int(0, 0, z)));
            }
            else
            {
                cellToNeighbour.Add(_inputGrid.GetANeighbour(_startCell, new Vector3Int(0, 0, -z)));
            }
        }

        return cellToNeighbour;
    }

    List<Cell> RetraceFullCellPath(Cell _startCell, Cell _penultimateCell, Cell _endCell, Grid _inputGrid)
    {
        List<Cell> path = new List<Cell>();

        Cell currentCell = _endCell;
        Cell nextCell = _endCell;

        Vector3Int toNextCell;

        toNextCell = _penultimateCell.position - _endCell.position;
        path.Add(currentCell);

        for (int x = 1; x < Mathf.Abs(toNextCell.x); x++)
        {
            if (toNextCell.x > 0)
            {
                path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(x, 0, 0)));
            }
            else
            {
                path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(-x, 0, 0)));
            }
        }

        for (int z = 1; z < Mathf.Abs(toNextCell.z); z++)
        {
            if (toNextCell.z > 0)
            {
                path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(0, 0, z)));
            }
            else
            {
                path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(0, 0, -z)));
            }
        }

        currentCell = _penultimateCell;

        while (currentCell != _startCell) // assemble path in reverse order
        {
            path.Add(currentCell);

            nextCell = currentCell.parent;
            toNextCell = nextCell.position - currentCell.position;

            for (int x = 1; x < Mathf.Abs(toNextCell.x); x++)
            {
                if (toNextCell.x > 0)
                {
                    path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(x, 0, 0)));
                }
                else
                {
                    path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(-x, 0, 0)));
                }
            }

            for (int z = 1; z < Mathf.Abs(toNextCell.z); z++)
            {
                if (toNextCell.z > 0)
                {
                    path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(0, 0, z)));
                }
                else
                {
                    path.Add(_inputGrid.GetANeighbour(currentCell, new Vector3Int(0, 0, -z)));
                }
            }

            currentCell = nextCell;
        }

        path.Add(_startCell);


        path.Reverse(); // reverse the path to the correct order
        return path;
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

        if (_testCell.position.x > 7 && _testCell.position.x < _inputGrid.gridSize.x - 8
             && _testCell.position.y > 0 && _testCell.position.y < _inputGrid.gridSize.y - 8
             && _testCell.position.z > 7 && _testCell.position.z < _inputGrid.gridSize.z - 8)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (y == -1 || y == 1)
                {
                    for (int z = -8; z <= 8; z++)
                    {
                        if (z > -5 && z < 5)
                        {
                            continue;
                        }

                        neighbours.Add(_inputGrid.cellsArray[_testCell.position.x + 0, _testCell.position.y + y, _testCell.position.z + z]);
                    }

                    for (int x = -8; x <= 8; x++)
                    {
                        if (x > -5 && x < 5)
                        {
                            continue;
                        }

                        neighbours.Add(_inputGrid.cellsArray[_testCell.position.x + x, _testCell.position.y + y, _testCell.position.z + 0]);
                    }
                }
                else
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
        }

        return neighbours;
    }

}
