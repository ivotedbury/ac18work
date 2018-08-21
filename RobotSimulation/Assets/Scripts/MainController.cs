using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

    public GameObject robotMeshes;
    public GameObject robotMeshContainer;
    public GameObject tracker;
    public GameObject legMarker;
    public GameObject legMarkerContainer;

    public GameObject fullBrickMesh;
    public GameObject halfBrickMesh;
    public GameObject brickContainer;

    //  public GameObject seedMarker;

    public GameObject cellMarkerContainer;
    public GameObject cellMarker;

    public GameObject lineRendererObject;
    public GameObject lineRendererContainer;

    public TextAsset brickImportData;

    public Material gridLineMaterial;

    Material robotMaterialMain;
    Material robotMaterialHighlight;

    List<GameObject> allRobotMeshes = new List<GameObject>();
    List<GameObject> allBrickMeshes = new List<GameObject>();
    List<GameObject> allCellMarkers = new List<GameObject>();
    List<GameObject> allLegMarkers = new List<GameObject>();
    List<GameObject> allLineRendererObjects = new List<GameObject>();
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    BuildManager buildManager;

    Vector3Int gridSize = new Vector3Int(100, 20, 100);
    Vector3Int seedPosition = new Vector3Int(20, 1, 20);

    Vector3 brickDisplayOffset = new Vector3(0, -0.0625f, 0);
    float gridXZDim = 0.05625f;
    float gridYDim = 0.0625f;

    float timeScaleFactor = 10f;
    float overallTime = 0;

    public Slider timeScaleSlider;
    public Text timeScaleFactorLabel;
    public Text totalBuildTimeLabel;
    public Text tripTimeLabel;
    public Button startSimulation;
    public Button calculateBuildTime;

    int startingBricks = 3; // 24
    int numberOfRobots = 1;

    bool runBuild = false;
    bool runMeshSimulation = false;
    bool buildInProgress = false;
    bool manualMode = false;
    bool buildComplete = false;

    BuildDataSet thisBuildDataSet;


    void Start()
    {
        SetupUI();

        buildManager = new BuildManager(gridSize, seedPosition, brickImportData, startingBricks, numberOfRobots);

        for (int i = 0; i < buildManager.allRobots.Count; i++)
        {
            allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));
            allRobotMeshes[i].transform.SetParent(robotMeshContainer.transform);
            allLegMarkers.Add(Instantiate(legMarker, legMarker.transform));
            allLegMarkers[i].transform.SetParent(legMarkerContainer.transform);
        }

        for (int i = 0; i < buildManager.brickStructure.bricksInTargetStructure.Count; i++)
        {
            if (i < buildManager.startingBricks)
            {
                if (buildManager.brickStructure.bricksInTargetStructure[i].brickType == 1)
                {
                    allBrickMeshes.Add(Instantiate(fullBrickMesh, buildManager.brickStructure.bricksInTargetStructure[i].originCell.actualPosition + brickDisplayOffset, buildManager.brickStructure.bricksInTargetStructure[i].rotation));

                }

                else if (buildManager.brickStructure.bricksInTargetStructure[i].brickType == 2)
                {
                    allBrickMeshes.Add(Instantiate(halfBrickMesh, buildManager.brickStructure.bricksInTargetStructure[i].originCell.actualPosition + brickDisplayOffset, buildManager.brickStructure.bricksInTargetStructure[i].rotation));
                }
            }

            else
            {
                if (buildManager.brickStructure.bricksInTargetStructure[i].brickType == 1)
                {
                    allBrickMeshes.Add(Instantiate(fullBrickMesh, buildManager.brickStructure.seedCell.actualPosition + brickDisplayOffset, Quaternion.Euler(0, 90, 0)));
                }

                else if (buildManager.brickStructure.bricksInTargetStructure[i].brickType == 2)
                {
                    allBrickMeshes.Add(Instantiate(halfBrickMesh, buildManager.brickStructure.seedCell.actualPosition + brickDisplayOffset, Quaternion.Euler(0, 90, 0)));
                }
            }

            //make aux bricks red
            if (buildManager.brickStructure.bricksInTargetStructure[i].auxBrick)
            {
                allBrickMeshes[i].GetComponent<Renderer>().material.color = Color.red;
            }

            allBrickMeshes[i].transform.SetParent(brickContainer.transform);
        }

        UpdateAvailableCells();
        CreateGridLines();


    }

    private void OnGUI()
    {
        string timescaleFactorForLabel;

        if (timeScaleFactor >= 10)
        {
            timescaleFactorForLabel = Mathf.Round(timeScaleFactor).ToString();
        }
        else if (timeScaleFactor >= 1 && timeScaleFactor < 10)
        {
            timescaleFactorForLabel = (Mathf.Round(timeScaleFactor * 10) / 10).ToString();
        }
        else
        {
            timescaleFactorForLabel = (Mathf.Round(timeScaleFactor * 100) / 100).ToString();
        }
        timeScaleFactorLabel.text = "Timescale Factor: " + timescaleFactorForLabel.ToString();


        int timeHours = (int)(overallTime) / 3600;
        int timeMinutes = ((int)(overallTime) - (timeHours * 3600)) / 60;
        int timeSeconds = (((int)overallTime - (timeHours * 3600)) - (timeMinutes * 60));

        totalBuildTimeLabel.text = "Total Build Time: " + timeHours.ToString() + ":" + timeMinutes.ToString() + ":" + timeSeconds.ToString();
    }

    void SetupUI()
    {
        Time.timeScale = timeScaleFactor;
        timeScaleSlider.value = timeScaleFactor;
        timeScaleSlider.onValueChanged.AddListener(delegate { TimeScaleSliderChange(); });

        Button startSimulationButton = startSimulation.GetComponent<Button>();
        Button calculateBuildTimeButton = calculateBuildTime.GetComponent<Button>();

        startSimulationButton.onClick.AddListener(StartBuildSimulation);
        calculateBuildTimeButton.onClick.AddListener(CalculateBuildTime);
    }

    void StartBuildSimulation()
    {
        runBuild = true;
        runMeshSimulation = true;

        buildManager.PlaceNextBrick(); ///////////////////////////////
        UpdateAvailableCells();
    }

    void CalculateBuildTime()
    {
        buildManager.allRobots[0].simulateMovements = false;
        runBuild = true;
        runMeshSimulation = true;
        thisBuildDataSet = new BuildDataSet(buildManager.brickStructure.bricksInTargetStructure);
        Debug.Log("totalBricks = " + thisBuildDataSet.totalBricks);

    }

    void TimeScaleSliderChange()
    {
        timeScaleFactor = timeScaleSlider.value;
        Time.timeScale = timeScaleFactor;
    }

    void Update()
    {

        if (runBuild)
        {
            buildManager.Update();

            if (!buildManager.readyForNextBrick)
            {
                UpdateAvailableCells();
            }

            if (buildManager.buildComplete)
            {
                PopulateDataSet(thisBuildDataSet);
                ExportBuildData(thisBuildDataSet);
                runBuild = false;
            }
        }

        if (manualMode)
        {
            DoManualCommands();
        }

        if (runMeshSimulation)
        {
            DisplayAllMeshes();
            if (!buildManager.buildComplete)
            {
                overallTime += Time.deltaTime;
            }
        }

    }

    void PopulateDataSet(BuildDataSet _data)
    {
        _data.pathCountOut = buildManager.pathCountOut;
        _data.pathCountBack = buildManager.pathCountBack;
        _data.climbingOut = buildManager.climbingOut;
        _data.climbingBack = buildManager.climbingBack;
        _data.climbingOutAverage = buildManager.climbingOutAverage;
        _data.climbingBackAverage = buildManager.climbingBackAverage;
        _data.distanceOut = buildManager.distanceOut;
        _data.distanceBack = buildManager.distanceBack;
        _data.distanceOutAverage = buildManager.distanceOutAverage;
        _data.distanceBackAverage = buildManager.distanceBackAverage;
        _data.robotActions = buildManager.allRobotActions;

        _data.moveTimeList = buildManager.allRobots[0].moveTimeList;

        _data.legARailJointPosition = buildManager.allRobots[0].legARailJointPosition;
        _data.legAVerticalJointPosition = buildManager.allRobots[0].legAVerticalJointPosition;
        _data.legARotationJointPosition = buildManager.allRobots[0].legARotationJointPosition;
        _data.legBRailJointPosition = buildManager.allRobots[0].legBRailJointPosition;
        _data.legBVerticalJointPosition = buildManager.allRobots[0].legBVerticalJointPosition;
        _data.legCRailJointPosition = buildManager.allRobots[0].legCRailJointPosition;
        _data.legCGripJointPosition = buildManager.allRobots[0].legCGripJointPosition;
        _data.legCRotationJointPosition = buildManager.allRobots[0].legCRotationJointPosition;

        _data.legARailJointDistToMove = buildManager.allRobots[0].legARailJointDistToMove;
        _data.legAVerticalJointDistToMove = buildManager.allRobots[0].legAVerticalJointDistToMove;
        _data.legARotationJointDistToMove = buildManager.allRobots[0].legARotationJointDistToMove;
        _data.legBRailJointDistToMove = buildManager.allRobots[0].legBRailJointDistToMove;
        _data.legBVerticalJointDistToMove = buildManager.allRobots[0].legBVerticalJointDistToMove;
        _data.legCRailJointDistToMove = buildManager.allRobots[0].legCRailJointDistToMove;
        _data.legCGripJointDistToMove = buildManager.allRobots[0].legCGripJointDistToMove;
        _data.legCRotationJointDistToMove = buildManager.allRobots[0].legCRotationJointDistToMove;

    }

    void ExportBuildData(BuildDataSet _data)
    {
        string buildDataExportPath = "Assets/ExportData/BuildDataSets/" + brickImportData.name.ToString() + "_buildDataSet.txt";

        DataSetExportItem[] _dataExport = new DataSetExportItem[1];
        _dataExport[0] = ConvertDataForExport(_data);

        string dataToExport = JsonUtility.ToJson(_data);
        // string dataToExport = JsonHelper.ToJson<DataSetExportItem>(_dataExport, true).ToString();
        Debug.Log(dataToExport);

        System.IO.File.WriteAllText(buildDataExportPath, dataToExport);
    }

    DataSetExportItem ConvertDataForExport(BuildDataSet _input)
    {
        DataSetExportItem _output = new DataSetExportItem();

        _output.totalBricks = _input.totalBricks;
        _output.auxBricks = _input.auxBricks;
        _output.targetBricks = _input.targetBricks;
        _output.targetBricksFull = _input.targetBricksFull;
        _output.targetBricksHalf = _input.targetBricksHalf;
        _output.auxBricksFull = _input.auxBricksFull;
        _output.auxBricksHalf = _input.auxBricksHalf;

        _output.robotActions = _input.robotActions;
        _output.moveTimeList = _input.moveTimeList;

        _output.legARailJointPosition = _input.legARailJointPosition;
        _output.legAVerticalJointPosition = _input.legAVerticalJointPosition;
        _output.legARotationJointPosition = _input.legARotationJointPosition;
        _output.legBRailJointPosition = _input.legBRailJointPosition;
        _output.legBVerticalJointPosition = _input.legBVerticalJointPosition;
        _output.legCRailJointPosition = _input.legCRailJointPosition;
        _output.legCGripJointPosition = _input.legCGripJointPosition;
        _output.legCRotationJointPosition = _input.legCRotationJointPosition;

        _output.legARailJointDistToMove = _input.legARailJointDistToMove;
        _output.legAVerticalJointDistToMove = _input.legAVerticalJointDistToMove;
        _output.legARotationJointDistToMove = _input.legARotationJointDistToMove;
        _output.legBRailJointDistToMove = _input.legBRailJointDistToMove;
        _output.legBVerticalJointDistToMove = _input.legBVerticalJointDistToMove;
        _output.legCRailJointDistToMove = _input.legCRailJointDistToMove;
        _output.legCGripJointDistToMove = _input.legCGripJointDistToMove;
        _output.legCRotationJointDistToMove = _input.legCRotationJointDistToMove;

        return _output;
    }

    void CreateGridLines()
    {
        for (int x = 0; x < buildManager.brickStructure.grid.gridSize.x; x++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.grid.cellsArray[x, 1, 0].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.grid.cellsArray[x, 1, buildManager.brickStructure.grid.gridSize.z - 1].actualPosition + brickDisplayOffset);

        }

        for (int z = 0; z < buildManager.brickStructure.grid.gridSize.z; z++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.grid.cellsArray[0, 1, z].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.grid.cellsArray[buildManager.brickStructure.grid.gridSize.x - 1, 1, z].actualPosition + brickDisplayOffset);
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
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, gridXZDim) + brickDisplayOffset + actualLineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, -gridXZDim) + brickDisplayOffset + actualLineOffset);
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
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(-2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
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
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, 0, z * gridXZDim) + brickDisplayOffset + lineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, gridYDim, z * gridXZDim) + brickDisplayOffset);
            }
        }
    }

    void UpdateAvailableCells()
    {
        foreach (GameObject cellMarker in allCellMarkers)
        {
            Destroy(cellMarker);
        }

        allCellMarkers.Clear();

        foreach (Cell cell in buildManager.brickStructure.availableCells)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            if (cell.currentStatus == 2)
            {
                allCellMarkers[allCellMarkers.Count - 1].GetComponent<Renderer>().material.color = Color.cyan;
            }
            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }
    }

    void DisplayAllMeshes()
    {
        for (int i = 0; i < buildManager.allRobots.Count; i++)
        {
            buildManager.allRobots[i].UpdateRobot();
            DisplayRobot(buildManager.allRobots[i], allRobotMeshes[i], allLegMarkers[i]);
        }
    }

    void DoManualCommands()
    {
        if (Input.GetKeyDown("r"))
        {
            buildManager.allRobots[0].HandleBrick(0, 4, -2, 0, 90, buildManager.brickStructure.bricksInTargetStructure[5], true, false);
        }

        if (Input.GetKeyDown("t"))
        {
            buildManager.allRobots[0].HandleBrick(0, 4, 1, 0, 90, buildManager.brickStructure.bricksInTargetStructure[5], true, false);
        }

        if (Input.GetKeyDown("y"))
        {
            buildManager.allRobots[0].HandleBrick(0, 4, 0, 0, 0, buildManager.brickStructure.bricksInTargetStructure[5], true, false);
        }

        if (Input.GetKeyDown("u"))
        {
            buildManager.allRobots[0].HandleBrick(-1, 4, 0, 0, 0, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }
        if (Input.GetKeyDown("i"))
        {
            buildManager.allRobots[0].HandleBrick(0, 4, 0, 0, 0, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }
        if (Input.GetKeyDown("o"))
        {
            buildManager.allRobots[0].HandleBrick(-1, 4, 0, 1, 0, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }

        if (Input.GetKeyDown("p"))
        {
            buildManager.allRobots[0].HandleBrick(0, 4, 0, 1, 0, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }

        if (Input.GetKeyDown("j"))
        {
            buildManager.allRobots[0].HandleBrick(-1, 4, -2, 0, 0, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }
        if (Input.GetKeyDown("k"))
        {
            buildManager.allRobots[0].HandleBrick(0, 4, 2, 0, 0, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }
        if (Input.GetKeyDown("l"))
        {
            buildManager.allRobots[0].HandleBrick(-1, 4, -2, 0, 90, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }

        if (Input.GetKeyDown(";"))
        {
            buildManager.allRobots[0].HandleBrick(0, 4, 2, 0, 90, buildManager.brickStructure.bricksInTargetStructure[5], false, false);
        }
    }

    void DisplayRobot(Robot robotToDisplay, GameObject meshToDisplay, GameObject _legMarkerMesh)
    {
        // for all the robots
        GameObject legAFoot = meshToDisplay.gameObject.transform.GetChild(0).gameObject;
        legAFoot.transform.position = robotToDisplay.legAFootPos;
        legAFoot.transform.rotation = robotToDisplay.legAFootRot;

        GameObject legA = meshToDisplay.gameObject.transform.GetChild(1).gameObject;
        legA.transform.position = robotToDisplay.legAPos;
        legA.transform.rotation = robotToDisplay.legARot;

        GameObject legBFoot = meshToDisplay.gameObject.transform.GetChild(2).gameObject;
        legBFoot.transform.position = robotToDisplay.legBFootPos;
        legBFoot.transform.rotation = robotToDisplay.legBFootRot;

        GameObject legB = meshToDisplay.gameObject.transform.GetChild(3).gameObject;
        legB.transform.position = robotToDisplay.legBPos;
        legB.transform.rotation = robotToDisplay.legBRot;

        GameObject mainBeam = meshToDisplay.gameObject.transform.GetChild(4).gameObject;
        mainBeam.transform.position = robotToDisplay.mainBeamPos;
        mainBeam.transform.rotation = robotToDisplay.mainBeamRot;

        GameObject legC = meshToDisplay.gameObject.transform.GetChild(5).gameObject;
        legC.transform.position = robotToDisplay.legCPos;
        legC.transform.rotation = robotToDisplay.legCRot;

        GameObject legCFoot = meshToDisplay.gameObject.transform.GetChild(6).gameObject;
        legCFoot.transform.position = robotToDisplay.legCFootPos;
        legCFoot.transform.rotation = robotToDisplay.legCFootRot;

        GameObject verticalToHorizontalA = meshToDisplay.gameObject.transform.GetChild(7).gameObject;
        verticalToHorizontalA.transform.position = robotToDisplay.verticalToHorizontalAPos;
        verticalToHorizontalA.transform.rotation = robotToDisplay.verticalToHorizontalARot;

        GameObject verticalToHorizontalB = meshToDisplay.gameObject.transform.GetChild(8).gameObject;
        verticalToHorizontalB.transform.position = robotToDisplay.verticalToHorizontalBPos;
        verticalToHorizontalB.transform.rotation = robotToDisplay.verticalToHorizontalBRot;

        GameObject gripper = meshToDisplay.gameObject.transform.GetChild(9).gameObject;
        GameObject grip1 = gripper.gameObject.transform.GetChild(0).gameObject;
        GameObject grip2 = gripper.gameObject.transform.GetChild(1).gameObject;

        grip1.transform.position = robotToDisplay.grip1Pos;
        grip1.transform.rotation = robotToDisplay.grip1Rot;

        grip2.transform.position = robotToDisplay.grip2Pos;
        grip2.transform.rotation = robotToDisplay.grip2Rot;

        _legMarkerMesh.transform.localScale = new Vector3(0.085f, 0.045f, 0.02f);
        _legMarkerMesh.transform.position = mainBeam.transform.position + mainBeam.transform.rotation * new Vector3(0, 0, -0.5f);
        _legMarkerMesh.transform.rotation = mainBeam.transform.rotation;

        // for brick being carried
        for (int i = 0; i < buildManager.brickStructure.bricksInTargetStructure.Count; i++)
        {
            if (buildManager.brickStructure.bricksInTargetStructure[i] == robotToDisplay.brickBeingCarried && robotToDisplay.brickIsAttached)
            {
                allBrickMeshes[i].transform.position = legCFoot.transform.position + new Vector3(0, -robotToDisplay.gridDimY, 0);
                allBrickMeshes[i].transform.rotation = legCFoot.transform.rotation;
            }
        }

    }
}
