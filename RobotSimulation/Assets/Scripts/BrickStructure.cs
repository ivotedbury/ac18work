using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickStructure
{

    

    public List<Brick> bricksInStructure = new List<Brick>();

    public Grid grid;
    public Cell seedCell;

    private int importOffset = 5;

    public BrickStructure(Vector3Int _gridSize, Vector3Int _seedCell, TextAsset _brickDataImport)
    {
        grid = new Grid(_gridSize);
        CreateBricksInArrangment(_brickDataImport);
        CreateSeed(_seedCell);
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

    void CreateSeed(Vector3Int _seed)
    {
        seedCell = grid.cellsArray[_seed.x, _seed.y, _seed.z];
    }

    Brick ConvertToBrick(BrickImportItem importedBrickItem)
    {
        Brick convertedBrick = null;

        convertedBrick = new Brick(grid.cellsArray[importedBrickItem.brickPosX + importOffset, importedBrickItem.brickPosZ, importedBrickItem.brickPosY + importOffset], importedBrickItem.rotation, importedBrickItem.brickType);

        convertedBrick.childCells = grid.GetChildren(convertedBrick); 

        return convertedBrick;
    }
    
}
