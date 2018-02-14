using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BrickArrangment
{

    float gridDimX = 0.05625f;
    float gridDimY = 0.0725f;
    float gridDimZ = 0.05625f;



    public BrickArrangment(int gridX, int gridY, int gridZ)
    {
        BaseGrid workGrid = new BaseGrid(new Vector3Int(20, 20, 41));
    }

    public BrickImportItem ImportBrickData1(TextAsset brickDataImport)
    {
        string jsonTextToConvert = brickDataImport.ToString();

        BrickImportItem importedBrick = JsonUtility.FromJson<BrickImportItem>(jsonTextToConvert);
        



       return importedBrick;

    }


    public BrickImportItem [] ImportBrickData(TextAsset brickDataImport)
    {
        string importDataString = brickDataImport.ToString();

        BrickImportItem [] brickImportArray = JsonHelper.FromJson<BrickImportItem>(importDataString);

           return brickImportArray;

    }
}
