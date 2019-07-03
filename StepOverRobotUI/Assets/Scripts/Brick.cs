using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick
{

    public Cell originCell;
    public Quaternion rotation;
    public int brickType;
    public List<Cell> childCells = new List<Cell>();
    public bool auxBrick;
    public int distanceFromSeedForSequencing;

    public Vector3 currentPosition;
    public Quaternion currentRotation;

    public Brick(Grid _inputGrid, Cell _inputOriginCell, float _inputRotation, int _brickType, bool _auxBrick) // creates a Brick given a origin cell in the grid and a rotation
    {
        originCell = _inputOriginCell;
        rotation = Quaternion.Euler(0, _inputRotation, 0);
        brickType = _brickType;

        auxBrick = _auxBrick;

        AssignChildCells(_inputGrid);

        currentPosition = originCell.actualPosition;
        currentRotation = rotation;
    }

    void AssignChildCells(Grid _inputGrid)
    {
        //   childCells.Add(originCell);

        childCells.Clear();

        if (brickType == 1)
        {
            if (rotation == Quaternion.Euler(0, 0, 0) || rotation == Quaternion.Euler(0, 180, 0))
            {
                childCells.Add(_inputGrid.GetANeighbour(originCell, new Vector3Int(0, 0, 1)));
                childCells.Add(_inputGrid.GetANeighbour(originCell, new Vector3Int(0, 0, 0)));
                childCells.Add(_inputGrid.GetANeighbour(originCell, new Vector3Int(0, 0, -1)));

            }

            if (rotation == Quaternion.Euler(0, 90, 0) || rotation == Quaternion.Euler(0, 270, 0))
            {
                childCells.Add(_inputGrid.GetANeighbour(originCell, new Vector3Int(1, 0, 0)));
                childCells.Add(_inputGrid.GetANeighbour(originCell, new Vector3Int(0, 0, 0)));
                childCells.Add(_inputGrid.GetANeighbour(originCell, new Vector3Int(-1, 0, 0)));
            }
        }
        else if (brickType == 2)
        {
            childCells.Add(_inputGrid.GetANeighbour(originCell, new Vector3Int(0, 0, 0)));
        }
    }

}
