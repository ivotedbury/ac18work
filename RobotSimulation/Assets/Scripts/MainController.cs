using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public GameObject robotMeshes;
    public GameObject robotMeshContainer;
    public GameObject tracker;

    Material robotMaterialMain;
    Material robotMaterialHighlight;

    List<Robot> allRobots = new List<Robot>();
    List<GameObject> allRobotMeshes = new List<GameObject>();

    bool move = true;

    IEnumerator MakeMove;

    void Start()
    {
        allRobots.Add(new Robot(new Vector3Int(5, 0, 0), 4, 0));
        allRobots.Add(new Robot(new Vector3Int(20, 0, 0), 4, 1));

        for (int i = 0; i < allRobots.Count; i++)
        {
            allRobotMeshes.Add(Instantiate(robotMeshes, robotMeshes.transform));
            allRobotMeshes[i].transform.SetParent(robotMeshContainer.transform);
        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            allRobots[0].TakeStep("Step along 4 lead A");
        }

        print(allRobots[0].moveCounter);

        for (int i = 0; i < allRobots.Count; i++)
        {
            allRobots[i].UpdateRobot();
            DisplayRobot(allRobots[i], allRobotMeshes[i]);
        }

        if (!allRobots[1].stepInProgress)
        {
            allRobots[1].TakeStep("Step along 4 lead B");
        }

        if (!allRobots[0].stepInProgress)
        {
            allRobots[0].TakeStep("Step along 4 lead A");
        }

        tracker.transform.position = allRobotMeshes[0].gameObject.transform.GetChild(0).gameObject.transform.position;
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
