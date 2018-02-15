using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickArrangment
{

    float gridDimX = 0.05625f;
    float gridDimY = 0.0725f;
    float gridDimZ = 0.05625f;

    BaseGrid workGrid;

   public List<Brick> bricks = new List<Brick>();

    public BrickArrangment(int gridX, int gridY, int gridZ)
    {
        workGrid = new BaseGrid(new Vector3Int(20, 20, 41));
    }

       public void CreateBricksInArrangment(TextAsset brickDataImport)
    {
        string importDataString = brickDataImport.ToString();

        BrickImportItem[] brickImportArray = JsonHelper.FromJson<BrickImportItem>(importDataString);

        for (int i = 0; i < brickImportArray.Length; i++)
        {
            bricks.Add(ConvertToBrick(brickImportArray[i]));
        }

    }

    Brick ConvertToBrick(BrickImportItem importedBrickItem)
    {
        Brick convertedBrick = null;

        for (int z = 0; z < workGrid._gridSize.z; z++)
        {
            for (int y = 0; y < workGrid._gridSize.y; y++)
            {
                for (int x = 0; x < workGrid._gridSize.x; x++)
                {
                    if (workGrid._cells[x, y, z].position == new Vector3Int(importedBrickItem.brickPosX, importedBrickItem.brickPosZ, importedBrickItem.brickPosY)) // swap y and z for rhino to unity!
                    {
                        convertedBrick = new Brick(workGrid._cells[x, y, z], importedBrickItem.rotation);
                    }
                }
            }
        }
        convertedBrick.childCells = workGrid.GetChildren(convertedBrick);

        return convertedBrick;
    }
}
