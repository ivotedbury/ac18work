using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    //joint names
    const char legARail = 'A';
    const char legAVertical = 'B';
    const char legARotation = 'C';
    const char legBRail = 'D';
    const char legBVertical = 'E';
    const char legCRail = 'F';
    const char legCGrip = 'G';
    const char legCRotation = 'H';

    List<RobotJoint> allJoints = new List<RobotJoint>();

    RobotJoint legARailJoint = new RobotJoint(legARail);
    RobotJoint legAVerticalJoint = new RobotJoint(legAVertical);
    RobotJoint legARotationJoint = new RobotJoint(legARotation);
    RobotJoint legBRailJoint = new RobotJoint(legBRail);
    RobotJoint legBVerticalJoint = new RobotJoint(legBVertical);
    RobotJoint legCRailJoint = new RobotJoint(legCRail);
    RobotJoint legCGripJoint = new RobotJoint(legCGrip);
    RobotJoint legCRotationJoint = new RobotJoint(legCRotation);

    //display mesh offset for vertical joints
    float verticalOffset = 0.124246f;

    //positions for display meshes
    public Vector3 legAFootPos;
    public Vector3 legAPos;
    public Vector3 legBFootPos;
    public Vector3 legBPos;
    public Vector3 mainBeamPos;
    public Vector3 legCPos;
    public Vector3 legCFootPos;
    public Vector3 verticalToHorizontalAPos;
    public Vector3 verticalToHorizontalBPos;
    public Vector3 grip1Pos;
    public Vector3 grip2Pos;

    //rotations for display meshes
    public Quaternion legAFootRot;
    public Quaternion legARot;
    public Quaternion legBFootRot;
    public Quaternion legBRot;
    public Quaternion mainBeamRot;
    public Quaternion legCRot;
    public Quaternion legCFootRot;
    public Quaternion verticalToHorizontalARot;
    public Quaternion verticalToHorizontalBRot;
    public Quaternion grip1Rot;
    public Quaternion grip2Rot;

    float gridDimXZ = 0.05625f;
    float gridDimY = 0.0725f;

    public int grippedPos = 4800;

    public Vector3Int currentLegAGrid;
    public Vector3Int currentLegBGrid;
    public Vector3Int currentLegCGrid;

    public int currentlyAttached; // legA or legB
    public bool moveInProgress = false;
    public bool stepInProgress = false;
    public int moveCounter = 0;

    public float speedFactor = 10;

    List<int[]> jointTargetList = new List<int[]>();

    public Robot(Vector3Int _startingPos, int _startingStance, int _currentlyAttached)
    {
        allJoints.Add(legARailJoint);
        allJoints.Add(legAVerticalJoint);
        allJoints.Add(legARotationJoint);
        allJoints.Add(legBRailJoint);
        allJoints.Add(legBVerticalJoint);
        allJoints.Add(legCRailJoint);
        allJoints.Add(legCGripJoint);
        allJoints.Add(legCRotationJoint);

        currentlyAttached = _currentlyAttached;

        if (currentlyAttached == 0)
        {
            legAFootPos = new Vector3(_startingPos.x * gridDimXZ, _startingPos.y * gridDimY, _startingPos.z * gridDimXZ);
            legAFootRot = Quaternion.identity;
        }
        if (currentlyAttached == 1)
        {
            legBFootPos = new Vector3(_startingPos.x * gridDimXZ, _startingPos.y * gridDimY, (_startingPos.z + _startingStance) * gridDimXZ);
            legBFootRot = Quaternion.identity;
        }
    }

    public void UpdateRobot()
    {
        CarryOutMoves();
        UpdateReferenceTransforms();
    }

    public void TakeStep(string stepDescription)
    {
        jointTargetList.Clear();

        if (stepDescription == "Step along 4 lead A")
        {
            int[] jointTargetListValues0 = { 7250, 2150, 0, 5000, 2150, 2750, 3000, 0, 1, 1 };
            int[] jointTargetListValues1 = { 7250, 1425, 0, 5000, 2150, 2750, 3000, 0, 1, 1 };
            int[] jointTargetListValues2 = { 9500, 1425, 0, 5000, 2150, 500, 3000, 0, 1, 1 };
            int[] jointTargetListValues3 = { 9500, 2150, 0, 5000, 2150, 500, 3000, 0, 1, 1 };
            int[] jointTargetListValues4 = { 5000, 2150, 0, 500, 2150, 8870, 3000, 0, 1, 1 };
            int[] jointTargetListValues5 = { 5000, 2150, 0, 500, 1425, 8870, 3000, 0, 1, 0 };
            int[] jointTargetListValues6 = { 5000, 2150, 0, 2750, 1425, 6935, 3000, 0, 2, 0 };
            int[] jointTargetListValues7 = { 5000, 2150, 0, 2750, 2150, 6935, 3000, 0, 2, 0 };
            int[] jointTargetListValues8 = { 6125, 2150, 0, 3875, 2150, 5000, 3000, 0, 1, 0 };

            jointTargetList.Add(jointTargetListValues0);
            jointTargetList.Add(jointTargetListValues1);
            jointTargetList.Add(jointTargetListValues2);
            jointTargetList.Add(jointTargetListValues3);
            jointTargetList.Add(jointTargetListValues4);
            jointTargetList.Add(jointTargetListValues5);
            jointTargetList.Add(jointTargetListValues6);
            jointTargetList.Add(jointTargetListValues7);
            jointTargetList.Add(jointTargetListValues8);
        }

        else if (stepDescription == "Step along 4 lead B")
        {
            int[] jointTargetListValues0 = { 5000, 2150, 0, 2750, 2150, 6935, 3000, 0, 1, 0 };
            int[] jointTargetListValues1 = { 5000, 2150, 0, 2750, 1425, 6935, 3000, 0, 1, 0 };
            int[] jointTargetListValues2 = { 5000, 2150, 0, 500, 1425, 8870, 3000, 0, 2, 0 };
            int[] jointTargetListValues3 = { 5000, 2150, 0, 500, 2150, 8870, 3000, 0, 2, 0 };
            int[] jointTargetListValues4 = { 9500, 2150, 0, 5000, 2150, 500, 3000, 0, 2, 1 };
            int[] jointTargetListValues5 = { 9500, 1425, 0, 5000, 2150, 500, 3000, 0, 2, 1 };
            int[] jointTargetListValues6 = { 7250, 1425, 0, 5000, 2150, 2750, 3000, 0, 1, 1 };
            int[] jointTargetListValues7 = { 7250, 2150, 0, 5000, 2150, 2750, 3000, 0, 1, 1 };
            int[] jointTargetListValues8 = { 6125, 2150, 0, 3876, 2150, 5000, 3000, 0, 1, 1 };

            jointTargetList.Add(jointTargetListValues0);
            jointTargetList.Add(jointTargetListValues1);
            jointTargetList.Add(jointTargetListValues2);
            jointTargetList.Add(jointTargetListValues3);
            jointTargetList.Add(jointTargetListValues4);
            jointTargetList.Add(jointTargetListValues5);
            jointTargetList.Add(jointTargetListValues6);
            jointTargetList.Add(jointTargetListValues7);
            jointTargetList.Add(jointTargetListValues8);

        }

        stepInProgress = true;
        moveCounter = 0;
    }

    public void MakeMove()
    {
        RobotMove(jointTargetList[moveCounter][0],
           jointTargetList[moveCounter][1],
           jointTargetList[moveCounter][2],
           jointTargetList[moveCounter][3],
           jointTargetList[moveCounter][4],
           jointTargetList[moveCounter][5],
           jointTargetList[moveCounter][6],
           jointTargetList[moveCounter][7],
           jointTargetList[moveCounter][8],
           jointTargetList[moveCounter][9]);
        moveCounter++;

        if (moveCounter == jointTargetList.Count)
        {
            stepInProgress = false;
        }
    }

    public void RobotMove(int _legARailTarget, int _legAVerticalTarget, int _legARotationTarget, int _legBRailTarget, int _legBVerticalTarget, int _legCRailTarget, int _legCGripTarget, int _legCRotationTarget, int _legCRailMoveType, int _currentlyAttached)
    {
        moveInProgress = true;

        currentlyAttached = _currentlyAttached;

        legARailJoint.targetPos = _legARailTarget;
        legAVerticalJoint.targetPos = _legAVerticalTarget;
        legARotationJoint.targetPos = _legARotationTarget;
        legBRailJoint.targetPos = _legBRailTarget;
        legBVerticalJoint.targetPos = _legBVerticalTarget;
        legCRailJoint.targetPos = _legCRailTarget;
        legCGripJoint.targetPos = _legCGripTarget;
        legCRotationJoint.targetPos = _legCRotationTarget;

        foreach (RobotJoint joint in allJoints)
        {
            joint.SetLerpValues(_legCRailMoveType);
        }

    }

    public void CarryOutMoves()
    {
        if (legARailJoint.JointNeedsToMove() ||
            legAVerticalJoint.JointNeedsToMove() ||
            legARotationJoint.JointNeedsToMove() ||
            legBRailJoint.JointNeedsToMove() ||
            legBVerticalJoint.JointNeedsToMove() ||
            legCRailJoint.JointNeedsToMove() ||
            legCGripJoint.JointNeedsToMove() ||
            legCRotationJoint.JointNeedsToMove())
        {
            moveInProgress = true;
        }

        else
        {
            moveInProgress = false;
        }

        if (!moveInProgress && stepInProgress)
        {
            MakeMove();
        }

        foreach (RobotJoint joint in allJoints)
        {
            joint.LerpJointPosition();
        }

    }

    void UpdateReferenceTransforms()
    {
        if (currentlyAttached == 0)
        {
            legAPos = legAFootPos;
            legARot = Quaternion.Euler(0, legARotationJoint.currentPos / 10, 0) * legAFootRot;

            verticalToHorizontalAPos = legAPos + new Vector3(0, verticalOffset + (legAVerticalJoint.currentPos * 0.0001f), 0);
            verticalToHorizontalARot = legARot;

            mainBeamPos = verticalToHorizontalAPos + legARot * new Vector3(0, 0.06008f, (legARailJoint.currentPos * 0.0001f) - 0.5f);
            mainBeamRot = legARot;

            verticalToHorizontalBPos = mainBeamPos - legARot * (new Vector3(0, 0.06008f, legBRailJoint.currentPos * 0.0001f - 0.5f));
            verticalToHorizontalBRot = legARot;

            legBPos = verticalToHorizontalBPos - new Vector3(0, verticalOffset + (legBVerticalJoint.currentPos * 0.0001f), 0);
            legBRot = legARot;

            legBFootPos = legBPos;
            legBFootRot = legARot;

            legCPos = mainBeamPos + legARot * (new Vector3(0, -0.192f, -(legCRailJoint.currentPos * 0.0001f - 0.5f)));
            legCRot = legARot;

            legCFootPos = legCPos;
            legCFootRot = legCRot * Quaternion.Euler(0, legCRotationJoint.currentPos / 10, 0);

            grip1Pos = legCFootPos + (legCRot * new Vector3(0, 0, -(grippedPos - legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCRot * new Vector3(0, 0, (grippedPos - legCGripJoint.currentPos) * 0.00001f));
            grip2Rot = legCFootRot;
        }

        if (currentlyAttached == 1)
        {
            legBPos = legBFootPos;
            legBRot = legBFootRot;

            verticalToHorizontalBPos = legBPos + new Vector3(0, verticalOffset + (legBVerticalJoint.currentPos * 0.0001f), 0);
            verticalToHorizontalBRot = legBRot;

            mainBeamPos = verticalToHorizontalBPos + legBRot * new Vector3(0, 0.06008f, (legBRailJoint.currentPos * 0.0001f) - 0.5f);
            mainBeamRot = legBRot;

            verticalToHorizontalAPos = mainBeamPos - legBRot * (new Vector3(0, 0.06008f, (legARailJoint.currentPos * 0.0001f - 0.5f)));
            verticalToHorizontalARot = legBRot;

            legAPos = verticalToHorizontalAPos - new Vector3(0, verticalOffset + (legAVerticalJoint.currentPos * 0.0001f), 0);
            legARot = legBRot;

            legAFootPos = legAPos;
            legAFootRot = legARot * Quaternion.Euler(0, legARotationJoint.currentPos / 10, 0) * legAFootRot;

            legCPos = mainBeamPos + legBRot * (new Vector3(0, -0.192f, -(legCRailJoint.currentPos * 0.0001f - 0.5f)));
            legCRot = legBRot;

            legCFootPos = legCPos;
            legCFootRot = legCRot * Quaternion.Euler(0, legCRotationJoint.currentPos / 10, 0);

            grip1Pos = legCFootPos + (legCRot * new Vector3(0, 0, -(grippedPos - legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCRot * new Vector3(0, 0, (grippedPos - legCGripJoint.currentPos) * 0.00001f));
            grip2Rot = legCFootRot;
        }
    }
}
