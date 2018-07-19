using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject seedMarker;

    public GameObject cellMarkerContainer;
    public GameObject cellMarker;

    public GameObject lineRendererObject;
    public GameObject lineRendererContainer;

    public TextAsset brickDataImport;

    public Material gridLineMaterial;

    Material robotMaterialMain;
    Material robotMaterialHighlight;

    List<Robot> allRobots = new List<Robot>();
    List<GameObject> allRobotMeshes = new List<GameObject>();
    List<GameObject> allBrickMeshes = new List<GameObject>();
    List<GameObject> allCellMarkers = new List<GameObject>();
    List<GameObject> allLegMarkers = new List<GameObject>();
    List<GameObject> allLineRendererObjects = new List<GameObject>();
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    BrickStructure brickStructure;

    Vector3Int gridSize = new Vector3Int(50, 20, 50);
    Vector3Int seedPosition = new Vector3Int(5, 0, 5);

    Vector3 brickDisplayOffset = new Vector3(0, -0.0625f, 0);
    int startingBricks = 3;

    void Start()
    {
        Time.timeScale = 2f;

        brickStructure = new BrickStructure(gridSize, seedPosition, brickDataImport);

        allRobots.Add(new Robot(brickStructure.bricksInTargetStructure[0], 4, 1, true));
        //allRobots.Add(new Robot(new Brick(brickStructure.grid.cellsArray[25,0,25],0,1), 4, 0, false));

        allRobots[0].brickTypeCurrentlyCarried = 0;

        for (int i = 0; i < allRobots.Count; i++)
        {
            allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));
            allRobotMeshes[i].transform.SetParent(robotMeshContainer.transform);
            allLegMarkers.Add(Instantiate(legMarker, legMarker.transform));
            allLegMarkers[i].transform.SetParent(legMarkerContainer.transform);
        }

        for (int i = 0; i < brickStructure.bricksInTargetStructure.Count; i++)
        {
            if (i < startingBricks)
            {
                if (brickStructure.bricksInTargetStructure[i].brickType == 1)
                {
                    allBrickMeshes.Add(Instantiate(fullBrickMesh, brickStructure.bricksInTargetStructure[i].originCell.actualPosition + brickDisplayOffset, brickStructure.bricksInTargetStructure[i].rotation));
                }

                else if (brickStructure.bricksInTargetStructure[i].brickType == 2)
                {
                    allBrickMeshes.Add(Instantiate(halfBrickMesh, brickStructure.bricksInTargetStructure[i].originCell.actualPosition + brickDisplayOffset, brickStructure.bricksInTargetStructure[i].rotation));
                }
                brickStructure.bricksInPlace.Add(brickStructure.bricksInTargetStructure[i]);
            }

            else
            {
                if (brickStructure.bricksInTargetStructure[i].brickType == 1)
                {
                    allBrickMeshes.Add(Instantiate(fullBrickMesh, brickStructure.seedCell.actualPosition + brickDisplayOffset, Quaternion.identity));
                }

                else if (brickStructure.bricksInTargetStructure[i].brickType == 2)
                {
                    allBrickMeshes.Add(Instantiate(fullBrickMesh, brickStructure.seedCell.actualPosition + brickDisplayOffset, Quaternion.identity));
                }
            }

            allBrickMeshes[i].transform.SetParent(brickContainer.transform);
        }

        seedMarker.transform.position = brickStructure.seedCell.actualPosition;

        UpdateAvailableCells();
        CreateGridLines();

       // print(brickStructure.FindDropOffCell(brickStructure.bricksInTargetStructure[startingBricks + 1], brickStructure.availableCells).actualPosition);
    }

    void CreateGridLines()
    {
        for (int x = 0; x < brickStructure.grid.gridSize.x; x++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, brickStructure.grid.cellsArray[x, 0, 0].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, brickStructure.grid.cellsArray[x, 0, brickStructure.grid.gridSize.z - 1].actualPosition + brickDisplayOffset);

        }

        for (int z = 0; z < brickStructure.grid.gridSize.z; z++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, brickStructure.grid.cellsArray[0, 0, z].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, brickStructure.grid.cellsArray[brickStructure.grid.gridSize.x - 1, 0, z].actualPosition + brickDisplayOffset);
        }
    }

    void UpdateAvailableCells()
    {
        brickStructure.UpdateAvailableCells();

        foreach (GameObject cellMarker in allCellMarkers)
        {
            Destroy(cellMarker);
        }

        allCellMarkers.Clear();

        foreach (Cell cell in brickStructure.availableCells)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown("h"))
        //{
        //    allRobots[0].TakeStep(0, 8, 0, 4, -90);
        //}

        //if (Input.GetKeyDown("j"))
        //{
        //    allRobots[0].TakeStep(0, 8, 0, 4, 90);
        //}
        //if (Input.GetKeyDown("k"))
        //{
        //    allRobots[0].TakeStep(0, 8, 0, 4, 180);
        //}
        //if (Input.GetKeyDown("n"))
        //{
        //    allRobots[0].TakeStep(0, 8, 1, 4, -90);
        //}

        //if (Input.GetKeyDown("m"))
        //{
        //    allRobots[0].TakeStep(0, 8, 1, 4, 90);
        //}
        //if (Input.GetKeyDown(","))
        //{
        //    allRobots[0].TakeStep(0, 8, 1, 4, 180);
        //}

        if (Input.GetKeyDown("r"))
        {
            allRobots[0].HandleBrick(0, 4, -2, 0, 90, brickStructure.bricksInTargetStructure[5], true);
        }

        if (Input.GetKeyDown("t"))
        {
            allRobots[0].HandleBrick(0, 4, 1, 0, 90, brickStructure.bricksInTargetStructure[5], true);
        }

        if (Input.GetKeyDown("y"))
        {
            allRobots[0].HandleBrick(0, 4, 0, 0, 0, brickStructure.bricksInTargetStructure[5], true);
        }

        if (Input.GetKeyDown("u"))
        {
            allRobots[0].HandleBrick(-1, 4, 0, 0, 0, brickStructure.bricksInTargetStructure[5], false);
        }
        if (Input.GetKeyDown("i"))
        {
            allRobots[0].HandleBrick(0, 4, 0, 0, 0, brickStructure.bricksInTargetStructure[5], false);
        }
        if (Input.GetKeyDown("o"))
        {
            allRobots[0].HandleBrick(-1, 4, 0, 1, 0, brickStructure.bricksInTargetStructure[5], false);
        }

        if (Input.GetKeyDown("p"))
        {
            allRobots[0].HandleBrick(0, 4, 0, 1, 0, brickStructure.bricksInTargetStructure[5], false);
        }

        if (Input.GetKeyDown("j"))
        {
            allRobots[0].HandleBrick(-1, 4, -2, 0, 0, brickStructure.bricksInTargetStructure[5], false);
        }
        if (Input.GetKeyDown("k"))
        {
            allRobots[0].HandleBrick(0, 4, 2, 0, 0, brickStructure.bricksInTargetStructure[5], false);
        }
        if (Input.GetKeyDown("l"))
        {
            allRobots[0].HandleBrick(-1, 4, -2, 0, 90, brickStructure.bricksInTargetStructure[5], false);
        }

        if (Input.GetKeyDown(";"))
        {
            allRobots[0].HandleBrick(0, 4, 2, 0, 90, brickStructure.bricksInTargetStructure[5], false);
        }

        //if (!allRobots[1].stepInProgress)
        //{
        //      allRobots[1].TakeStep("Step along 4 lead B");
        //}

        //if (!allRobots[0].stepInProgress)
        //{
        //    allRobots[0].TakeStep(0, 8, 1, 4, 90); 
        //}

        DisplayAllMeshes();

        // tracker.transform.position = allRobotMeshes[0].gameObject.transform.GetChild(0).gameObject.transform.position;
    }


    void DisplayAllMeshes()
    {
        for (int i = 0; i < allRobots.Count; i++)
        {
            allRobots[i].UpdateRobot();
            DisplayRobot(allRobots[i], allRobotMeshes[i], allLegMarkers[i]);
        }
    }


    void DisplayRobot(Robot robotToDisplay, GameObject meshToDisplay, GameObject _legMarkerMesh)
    {
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

        //brick being carried
        for (int i = 0; i < brickStructure.bricksInTargetStructure.Count; i++)
        {
            if (brickStructure.bricksInTargetStructure[i] == robotToDisplay.brickBeingCarried && robotToDisplay.brickIsAttached)
            {
                allBrickMeshes[i].transform.position = legCFoot.transform.position + new Vector3(0, -robotToDisplay.gridDimY, 0);
                allBrickMeshes[i].transform.rotation = legCFoot.transform.rotation;
            }
        }

    }
}
