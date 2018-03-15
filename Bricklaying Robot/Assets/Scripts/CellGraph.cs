using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGraph
{
    List<Cell> allCellsInArrangement = new List<Cell>();
    public List<Cell> availableCells = new List<Cell>();

    public List<GraphBranch> graphBranches = new List<GraphBranch>();

    //horizontalSteps
    Vector3Int[] horizontalStepTwo = new Vector3Int[] { new Vector3Int(2, 0, 0), new Vector3Int(0, 0, -2), new Vector3Int(-2, 0, 0), new Vector3Int(0, 0, 2) };
    Vector3Int[] horizontalStepThree = new Vector3Int[] { new Vector3Int(3, 0, 0), new Vector3Int(0, 0, -3), new Vector3Int(-3, 0, 0), new Vector3Int(0, 0, 3) };
    Vector3Int[] horizontalStepFour = new Vector3Int[] { new Vector3Int(4, 0, 0), new Vector3Int(0, 0, -4), new Vector3Int(-4, 0, 0), new Vector3Int(0, 0, 4) };
    Vector3Int[] horizontalStepFive = new Vector3Int[] { new Vector3Int(5, 0, 0), new Vector3Int(0, 0, -5), new Vector3Int(-5, 0, 0), new Vector3Int(0, 0, 5) };

    //upSteps
    Vector3Int[] upStepTwo = new Vector3Int[] { new Vector3Int(2, 1, 0), new Vector3Int(0, 1, -2), new Vector3Int(-2, 1, 0), new Vector3Int(0, 1, 2) };
    Vector3Int[] upStepThree = new Vector3Int[] { new Vector3Int(3, 1, 0), new Vector3Int(0, 1, -3), new Vector3Int(-3, 1, 0), new Vector3Int(0, 1, 3) };
    Vector3Int[] upStepFour = new Vector3Int[] { new Vector3Int(4, 1, 0), new Vector3Int(0, 1, -4), new Vector3Int(-4, 1, 0), new Vector3Int(0, 1, 4) };
    Vector3Int[] upStepFive = new Vector3Int[] { new Vector3Int(5, 1, 0), new Vector3Int(0, 1, -5), new Vector3Int(-5, 1, 0), new Vector3Int(0, 1, 5) };

    //downSteps
    Vector3Int[] downStepTwo = new Vector3Int[] { new Vector3Int(2, -1, 0), new Vector3Int(0, -1, -2), new Vector3Int(-2, -1, 0), new Vector3Int(0, -1, 2) };
    Vector3Int[] downStepThree = new Vector3Int[] { new Vector3Int(3, -1, 0), new Vector3Int(0, -1, -3), new Vector3Int(-3, -1, 0), new Vector3Int(0, -1, 3) };
    Vector3Int[] downStepFour = new Vector3Int[] { new Vector3Int(4, -1, 0), new Vector3Int(0, -1, -4), new Vector3Int(-4, -1, 0), new Vector3Int(0, -1, 4) };
    Vector3Int[] downStepFive = new Vector3Int[] { new Vector3Int(5, -1, 0), new Vector3Int(0, -1, -5), new Vector3Int(-5, -1, 0), new Vector3Int(0, -1, 5) };

    //specialPlacementOnlySteps
    Vector3Int[] diagonalHorizontalStepTwoA = new Vector3Int[] { new Vector3Int(2, 0, 1), new Vector3Int(2, 0, -1), new Vector3Int(-2, 0, -1), new Vector3Int(-2, 0, 1) };
    Vector3Int[] diagonalHorizontalStepTwoB = new Vector3Int[] { new Vector3Int(1, 0, 2), new Vector3Int(1, 0, -2), new Vector3Int(-1, 0, -2), new Vector3Int(-1, 0, 2) };

    Vector3Int[] diagonalUpStepTwoA = new Vector3Int[] { new Vector3Int(2, 1, 1), new Vector3Int(2, 1, -1), new Vector3Int(-2, 1, -1), new Vector3Int(-2, 1, 1) };
    Vector3Int[] diagonalUpStepTwoB = new Vector3Int[] { new Vector3Int(1, 1, 2), new Vector3Int(1, 1, -2), new Vector3Int(-1, 1, -2), new Vector3Int(-1, 1, 2) };

    Vector3Int[] diagonalDownStepTwoA = new Vector3Int[] { new Vector3Int(2, -1, 1), new Vector3Int(2, -1, -1), new Vector3Int(-2, -1, -1), new Vector3Int(-2, -1, 1) };
    Vector3Int[] diagonalDownStepTwoB = new Vector3Int[] { new Vector3Int(1, -1, 2), new Vector3Int(1, -1, -2), new Vector3Int(-1, -1, -2), new Vector3Int(-1, -1, 2) };

    //////////////////////////////////////////////////////////////////////////////////////////

    public void GenerateCellGraph(List<Cell> _allCellsInArrangement)
    {
        allCellsInArrangement = _allCellsInArrangement;

        ComputeCellGraph(_allCellsInArrangement);
    }

    public void ComputeCellGraph(List<Cell> _allCellsInArrangement)
    {

        availableCells.Clear();
        availableCells = FilterTopCells(_allCellsInArrangement);

        graphBranches.Clear();
        MakeGraphBranches(availableCells);

    }

    //public void CheckGraphConnectivity(List<GraphBranch> _graphBranches)
    //{
    //    for ()
    //}

    public void MakeGraphBranches(List<Cell> _availableCells)
    {
        Vector3Int startToEnd;

        for (int i = 0; i < _availableCells.Count; i++)
        {
            for (int j = 0; j < _availableCells.Count; j++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    startToEnd = _availableCells[j].position - _availableCells[i].position;

                    // check for horizontalSteps
                    if (startToEnd == horizontalStepTwo[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 2));
                    }
                    if (startToEnd == horizontalStepThree[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 3));
                    }
                    if (startToEnd == horizontalStepFour[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 4));
                    }
                    if (startToEnd == horizontalStepFive[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 5));
                    }

                    // check for upSteps
                    if (startToEnd == upStepTwo[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 12));
                    }
                    if (startToEnd == upStepThree[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 13));
                    }
                    if (startToEnd == upStepFour[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 14));
                    }
                    if (startToEnd == upStepFive[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], 15));
                    }

                    // check for downSteps
                    if (startToEnd == downStepTwo[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], -2));
                    }
                    if (startToEnd == downStepThree[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], -3));
                    }
                    if (startToEnd == downStepFour[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], -4));
                    }
                    if (startToEnd == downStepFive[k])
                    {
                        graphBranches.Add(new GraphBranch(_availableCells[i], _availableCells[j], -5));
                    }

                }
            }
        }
    }


    public List<Cell> FilterTopCells(List<Cell> allCells) // filter out the exposed cells 
    {
        List<Cell> coveredCells = new List<Cell>();
        List<Cell> topCellsOnly = new List<Cell>();

        for (int i = 0; i < allCells.Count; i++) // add all cells to list
        {
            topCellsOnly.Add(allCells[i]);
        }

        for (int i = 0; i < allCells.Count; i++)
        {
            for (int j = 0; j < topCellsOnly.Count; j++)
            {
                if (allCells[i].position + new Vector3Int(0, 1, 0) == allCells[j].position ||
                    allCells[i].position + new Vector3Int(1, 1, 0) == allCells[j].position ||
                    allCells[i].position + new Vector3Int(0, 1, -1) == allCells[j].position ||
                    allCells[i].position + new Vector3Int(-1, 1, 0) == allCells[j].position ||
                    allCells[i].position + new Vector3Int(0, 1, 1) == allCells[j].position) // if there are cells above then add to list of covered cells
                {
                    coveredCells.Add(allCells[i]);
                }
            }
        }

        for (int i = 0; i < coveredCells.Count; i++)
        {
            if (topCellsOnly.Contains(coveredCells[i])) // remove each covered cell from final list of top cells
            {
                topCellsOnly.Remove(coveredCells[i]);
            }
        }

        foreach (Cell topCell in topCellsOnly)
        {
            topCell.isAvailable = true;
        }

        foreach (Cell coveredCell in coveredCells)
        {
            coveredCell.isAvailable = false;
        }

        return topCellsOnly;
    }

    public List<Cell> GetPathFinderNeighbours(Cell testCell)
    {
        List<Cell> neighbours = new List<Cell>();

        for (int i = 0; i < availableCells.Count; i++)
        {
            for (int k = 0; k <= 3; k++)
            {

                if (testCell.position - availableCells[i].position == horizontalStepTwo[k] ||
                    testCell.position - availableCells[i].position == horizontalStepThree[k] ||
                    testCell.position - availableCells[i].position == horizontalStepFour[k] ||
                    testCell.position - availableCells[i].position == horizontalStepFive[k] ||

                    testCell.position - availableCells[i].position == upStepTwo[k] ||
                    testCell.position - availableCells[i].position == upStepThree[k] ||
                    testCell.position - availableCells[i].position == upStepFour[k] ||
                    testCell.position - availableCells[i].position == upStepFive[k] ||

                    testCell.position - availableCells[i].position == downStepTwo[k] ||
                    testCell.position - availableCells[i].position == downStepThree[k] ||
                    testCell.position - availableCells[i].position == downStepFour[k] ||
                    testCell.position - availableCells[i].position == downStepFive[k])

                {
                    neighbours.Add(availableCells[i]);
                }
            }
        }
        return neighbours;
    }


    public Cell FindBestNeighbour(Brick brickToPlace, bool _onDelivery)
    {
        Cell bestNeighbour = null;

        Vector3Int differenceVector;

        List<Cell> availableCellsReordered = new List<Cell>();
        availableCellsReordered = availableCells;
        availableCellsReordered.Reverse();

        for (int i = 0; i < availableCellsReordered.Count; i++)
        {
            for (int k = 0; k <= 3; k++)
            {
                differenceVector = brickToPlace.originCell.position - availableCellsReordered[i].position;

                if (!_onDelivery)
                {
                    if (differenceVector == horizontalStepTwo[k] || differenceVector == horizontalStepThree[k])
                    {
                        bestNeighbour = availableCellsReordered[i];
                        return bestNeighbour;
                    }
                    else if (differenceVector == downStepTwo[k] || differenceVector == downStepThree[k])
                    {
                        bestNeighbour = availableCellsReordered[i];
                        return bestNeighbour;
                    }
                    else if (differenceVector == upStepTwo[k] || differenceVector == upStepThree[k])
                    {
                        bestNeighbour = availableCellsReordered[i];
                        return bestNeighbour;
                    }
                }

                if (_onDelivery)
                {

                    if (differenceVector == horizontalStepTwo[k] || differenceVector == horizontalStepThree[k])
                    {
                        bestNeighbour = availableCellsReordered[i];
                        return bestNeighbour;
                    }
                    else if (differenceVector == downStepTwo[k] || differenceVector == downStepThree[k])
                    {
                        bestNeighbour = availableCellsReordered[i];
                        return bestNeighbour;
                    }
                    else if (differenceVector == upStepTwo[k] || differenceVector == upStepThree[k])
                    {
                        bestNeighbour = availableCellsReordered[i];
                        return bestNeighbour;
                    }
                    else if (differenceVector == diagonalHorizontalStepTwoA[k] || differenceVector == diagonalHorizontalStepTwoB[k])
                    {
                        bestNeighbour = availableCellsReordered[i];
                        return bestNeighbour;
                    }
                }
            }
        }
        return bestNeighbour;

    }

    public bool checkPathComplete()
    {
        bool pathIsComplete = false;



    return pathIsComplete;
    }
}



//(availableCells[i].position - testCell.position ==  horizontalStepTwo[k] || 
//                    availableCells[i].position - testCell.position == horizontalStepThree[k] ||
//                    availableCells[i].position - testCell.position == horizontalStepFour[k] ||
//                    availableCells[i].position - testCell.position == horizontalStepFive[k] ||

//                    availableCells[i].position - testCell.position == upStepTwo[k] ||
//                    availableCells[i].position - testCell.position == upStepThree[k] ||
//                    availableCells[i].position - testCell.position == upStepFour[k] ||
//                    availableCells[i].position - testCell.position == upStepFive[k] ||

//                    availableCells[i].position - testCell.position == downStepTwo[k] ||
//                    availableCells[i].position - testCell.position == downStepThree[k] ||
//                    availableCells[i].position - testCell.position == downStepFour[k] ||
//                    availableCells[i].position - testCell.position == downStepFive[k])