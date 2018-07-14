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

    const int legA = 0;
    const int legB = 1;
    const int legC = 2;
    const int bothLegs = 3;


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
    float gridDimY = 0.0625f;
    float clearanceHeight = 0.02f;

    public int grippedPos = 4800;

    public Vector3Int currentLegAGrid;
    public Vector3Int currentLegBGrid;
    public Vector3Int currentLegCGrid;

    public int currentlyAttached; // legA or legB
    public bool moveInProgress = false;
    public bool stepInProgress = false;
    public int moveCounter = 0;

    int legCRailMoveTypeStore = 0;
    int currentStance;
    int brickCurrentlyCarried = 0; // 0 = no brick // 1 = full brick // 2 = half brick

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

        currentStance = 4;

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

    int[] LiftLeg(int _legToLift, int _gridStepsToMove)
    {
        int[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_legToLift == legA)
        {
            outputTargetValues[1] = (int)(legAVerticalJoint.resetPos - (((_gridStepsToMove * gridDimY) + clearanceHeight) * 10000));
         //   outputTargetValues[9] = legB;
        }

        if (_legToLift == legB)
        {
            outputTargetValues[4] = (int)(legBVerticalJoint.resetPos - (((_gridStepsToMove * gridDimY) + clearanceHeight) * 10000));
         //   outputTargetValues[9] = legA;
        }

        return outputTargetValues;
    }

    int[] PlaceLeg(int _legToPlace, int _gridStepsToMove)
    {
        int[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_legToPlace == legA)
        {
            outputTargetValues[1] = (int)(legAVerticalJoint.resetPos - (((_gridStepsToMove * gridDimY) * 10000)));
          //  outputTargetValues[9] = legA;
        }

        if (_legToPlace == legB)
        {
            outputTargetValues[4] = (int)(legBVerticalJoint.resetPos - (((_gridStepsToMove * gridDimY) * 10000)));
           // outputTargetValues[9] = legB;

        }

        return outputTargetValues;
    }

    int[] ReturnToStance(int _currentStance)
    {
        int[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        outputTargetValues[0] = (int)(5000 + ((_currentStance * gridDimXZ) * 10000 / 2));
        outputTargetValues[3] = (int)(5000 - ((_currentStance * gridDimXZ) * 10000 / 2));
        outputTargetValues[5] = 5000;

        return outputTargetValues;
    }

    int[] SetForCounterbalance(int _pivotLeg, int _spacing)
    {
        int[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_pivotLeg == 0)
        {
            outputTargetValues[0] = 5000;
            outputTargetValues[3] = (int)(5000 - (_spacing * gridDimXZ * 10000));

            if (brickCurrentlyCarried == 0)
            {
                outputTargetValues[5] = (int)(5000 + (_spacing * gridDimXZ * 10000 * legCRailJoint.normalLegBFactor));
                outputTargetValues[8] = 2;
            }
            else if (brickCurrentlyCarried == 1)
            {
                outputTargetValues[5] = (int)(5000 + (_spacing * gridDimXZ * 10000 * legCRailJoint.fullBrickLegBFactor));
                outputTargetValues[8] = 4;
            }
            else if (brickCurrentlyCarried == 2)
            {
                outputTargetValues[5] = (int)(5000 + (_spacing * gridDimXZ * 10000 * legCRailJoint.halfBrickLegBFactor));
                outputTargetValues[8] = 6;
            }
        }

        if (_pivotLeg == 1)
        {
            outputTargetValues[3] = 5000;
            outputTargetValues[0] = (int)(5000 + (_spacing * gridDimXZ * 10000));

            if (brickCurrentlyCarried == 0)
            {
                outputTargetValues[5] = (int)(5000 - (_spacing * gridDimXZ * 10000 * legCRailJoint.normalLegAFactor));
                outputTargetValues[8] = 1;
            }
            else if (brickCurrentlyCarried == 1)
            {
                outputTargetValues[5] = (int)(5000 - (_spacing * gridDimXZ * 10000 * legCRailJoint.fullBrickLegAFactor));
                outputTargetValues[8] = 3;
            }
            else if (brickCurrentlyCarried == 2)
            {
                outputTargetValues[5] = (int)(5000 - (_spacing * gridDimXZ * 10000 * legCRailJoint.halfBrickLegAFactor));
                outputTargetValues[8] = 5;
            }
        }

        outputTargetValues[9] = _pivotLeg;

        return outputTargetValues;
    }


    public void TakeStep(int _stepGradient, int _stepSize, int _leadLeg)
    {
        jointTargetList.Clear();

        int leadingLeg;
        int trailingLeg;

        if (_leadLeg == legA)
        {
            leadingLeg = legA;
            trailingLeg = legB;
        }

        else
        {
            leadingLeg = legB;
            trailingLeg = legA;
        }

        int[] jointTargetListValues0 = SetForCounterbalance(trailingLeg, currentStance);
        int[] jointTargetListValues1 = LiftLeg(leadingLeg, _stepGradient+1);
        int[] jointTargetListValues2 = SetForCounterbalance(trailingLeg,_stepSize);
        int[] jointTargetListValues3 = PlaceLeg(leadingLeg, _stepGradient);
        int[] jointTargetListValues4 = SetForCounterbalance(leadingLeg,_stepSize);
        int[] jointTargetListValues5 = LiftLeg(trailingLeg, _stepGradient+1);
        int[] jointTargetListValues6 = SetForCounterbalance(leadingLeg, currentStance);
        int[] jointTargetListValues7 = PlaceLeg(trailingLeg, _stepGradient);
        int[] jointTargetListValues8 = ReturnToStance(currentStance);

        jointTargetList.Add(jointTargetListValues0);
        jointTargetList.Add(jointTargetListValues1);
        jointTargetList.Add(jointTargetListValues2);
        jointTargetList.Add(jointTargetListValues3);
        jointTargetList.Add(jointTargetListValues4);
        jointTargetList.Add(jointTargetListValues5);
        jointTargetList.Add(jointTargetListValues6);
        jointTargetList.Add(jointTargetListValues7);
        jointTargetList.Add(jointTargetListValues8);

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

        if (_currentlyAttached != -1)
        {
            currentlyAttached = _currentlyAttached;
        }

        if (_legARailTarget != -1)
        {
            legARailJoint.targetPos = _legARailTarget;
        }

        if (_legAVerticalTarget != -1)
        {
            legAVerticalJoint.targetPos = _legAVerticalTarget;
        }

        if (_legARotationTarget != -1)
        {
            legARotationJoint.targetPos = _legARotationTarget;
        }

        if (_legBRailTarget != -1)
        {
            legBRailJoint.targetPos = _legBRailTarget;
        }

        if (_legBVerticalTarget != -1)
        {
            legBVerticalJoint.targetPos = _legBVerticalTarget;
        }

        if (_legCRailTarget != -1)
        {
            legCRailJoint.targetPos = _legCRailTarget;
        }

        if (_legCGripTarget != -1)
        {
            legCGripJoint.targetPos = _legCGripTarget;
        }

        if (_legCRotationTarget != -1)
        {
            legCRotationJoint.targetPos = _legCRotationTarget;
        }

        foreach (RobotJoint joint in allJoints)
        {
            if (_legCRailMoveType == -1)
            {
                joint.SetLerpValues(legCRailMoveTypeStore);
            }
            else
            {
                joint.SetLerpValues(_legCRailMoveType);
                legCRailMoveTypeStore = _legCRailMoveType;
            }
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
