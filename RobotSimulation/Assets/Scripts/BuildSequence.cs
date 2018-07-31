using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSequence
{

    List<Brick> inputStructure = new List<Brick>();
    List<Brick> additionalStartingBricks = new List<Brick>();


    public List<Brick> finalStructureToBuild = new List<Brick>();

    BrickPathFinder brickPathFinder = new BrickPathFinder();

    public BuildSequence(List<Brick> _inputStructure, Grid _inputGrid, Cell _seedCell)
    {
        inputStructure = _inputStructure;
        Debug.Log(inputStructure.Count);
        additionalStartingBricks.Add(new Brick(_inputGrid.GetANeighbour(_seedCell, new Vector3Int(4, 0, 0)), 90, 1));
        additionalStartingBricks.Add(new Brick(_inputGrid.GetANeighbour(_seedCell, new Vector3Int(8, 0, 0)), 90, 1));
        additionalStartingBricks.Add(new Brick(_inputGrid.GetANeighbour(_seedCell, new Vector3Int(12, 0, 0)), 90, 1));


        //  finalStructureToBuild = ReorderBricks(intputStructure, _inputGrid, _seedCell);
        finalStructureToBuild = GenerateCompletePaths(inputStructure, additionalStartingBricks, _inputGrid, _seedCell); // was finalStructureToBuild for reordering
    }

    private List<Brick> GenerateCompletePaths(List<Brick> _inputStructure, List<Brick> _additionalStartingBricks, Grid _inputGrid, Cell _seedCell)
    {
        List<Brick> brickStructureCompletedPaths = new List<Brick>();

        List<Cell> availableCells = new List<Cell>();
        List<Cell> forbiddenCells = new List<Cell>();

        List<Brick> bricksInPlace = new List<Brick>();
        Brick brickToPlaceNext;
        List<Brick> newBricksRequired = new List<Brick>();
        Cell pathStartingCell = _additionalStartingBricks[2].originCell;

        //for (int i = _inputStructure.Count - 1; i >= 0; i--)
        //{
        int i = _inputStructure.Count - 1;

        brickToPlaceNext = _inputStructure[i];
        // add the bricks already in place
        bricksInPlace.Clear();

        bricksInPlace = _additionalStartingBricks;
        for (int j = 0; j < i; j++)
        {
            bricksInPlace.Add(_inputStructure[j]);
        }

        availableCells = GetAvailableCells(bricksInPlace, _inputGrid);
        forbiddenCells = GetForbiddenCells(bricksInPlace, availableCells, _inputGrid);

        List<Cell> desiredPath = new List<Cell>();
        desiredPath = brickPathFinder.CalculatePathForSequencing(_inputGrid, availableCells, forbiddenCells, pathStartingCell, brickToPlaceNext.originCell, true);

        List<Cell> extraCellsRequired = new List<Cell>();
        foreach (Cell pathCell in desiredPath)
        {
            if (!availableCells.Contains(pathCell) && pathCell != pathStartingCell && pathCell != brickToPlaceNext.originCell)
            {
                extraCellsRequired.Add(pathCell);
            }
        }
        
        foreach (Cell extraCellRequired in extraCellsRequired)
        {
            newBricksRequired.Add(new Brick(extraCellRequired, 0, 2));
            newBricksRequired[newBricksRequired.Count - 1].auxBrick = true;
        }

        //}

        foreach (Brick brickInPlace in bricksInPlace)
        {
            brickStructureCompletedPaths.Add(brickInPlace);
        }

        foreach (Brick newBrickRequired in newBricksRequired)
        {
            brickStructureCompletedPaths.Add(newBrickRequired);
        }

        return brickStructureCompletedPaths;
    }

    public List<Cell> GetForbiddenCells(List<Brick> _bricksInPlace, List<Cell> _availableCells, Grid _inputGrid)
    {
        List<Cell> forbiddenCells = new List<Cell>();

        List<Cell> allChildCells = new List<Cell>();

        for (int i = 0; i < _bricksInPlace.Count; i++)
        {
            for (int j = 0; j < _bricksInPlace[i].childCells.Count; j++)
            {
                allChildCells.Add(_bricksInPlace[i].childCells[j]);
               // forbiddenCells.Add(_bricksInPlace[i].childCells[j]);
            }
        }

        for (int i = 0; i < allChildCells.Count; i++)
        {
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(0, 0, 0)));

            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(0, 0, 1)));
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(1, 0, 1)));
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(1, 0, 0)));
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(1, 0, -1)));
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(0, 0, -1)));
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(-1, 0, -1)));
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(-1, 0, 0)));
            forbiddenCells.Add(_inputGrid.GetANeighbour(allChildCells[i], new Vector3Int(-1, 0, 1)));

        }

        for (int i = 0; i < _availableCells.Count; i++)
        {
            forbiddenCells.Remove(_availableCells[i]);
        }

        return forbiddenCells;
    }

    public List<Cell> GetAvailableCells(List<Brick> _bricksInPlace, Grid _inputGrid)
    {
        List<Cell> availableCells = new List<Cell>();

        List<Cell> allChildCells = new List<Cell>();

        for (int i = 0; i < _bricksInPlace.Count; i++)
        {
            for (int j = 0; j < _bricksInPlace[i].childCells.Count; j++)
            {
                allChildCells.Add(_bricksInPlace[i].childCells[j]);
                availableCells.Add(_bricksInPlace[i].childCells[j]);
            }
        }

        for (int i = 0; i < allChildCells.Count; i++)
        {
            for (int j = 0; j < allChildCells.Count; j++)
            {
                if (_inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(0, 1, 0)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(0, 1, 1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 1, 1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 1, 0)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 1, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(0, 1, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 1, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 1, 0)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 1, 1)) == allChildCells[i] ||

                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(0, 2, 1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 2, 1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 2, 0)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 2, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(0, 2, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 2, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 2, 0)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 2, 1)) == allChildCells[i] ||

                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(0, 3, 1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 3, 1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 3, 0)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(1, 3, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(0, 3, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 3, -1)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 3, 0)) == allChildCells[i] ||
                    _inputGrid.GetANeighbour(allChildCells[j], new Vector3Int(-1, 3, 1)) == allChildCells[i])
                {
                    availableCells.Remove(allChildCells[j]);
                }
            }
        }
        return availableCells;
    }

    private List<Brick> ReorderBricks(List<Brick> _inputStructure, Grid _inputGrid, Cell _seedCell)
    {
        List<Brick> bricksStillToOrder = new List<Brick>();
        bricksStillToOrder = _inputStructure;
        List<Brick> reorderedStructure = new List<Brick>();
        List<Cell> availableCells = new List<Cell>();
        List<Cell> forbiddenCells = new List<Cell>();

        availableCells.Add(_seedCell);

        while (bricksStillToOrder.Count > 0)
        {
            int bestCurrentCost = 1000000000;
            Brick bestCurrentBrick = null;

            for (int j = 0; j < bricksStillToOrder.Count; j++)
            {
                Debug.Log("BricksStillToOrderCount: " + bricksStillToOrder.Count);
                List<Cell> testPath = new List<Cell>();
                testPath = brickPathFinder.CalculatePathForSequencing(_inputGrid, availableCells, forbiddenCells, _seedCell, bricksStillToOrder[j].originCell, false);
                Debug.Log("totalCostOfTrip for " + j + ": " + brickPathFinder.totalCostOfTrip);
                if (brickPathFinder.totalCostOfTrip < bestCurrentCost)
                {
                    bestCurrentCost = brickPathFinder.totalCostOfTrip;
                    bestCurrentBrick = bricksStillToOrder[j];
                }
            }

            if (bestCurrentBrick != null)
            {
                reorderedStructure.Add(bestCurrentBrick);
                bricksStillToOrder.Remove(bestCurrentBrick);

                foreach (Cell childCell in bestCurrentBrick.childCells)
                {
                    availableCells.Add(childCell);
                }
            }
        }

        return reorderedStructure;
    }
}
