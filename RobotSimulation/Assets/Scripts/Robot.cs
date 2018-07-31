using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    bool simulateMovements = true;
    float totalProgramTime = 0;

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
    float verticalOffset = 0.114246f;

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

    public float gridDimXZ = 0.05625f; //brick grid dimensions
    public float gridDimY = 0.0625f;

    float clearanceHeight = 0.00f; //extra height to clear the brick when lifting a leg

    float gripClosedPos = 4800;
    float gripOpenPos = 3000;

    public Vector3Int currentLegAGrid;
    public Vector3Int currentLegBGrid;
    public Vector3Int currentLegCGrid;

    public int currentlyAttached; // legA or legB
    public bool moveInProgress = false;

    public bool stepInProgress = false;   //type of Move
    public bool pickupInProgress = false;
    public bool placementInProgress = false;
    public int moveCounter = 0;

    public bool brickIsAttached = false; //for external coordination with the brick
    public Brick brickBeingCarried;
    public int brickTypeCurrentlyCarried = 0; // 0 = no brick // 1 = full brick // 2 = half brick


    int legCRailMoveTypeStore = 0;
    int currentStance;
    bool footAHeelIn;

    List<float[]> jointTargetList = new List<float[]>();

    List<RobotAction> actionList = new List<RobotAction>();

    public Robot(Brick _startingBrick, int _startingStance, int _currentlyAttached, bool _footAHeelIn)
    {
        allJoints.Add(legARailJoint);
        allJoints.Add(legAVerticalJoint);
        allJoints.Add(legARotationJoint);
        allJoints.Add(legBRailJoint);
        allJoints.Add(legBVerticalJoint);
        allJoints.Add(legCRailJoint);
        allJoints.Add(legCGripJoint);
        allJoints.Add(legCRotationJoint);

        Vector3 startingPos = _startingBrick.originCell.position;
        Quaternion startingRot = _startingBrick.rotation;

        currentlyAttached = _currentlyAttached;

        currentStance = _startingStance;
        footAHeelIn = _footAHeelIn;

        if (!footAHeelIn) // if heel is supposed to be out, set the current position to 1800
        {
            allJoints[2].targetPos = 1800;
        }

        if (currentlyAttached == 0)
        {
            legAFootPos = new Vector3(startingPos.x * gridDimXZ, (startingPos.y + 1) * gridDimY, startingPos.z * gridDimXZ);
            legAFootRot = startingRot;
        }
        if (currentlyAttached == 1)
        {
            legBFootPos = new Vector3(startingPos.x * gridDimXZ, (startingPos.y + 1) * gridDimY, startingPos.z * gridDimXZ);
            legBFootRot = startingRot * Quaternion.Euler(0, 180, 0);
        }
    }

    public void UpdateRobot()
    {
        if (simulateMovements)
        {
            CarryOutMoves();
            UpdateReferenceTransforms();
        }

        else
        {
            totalProgramTime += AddUpTimeForMoves();
        }
    }

    float[] LiftLeg(int _legToLift, int _gridStepsToMove)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_legToLift == legA)
        {
            outputTargetValues[1] = (legAVerticalJoint.resetPos - ((gridDimY + clearanceHeight) * 10000));
            outputTargetValues[9] = legB;
        }

        if (_legToLift == legB)
        {
            outputTargetValues[4] = (legBVerticalJoint.resetPos - ((gridDimY + clearanceHeight) * 10000));
            outputTargetValues[9] = legA;
        }

        return outputTargetValues;
    }

    float[] LiftBothLegs(int _gridStepsToMove)
    {

        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_gridStepsToMove > 0)
        {
            outputTargetValues[1] = (legAVerticalJoint.resetPos + (gridDimY * 10000));
            outputTargetValues[4] = (legBVerticalJoint.resetPos + (gridDimY * 10000));
        }
        else if (_gridStepsToMove < 0)
        {
            outputTargetValues[1] = (legAVerticalJoint.resetPos);
            outputTargetValues[4] = (legBVerticalJoint.resetPos);
        }

        return outputTargetValues;
    }

    float[] PlaceLeg(int _legToPlace, int _gridStepsToMove)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_legToPlace == legA)
        {
            if (_gridStepsToMove < 0)
            {
                outputTargetValues[1] = legAVerticalJoint.resetPos - (_gridStepsToMove * gridDimY * 10000);
            }
            else
            {
                outputTargetValues[1] = legAVerticalJoint.resetPos;
            }
        }

        if (_legToPlace == legB)
        {
            if (_gridStepsToMove < 0)
            {
                outputTargetValues[4] = legBVerticalJoint.resetPos - (_gridStepsToMove * gridDimY * 10000);
            }
            else
            {
                outputTargetValues[4] = legBVerticalJoint.resetPos;
            }
        }

        return outputTargetValues;
    }

    float[] ReturnToStance(int _currentStance)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        outputTargetValues[0] = 5000 + ((_currentStance * gridDimXZ) * 10000 / 2);
        outputTargetValues[3] = 5000 - ((_currentStance * gridDimXZ) * 10000 / 2);
        outputTargetValues[5] = 5000;

        return outputTargetValues;
    }

    float[] SetForCounterbalance(int _pivotLeg, float _spacing, int _brickTypeCurrentlyBeingCarried)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_pivotLeg == 0)
        {
            outputTargetValues[0] = 5000;
            outputTargetValues[3] = 5000 - (_spacing * gridDimXZ * 10000);

            if (_brickTypeCurrentlyBeingCarried == 0)
            {
                outputTargetValues[5] = 5000 + (_spacing * gridDimXZ * 10000 * legCRailJoint.normalLegBFactor);
                outputTargetValues[8] = 2;
            }
            else if (_brickTypeCurrentlyBeingCarried == 1)
            {
                outputTargetValues[5] = 5000 + (_spacing * gridDimXZ * 10000 * legCRailJoint.fullBrickLegBFactor);
                outputTargetValues[8] = 4;
            }
            else if (_brickTypeCurrentlyBeingCarried == 2)
            {
                outputTargetValues[5] = 5000 + (_spacing * gridDimXZ * 10000 * legCRailJoint.halfBrickLegBFactor);
                outputTargetValues[8] = 6;
            }
        }

        if (_pivotLeg == 1)
        {
            outputTargetValues[3] = 5000;
            outputTargetValues[0] = 5000 + (_spacing * gridDimXZ * 10000);

            if (_brickTypeCurrentlyBeingCarried == 0)
            {
                outputTargetValues[5] = 5000 - (_spacing * gridDimXZ * 10000 * legCRailJoint.normalLegAFactor);
                outputTargetValues[8] = 1;
            }
            else if (_brickTypeCurrentlyBeingCarried == 1)
            {
                outputTargetValues[5] = 5000 - (_spacing * gridDimXZ * 10000 * legCRailJoint.fullBrickLegAFactor);
                outputTargetValues[8] = 3;
            }
            else if (_brickTypeCurrentlyBeingCarried == 2)
            {
                outputTargetValues[5] = 5000 - (_spacing * gridDimXZ * 10000 * legCRailJoint.halfBrickLegAFactor);
                outputTargetValues[8] = 5;
            }
        }

        outputTargetValues[9] = _pivotLeg;

        return outputTargetValues;
    }

    float[] RotateLegA(float _turnAngle)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        outputTargetValues[2] = _turnAngle;

        return outputTargetValues;
    }

    float[] PrepareLegsForGrip(int _baseLeg, float _distanceInFront, bool _straightSequence, int _brickTypeCurrentlyBeingCarried)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_baseLeg == 0)
        {
            outputTargetValues[0] = 5000;
            outputTargetValues[5] = 5000 + (_distanceInFront * gridDimXZ * 10000);

            if (_brickTypeCurrentlyBeingCarried == 0 && !_straightSequence)
            {

                outputTargetValues[3] = 5000 - (_distanceInFront * gridDimXZ * 10000 / legCRailJoint.normalLegBFactor);
                outputTargetValues[8] = 2;
            }
            else if (_brickTypeCurrentlyBeingCarried == 1 && !_straightSequence)
            {
                outputTargetValues[3] = 5000 - (_distanceInFront * gridDimXZ * 10000 / legCRailJoint.fullBrickLegBFactor);
                outputTargetValues[8] = 4;
            }
            else if (_brickTypeCurrentlyBeingCarried == 2 && !_straightSequence)
            {
                outputTargetValues[3] = 5000 - (_distanceInFront * gridDimXZ * 10000 / legCRailJoint.halfBrickLegBFactor);
                outputTargetValues[8] = 6;
            }
        }

        if (_baseLeg == 1)
        {
            outputTargetValues[3] = 5000;
            outputTargetValues[5] = 5000 - (_distanceInFront * gridDimXZ * 10000);

            if (_brickTypeCurrentlyBeingCarried == 0 && !_straightSequence)
            {
                outputTargetValues[0] = 5000 + (_distanceInFront * gridDimXZ * 10000 / legCRailJoint.normalLegAFactor);
                outputTargetValues[8] = 1;
            }
            else if (_brickTypeCurrentlyBeingCarried == 1 && !_straightSequence)
            {
                outputTargetValues[0] = 5000 + (_distanceInFront * gridDimXZ * 10000 / legCRailJoint.fullBrickLegAFactor);
                outputTargetValues[8] = 3;
            }
            else if (_brickTypeCurrentlyBeingCarried == 2 && !_straightSequence)
            {
                outputTargetValues[0] = 5000 + (_distanceInFront * gridDimXZ * 10000 / legCRailJoint.halfBrickLegAFactor);
                outputTargetValues[8] = 5;
            }
        }

        outputTargetValues[9] = _baseLeg;

        return outputTargetValues;
    }

    float[] OpenGrip()
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        outputTargetValues[6] = gripOpenPos;

        return outputTargetValues;
    }

    float[] RotateGrip(float _turnAngle)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        outputTargetValues[7] = _turnAngle;

        return outputTargetValues;
    }

    float[] CloseGrip()
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        outputTargetValues[6] = gripClosedPos;

        return outputTargetValues;
    }

    float[] LiftBothLegsForBrick(int _baseLeg, int _relativeBrickHeight, bool _straightSequence)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_baseLeg == legA)
        {
            outputTargetValues[1] = legAVerticalJoint.resetPos;

            if (_straightSequence)
            {
                outputTargetValues[4] = legBVerticalJoint.resetPos;
            }
            else
            {
                outputTargetValues[4] = (legBVerticalJoint.resetPos - (gridDimY * 10000));
            }
        }
        else if (_baseLeg == legB)
        {
            outputTargetValues[4] = legBVerticalJoint.resetPos;

            if (_straightSequence)
            {
                outputTargetValues[1] = legBVerticalJoint.resetPos;
            }
            else
            {
                outputTargetValues[1] = (legBVerticalJoint.resetPos - (gridDimY * 10000));
            }
        }

        return outputTargetValues;
    }

    float[] LowerBothLegsForBrick(int _baseLeg, int _relativeBrickHeight, bool _straightSequence)
    {
        float[] outputTargetValues = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        float pickupOffsetFromResetPos = 0.135321f;

        if (_baseLeg == legA)
        {
            outputTargetValues[1] = (legAVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight) * gridDimY + pickupOffsetFromResetPos) * 10000);

            if (_relativeBrickHeight < 0)
            {
                outputTargetValues[4] = (legBVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight) * gridDimY + pickupOffsetFromResetPos) * 10000);
            }

            else
            {
                if (_straightSequence)
                {
                    outputTargetValues[4] = (legBVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight) * gridDimY + pickupOffsetFromResetPos) * 10000);
                }
                else
                {
                    outputTargetValues[4] = (legBVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight - 1) * gridDimY + pickupOffsetFromResetPos) * 10000);
                }
            }
        }
        else if (_baseLeg == legB)
        {
            outputTargetValues[4] = (legBVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight) * gridDimY + pickupOffsetFromResetPos) * 10000);

            if (_relativeBrickHeight < 0)
            {
                outputTargetValues[1] = (legBVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight) * gridDimY + pickupOffsetFromResetPos) * 10000);
            }

            else
            {
                if (_straightSequence)
                {
                    outputTargetValues[1] = (legBVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight) * gridDimY + pickupOffsetFromResetPos) * 10000);
                }
                else
                {
                    outputTargetValues[1] = (legBVerticalJoint.resetPos - (Mathf.Abs(_relativeBrickHeight - 1) * gridDimY + pickupOffsetFromResetPos) * 10000);
                }
            }
        }

        return outputTargetValues;
    }

    public void ChangeFootAHeelPosition(int _newPosition)
    {
        bool changeIsNeeded = false;

        if (_newPosition == 0 && footAHeelIn == false)
        {
            footAHeelIn = true;
            changeIsNeeded = true;
        }
        else if (_newPosition == 180 && footAHeelIn == true)
        {
            footAHeelIn = false;
            changeIsNeeded = true;
        }

        jointTargetList.Clear();

        float[] jointTargetListValues0 = SetForCounterbalance(legB, currentStance, brickTypeCurrentlyCarried);
        float[] jointTargetListValues1 = LiftLeg(legA, 1);
        float[] jointTargetListValues2 = RotateLegA(_newPosition * 10);
        float[] jointTargetListValues3 = PlaceLeg(legA, 1);
        float[] jointTargetListValues4 = ReturnToStance(currentStance);

        jointTargetList.Add(jointTargetListValues0);
        jointTargetList.Add(jointTargetListValues1);
        jointTargetList.Add(jointTargetListValues2);
        jointTargetList.Add(jointTargetListValues3);
        jointTargetList.Add(jointTargetListValues4);

        if (changeIsNeeded)
        {
            stepInProgress = true;
            placementInProgress = true;
            moveCounter = 0;
        }

    }

    public void HandleBrick(int _relativeBrickHeight, int _distanceInFront, int _distanceToSide, int _leadLeg, int _gripRotation, Brick _brickToMove, bool _pickupMode, bool _backWeight)
    {
        jointTargetList.Clear();
        int brickTypeToBeCarriedAfterHandle = 0;

        if (_pickupMode == true)
        {
            brickTypeToBeCarriedAfterHandle = _brickToMove.brickType;
        }

        brickBeingCarried = _brickToMove;

        bool straightSequence = false;

        if (_distanceToSide == 0 && _backWeight)
        {
            straightSequence = true;
        }
        bool preTurnIsNeeded = false;
        float preTurnAngle = 0;
        bool postTurnIsNeeded = false;
        float postTurnAngle = 0;

        // account for foot A heel in or out
        float currentLegABase = 0;

        if (_distanceToSide <= 0 && !footAHeelIn)
        {
            currentLegABase = 180;
        }

        else if (_distanceToSide > 0 && !footAHeelIn)
        {
            preTurnIsNeeded = true;
            preTurnAngle = 0;
            postTurnIsNeeded = true;
            postTurnAngle = 1800;
            currentLegABase = 0;
        }

        else if (_distanceToSide < 0 && footAHeelIn)
        {
            preTurnIsNeeded = true;
            preTurnAngle = 1800;
            postTurnIsNeeded = true;
            postTurnAngle = 0;
            currentLegABase = 180;
        }

        else if (_distanceToSide >= 0 && footAHeelIn)
        {
            currentLegABase = 0;
        }

        float legARotation = RadianToDegree(Mathf.Atan((float)_distanceToSide / _distanceInFront));

        float gripRotation = 0;

        if (legARotation >= 0)
        {
            gripRotation = _gripRotation + legARotation;
        }
        else if (legARotation < 0)
        {
            gripRotation = 180 - _gripRotation + legARotation;
        }

        float legCPlacementDistance = Mathf.Sqrt(Mathf.Pow(_distanceInFront, 2) + Mathf.Pow(_distanceToSide, 2));


        //set leg types
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
            legARotation = 0;
        }

        float[] jointTargetListValues0 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues1 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues2 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues3 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues4 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (preTurnIsNeeded)
        {
            jointTargetListValues0 = SetForCounterbalance(legB, currentStance, brickTypeCurrentlyCarried);
            jointTargetListValues1 = LiftLeg(legA, 1);
            jointTargetListValues2 = RotateLegA(preTurnAngle);
            jointTargetListValues3 = PlaceLeg(legA, 1);
            jointTargetListValues4 = ReturnToStance(currentStance);
        }

        float[] jointTargetListValues5 = SetForCounterbalance(leadingLeg, currentStance, brickTypeCurrentlyCarried);
        float[] jointTargetListValues6 = LiftLeg(trailingLeg, 1);

        float[] jointTargetListValues7 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (straightSequence)
        {
            jointTargetListValues7 = SetForCounterbalance(leadingLeg, 8, brickTypeCurrentlyCarried);
        }
        else
        {
            jointTargetListValues7 = SetForCounterbalance(leadingLeg, 4, brickTypeCurrentlyCarried);
        }

        float[] jointTargetListValues8 = RotateLegA((currentLegABase + legARotation) * 10); // non straight only

        float[] jointTargetListValues9 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (straightSequence)
        {
            jointTargetListValues9 = PlaceLeg(trailingLeg, 1);
        }

        float[] jointTargetListValues10 = PrepareLegsForGrip(leadingLeg, legCPlacementDistance, straightSequence, brickTypeCurrentlyCarried);
        float[] jointTargetListValues11 = RotateGrip((gripRotation * 10));
        float[] jointTargetListValues12 = LowerBothLegsForBrick(leadingLeg, _relativeBrickHeight, straightSequence);

        float[] jointTargetListValues13 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_pickupMode)
        {
            jointTargetListValues13 = CloseGrip();
        }
        else
        {
            jointTargetListValues13 = OpenGrip();
        }

        float[] jointTargetListValues14 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (!straightSequence)
        {
            jointTargetListValues14 = PrepareLegsForGrip(leadingLeg, legCPlacementDistance, straightSequence, brickTypeToBeCarriedAfterHandle);
        }

        float[] jointTargetListValues15 = LiftBothLegsForBrick(leadingLeg, _relativeBrickHeight, straightSequence);
        float[] jointTargetListValues16 = RotateGrip(0);
        float[] jointTargetListValues17 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues18 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (straightSequence)
        {
            jointTargetListValues17 = SetForCounterbalance(leadingLeg, 8, brickTypeToBeCarriedAfterHandle);
            jointTargetListValues18 = LiftLeg(trailingLeg, 1);
        }


        float[] jointTargetListValues19 = SetForCounterbalance(leadingLeg, currentStance, brickTypeToBeCarriedAfterHandle);
        float[] jointTargetListValues20 = RotateLegA(currentLegABase * 10);
        float[] jointTargetListValues21 = PlaceLeg(trailingLeg, 1);
        float[] jointTargetListValues22 = ReturnToStance(currentStance);

        float[] jointTargetListValues23 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues24 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues25 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues26 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues27 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (postTurnIsNeeded)
        {
            jointTargetListValues23 = SetForCounterbalance(legB, currentStance, brickTypeToBeCarriedAfterHandle);
            jointTargetListValues24 = LiftLeg(legA, 1);
            jointTargetListValues25 = RotateLegA(postTurnAngle);
            jointTargetListValues26 = PlaceLeg(legA, 1);
            jointTargetListValues27 = ReturnToStance(currentStance);
        }

        jointTargetList.Add(jointTargetListValues0);
        jointTargetList.Add(jointTargetListValues1);
        jointTargetList.Add(jointTargetListValues2);
        jointTargetList.Add(jointTargetListValues3);
        jointTargetList.Add(jointTargetListValues4);
        jointTargetList.Add(jointTargetListValues5);
        jointTargetList.Add(jointTargetListValues6);
        jointTargetList.Add(jointTargetListValues7);
        jointTargetList.Add(jointTargetListValues8);
        jointTargetList.Add(jointTargetListValues9);
        jointTargetList.Add(jointTargetListValues10);
        jointTargetList.Add(jointTargetListValues11);
        jointTargetList.Add(jointTargetListValues12);
        jointTargetList.Add(jointTargetListValues13);
        jointTargetList.Add(jointTargetListValues14);
        jointTargetList.Add(jointTargetListValues15);
        jointTargetList.Add(jointTargetListValues16);
        jointTargetList.Add(jointTargetListValues17);
        jointTargetList.Add(jointTargetListValues18);
        jointTargetList.Add(jointTargetListValues19);
        jointTargetList.Add(jointTargetListValues20);
        jointTargetList.Add(jointTargetListValues21);
        jointTargetList.Add(jointTargetListValues22);
        jointTargetList.Add(jointTargetListValues23);
        jointTargetList.Add(jointTargetListValues24);
        jointTargetList.Add(jointTargetListValues25);
        jointTargetList.Add(jointTargetListValues26);
        jointTargetList.Add(jointTargetListValues27);


        if (_pickupMode)
        {
            pickupInProgress = true;
        }
        else
        {
            placementInProgress = true;
        }

        stepInProgress = true;
        moveCounter = 0;
    }

    public void TakeStep(int _stepGradient, int _stepSize, int _leadLeg, int _endStance, int _turnAngle, int _footAHeel)
    {
        jointTargetList.Clear();

        int leadingLeg;
        int trailingLeg;

        float preTurnAngle = 0;
        float initialTurnAngle = 0;
        float finalTurnAngle = 0;
        float postTurnAngle = 0;

        bool preTurnIsNeeded = false;
        bool postTurnIsNeeded = false;

        if (footAHeelIn == true) // heel in - 0 is the resting position
        {

            if (_leadLeg == legA)
            {
                leadingLeg = legA;
                trailingLeg = legB;

                if (_turnAngle == 90) // turn right
                {
                    initialTurnAngle = 900;
                    finalTurnAngle = 0;
                }
                else if (_turnAngle == -90) // turn left
                {
                    initialTurnAngle = 900;
                    finalTurnAngle = 1800;
                    postTurnAngle = 0;
                    postTurnIsNeeded = true;
                }
                else if (_turnAngle == 180) // turn 180
                {
                    //initialTurnAngle = 1800;
                    //finalTurnAngle = 0;

                    initialTurnAngle = 0;
                    finalTurnAngle = 1800;

                    postTurnAngle = 0;
                    postTurnIsNeeded = true;
                }

                if (_footAHeel == 180 && _turnAngle == 0)
                {
                    initialTurnAngle = 1800;
                    finalTurnAngle = 1800;
                    footAHeelIn = false;
                }
            }

            else
            {
                leadingLeg = legB;
                trailingLeg = legA;

                if (_turnAngle == -90) // turn right
                {
                    preTurnAngle = 1800;
                    initialTurnAngle = 900;
                    finalTurnAngle = 0;
                    preTurnIsNeeded = true;
                }
                else if (_turnAngle == 90) // turn left
                {
                    initialTurnAngle = 900;
                    finalTurnAngle = 0;
                }
                else if (_turnAngle == 180) // turn 180
                {
                    initialTurnAngle = 1800;
                    finalTurnAngle = 0;
                }

                if (_footAHeel == 180 && _turnAngle == 0)
                {
                    finalTurnAngle = 1800;
                    footAHeelIn = false;
                }
            }

        }

        else // heel out - 180 is the resting position
        {
            if (_leadLeg == legA)
            {
                leadingLeg = legA;
                trailingLeg = legB;

                if (_turnAngle == 90) // turn right
                {

                    initialTurnAngle = 1800;
                    finalTurnAngle = 900;
                    postTurnAngle = 1800;
                    postTurnIsNeeded = true;
                }
                else if (_turnAngle == -90) // turn left
                {
                    initialTurnAngle = 900;
                    finalTurnAngle = 1800;
                }
                else if (_turnAngle == 180) // turn 180
                {
                    initialTurnAngle = 0;
                    finalTurnAngle = 1800;
                }

                if (_footAHeel == 0 && _turnAngle == 0)
                {
                    initialTurnAngle = 0;
                    finalTurnAngle = 0;
                    footAHeelIn = true;
                }
            }

            else
            {
                leadingLeg = legB;
                trailingLeg = legA;

                if (_turnAngle == -90) // turn right
                {
                    //preTurnAngle = 0;
                    initialTurnAngle = 900;
                    finalTurnAngle = 1800;
                    //preTurnIsNeeded = true;
                }
                else if (_turnAngle == 90) // turn left
                {
                    preTurnAngle = 0;
                    initialTurnAngle = 900;
                    finalTurnAngle = 1800;
                    preTurnIsNeeded = true;
                }
                else if (_turnAngle == 180) // turn 180
                {
                    initialTurnAngle = 0;
                    finalTurnAngle = 1800;
                }

                if (_footAHeel == 180 && _turnAngle == 0)
                {
                    finalTurnAngle = 0;
                    footAHeelIn = true;
                }
            }
        }

        //PRE TURN
        float[] jointTargetListValues0 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues1 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues2 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues3 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (preTurnIsNeeded)
        {
            jointTargetListValues0 = SetForCounterbalance(leadingLeg, currentStance, brickTypeCurrentlyCarried);
            jointTargetListValues1 = LiftLeg(trailingLeg, 1);
            jointTargetListValues2 = RotateLegA(preTurnAngle);
            jointTargetListValues3 = PlaceLeg(trailingLeg, 1);
        }

        //SHIFT HEIGHT IF GOING UP
        float[] jointTargetListValues4 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_stepGradient > 0)
        {
            jointTargetListValues4 = LiftBothLegs(_stepGradient);
        }

        //MAIN STEP
        float[] jointTargetListValues5 = SetForCounterbalance(trailingLeg, currentStance, brickTypeCurrentlyCarried);
        float[] jointTargetListValues6 = LiftLeg(leadingLeg, _stepGradient);
        float[] jointTargetListValues7 = SetForCounterbalance(trailingLeg, _stepSize, brickTypeCurrentlyCarried);
        float[] jointTargetListValues8 = RotateLegA(initialTurnAngle);
        float[] jointTargetListValues9 = PlaceLeg(leadingLeg, _stepGradient);
        float[] jointTargetListValues10 = SetForCounterbalance(leadingLeg, _stepSize, brickTypeCurrentlyCarried);
        float[] jointTargetListValues11 = LiftLeg(trailingLeg, _stepGradient + 1);
        float[] jointTargetListValues12 = SetForCounterbalance(leadingLeg, _endStance, brickTypeCurrentlyCarried);
        float[] jointTargetListValues13 = RotateLegA(finalTurnAngle);
        float[] jointTargetListValues14 = PlaceLeg(trailingLeg, _stepGradient);

        float[] jointTargetListValues15 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (!postTurnIsNeeded)
        {
            jointTargetListValues15 = ReturnToStance(_endStance);
        }

        //SHIFT HEIGHT IF GOING DOWN
        float[] jointTargetListValues16 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_stepGradient < 0)
        {
            jointTargetListValues16 = LiftBothLegs(_stepGradient);
        }

        // POST TURN
        float[] jointTargetListValues17 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues18 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues19 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues20 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        float[] jointTargetListValues21 = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (postTurnIsNeeded)
        {
            jointTargetListValues17 = SetForCounterbalance(trailingLeg, currentStance, brickTypeCurrentlyCarried);
            jointTargetListValues18 = LiftLeg(leadingLeg, 1);
            jointTargetListValues19 = RotateLegA(postTurnAngle);
            jointTargetListValues20 = PlaceLeg(leadingLeg, 1);
            jointTargetListValues21 = ReturnToStance(_endStance);

        }

        if (preTurnIsNeeded)
        {
            jointTargetList.Add(jointTargetListValues0);
            jointTargetList.Add(jointTargetListValues1);
            jointTargetList.Add(jointTargetListValues2);
            jointTargetList.Add(jointTargetListValues3);
        }

        jointTargetList.Add(jointTargetListValues4);
        jointTargetList.Add(jointTargetListValues5);
        jointTargetList.Add(jointTargetListValues6);
        jointTargetList.Add(jointTargetListValues7);
        jointTargetList.Add(jointTargetListValues8);
        jointTargetList.Add(jointTargetListValues9);
        jointTargetList.Add(jointTargetListValues10);
        jointTargetList.Add(jointTargetListValues11);
        jointTargetList.Add(jointTargetListValues12);
        jointTargetList.Add(jointTargetListValues13);
        jointTargetList.Add(jointTargetListValues14);
        jointTargetList.Add(jointTargetListValues15);
        jointTargetList.Add(jointTargetListValues16);

        if (postTurnIsNeeded)
        {
            jointTargetList.Add(jointTargetListValues17);
            jointTargetList.Add(jointTargetListValues18);
            jointTargetList.Add(jointTargetListValues19);
            jointTargetList.Add(jointTargetListValues20);
            jointTargetList.Add(jointTargetListValues21);
        }

        currentStance = _endStance;
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

        if (pickupInProgress && moveCounter == 15)
        {
            brickIsAttached = true;
            brickTypeCurrentlyCarried = brickBeingCarried.brickType;
        }

        if (placementInProgress && moveCounter == 15)
        {
            brickIsAttached = false;
            brickTypeCurrentlyCarried = 0;
        }

        if (moveCounter == jointTargetList.Count)
        {
            stepInProgress = false;
            pickupInProgress = false;
            placementInProgress = false;
        }
    }

    public void RobotMove(float _legARailTarget, float _legAVerticalTarget, float _legARotationTarget, float _legBRailTarget, float _legBVerticalTarget, float _legCRailTarget, float _legCGripTarget, float _legCRotationTarget, float _legCRailMoveType, float _currentlyAttached)
    {
        moveInProgress = true;

        if (_currentlyAttached != -1)
        {
            currentlyAttached = (int)_currentlyAttached;
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
                joint.SetLerpValues((int)_legCRailMoveType);
                legCRailMoveTypeStore = (int)_legCRailMoveType;
            }
        }

    }

    public float AddUpTimeForMoves()
    {
        float timeForMoves = 0;
        float longestTimeForMove = 0;
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
            if (joint.GetTimeForMove() > longestTimeForMove)
            {
                longestTimeForMove = joint.GetTimeForMove();
            }
            joint.currentPos = joint.targetPos;
        }

        timeForMoves = longestTimeForMove;

        return timeForMoves;
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
            legARot = Quaternion.Euler(0, (legARotationJoint.currentPos / 10), 0) * legAFootRot;

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
            legCFootRot = legCRot * Quaternion.Euler(0, -legCRotationJoint.currentPos / 10, 0);

            grip1Pos = legCFootPos + (legCRot * new Vector3(0, 0, -(gripClosedPos - legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCRot * new Vector3(0, 0, (gripClosedPos - legCGripJoint.currentPos) * 0.00001f));
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
            legAFootRot = legARot * Quaternion.Euler(0, -legARotationJoint.currentPos / 10, 0);

            legCPos = mainBeamPos + legBRot * (new Vector3(0, -0.192f, -(legCRailJoint.currentPos * 0.0001f - 0.5f)));
            legCRot = legBRot;

            legCFootPos = legCPos;
            legCFootRot = legCRot * Quaternion.Euler(0, -legCRotationJoint.currentPos / 10, 0);

            grip1Pos = legCFootPos + (legCRot * new Vector3(0, 0, -(gripClosedPos - legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCRot * new Vector3(0, 0, (gripClosedPos - legCGripJoint.currentPos) * 0.00001f));
            grip2Rot = legCFootRot;
        }
    }

    private float RadianToDegree(float angle)
    {
        return (angle * (180 / Mathf.PI));
    }

    private float DegreeToRadian(float angle)
    {
        return (Mathf.PI * angle / 180);
    }
}
