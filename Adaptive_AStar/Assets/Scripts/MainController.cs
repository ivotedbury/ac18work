using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    // Overall Grid
    Grid grid;
    int gridSizeX = 100;
    int gridSizeY = 100;
    int gridSizeZ = 100;
    float gridDimension = 0.05625f;

    List<Cell> path = new List<Cell>();
    List<GameObject> brickMeshes = new List<GameObject>();

    List<Cell> targetStructure = new List<Cell>();

    public GameObject brickMesh;
    Cell seed;

    void Start()
    {
        grid = new Grid(new Vector3Int(gridSizeX, gridSizeY, gridSizeZ));
        SetSeed(new Vector3Int(5, 0, 5));

        CreateTower(20, 20, 20, new Vector3Int(50, 0, 50));

        UpdateCellDisplay();
    }


    void Update()
    {

    }

    void UpdateCellDisplay()
    {
        for (int z = 0; z < gridSizeZ; z++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    if (grid.cells[x, y, z].isSolid)
                    {
                        brickMeshes.Add(Instantiate(brickMesh, grid.cells[x, y, z].position, Quaternion.identity));
                    }

                    if (grid.cells[x, y, z].isSeed)
                    {
                        brickMeshes[brickMeshes.Count - 1].GetComponent<Renderer>().material.color = Color.green;
                    }
                }
            }
        }
    }

    void CreateTargetTower(int dimX, int dimY, int dimZ, Vector3Int targetOrigin)
    {
        for (int z = 0; z < dimZ; z++)
        {
            for (int y = 0; y < dimY; y++)
            {
                for (int x = 0; x < dimX; x++)
                {
                    targetStructure.Add(grid.cells[targetOrigin.x + x, targetOrigin.y + y, targetOrigin.z + z]);
                }
            }
        }
    }

    List<Cell> ReorderTargetStructure(List<Cell> inputTargetStructure, Cell inputSeed)
    {
        List<Cell> reorderedTargetStructure = new List<Cell>();

        int currentClosestDistance;
        int testDistance;
        int listLength = inputTargetStructure.Count;

        Cell bestCurrentCell = null;
        bool betterCellFound = false;

        for (int currentSearchLayer = 0; currentSearchLayer < gridSizeY; currentSearchLayer++)
        {
            for (int listCounter = 0; listCounter < listLength; listCounter++)
            {
                currentClosestDistance = 1000000;
                betterCellFound = false;

                for (int i = 0; i < inputTargetStructure.Count; i++)
                {
                    if (inputTargetStructure[i].position.y == currentSearchLayer)
                    {
                        testDistance = ///////////////////////////////////////////////
                    }
                }
            }
        }

        return reorderedTargetStructure;
    }

    void SetSeed(Vector3Int seedLocation)
    {
        seed = grid.cells[seedLocation.x, seedLocation.y, seedLocation.z];
        seed.isSolid = true;
        seed.isSeed = true;
    }
}
