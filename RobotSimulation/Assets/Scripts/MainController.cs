using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public GameObject robotMeshes;
    public GameObject robotMeshContainer;
    public GameObject tracker;

    public GameObject fullBrickMesh;
    public GameObject halfBrickMesh;
    public GameObject brickContainer;

    public GameObject seedMarker;
    public GameObject cellMarker;

    public TextAsset brickDataImport;

    Material robotMaterialMain;
    Material robotMaterialHighlight;

    List<Robot> allRobots = new List<Robot>();
    List<GameObject> allRobotMeshes = new List<GameObject>();
    List<GameObject> allBrickMeshes = new List<GameObject>();

    BrickStructure brickStructure;

    Vector3Int gridSize = new Vector3Int(100, 100, 100);
    Vector3Int seedPosition = new Vector3Int(5, 0, 5);


    void Start()
    {
        Time.timeScale = 4f;

        brickStructure = new BrickStructure(gridSize, seedPosition, brickDataImport);

        allRobots.Add(new Robot(new Vector3Int(50, 0, 50), 4, 1));
        allRobots.Add(new Robot(new Vector3Int(20, 0, 20), 4, 0));

        for (int i = 0; i < allRobots.Count; i++)
        {
            allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));
            allRobotMeshes[i].transform.SetParent(robotMeshContainer.transform);
        }

        for (int i = 0; i < brickStructure.bricksInStructure.Count; i++)
        {
            if (brickStructure.bricksInStructure[i].brickType == 0)
            {
                allBrickMeshes.Add(Instantiate(fullBrickMesh, brickStructure.bricksInStructure[i].originCell.actualPosition, brickStructure.bricksInStructure[i].rotation));
            }

            if (brickStructure.bricksInStructure[i].brickType == 1)
            {
                allBrickMeshes.Add(Instantiate(halfBrickMesh, brickStructure.bricksInStructure[i].originCell.actualPosition, brickStructure.bricksInStructure[i].rotation));
            }

            allBrickMeshes[i].transform.SetParent(brickContainer.transform);
        }

        seedMarker.transform.position = brickStructure.seedCell.actualPosition;

        for (int z = 0; z < brickStructure.grid.gridSize.z; z++)
        {
            for (int x = 0; x < brickStructure.grid.gridSize.x; x++)
            {
                Instantiate(cellMarker, brickStructure.grid.cellsArray[x, 0, z].actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            allRobots[0].TakeStep(1, 8, 0);
        }

        if (Input.GetKeyDown("j"))
        {
            allRobots[0].TakeStep(0, 7, 0);
        }
        if (Input.GetKeyDown("k"))
        {
            allRobots[0].TakeStep(0, 6, 0);
        }
        if (Input.GetKeyDown("l"))
        {
            allRobots[0].TakeStep(0, 5, 0);
        }

        if (Input.GetKeyDown("n"))
        {
            allRobots[0].TakeStep(0, 8, 1);
        }

        if (Input.GetKeyDown("m"))
        {
            allRobots[0].TakeStep(0, 7, 1);
        }
        if (Input.GetKeyDown(","))
        {
            allRobots[0].TakeStep(0, 6, 1);
        }
        if (Input.GetKeyDown("."))
        {
            allRobots[0].TakeStep(0, 5, 1);
        }
        if (!allRobots[1].stepInProgress)
        {
            //      allRobots[1].TakeStep("Step along 4 lead B");
        }

        if (!allRobots[0].stepInProgress)
        {
            //       allRobots[0].TakeStep("Step along 4 lead A");
        }

        DisplayAllMeshes();

        tracker.transform.position = allRobotMeshes[0].gameObject.transform.GetChild(0).gameObject.transform.position;
            }


    void DisplayAllMeshes()
    {
        for (int i = 0; i < allRobots.Count; i++)
        {
            allRobots[i].UpdateRobot();
            DisplayRobot(allRobots[i], allRobotMeshes[i]);
        }
    }

    void DisplayRobot(Robot robotToDisplay, GameObject meshToDisplay)
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
    }
}
