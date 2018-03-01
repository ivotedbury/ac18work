using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int position { get; set; }
    public Vector3 worldPosition { get; set; }

    public bool isAvailable;

    public Cell(Vector3Int cellPosition)
    {
        position = cellPosition;
        isAvailable = false;
    }
}
