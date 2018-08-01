using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int position { get; set; }
    public Vector3 actualPosition { get; set; }

    float gridDimX = 0.05625f;
    float gridDimY = 0.0625f; // 0.0725 with mortar space
    float gridDimZ = 0.05625f;

    public int currentStatus = 0; // normal / start / path / end

    public float gCost;
    public float hCost;
    public float availableCost;

    public Cell parent;

    public Cell(Vector3Int _position)
    {
        position = _position;
        actualPosition = new Vector3(position.x * gridDimX, (position.y + 1) * gridDimY, position.z * gridDimZ);
    }

    public void ResetCosts()
    {
        gCost = 0;
        hCost = 0;
        availableCost = 0;
    }
    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
