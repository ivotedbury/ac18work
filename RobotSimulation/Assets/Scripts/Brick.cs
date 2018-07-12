using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick
{

    public Cell originCell;
    public Quaternion rotation;
    public int brickType;
    public List<Cell> childCells = new List<Cell>();

    public Brick(Cell _inputOriginCell, float _inputRotation, int _brickType) // creates a Brick given a origin cell in the grid and a rotation
    {
        originCell = _inputOriginCell;
        rotation = Quaternion.Euler(0, _inputRotation, 0);
        brickType = _brickType;
    }

}
