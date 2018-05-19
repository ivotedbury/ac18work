using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

    // Overall Grid
    Grid grid;
    int gridSizeX = 25;
    int gridSizeY = 25;
    int gridSizeZ = 25;
    float gridDimension = 0.05625f;

    List<Cell> path = new List<Cell>();
    List<GameObject> brickMeshes = new List<GameObject>();

    List<Cell> targetStructure = new List<Cell>();

    public GameObject brickMesh;
    Cell seed;
    Cell currentTargetCell;

    public Slider buildProgress;


    void Start()
    {
       

        grid = new Grid(new Vector3Int(gridSizeX, gridSizeY, gridSizeZ));
        SetSeed(new Vector3Int(3, 0, 3));

        CreateTargetTower(10, 10, 10, new Vector3Int(5, 0, 5)); // dimensions x,y,z, origin
        targetStructure = ReorderTargetStructure(targetStructure, seed);
        MakeStructureSolid(targetStructure);

        // UI Setup
        buildProgress.maxValue = targetStructure.Count - 1;
      buildProgress.wholeNumbers = true;
        buildProgress.onValueChanged.AddListener(delegate { BuildProgressValueChangeCheck(); });

        UpdateCellDisplay();
    }


    void Update()
    {

    }

    public void BuildProgressValueChangeCheck()
    {
        currentTargetCell = targetStructure[(int) buildProgress.value];
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

    void MakeStructureSolid(List<Cell> inputTargetStructure)
    {
        foreach(Cell cell in inputTargetStructure)
        {
            cell.isSolid = true;
        }
    }

    List<Cell> ReorderTargetStructure(List<Cell> inputTargetStructure, Cell inputSeed)
    {
        List<Cell> reorderedTargetStructure = new List<Cell>();

        float currentClosestDistance;
        float testDistance;
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
                        testDistance = Mathf.Abs(Vector3.Distance(inputTargetStructure[i].position, inputSeed.position));

                        if (testDistance < currentClosestDistance)
                        {
                            currentClosestDistance = testDistance;
                            bestCurrentCell = inputTargetStructure[i];
                            betterCellFound = true;
                        }
                    }
                }
                if (betterCellFound)
                {
                    reorderedTargetStructure.Add(bestCurrentCell);
                    inputTargetStructure.Remove(bestCurrentCell);
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
