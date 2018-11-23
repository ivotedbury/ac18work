using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public GameObject cameraTracker;
    public GameObject robotMeshes;
    public GameObject robotMeshContainer;

    public GameObject fullBrickMesh;
    public GameObject halfBrickMesh;

    public GameObject lineRendererObject;
    public GameObject lineRendererContainer;

    public Material gridLineMaterial;

    List<GameObject> allRobotMeshes = new List<GameObject>();
    List<GameObject> allLineRendererObjects = new List<GameObject>();
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    List<Robot> allRobots = new List<Robot>();

    Grid grid;
    Brick fullBrick;
    Brick halfBrick;

    // grid dimensions
    float gridXZDim = 0.05625f;
    float gridYDim = 0.0725f;

    // step variables for testing - if stepping continuously, these will be the inputs
    int testStepSize = 4;
    int testTurnAngle = 0;
    int testStepHeight = 0;

    Vector3Int gridSize = new Vector3Int(40, 40, 40);

    int numberOfRobots = 1;

    void Start()
    {
        Time.timeScale = 0.25f;

        grid = new Grid(gridSize);

        //fullbrick for testing
        fullBrick = new Brick(grid, grid.cellsArray[3, 0, 3], 0, 1, false);
        fullBrickMesh.transform.position = fullBrick.currentPosition;
        fullBrickMesh.transform.rotation = fullBrick.currentRotation;

        //halfbrick for testing
        halfBrick = new Brick(grid, grid.cellsArray[6, 0, 3], 0, 2, false);
        halfBrickMesh.transform.position = halfBrick.currentPosition;
        halfBrickMesh.transform.rotation = halfBrick.currentRotation;

        allRobots.Add(new Robot(new Vector3Int(0, 0, 4), 0, 4, 0));
        // allRobots[0].brickCurrentlyBeingCarried = fullBrick;

        allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));
        allRobotMeshes[0].transform.SetParent(robotMeshContainer.transform);

        CreateGridLines();
    }

    void Update()
    {
        DisplayAllMeshes();
        DoManualCommands();

        // looping function to make the robot walk continuously

        //for (int i = 0; i < allRobots.Count; i++)
        //{
        //    if (!allRobots[i].moveInProgress)
        //    {
        //        allRobots[i].TakeStep(testStepSize, testStepHeight, testTurnAngle);
        //    }
        //}

        cameraTracker.transform.position = allRobots[0].averageRobotPos; // move the camera target position to the average position of the robot

        UpdateBricks();
    }

    void UpdateBricks()
    {
        fullBrickMesh.transform.position = fullBrick.currentPosition;
        fullBrickMesh.transform.rotation = fullBrick.currentRotation;

        halfBrickMesh.transform.position = halfBrick.currentPosition;
        halfBrickMesh.transform.rotation = halfBrick.currentRotation;
    }

    void DisplayAllMeshes()
    {
        for (int i = 0; i < allRobots.Count; i++)
        {
            allRobots[i].UpdateRobot();
            DisplayRobot(allRobots[i], allRobotMeshes[i]);
        }
    }

    void DoManualCommands()
    {
        if (Input.GetKeyDown("t"))
        {
            allRobots[0].TakeStep(4, 0, 0);
            Debug.Log("T");
        }

        if (Input.GetKeyDown("y"))
        {
            allRobots[0].TakeStep(4, +1, 0);
            Debug.Log("Y");
        }

        if (Input.GetKeyDown("r"))
        {
            allRobots[0].TakeStep(4, -1, 0);
            Debug.Log("R");
        }

        if (Input.GetKeyDown("n"))
        {
            allRobots[0].PlaceBrick(0, 3, 0, 1, 0, fullBrick);
            Debug.Log("Place");
        }

        if (Input.GetKeyDown("m"))
        {
            allRobots[0].PlaceBrick(-1, 3, -2, 1, 0, fullBrick);
            Debug.Log("Place");
        }

        if (Input.GetKeyDown(","))
        {
            allRobots[0].PickupBrick(0, 3, 2, 1, 0, fullBrick);
            Debug.Log("Pickup");
        }

        if (Input.GetKeyDown("."))
        {
            allRobots[0].PickupBrick(0, 3, 0, 1, 0, fullBrick);
            Debug.Log("Pickup");
        }


        if (Input.GetKeyDown("e"))
        {
            testStepSize++;
        }
        if (Input.GetKeyDown("w"))
        {
            testStepSize--;
        }

        if (Input.GetKeyDown("a"))
        {
            testStepHeight = -1;
        }
        if (Input.GetKeyDown("s"))
        {
            testStepHeight = 0;
        }
        if (Input.GetKeyDown("d"))
        {
            testStepHeight = 1;
        }

        if (Input.GetKeyDown("i"))
        {
            testTurnAngle = 0;
        }
        if (Input.GetKeyDown("j"))
        {
            testTurnAngle = -90;
        }
        if (Input.GetKeyDown("l"))
        {
            testTurnAngle = 90;
        }
        if (Input.GetKeyDown("k"))
        {
            testTurnAngle = 180;
        }
    }

    void DisplayRobot(Robot robotToDisplay, GameObject meshToDisplay)
    {
        // robot meshes are child gameobjects of the main robot gameobject. Each one is updated according to the position and rotation of each transform in the robot class.

        GameObject legAFoot = meshToDisplay.gameObject.transform.GetChild(0).gameObject;
        legAFoot.transform.position = robotToDisplay.legAFootPos;
        legAFoot.transform.rotation = robotToDisplay.legAFootRot;

        GameObject legA = meshToDisplay.gameObject.transform.GetChild(1).gameObject;
        legA.transform.position = robotToDisplay.legAPos;
        legA.transform.rotation = robotToDisplay.legARot;

        GameObject legAHip = meshToDisplay.gameObject.transform.GetChild(2).gameObject;
        legAHip.transform.position = robotToDisplay.legAHipPos;
        legAHip.transform.rotation = robotToDisplay.legAHipRot;

        GameObject legBFoot = meshToDisplay.gameObject.transform.GetChild(3).gameObject;
        legBFoot.transform.position = robotToDisplay.legBFootPos;
        legBFoot.transform.rotation = robotToDisplay.legBFootRot;

        GameObject legB = meshToDisplay.gameObject.transform.GetChild(4).gameObject;
        legB.transform.position = robotToDisplay.legBPos;
        legB.transform.rotation = robotToDisplay.legBRot;

        GameObject legBHip = meshToDisplay.gameObject.transform.GetChild(5).gameObject;
        legBHip.transform.position = robotToDisplay.legBHipPos;
        legBHip.transform.rotation = robotToDisplay.legBHipRot;

        GameObject mainBeam = meshToDisplay.gameObject.transform.GetChild(6).gameObject;
        mainBeam.transform.position = robotToDisplay.mainBeamPos;
        mainBeam.transform.rotation = robotToDisplay.mainBeamRot;

        GameObject legC = meshToDisplay.gameObject.transform.GetChild(7).gameObject;
        legC.transform.position = robotToDisplay.legCShoulderPos;
        legC.transform.rotation = robotToDisplay.legCShoulderRot;

        GameObject legCFoot = meshToDisplay.gameObject.transform.GetChild(8).gameObject;
        legCFoot.transform.position = robotToDisplay.legCFootPos;
        legCFoot.transform.rotation = robotToDisplay.legCFootRot;

        GameObject grip1 = meshToDisplay.gameObject.transform.GetChild(9).gameObject;
        grip1.transform.position = robotToDisplay.grip1Pos;
        grip1.transform.rotation = robotToDisplay.grip1Rot;


        GameObject grip2 = meshToDisplay.gameObject.transform.GetChild(10).gameObject;
        grip2.transform.position = robotToDisplay.grip2Pos;
        grip2.transform.rotation = robotToDisplay.grip2Rot;

        GameObject nozzle = meshToDisplay.gameObject.transform.GetChild(11).gameObject;
        nozzle.transform.position = robotToDisplay.nozzlePos;
        nozzle.transform.rotation = robotToDisplay.nozzleRot;

    }

    void CreateGridLines()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, new Vector3(x * gridXZDim, 0, 0));
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, new Vector3(x * gridXZDim, 0, (gridSize.z - 1) * gridXZDim));

        }

        for (int z = 0; z < gridSize.z; z++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, new Vector3(0, 0, z * gridXZDim));
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, new Vector3((gridSize.x - 1) * gridXZDim, 0, z * gridXZDim));
        }
    }
}
