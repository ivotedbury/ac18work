using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int position { get; set; }
    public Vector3 worldPosition { get; set; }

    public bool isAvailable;
    public bool isPath;
    public bool isStart;
    public bool isEnd;


    // pathfinding variables
    public int gCost;
    public int hCost;
    public Cell parent;

    public int currentDirection;

    public Cell(Vector3Int cellPosition)
    {
        position = cellPosition;
        isAvailable = false;
        isPath = false;
        isStart = false;
        isEnd = false;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
