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

    //Cell[] path;
    bool requestANewPath = false;

    int placementCounter = 1;
    public bool graphIsGenerated = false;

    PathFinder pathFinder = new PathFinder();

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
       // FindPath();
        allBricks[allBricks.Count - placementCounter] = finalBricks[placementCounter - 1];
      //  allBricks[allBricks.Count - placementCounter] = finalBricks[placementCounter - 1];
        
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

    public void FindPath()
    {
        Cell startCell = allBricks[allBricks.Count - placementCounter + 1].originCell;
        Cell targetCell = allBricks[allBricks.Count - placementCounter].originCell;

        startCell.isStart = true;
        targetCell.isEnd = true;

       Cell [] waypoints = pathFinder.FindPath(arrangementGraph, startCell, targetCell);

        foreach (Cell cell in waypoints)
        {
            cell.isPath = true;
        }
    }

    //void RequestPath()
    //{
    //    PathRequestManager.RequestPath(allBricks[allBricks.Count - placementCounter].originCell, finalBricks[placementCounter - 1].originCell, OnPathFound);
    //}

    //public void OnPathFound(Cell[] newPath, bool pathSuccessful)
    //{
    //    if (pathSuccessful)
    //    {
    //        //ResetPath();
    //        //path = newPath;
    //    }

    //    foreach (Cell cell in newPath)
    //    {
    //        cell.isPath = true;
    //    }
    //}

    void ResetPath()
    {
        foreach (Brick brick in allBricks)
        {
            foreach (Cell cell in brick.childCells)
            {
                cell.isPath = false;
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
