using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickArrangement
{
    // specific dimensions
    float gridDimX = 0.05625f;
    float gridDimY = 0.0725f;
    float gridDimZ = 0.05625f;
    public Vector3 displayCellOffset = new Vector3(0, 0.03625f, 0);

    public Vector3Int gateCell;

    public BaseGrid workGrid;
    public CellGraph arrangementGraph = new CellGraph();

    public List<Brick> finalBricks = new List<Brick>();
    public List<Brick> stackBricks = new List<Brick>();
    public List<Brick> allBricks = new List<Brick>();

    public List<Cell> currentPath = new List<Cell>();
    Cell previousTarget;
    Cell previousPickup;

    //Cell[] path;
    bool requestANewPath = false;

    int currentDirection = 0;
    int placementCounter = 1;
    public bool graphIsGenerated = false;

    public PathFinder pathFinder = new PathFinder();

    public BrickArrangement(int gridX, int gridY, int gridZ)
    {
        workGrid = new BaseGrid(new Vector3Int(20, 20, 41));
    }

    public void CreateBricksInArrangment(TextAsset brickDataImport)
    {
        string importDataString = brickDataImport.ToString();

        BrickImportItem[] brickImportArray = JsonHelper.FromJson<BrickImportItem>(importDataString);

        for (int i = 0; i < brickImportArray.Length; i++)
        {
            finalBricks.Add(ConvertToBrick(brickImportArray[i]));
        }

        finalBricks = ReorderStack(finalBricks, gateCell);
    }

    public void DepositBrick()
    {
        allBricks[allBricks.Count - placementCounter] = finalBricks[placementCounter - 1];

        placementCounter++;
    }

    public void CreateStack(TextAsset stackDataImport)
    {
        string importDataString = stackDataImport.ToString();

        BrickImportItem[] stackImportArray = JsonHelper.FromJson<BrickImportItem>(importDataString);

        for (int i = 0; i < stackImportArray.Length; i++)
        {
            stackBricks.Add(ConvertToBrick(stackImportArray[i]));
        }

        stackBricks = ReorderStack(stackBricks, gateCell);

        for (int i = 0; i < stackBricks.Count; i++)
        {
            allBricks.Add(stackBricks[i]);
        }

        GenerateGraph();
    }

    public bool CheckPath()
    {
       bool pathFound = false;

        Cell startCell = stackBricks[stackBricks.Count - placementCounter].originCell;
        Cell targetCell = finalBricks[placementCounter - 1].originCell;

        // Cell previousTargetCell;

        //if (toDeliver)
        //{
        //    Brick brickToPlace = finalBricks[placementCounter - 1];
        //    Brick brickToPickup = stackBricks[stackBricks.Count - placementCounter];

        //    startCell = FindBestDropCell(brickToPickup, !toDeliver);
        //    targetCell = FindBestDropCell(brickToPlace, toDeliver);
        //    previousTarget = targetCell;
        //}

        //else
        //{
        //    startCell = previousTarget;
        //    targetCell = FindBestDropCell(stackBricks[stackBricks.Count - placementCounter - 1], toDeliver);
        //    //  previousPickup = targetCell;
        //}

        startCell.isStart = true;
        targetCell.isEnd = true;

        pathFound = pathFinder.CheckPath(arrangementGraph, startCell, targetCell);
        return pathFound;
    }

    public void FindPath(bool toDeliver)
    {
        ResetPath();

        Cell startCell = FindBestDropCell(stackBricks[stackBricks.Count - 1], toDeliver);
        Cell targetCell = FindBestDropCell(finalBricks[placementCounter - 1], toDeliver);

        // Cell previousTargetCell;

        if (toDeliver)
        {
            Brick brickToPlace = finalBricks[placementCounter - 1];
            Brick brickToPickup = stackBricks[stackBricks.Count - placementCounter];

            startCell = FindBestDropCell(brickToPickup, !toDeliver);
            targetCell = FindBestDropCell(brickToPlace, toDeliver);
            previousTarget = targetCell;
        }

        else
        {
            startCell = previousTarget;
            targetCell = FindBestDropCell(stackBricks[stackBricks.Count - placementCounter - 1], toDeliver);
            //  previousPickup = targetCell;
        }

        startCell.isStart = true;
        targetCell.isEnd = true;

        List<Cell> waypoints = pathFinder.FindPath(arrangementGraph, startCell, targetCell, currentDirection);
        currentPath = waypoints;

        foreach (Cell cell in waypoints)
        {
            cell.isPath = true;
        }
    }

    public Cell FindBestDropCell(Brick _brickToPlace, bool _onDelivery)
    {
        Cell bestDropCell; // = _brickToPlace.originCell;

        bestDropCell = arrangementGraph.FindBestNeighbour(_brickToPlace, _onDelivery);

        return bestDropCell;
    }

    void ResetPath()
    {
        foreach (Brick brick in allBricks)
        {
            foreach (Cell cell in brick.childCells)
            {
                cell.isPath = false;
                cell.isEnd = false;
                cell.isStart = false;
            }
        }
    }

    public bool SetGateCell(Vector3Int gateCellInputLocation)
    {
        if (gateCellInputLocation.x <= workGrid.gridSize.x && gateCellInputLocation.y <= workGrid.gridSize.y && gateCellInputLocation.z <= workGrid.gridSize.z)
        {
            gateCell = gateCellInputLocation;
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<Brick> ReorderStack(List<Brick> unsortedStack, Vector3Int gateCellPos)
    {
        List<Brick> reorderedStack = new List<Brick>();

        int aLargeNumber = 20;

        float currentClosestDistance;
        float testDistance;
        int listLength = unsortedStack.Count;

        Brick bestCurrentBrick = null;
        bool betterBrickFound = false;

        for (int currentSearchLayer = 0; currentSearchLayer < aLargeNumber; currentSearchLayer++)
        {
            for (int j = 0; j < listLength; j++)
            {
                currentClosestDistance = 100;
                betterBrickFound = false;

                for (int i = 0; i < unsortedStack.Count; i++) // current closest brick 
                {
                    if (unsortedStack[i].originCell.position.y == currentSearchLayer) // if Y is on current search layer
                    {
                        testDistance = Vector3.Magnitude(gateCellPos - unsortedStack[i].originCell.position);

                        if (testDistance < currentClosestDistance)
                        {
                            currentClosestDistance = testDistance;
                            bestCurrentBrick = unsortedStack[i];
                            betterBrickFound = true;
                        }
                    }
                }

                if (betterBrickFound == true)
                {
                    reorderedStack.Add(bestCurrentBrick);

                    unsortedStack.Remove(bestCurrentBrick);
                }
            }
        }
        return reorderedStack;
    }

    Brick ConvertToBrick(BrickImportItem importedBrickItem)
    {
        Brick convertedBrick = null;

        for (int z = 0; z < workGrid.gridSize.z; z++)
        {
            for (int y = 0; y < workGrid.gridSize.y; y++)
            {
                for (int x = 0; x < workGrid.gridSize.x; x++)
                {
                    if (workGrid.cells[x, y, z].position == new Vector3Int(importedBrickItem.brickPosX, importedBrickItem.brickPosZ, importedBrickItem.brickPosY)) // swap y and z for rhino to unity!
                    {
                        convertedBrick = new Brick(workGrid.cells[x, y, z], importedBrickItem.rotation);
                    }
                }
            }
        }
        convertedBrick.childCells = workGrid.GetChildren(convertedBrick);

        return convertedBrick;
    }

    public Vector3 GetRealBrickPosition(Brick _brick)
    {
        Vector3 brickPosition = GetRealCellPosition(_brick.childCells[1]);

        return brickPosition;
    }

    public Vector3 GetRealCellPosition(Cell inputCell)
    {
        Vector3Int inputCellPosition = inputCell.position;
        Vector3 realPosition = new Vector3(inputCellPosition.x * gridDimX, inputCellPosition.y * gridDimY, inputCellPosition.z * gridDimZ);

        return realPosition;
    }

    public Vector3 GetRealPosition(Vector3Int intPosition)
    {
        Vector3 realPosition = new Vector3(intPosition.x * gridDimX, intPosition.y * gridDimY, intPosition.z * gridDimZ);

        return realPosition;
    }

    public List<Cell> GetCellsInArrangement(List<Brick> _allBricksInArrangement)
    {
        List<Cell> cellsInArrangement = new List<Cell>();

        for (int i = 0; i < _allBricksInArrangement.Count; i++)
        {
            for (int j = 0; j < workGrid.GetChildren(_allBricksInArrangement[i]).Count; j++)
            {
                cellsInArrangement.Add(workGrid.GetChildren(_allBricksInArrangement[i])[j]);
            }
        }

        return cellsInArrangement;
    }

    public void GenerateGraph()
    {
        arrangementGraph.GenerateCellGraph(GetCellsInArrangement(allBricks));
        graphIsGenerated = true;
    }
}
