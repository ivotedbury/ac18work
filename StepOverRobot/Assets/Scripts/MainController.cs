using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

    public GameObject robotMeshes;
    public GameObject robotMeshContainer;

    public GameObject lineRendererObject;
    public GameObject lineRendererContainer;

    public Material gridLineMaterial;

    List<GameObject> allRobotMeshes = new List<GameObject>();
    List<GameObject> allLineRendererObjects = new List<GameObject>();
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    List<Robot> allRobots = new List<Robot>();

    float gridXZDim = 0.05625f;
    float gridYDim = 0.0625f;

    int testStepSize = 4;

    Vector3Int gridSize = new Vector3Int(40, 40, 40);

    int numberOfRobots = 1;

    void Start()
    {
        Time.timeScale = 3f;

        allRobots.Add(new Robot(new Vector3Int(0, 0, 4), 0, 4));
        allRobots.Add(new Robot(new Vector3Int(12, 0, 16), 0, 4));
        allRobots.Add(new Robot(new Vector3Int(24, 0, 28), 0, 4));

        allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));
        allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));
        allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));

        allRobotMeshes[0].transform.SetParent(robotMeshContainer.transform);
        allRobotMeshes[1].transform.SetParent(robotMeshContainer.transform);
        allRobotMeshes[2].transform.SetParent(robotMeshContainer.transform);
        CreateGridLines();

    }

    void Update()
    {
        DisplayAllMeshes();
        DoManualCommands();

        for (int i = 0; i < allRobots.Count; i++)
        {
            if (!allRobots[i].moveInProgress)
            {
                allRobots[i].TakeStep(testStepSize, 0, 90);
            }
        }
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
            allRobots[0].TakeStep(4, 0, 90);
            Debug.Log("T");
        }
        if (Input.GetKeyDown("e"))
        {
            testStepSize++;
        }
        if (Input.GetKeyDown("w"))
        {
            testStepSize--;
        }
    }

    void DisplayRobot(Robot robotToDisplay, GameObject meshToDisplay)
    {
        // for all the robots
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

        //       // for brick being carried
        //for (int i = 0; i < buildManager.brickStructure.bricksInTargetStructure.Count; i++)
        //{
        //    if (buildManager.brickStructure.bricksInTargetStructure[i] == robotToDisplay.brickBeingCarried && robotToDisplay.brickIsAttached)
        //    {
        //        allBrickMeshes[i].transform.position = legCFoot.transform.position + new Vector3(0, -robotToDisplay.gridDimY, 0);
        //        allBrickMeshes[i].transform.rotation = legCFoot.transform.rotation;
        //    }
        //}
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

        // pickup zone 

        //float lineWidth = 0.005f;
        //Vector3 lineOffset = new Vector3(0, lineWidth, 0);


        //for (int y = 0; y <= 1; y++)
        //{
        //    Vector3 actualLineOffset = lineOffset;

        //    if (y == 1)
        //    {
        //        actualLineOffset = new Vector3(0, 0, 0);
        //    }

        //    for (int x = -1; x <= 1; x++)
        //    {
        //        if (x == 0)
        //        {
        //            continue;
        //        }

        //        allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
        //        allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
        //        allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
        //        allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
        //        allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
        //        allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
        //        allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, gridXZDim) + brickDisplayOffset + actualLineOffset);
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, -gridXZDim) + brickDisplayOffset + actualLineOffset);
        //    }

        //    for (int z = -1; z <= 1; z++)
        //    {
        //        if (z == 0)
        //        {
        //            continue;
        //        }

        //        allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
        //        allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
        //        allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
        //        allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
        //        allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
        //        allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
        //        allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(-2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
        //    }
        //}

        //for (int x = -1; x <= 1; x++)
        //{
        //    if (x == 0)
        //    {
        //        continue;
        //    }

        //    for (int z = -1; z <= 1; z++)
        //    {
        //        if (z == 0)
        //        {
        //            continue;
        //        }

        //        allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
        //        allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
        //        allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
        //        allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
        //        allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
        //        allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
        //        allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, 0, z * gridXZDim) + brickDisplayOffset + lineOffset);
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, gridYDim, z * gridXZDim) + brickDisplayOffset);
        //    }
        //}
    }
}
