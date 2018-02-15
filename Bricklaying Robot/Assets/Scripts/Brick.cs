using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick
{

    public Cell originCell;
    public Quaternion rotation;
    public List<Cell> childCells = new List<Cell>();
    public Vector3Int gridPosition;

    private Grid _grid;

    public Brick(Cell _inputOriginCell, float _inputRotation) // creates a Brick given a origin cell in the grid and a rotation
    {
        originCell = _inputOriginCell;
        rotation = Quaternion.Euler(0, _inputRotation, 0);
    }

}
