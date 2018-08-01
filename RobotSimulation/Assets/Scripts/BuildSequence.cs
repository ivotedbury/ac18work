using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSequence
{
    public List<Brick> inputStructure = new List<Brick>();
    List<Brick> additionalStartingBricks = new List<Brick>();
    public List<Brick> completeStructure = new List<Brick>();
    List<Brick> newBricksRequired = new List<Brick>();

    public Grid grid;
    public Cell seedCell;

    public List<Brick> finalStructureToBuild = new List<Brick>();
    public List<Cell> availableCells = new List<Cell>();
    public List<Cell> forbiddenCells = new List<Cell>();
    public List<Cell> desiredPath = new List<Cell>();
    public List<Cell> fullDesiredPath = new List<Cell>();

    BrickPathFinder brickPathFinder = new BrickPathFinder();

    public BuildSequence(Vector3Int _gridSize, Vector3Int _seedCell, TextAsset _brickDataImport)
    {

        grid = new Grid(_gridSize);
        CreateSeed(_seedCell);

        additionalStartingBricks.Add(new Brick(grid, grid.GetANeighbour(seedCell, new Vector3Int(4, 0, 0)), 90, 1, true));
        additionalStartingBricks.Add(new Brick(grid, grid.GetANeighbour(seedCell, new Vector3Int(8, 0, 0)), 90, 1, true));
        additionalStartingBricks.Add(new Brick(grid, grid.GetANeighbour(seedCell, new Vector3Int(12, 0, 0)), 90, 1, true));

        completeStructure = additionalStartingBricks;
        inputStructure = CreateBricksInArrangment(_brickDataImport);

        for (int i = 0; i < inputStructure.Count; i++)
        {
            completeStructure.Add(inputStructure[i]);
        }

        availableCells = FindAvailableCells(completeStructure);
        forbiddenCells = FindForbiddenCells(completeStructure, availableCells);
    }

    public void GenerateCompletePaths()
    {
        //List<Brick> brickStructureCompletedPaths = new List<Brick>();

        Brick brickToPlaceNext;
        Cell pathStartingCell = completeStructure[2].childCells[0];

        brickToPlaceNext = completeStructure[completeStructure.Count - 1];

        desiredPath = brickPathFinder.CalculatePathForSequencing(grid, availableCells, forbiddenCells, pathStartingCell, brickToPlaceNext.originCell, 1);
    }

    public void GenerateAdditionalBricks(TextAsset _correspondingBrickDataFile)
    {
        List<Cell> cellsInPath = new List<Cell>();

        cellsInPath = ConvertCellsFromImport(_correspondingBrickDataFile);

        desiredPath.Clear();
        desiredPath = cellsInPath;

        newBricksRequired = ConvertPathToBricks(desiredPath);

        for (int i = 0; i < newBricksRequired.Count; i++)
        {
            completeStructure.Add(newBricksRequired[i]);
        }
    }

    List<Brick> ConvertPathToBricks(List<Cell> _desiredPath)
    {
        List<Brick> _additionalBricks = new List<Brick>();

        int pathCounter = 2;

        while (pathCounter < _desiredPath.Count - 3)
        {
            int requiredDirection = brickPathFinder.GetDirection(_desiredPath[pathCounter - 1], _desiredPath[pathCounter]);

            _additionalBricks.Add(new Brick(grid, _desiredPath[pathCounter], requiredDirection * 90, 2, true));

            pathCounter += 2;
        }

        List<Brick> _compactedAdditionalBricks = new List<Brick>();

        int additionalBricksCounter = 1;
        while (additionalBricksCounter < _additionalBricks.Count - 1)
        {
            if (_additionalBricks[additionalBricksCounter - 1].brickType == 2 && _additionalBricks[additionalBricksCounter].brickType == 2)
            {
                if (
                    ((_additionalBricks[additionalBricksCounter - 1].rotation == Quaternion.Euler(0, 0, 0) ||
                    _additionalBricks[additionalBricksCounter - 1].rotation == Quaternion.Euler(0, 180, 0))
                    &&
                    (_additionalBricks[additionalBricksCounter].rotation == Quaternion.Euler(0, 0, 0) ||
                    _additionalBricks[additionalBricksCounter].rotation == Quaternion.Euler(0, 180, 0)))

                    ||

                    ((_additionalBricks[additionalBricksCounter - 1].rotation == Quaternion.Euler(0, 90, 0) ||
                    _additionalBricks[additionalBricksCounter - 1].rotation == Quaternion.Euler(0, 270, 0))
                    &&
                    (_additionalBricks[additionalBricksCounter].rotation == Quaternion.Euler(0, 90, 0) ||
                    _additionalBricks[additionalBricksCounter].rotation == Quaternion.Euler(0, 270, 0))))
                {
                    Vector3Int newBrickOriginCellPosition = new Vector3Int((_additionalBricks[additionalBricksCounter - 1].originCell.position.x + _additionalBricks[additionalBricksCounter].originCell.position.x) / 2,
                                                                   (_additionalBricks[additionalBricksCounter - 1].originCell.position.y + _additionalBricks[additionalBricksCounter].originCell.position.y) / 2,
                                                                   (_additionalBricks[additionalBricksCounter - 1].originCell.position.z + _additionalBricks[additionalBricksCounter].originCell.position.z) / 2);

                    int newBrickRotation = 0;

                    if (_additionalBricks[additionalBricksCounter].rotation == Quaternion.Euler(0, 90, 0) || _additionalBricks[additionalBricksCounter].rotation == Quaternion.Euler(0, 270, 0))
                    {
                        newBrickRotation = 90;
                    }

                    _compactedAdditionalBricks.Add(new Brick(grid, grid.cellsArray[newBrickOriginCellPosition.x, newBrickOriginCellPosition.y, newBrickOriginCellPosition.z], newBrickRotation, 1, true));

                    if (additionalBricksCounter == _additionalBricks.Count - 2)
                    {
                        _compactedAdditionalBricks.Add(_additionalBricks[additionalBricksCounter + 1]);
                        additionalBricksCounter++;
                    }
                    else
                    {
                        additionalBricksCounter += 2;
                    }
                }
                else
                {
                    _compactedAdditionalBricks.Add(_additionalBricks[additionalBricksCounter - 1]);

                    additionalBricksCounter++;
                }
            }
            else
            {
                additionalBricksCounter++;
            }
        }

        List<Brick> _supportedFinalAdditionalBricks = new List<Brick>();

        foreach (Brick brick in _compactedAdditionalBricks)
        {
            _supportedFinalAdditionalBricks.Add(brick);

            for (int i = 1; i < brick.originCell.position.y; i++)
            {
                int subBrickRotation = 0;

                if (brick.rotation == Quaternion.Euler(0, 90, 0) || brick.rotation == Quaternion.Euler(0, 270, 0))
                {
                    subBrickRotation = 90;
                }
                _supportedFinalAdditionalBricks.Add(new Brick(grid, grid.GetANeighbour(brick.originCell, new Vector3Int(0, -i, 0)), subBrickRotation, brick.brickType, brick.auxBrick));
                _supportedFinalAdditionalBricks[_supportedFinalAdditionalBricks.Count - 1].auxBrick = true;
            }
        }

        return _supportedFinalAdditionalBricks;
    }
    List<Cell> ConvertCellsFromImport(TextAsset _correspondingBrickDataFile)
    {
        List<Cell> cellsInPath = new List<Cell>();

        string pathImportPath = "Assets/ExportData/" + _correspondingBrickDataFile.name.ToString() + "_additionalPath.txt";

        string importDataString = System.IO.File.ReadAllText(pathImportPath);

        CellImportItem[] cellImportArray = JsonHelper.FromJson<CellImportItem>(importDataString);

        for (int i = 0; i < cellImportArray.Length; i++)
        {
            cellsInPath.Add(new Cell(new Vector3Int(cellImportArray[i].posX, cellImportArray[i].posY, cellImportArray[i].posZ)));
        }

        return cellsInPath;
    }

    List<Cell> FindAvailableCells(List<Brick> _bricksInPlace)
    {
        List<Cell> _availableCells = new List<Cell>();
        List<Cell> _allChildCells = new List<Cell>();

        for (int i = 3; i < _bricksInPlace.Count; i++) // first two bricks are the starting bricks for pickup - cannot alter or add to these.
        {
            for (int j = 0; j < _bricksInPlace[i].childCells.Count; j++)
            {
                _allChildCells.Add(_bricksInPlace[i].childCells[j]);
                _availableCells.Add(_bricksInPlace[i].childCells[j]);
            }
        }

        _availableCells.Add(_bricksInPlace[2].childCells[0]);

        for (int i = 0; i < _allChildCells.Count; i++)
        {
            for (int j = 0; j < _allChildCells.Count; j++)
            {
                if (grid.GetANeighbour(_allChildCells[j], new Vector3Int(0, 1, 0)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(0, 1, 1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 1, 1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 1, 0)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 1, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(0, 1, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 1, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 1, 0)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 1, 1)) == _allChildCells[i] ||

                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(0, 2, 1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 2, 1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 2, 0)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 2, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(0, 2, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 2, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 2, 0)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 2, 1)) == _allChildCells[i] ||

                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(0, 3, 1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 3, 1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 3, 0)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(1, 3, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(0, 3, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 3, -1)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 3, 0)) == _allChildCells[i] ||
                    grid.GetANeighbour(_allChildCells[j], new Vector3Int(-1, 3, 1)) == _allChildCells[i])
                {
                    _availableCells.Remove(_allChildCells[j]);
                }
            }
        }

        return _availableCells;
    }

    List<Cell> FindForbiddenCells(List<Brick> _bricksInPlace, List<Cell> _availableCells)
    {
        List<Cell> _forbiddenCells = new List<Cell>();
        List<Cell> _forbiddenChildCells = new List<Cell>();
        List<Brick> _bricksInScene = new List<Brick>();

        for (int i = 0; i < _bricksInPlace.Count; i++)
        {
            _bricksInScene.Add(_bricksInPlace[i]);
        }

        _bricksInScene.Insert(0, new Brick(grid, seedCell, 90, 1, true));

        for (int i = 0; i < _bricksInScene.Count; i++)
        {
            for (int j = 0; j < _bricksInScene[i].childCells.Count; j++)
            {
                if (i == 3 && j == 1)
                {
                    continue;
                }
                if (!_availableCells.Contains(_bricksInScene[i].childCells[j]))
                {
                    _forbiddenChildCells.Add(_bricksInScene[i].childCells[j]);
                }
            }
        }

        for (int y = 1; y < grid.gridSize.y - 1; y++)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < _bricksInScene[i].childCells.Count; j++)
                {
                    if (i == 3 && (j == 0 || j == 1))
                    {
                        continue;
                    }
                    _forbiddenChildCells.Add(grid.GetANeighbour(_bricksInScene[i].childCells[j], new Vector3Int(0, y, 0)));
                }
            }
        }

        for (int i = 0; i < _forbiddenChildCells.Count; i++)
        {
            List<Cell> _candidateCells = new List<Cell> {
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(0,0,0)),

                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(0,0,1)),
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(1,0,1)),
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(1,0,0)),
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(1,0,-1)),
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(0,0,-1)),
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(-1,0,-1)),
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(-1,0,0)),
                grid.GetANeighbour(_forbiddenChildCells[i], new Vector3Int(-1,0,1)),
            };

            for (int j = 0; j < _candidateCells.Count; j++)
            {
                if (!_forbiddenCells.Contains(_candidateCells[j]))
                {
                    _forbiddenCells.Add(_candidateCells[j]);
                }
            }

        }

        return _forbiddenCells;
    }

    void CreateSeed(Vector3Int _seed)
    {
        seedCell = grid.cellsArray[_seed.x, _seed.y, _seed.z];
    }


    List<Brick> CreateBricksInArrangment(TextAsset _brickDataImport)
    {
        List<Brick> _inputStructure = new List<Brick>();
        string importDataString = _brickDataImport.ToString();

        BrickImportItem[] brickImportArray = JsonHelper.FromJson<BrickImportItem>(importDataString);

        for (int i = 0; i < brickImportArray.Length; i++)
        {
            _inputStructure.Add(ConvertToBrick(brickImportArray[i]));
        }

        _inputStructure = ReorderBricks(_inputStructure, seedCell); //////////////////////////////////////////////////

        return _inputStructure;
    }

    Brick ConvertToBrick(BrickImportItem importedBrickItem)
    {
        Brick convertedBrick = null;

        convertedBrick = new Brick(grid, grid.cellsArray[importedBrickItem.brickPosX + seedCell.position.x, importedBrickItem.brickPosZ, importedBrickItem.brickPosY + seedCell.position.z], importedBrickItem.rotation, importedBrickItem.brickType, importedBrickItem.auxBrick);

        // convertedBrick.childCells = grid.GetChildren(convertedBrick);

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



    //private List<Brick> ReorderBricks(List<Brick> _inputStructure, Grid _inputGrid, Cell _seedCell)
    //{
    //    List<Brick> bricksStillToOrder = new List<Brick>();
    //    bricksStillToOrder = _inputStructure;
    //    List<Brick> reorderedStructure = new List<Brick>();
    //    List<Cell> availableCells = new List<Cell>();
    //    List<Cell> forbiddenCells = new List<Cell>();

    //    availableCells.Add(_seedCell);

    //    while (bricksStillToOrder.Count > 0)
    //    {
    //        int bestCurrentCost = 1000000000;
    //        Brick bestCurrentBrick = null;

    //        for (int j = 0; j < bricksStillToOrder.Count; j++)
    //        {
    //            Debug.Log("BricksStillToOrderCount: " + bricksStillToOrder.Count);
    //            List<Cell> testPath = new List<Cell>();
    //            testPath = brickPathFinder.CalculatePathForSequencing(_inputGrid, availableCells, forbiddenCells, _seedCell, bricksStillToOrder[j].originCell, false);
    //            Debug.Log("totalCostOfTrip for " + j + ": " + brickPathFinder.totalCostOfTrip);
    //            if (brickPathFinder.totalCostOfTrip < bestCurrentCost)
    //            {
    //                bestCurrentCost = brickPathFinder.totalCostOfTrip;
    //                bestCurrentBrick = bricksStillToOrder[j];
    //            }
    //        }

    //        if (bestCurrentBrick != null)
    //        {
    //            reorderedStructure.Add(bestCurrentBrick);
    //            bricksStillToOrder.Remove(bestCurrentBrick);

    //            foreach (Cell childCell in bestCurrentBrick.childCells)
    //            {
    //                availableCells.Add(childCell);
    //            }
    //        }
    //    }

    //    return reorderedStructure;
    //}
}
