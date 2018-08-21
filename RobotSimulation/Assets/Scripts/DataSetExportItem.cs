using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class DataSetExportItem {
    public int totalBricks;
    public int auxBricks;
    public int targetBricks;
    public int targetBricksFull;
    public int targetBricksHalf;
    public int auxBricksFull;
    public int auxBricksHalf;

    public List <RobotAction> robotActions = new List<RobotAction>();
    public List<float> moveTimeList = new List<float>();

    public List<float> legARailJointPosition = new List<float>();
    public List<float> legAVerticalJointPosition = new List<float>();
    public List<float> legARotationJointPosition = new List<float>();
    public List<float> legBRailJointPosition = new List<float>();
    public List<float> legBVerticalJointPosition = new List<float>();
    public List<float> legCRailJointPosition = new List<float>();
    public List<float> legCGripJointPosition = new List<float>();
    public List<float> legCRotationJointPosition = new List<float>();

    public List<float> legARailJointDistToMove = new List<float>();
    public List<float> legAVerticalJointDistToMove = new List<float>();
    public List<float> legARotationJointDistToMove = new List<float>();
    public List<float> legBRailJointDistToMove = new List<float>();
    public List<float> legBVerticalJointDistToMove = new List<float>();
    public List<float> legCRailJointDistToMove = new List<float>();
    public List<float> legCGripJointDistToMove = new List<float>();
    public List<float> legCRotationJointDistToMove = new List<float>();

}
