using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSequence
{

    List<Brick> inputStructure = new List<Brick>();

   public List<Brick> finalStructureToBuild = new List<Brick>();

    BrickPathFinder brickPathFinder = new BrickPathFinder();

    public BuildSequence(List<Brick> _inputStructure, Grid _inputGrid, Cell _seedCell)
    {
        inputStructure = _inputStructure;
        Debug.Log(inputStructure.Count);
       finalStructureToBuild = ReorderBricks(_inputStructure, _inputGrid, _seedCell);
    }

    private List<Brick> ReorderBricks(List<Brick> _inputStructure, Grid _inputGrid, Cell _seedCell)
    {
        List<Brick> bricksStillToOrder = new List<Brick>();
        bricksStillToOrder = _inputStructure;
        List<Brick> reorderedStructure = new List<Brick>();
        List<Cell> availableCells = new List<Cell>();

        int bestCurrentCost = 100000000;
        Brick bestCurrentBrick = null;

        for (int i = 0; i < _inputStructure.Count; i ++)
        {
            Debug.Log("BricksStillToOrderCount: " + bricksStillToOrder.Count);
            for (int j = 0; j < bricksStillToOrder.Count; j++)
            {
                Debug.Log("Catch1");

                brickPathFinder.CalculatePathForSequencing(_inputGrid, availableCells, _seedCell, bricksStillToOrder[j].originCell);
                Debug.Log("Catch2");
                if (brickPathFinder.totalCostOfTrip < bestCurrentCost)
                {
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
