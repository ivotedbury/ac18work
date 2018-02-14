using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class BrickImportItem {

    public int brickPosX;
    public int brickPosY;
    public int brickPosZ;

    public float rotation;

    public Brick ConvertToBrick (BrickImportItem importedBrickItem, BaseGrid inputGrid)
    {
        Brick convertedBrick;

        for(int z = 0)


        for (int i = 0; i < inputGrid._cell.count; i++)

        convertedBrick = new Brick(_gridPosition, float _inputRotation)

        return convertedBrick;

    }
}
