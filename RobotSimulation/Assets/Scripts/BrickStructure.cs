using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickStructure
{

    

    public List<Brick> bricksInStructure = new List<Brick>();

    public Grid grid;

    public BrickStructure(Vector3Int _gridSize, TextAsset _brickDataImport)
    {
        grid = new Grid(_gridSize);
        CreateBricksInArrangment(_brickDataImport);
    }

    void CreateBricksInArrangment(TextAsset _brickDataImport)
    {
        string importDataString = _brickDataImport.ToString();

        BrickImportItem[] brickImportArray = JsonHelper.FromJson<BrickImportItem>(importDataString);

        for (int i = 0; i < brickImportArray.Length; i++)
        {
            bricksInStructure.Add(ConvertToBrick(brickImportArray[i]));
        }
        
    }

    Brick ConvertToBrick(BrickImportItem importedBrickItem)
    {
        Brick convertedBrick = null;

        convertedBrick = new Brick(grid.cellsArray[importedBrickItem.brickPosX, importedBrickItem.brickPosZ, importedBrickItem.brickPosY], importedBrickItem.rotation, importedBrickItem.brickType);

        convertedBrick.childCells = grid.GetChildren(convertedBrick); 

        return convertedBrick;
    }
    
}
