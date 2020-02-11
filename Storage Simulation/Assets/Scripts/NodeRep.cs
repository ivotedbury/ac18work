using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeRep
{
    public Vector3Int pos;
    public Node parent;

    public float gCost;
    public float hCost;
    public bool walkable;

    public float fCost()
    {
        return gCost + hCost;
    }

    public NodeRep(Vector3Int _inputPos)
    {
        pos = _inputPos;
        gCost = 0;
        hCost = 0;
        walkable = true;
    }
}

