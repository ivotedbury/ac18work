using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{

    public Vector3Int position { get; set; }
    public Vector3 worldPosition { get; set; }

    public bool isGround;
    public bool isSolid;
    public bool isSeed;
    public bool isTarget;

    bool ground;
    bool isAlreadyUsedInPath;


    bool temporary;
    int normalStartPathEndSupport;

    public int bCost; // distance from start
    public int hCost; // distance from target
    public int mCost; // cost to make
    int alreadyUsedInPathCost;
    public int coveredCost;

    public Cell parent;

    public Cell(Vector3Int _position)
    {
        position = _position;
        isSolid = false;
        isSeed = false;
        isTarget = false;
    }

    public int fCost()
    {
        int fCost = bCost + hCost + mCost;
        return fCost;
    }

    public int gCost()
    {
        int gCost = bCost + mCost + coveredCost;
        return gCost;
    }

    public void resetCosts()
    {
        bCost = 0;
        hCost = 0;
        mCost = 0;
        alreadyUsedInPathCost = 0;
        coveredCost = 0;
    }
}
