using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PreProcessing : MonoBehaviour
{
    public GameObject fullBrickMesh;
    public GameObject halfBrickMesh;
    public GameObject brickContainer;

    public GameObject cellMarkerContainer;
    public GameObject cellMarker;

    public GameObject lineRendererObject;
    public GameObject lineRendererContainer;

    public TextAsset brickImportData;
    //   public TextAsset cellImportData;

    public Material gridLineMaterial;

    Material robotMaterialMain;
    Material robotMaterialHighlight;

    List<GameObject> allBrickMeshes = new List<GameObject>();
    List<GameObject> allCellMarkers = new List<GameObject>();
    List<GameObject> allLineRendererObjects = new List<GameObject>();
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    BuildSequence buildSequence;

    Vector3Int gridSize = new Vector3Int(100, 20, 100);
    Vector3Int seedPosition = new Vector3Int(20, 1, 20);

    Vector3 brickDisplayOffset = new Vector3(0, -0.0625f, 0);
    float gridXZDim = 0.05625f;
    float gridYDim = 0.0625f;

    float timeScaleFactor = 10f;
    float overallTime = 0;

    public Button completePath;
    public Button generateBricks;
    public Button reorderExport;


    void Start()
    {
        buildSequence = new BuildSequence(gridSize, seedPosition, brickImportData);

        CreateBricks();
        CreateGridLines();
        UpdateCellDisplay();

        Button completePathButton = completePath.GetComponent<Button>();
        Button generateBricksButton = generateBricks.GetComponent<Button>();
        Button reorderExportButton = reorderExport.GetComponent<Button>();

        completePathButton.onClick.AddListener(CompletePath);
        generateBricksButton.onClick.AddListener(GenerateBricks);
        reorderExportButton.onClick.AddListener(ReorderExport);

    }

    void CompletePath()
    {
        buildSequence.GenerateCompletePaths();
        WritePathData();
        Debug.Log("data has been written");
        UpdateCellDisplay();
    }

    void GenerateBricks()
    {
        buildSequence.GenerateAdditionalBricks(brickImportData);

        foreach (GameObject brickMesh in allBrickMeshes)
        {
            Destroy(brickMesh);
        }
        allBrickMeshes.Clear();

        CreateBricks();
        UpdateCellDisplay();
    }

    void ReorderExport()
    {
        buildSequence.ReorderFinalBricks();

        string allBricksExportPath = "Assets/ExportData/" + brickImportData.name.ToString() + "_allBricksInBuild.txt";

        BrickImportItem[] bricksToExport = ConvertToBrickImportItem(buildSequence.finalStructureToBuild);

        string dataToExport = JsonHelper.ToJson<BrickImportItem>(bricksToExport, true).ToString();
        Debug.Log(dataToExport);

        System.IO.File.WriteAllText(allBricksExportPath, dataToExport);
    }

    void WritePathData()
    {
        string pathExportPath = "Assets/ExportData/" + brickImportData.name.ToString() + "_additionalPath.txt";

        CellImportItem[] cellPathToExport = ConvertToCellImportItem(buildSequence.desiredPath);
        Debug.Log(cellPathToExport.Length);
        string dataToExport = JsonHelper.ToJson<CellImportItem>(cellPathToExport, true).ToString();
        Debug.Log(dataToExport);

        System.IO.File.WriteAllText(pathExportPath, dataToExport);
    }

    private void OnGUI()
    {

    }

    void SetupUI()
    {

    }

    void CreateBricks()
    {
        for (int i = 0; i < buildSequence.completeStructure.Count; i++)
        {

            if (buildSequence.completeStructure[i].brickType == 1)
            {
                allBrickMeshes.Add(Instantiate(fullBrickMesh, buildSequence.completeStructure[i].originCell.actualPosition + brickDisplayOffset, buildSequence.completeStructure[i].rotation));
            }

            else if (buildSequence.completeStructure[i].brickType == 2)
            {
                allBrickMeshes.Add(Instantiate(halfBrickMesh, buildSequence.completeStructure[i].originCell.actualPosition + brickDisplayOffset, buildSequence.completeStructure[i].rotation));
            }

            //make aux bricks red
            if (buildSequence.completeStructure[i].auxBrick)
            {
                allBrickMeshes[i].GetComponent<Renderer>().material.color = Color.red;
            }

            allBrickMeshes[i].transform.SetParent(brickContainer.transform);
        }
    }

    BrickImportItem[] ConvertToBrickImportItem(List<Brick> _inputBrickList)
    {
        BrickImportItem[] outputBrickImportItemArray = new BrickImportItem[_inputBrickList.Count];

        for (int i = 0; i < _inputBrickList.Count; i++)
        {
            int rotation = 0;

            if (_inputBrickList[i].rotation == Quaternion.Euler(0, 90, 0) || _inputBrickList[i].rotation == Quaternion.Euler(0, 270, 0))
            {
                rotation = 90;
            }

            outputBrickImportItemArray[i] = new BrickImportItem();
            outputBrickImportItemArray[i].brickPosX = _inputBrickList[i].originCell.position.x;
            outputBrickImportItemArray[i].brickPosY = _inputBrickList[i].originCell.position.z;
            outputBrickImportItemArray[i].brickPosZ = _inputBrickList[i].originCell.position.y;
            outputBrickImportItemArray[i].brickType = _inputBrickList[i].brickType;
            outputBrickImportItemArray[i].auxBrick = _inputBrickList[i].auxBrick;
            outputBrickImportItemArray[i].rotation = rotation;

        }

        return outputBrickImportItemArray;
    }

    CellImportItem[] ConvertToCellImportItem(List<Cell> _inputCellList)
    {
        CellImportItem[] outputCellImportItemArray = new CellImportItem[_inputCellList.Count];

        for (int i = 0; i < _inputCellList.Count; i++)
        {
            outputCellImportItemArray[i] = new CellImportItem();
            outputCellImportItemArray[i].posX = _inputCellList[i].position.x;
            outputCellImportItemArray[i].posY = _inputCellList[i].position.y;
            outputCellImportItemArray[i].posZ = _inputCellList[i].position.z;
        }

        return outputCellImportItemArray;
    }

    void CreateGridLines()
    {
        for (int x = 0; x < buildSequence.grid.gridSize.x; x++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.grid.cellsArray[x, 1, 0].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.grid.cellsArray[x, 1, buildSequence.grid.gridSize.z - 1].actualPosition + brickDisplayOffset);

        }

        for (int z = 0; z < buildSequence.grid.gridSize.z; z++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.grid.cellsArray[0, 1, z].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.grid.cellsArray[buildSequence.grid.gridSize.x - 1, 1, z].actualPosition + brickDisplayOffset);
        }

        // pickup zone 

        float lineWidth = 0.005f;
        Vector3 lineOffset = new Vector3(0, lineWidth, 0);


        for (int y = 0; y <= 1; y++)
        {
            Vector3 actualLineOffset = lineOffset;

            if (y == 1)
            {
                actualLineOffset = new Vector3(0, 0, 0);
            }

            for (int x = -1; x <= 1; x++)
            {
                if (x == 0)
                {
                    continue;
                }

                allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
                allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
                allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
                allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
                allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
                allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
                allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, gridXZDim) + brickDisplayOffset + actualLineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, -gridXZDim) + brickDisplayOffset + actualLineOffset);
            }

            for (int z = -1; z <= 1; z++)
            {
                if (z == 0)
                {
                    continue;
                }

                allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
                allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
                allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
                allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
                allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
                allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
                allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.seedCell.actualPosition + new Vector3(2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.seedCell.actualPosition + new Vector3(-2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
            }
        }

        for (int x = -1; x <= 1; x++)
        {
            if (x == 0)
            {
                continue;
            }

            for (int z = -1; z <= 1; z++)
            {
                if (z == 0)
                {
                    continue;
                }

                allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
                allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
                allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
                allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
                allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
                allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
                allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, 0, z * gridXZDim) + brickDisplayOffset + lineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, gridYDim, z * gridXZDim) + brickDisplayOffset);
            }
        }
    }

    void UpdateCellDisplay()
    {
        foreach (GameObject cellMarker in allCellMarkers)
        {
            Destroy(cellMarker);
        }

        allCellMarkers.Clear();

        foreach (Cell cell in buildSequence.availableCells)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            allCellMarkers[allCellMarkers.Count - 1].GetComponent<Renderer>().material.color = Color.yellow;

            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }

        foreach (Cell cell in buildSequence.forbiddenCells)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            allCellMarkers[allCellMarkers.Count - 1].GetComponent<Renderer>().material.color = Color.magenta;
            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }

        foreach (Cell cell in buildSequence.desiredPath)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            allCellMarkers[allCellMarkers.Count - 1].GetComponent<Renderer>().material.color = Color.blue;
            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }
    }


    void Update()
    {

    }

}
