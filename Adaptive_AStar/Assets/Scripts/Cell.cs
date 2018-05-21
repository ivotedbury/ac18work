using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {

    public Vector3Int position { get; set; }
    public Vector3 worldPosition { get; set; }

   public bool isSolid;
    public bool isSeed;
   public bool isTarget;

    bool ground;
    bool isAlreadyUsedInPath;
   
    
    bool temporary;
    int normalStartPathEndSupport;

    int bCost; // distance from start
    int hCost; // distance from target
    int mCost; // cost to make
    int alreadyUsedInPathCost;
    int coveredCost;
    Cell parent;

    public Cell(Vector3Int _position)
    {
        position = _position;
        isSolid = false;
        isSeed = false;
        isTarget = false;
    }

    int fCost()
    {
        int fCost = bCost + hCost + mCost;
        return fCost;
    }

    int gCost()
    {
        int gCost = bCost + mCost + coveredCost;
        return gCost;
    }
}
