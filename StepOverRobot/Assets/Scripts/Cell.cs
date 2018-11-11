using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int position;
    public Vector3 actualPosition;

    float gridDimX = 0.05625f;
    float gridDimY = 0.0725f; // 0.0725 with mortar space
    float gridDimZ = 0.05625f;

    public Cell(Vector3Int _position)
    {
        position = _position;
        actualPosition = new Vector3(position.x * gridDimX, position.y * gridDimY, position.z * gridDimZ);
    }
}
