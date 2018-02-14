using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Brick {

    public Cell originCell;
    public Quaternion rotation;
    List<Cell> childCells = new List<Cell>();
    public Vector3Int gridPosition;

    private Grid _grid;

    public Brick(Cell _inputOriginCell, float _inputRotation)
    {
        originCell = _inputOriginCell;
             rotation = Quaternion.Euler(0, _inputRotation, 0);

    }

    

}
