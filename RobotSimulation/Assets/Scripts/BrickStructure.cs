using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickStructure
{
    public List<Brick> bricksInTargetStructure = new List<Brick>();
    public List<Brick> bricksInPlace = new List<Brick>();

    public List<Cell> availableCells = new List<Cell>();

    public Grid grid;
    PathFinder pathFinder = new PathFinder();
    public Cell seedCell;

    public Cell legAPickupPoint;

    private int importOffset = 8;

    public BrickStructure(Vector3Int _gridSize, Vector3Int _seedCell, TextAsset _brickDataImport)
    {
        grid = new Grid(_gridSize);
        CreateSeed(_seedCell);
        CreateBricksInArrangment(_brickDataImport);
        legAPickupPoint = grid.cellsArray[_seedCell.x + 8, _seedCell.y, _seedCell.z];
    }

    void CreateBricksInArrangment(TextAsset _brickDataImport)
    {
        string importDataString = _brickDataImport.ToString();

        BrickImportItem[] brickImportArray = JsonHelper.FromJson<BrickImportItem>(importDataString);

        for (int i = 0; i < brickImportArray.Length; i++)
        {
            bricksInTargetStructure.Add(ConvertToBrick(brickImportArray[i]));
            bricksInTargetStructure[i].AssignChildCells(grid);
        }

        bricksInTargetStructure = ReorderBricks(bricksInTargetStructure, seedCell);
    }

    public List<Cell> FindPathOneWay(Cell _startCell, Cell _endCell, int _startDiection)
    {
        List<Cell> path = new List<Cell>();

        path = pathFinder.FindPath(grid, availableCells, _startCell, _endCell, _startDiection);

        return path;
    }

    public void UpdateAvailableCells()
    {
        availableCells.Clear();

        List<Cell> allChildCells = new List<Cell>();

        for (int i = 0; i < bricksInPlace.Count; i++)
        {
            for (int j = 0; j < bricksInPlace[i].childCells.Count; j++)
            {
                allChildCells.Add(bricksInPlace[i].childCells[j]);
                availableCells.Add(bricksInPlace[i].childCells[j]);
            }
        }

        for (int i = 0; i < allChildCells.Count; i++)
        {
            for (int j = 0; j < allChildCells.Count; j++)
            {
                if (grid.GetANeighbour(allChildCells[j], new Vector3Int(0, 1, 0)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(0, 1, 1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 1, 1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 1, 0)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 1, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(0, 1, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 1, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 1, 0)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 1, 1)) == allChildCells[i] ||

                    grid.GetANeighbour(allChildCells[j], new Vector3Int(0, 2, 1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 2, 1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 2, 0)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 2, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(0, 2, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 2, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 2, 0)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 2, 1)) == allChildCells[i] ||

                    grid.GetANeighbour(allChildCells[j], new Vector3Int(0, 3, 1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 3, 1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 3, 0)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(1, 3, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(0, 3, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 3, -1)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 3, 0)) == allChildCells[i] ||
                    grid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 3, 1)) == allChildCells[i])
                {
                    availableCells.Remove(allChildCells[j]);
                }
            }
        }
    }

    public Cell FindCompanionCell(Cell _cell, int _leadLeg, int _robotOrientation, List<Cell> _availableCells)
    {
        List<Cell> potentialListOfCompanions = new List<Cell>();

        if (_robotOrientation == 0 && _leadLeg == 0)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 4]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 3]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 2]);
        }
        if (_robotOrientation == 1 && _leadLeg == 0)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 4, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 3, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 2, _cell.position.y, _cell.position.z]);
        }
        if (_robotOrientation == 2 && _leadLeg == 0)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 4]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 3]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 2]);
        }
        if (_robotOrientation == 3 && _leadLeg == 0)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 4, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 3, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 2, _cell.position.y, _cell.position.z]);
        }
        //////////////////////////////////////////////////////////////////////////////
        if (_robotOrientation == 0 && _leadLeg == 1)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 4]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 3]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 2]);
        }
        if (_robotOrientation == 1 && _leadLeg == 1)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 4, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 3, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 2, _cell.position.y, _cell.position.z]);
        }
        if (_robotOrientation == 2 && _leadLeg == 1)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 4]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 3]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 2]);
        }
        if (_robotOrientation == 3 && _leadLeg == 1)
        {
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 4, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 3, _cell.position.y, _cell.position.z]);
            potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 2, _cell.position.y, _cell.position.z]);
        }

        List<Cell> availableListofCompanions = new List<Cell>();

        for (int i = 0; i < potentialListOfCompanions.Count; i++)
        {
            if (availableCells.Contains(potentialListOfCompanions[i]))
            {
                availableListofCompanions.Add(potentialListOfCompanions[i]);
            }
        }

       // Debug.Log("A " + _cell.position + "B " + availableListofCompanions.Count);

        Cell companionCell = null;

        if (availableListofCompanions.Count > 0)
        {
            companionCell = availableListofCompanions[0];
        }

        return companionCell;

    }

    public int GetCurrentDirectionOfTravel(Cell _start, Cell _end)
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

    public Cell FindDropOffCell(Brick _targetBrick, List<Cell> _availableCells)
    {
        Cell dropOffCell = null;

        List<Cell> potentialDropOffCells = new List<Cell>();
        List<Cell> possibleDropOffCells = new List<Cell>();
        List<Cell> shortlistDropOffCells = new List<Cell>();


        Cell targetCell = _targetBrick.originCell;

        for (int relativeHeight = -1; relativeHeight < 1; relativeHeight++) // should be -1
        {
            for (int relativeDistance = 2; relativeDistance < 5; relativeDistance++)
            {
                potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(0, relativeHeight, relativeDistance)));
                potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(relativeDistance, relativeHeight, 0)));
                potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-relativeDistance, relativeHeight, 0)));
                potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(0, relativeHeight, -relativeDistance)));
            }

            int value1 = 1;
            int value2 = 2;
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, value2)));

            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, value1)));

            value1 = 1;
            value2 = 3;

            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, value2)));

            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, value1)));

            value1 = 2;
            value2 = 2;

            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, value2)));

            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, value1)));

            value1 = 2;
            value2 = 3;

            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, -value2)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value1, relativeHeight, value2)));

            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, -value1)));
            potentialDropOffCells.Add(grid.GetANeighbour(targetCell, new Vector3Int(-value2, relativeHeight, value1)));

        }

        Debug.Log(potentialDropOffCells.Count);

        for (int i = 0; i < potentialDropOffCells.Count; i++)
        {
            for (int j = 0; j < _availableCells.Count; j++)
            {
                if (_availableCells[j] == potentialDropOffCells[i])
                {
                    possibleDropOffCells.Add(potentialDropOffCells[i]);
                }
            }
        }
        Debug.Log(possibleDropOffCells.Count);

        for (int i = 0; i < possibleDropOffCells.Count; i++)
        {
            Vector3Int dropOffToTarget = possibleDropOffCells[i].position - targetCell.position;

            if (_targetBrick.brickType == 1)
            {
                if ((_targetBrick.rotation == Quaternion.Euler(0, 90, 0) || _targetBrick.rotation == Quaternion.Euler(0, 270, 0)) && Mathf.Abs(dropOffToTarget.z) == 0)
                {

                    if (Mathf.Abs(dropOffToTarget.x) > 2)
                    {
                        shortlistDropOffCells.Add(possibleDropOffCells[i]);
                    }
                }
                else if ((_targetBrick.rotation == Quaternion.Euler(0, 0, 0) || _targetBrick.rotation == Quaternion.Euler(0, 180, 0)) && Mathf.Abs(dropOffToTarget.x) == 0)
                {
                    if (Mathf.Abs(dropOffToTarget.z) > 2 && Mathf.Abs(dropOffToTarget.x) == 0)
                    {
                        shortlistDropOffCells.Add(possibleDropOffCells[i]);
                    }
                }

                else
                {
                    shortlistDropOffCells.Add(possibleDropOffCells[i]);
                }
            }
            else
            {
                shortlistDropOffCells = possibleDropOffCells;
            }
        }

        Debug.Log(shortlistDropOffCells.Count);


        List<Cell> finalListDropOffCells = new List<Cell>();

        for (int i = 0; i < shortlistDropOffCells.Count; i++)
        {
            for (int _robotOrientation = 0; _robotOrientation < 4; _robotOrientation++)
            {
                if (FindCompanionCell(shortlistDropOffCells[i], 0, _robotOrientation, availableCells) != null)
                {
                    finalListDropOffCells.Add(shortlistDropOffCells[i]);
                }
            }
        }

        int bestTripCost = 100000000;

        for (int i = 0; i < finalListDropOffCells.Count; i++)
        {
            List<Cell> potentialPath = new List<Cell>();
            potentialPath = pathFinder.FindPath(grid, availableCells, bricksInTargetStructure[0].originCell, finalListDropOffCells[i], 1);
            int testTripCost = bestTripCost;
            if (potentialPath.Count > 0)
            {
                testTripCost = pathFinder.totalCostOfTrip;
            }
            Debug.Log("potentialPath length: " + potentialPath.Count);
            Debug.Log("finalListDropOffCells position: " + i + "-" + finalListDropOffCells[i].position);
            Debug.Log("testTripCost " + i + "-" + testTripCost);

            if (testTripCost < bestTripCost)
            {
                bestTripCost = testTripCost;
                dropOffCell = finalListDropOffCells[i];
            }
            Debug.Log(dropOffCell.position);

        }



        //    float bestCurrentDistance = 10000000;

        //for (int i = 0; i < finalListDropOffCells.Count; i++)
        //{
        //    float distanceFromSeed = (finalListDropOffCells[i].position - seedCell.position).magnitude;
        //    if (distanceFromSeed < bestCurrentDistance)
        //    {
        //        dropOffCell = finalListDropOffCells[i];
        //        bestCurrentDistance = distanceFromSeed;
        //    }
        //}

        Debug.Log("dropOffCell Position: "+ dropOffCell.position);

        return dropOffCell;
    }

    //Cell FindCompanionCell(Cell _cell, int _stepOrientation)
    //{
    //    List<Cell> potentialListOfCompanions = new List<Cell>();

    //    if (_stepOrientation == 0)
    //    {
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 4]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 3]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z - 2]);
    //    }
    //    if (_stepOrientation == 1)
    //    {
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 4, _cell.position.y, _cell.position.z]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 3, _cell.position.y, _cell.position.z]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x - 2, _cell.position.y, _cell.position.z]);
    //    }
    //    if (_stepOrientation == 2)
    //    {
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 4]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 3]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x, _cell.position.y, _cell.position.z + 2]);
    //    }
    //    if (_stepOrientation == 3)
    //    {
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 4, _cell.position.y, _cell.position.z]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 3, _cell.position.y, _cell.position.z]);
    //        potentialListOfCompanions.Add(grid.cellsArray[_cell.position.x + 2, _cell.position.y, _cell.position.z]);
    //    }

    //    return potentialListOfCompanions;
    //}

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

    void CreateSeed(Vector3Int _seed)
    {
        seedCell = grid.cellsArray[_seed.x, _seed.y, _seed.z];
    }

    Brick ConvertToBrick(BrickImportItem importedBrickItem)
    {
        Brick convertedBrick = null;

        convertedBrick = new Brick(grid.cellsArray[importedBrickItem.brickPosX + importOffset, importedBrickItem.brickPosZ, importedBrickItem.brickPosY + importOffset], importedBrickItem.rotation, importedBrickItem.brickType);

        convertedBrick.childCells = grid.GetChildren(convertedBrick);

        return convertedBrick;
    }

    List<Brick> ReorderBricks(List<Brick> _inputTargetStructure, Cell _inputSeed)
    {
        List<Brick> reorderedTargetStructure = new List<Brick>();

        float currentClosestDistance;
        float currentFurthestDistance;

        float testDistance;
        int listLength = _inputTargetStructure.Count;

        Brick bestCurrentBrick = null;
        bool betterBrickFound = false;

        for (int currentSearchLayer = 1; currentSearchLayer < grid.gridSize.y; currentSearchLayer++)
        {
            for (int listCounter = 0; listCounter < listLength; listCounter++)
            {
                currentClosestDistance = 1000000;
                currentFurthestDistance = 0;
                betterBrickFound = false;

                if (currentSearchLayer == 1)
                {

                    for (int i = 0; i < _inputTargetStructure.Count; i++)
                    {
                        if (_inputTargetStructure[i].originCell.position.y == currentSearchLayer)
                        {
                            testDistance = Mathf.Abs(Vector3.Distance(_inputTargetStructure[i].originCell.position, _inputSeed.position));

                            if (testDistance < currentClosestDistance)
                            {
                                currentClosestDistance = testDistance;
                                bestCurrentBrick = _inputTargetStructure[i];
                                betterBrickFound = true;
                            }
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < _inputTargetStructure.Count; i++)
                    {
                        if (_inputTargetStructure[i].originCell.position.y == currentSearchLayer)
                        {
                            testDistance = Mathf.Abs(Vector3.Distance(_inputTargetStructure[i].originCell.position, _inputSeed.position));

                            if (testDistance > currentFurthestDistance)
                            {
                                currentFurthestDistance = testDistance;
                                bestCurrentBrick = _inputTargetStructure[i];
                                betterBrickFound = true;
                            }
                        }
                    }
                }

                if (betterBrickFound)
                {
                    reorderedTargetStructure.Add(bestCurrentBrick);
                    _inputTargetStructure.Remove(bestCurrentBrick);
                }
            }
        }
        return reorderedTargetStructure;
    }

}
