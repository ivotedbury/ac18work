using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Brick {

    public Cell originCell;
    public Quaternion rotation;
    List<Cell> childCells = new List<Cell>();
    public Vector3Int gridPosition;

    private Grid _grid;

    public Brick(Vector3 _gridPosition, float _inputRotation)
    {
        gridPosition = new Vector3Int((int)_gridPosition.x, (int)_gridPosition.y, (int)_gridPosition.x);
       // originCell = inputOriginCell;
        rotation = Quaternion.Euler(0, _inputRotation, 0);

    }

}
