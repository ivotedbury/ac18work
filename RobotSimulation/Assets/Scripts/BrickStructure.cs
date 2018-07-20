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

    private int importOffset = 5;

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

    public List<Cell> FindPathOneWay(Cell _startCell, Cell _endCell)
    {
        List<Cell> path = new List<Cell>();

        path = pathFinder.FindPath(grid, availableCells, _startCell, _endCell);

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

    public Cell FindDropOffCell(Brick _targetBrick, List<Cell> _availableCells)
    {
        Cell dropOffCell = null;

        List<Cell> potentialDropOffCells = new List<Cell>();
        List<Cell> possibleDropOffCells = new List<Cell>();


        for (int relativeHeight = -1; relativeHeight < 1; relativeHeight++) // should be -1
        {
            for (int relativeDistance = 2; relativeDistance < 5; relativeDistance++)
            {
                potentialDropOffCells.Add(grid.GetANeighbour(_targetBrick.originCell, new Vector3Int(0, relativeHeight, relativeDistance)));
                potentialDropOffCells.Add(grid.GetANeighbour(_targetBrick.originCell, new Vector3Int(relativeDistance, relativeHeight, 0)));
                potentialDropOffCells.Add(grid.GetANeighbour(_targetBrick.originCell, new Vector3Int(-relativeDistance, relativeHeight, 0)));
                potentialDropOffCells.Add(grid.GetANeighbour(_targetBrick.originCell, new Vector3Int(0, relativeHeight, -relativeDistance)));
            }
        }


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

        if (possibleDropOffCells.Count >= 2)
        {
            dropOffCell = possibleDropOffCells[1];
        }
        else
        {
            dropOffCell = possibleDropOffCells[0];
        }

        return dropOffCell;
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
        float testDistance;
        int listLength = _inputTargetStructure.Count;

        Brick bestCurrentBrick = null;
        bool betterCellFound = false;

        for (int currentSearchLayer = 0; currentSearchLayer < grid.gridSize.y; currentSearchLayer++)
        {
            for (int listCounter = 0; listCounter < listLength; listCounter++)
            {
                currentClosestDistance = 1000000;
                betterCellFound = false;

                for (int i = 0; i < _inputTargetStructure.Count; i++)
                {
                    if (_inputTargetStructure[i].originCell.position.y == currentSearchLayer)
                    {
                        testDistance = Mathf.Abs(Vector3.Distance(_inputTargetStructure[i].originCell.position, _inputSeed.position));

                        if (testDistance < currentClosestDistance)
                        {
                            currentClosestDistance = testDistance;
                            bestCurrentBrick = _inputTargetStructure[i];
                            betterCellFound = true;
                        }
                    }
                }
                if (betterCellFound)
                {
                    reorderedTargetStructure.Add(bestCurrentBrick);
                    _inputTargetStructure.Remove(bestCurrentBrick);
                }
            }
        }
        return reorderedTargetStructure;
    }

}
