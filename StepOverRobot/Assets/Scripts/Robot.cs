using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    // reset positions
    float legARailResetPos = 0.225f;
    float legAVerticalResetPos = 0.2125f;
    float legARotationResetPos = 0;
    float legBRailResetPos = 0.45f;
    float legBVerticalResetPos = 0.2125f;
    float legBRotationResetPos = 0;
    float legCRailResetPos = 0.3375f;
    float legCRotationResetPos = 0;
    float legCGripResetPos = 0;
    float nozzleVerticalResetPos = 0;
    float nozzleRotationResetPos = 0;
    float nozzleMortarResetPos = 0;

    // speeds
    float railBasicSpeed = 0.5f;
    float legARailResetSpeed = 0.25f;
    float legAVerticalResetSpeed = 0.25f;
    float legARotationResetSpeed = 90;
    float legBRailResetSpeed = 0.25f;
    float legBVerticalResetSpeed = 0.25f;
    float legBRotationResetSpeed = 90;
    float legCRailResetSpeed = 0.25f;
    float legCRotationResetSpeed = 90;
    float legCGripResetSpeed = 0.25f;
    float nozzleVerticalResetSpeed = 0.25f;
    float nozzleRotationResetSpeed = 90f;
    float nozzleMortarResetSpeed = 0.1f;

    //grid dimensions
    float gridXZDim = 0.05625f;
    float gridYDim = 0.0725f;

    // nozzle values
    Vector3 nozzleOffsetVector = new Vector3(0, 0, 0.39250f);
    float nozzleExtendedDistance = 0.12350f; //nozzle extended vertical distance;

    // general for robot
    List<RobotJoint> allJoints = new List<RobotJoint>();
    RobotGesture robotGesture = new RobotGesture();
    List<float[]> jointTargetList = new List<float[]>();

    bool gestureInProgress = false;
    bool stepInProgress = false;
    int gestureCounter;
    public bool moveInProgress; // checked to see when the next step can be taken

    // leg states
    int currentlyAttached; // leg currently attached
    int leadingLeg; // leg that needs to be lifted next to take a step

    bool steppingDown; // short 'memory' of whether the robot has recently stepped up or down
    bool steppingUp;

    int previousStepLength; // short 'memory' of how spaced out its legs are from the previous step

    // list of joints - the number corresponds to the index in the output of each 'gesture' requested
    RobotJoint legARailJoint; // 0
    RobotJoint legAVerticalJoint; // 1
    RobotJoint legARotationJoint; // 2
    RobotJoint legBRailJoint; // 3
    RobotJoint legBVerticalJoint; // 4
    RobotJoint legBRotationJoint; // 5
    RobotJoint legCRailJoint; // 6
    RobotJoint legCRotationJoint; // 7
    RobotJoint legCGripJoint; // 8
    RobotJoint nozzleVerticalJoint; // 9
    RobotJoint nozzleRotationJoint; // 10
    RobotJoint nozzleMortarJoint; // 11
                                  //legARailSpeed 12
                                  //legBRailSpeed 13
                                  //legCRailSpeed 14

    // position used for camera target
    public Vector3 averageRobotPos;

    //positions for display meshes
    public Vector3 legAFootPos;
    public Vector3 legAPos;
    public Vector3 legAHipPos;
    public Vector3 legBFootPos;
    public Vector3 legBPos;
    public Vector3 legBHipPos;
    public Vector3 mainBeamPos;
    public Vector3 legCShoulderPos;
    public Vector3 legCFootPos;
    public Vector3 grip1Pos;
    public Vector3 grip2Pos;
    public Vector3 nozzlePos;

    //rotations for display meshes
    public Quaternion legAFootRot;
    public Quaternion legARot;
    public Quaternion legAHipRot;
    public Quaternion legBFootRot;
    public Quaternion legBRot;
    public Quaternion legBHipRot;
    public Quaternion mainBeamRot;
    public Quaternion legCShoulderRot;
    public Quaternion legCFootRot;
    public Quaternion grip1Rot;
    public Quaternion grip2Rot;
    public Quaternion nozzleRot;

    //leg heights - to be calibrated
    float liftNormal = 0.88f;
    float liftHigher = 1.25f;
    float baseHeight = 0;

    // leg types
    int legA = 0;
    int legB = 1;

    //brick type being carried
    int brickCurrentlyCarried;

    int noBrick = 0;
    int fullBrick = 1;
    int halfBrick = 2;

    public Brick brickCurrentlyBeingCarried = null;

    public Robot(Vector3Int _startingCell, int _currentlyAttached, int _startingStance)
    {
        legARailJoint = new RobotJoint(railBasicSpeed, legARailResetPos);
        legAVerticalJoint = new RobotJoint(legAVerticalResetSpeed, legAVerticalResetPos);
        legARotationJoint = new RobotJoint(legARotationResetSpeed, legARotationResetPos);
        legBRailJoint = new RobotJoint(railBasicSpeed, legBRailResetPos);
        legBVerticalJoint = new RobotJoint(legBVerticalResetSpeed, legBVerticalResetPos);
        legBRotationJoint = new RobotJoint(legBRotationResetSpeed, legBRotationResetPos);
        legCRailJoint = new RobotJoint(railBasicSpeed, legCRailResetPos);
        legCRotationJoint = new RobotJoint(legCRotationResetSpeed, legCRotationResetPos);
        legCGripJoint = new RobotJoint(legCGripResetSpeed, legCGripResetPos);
        nozzleVerticalJoint = new RobotJoint(nozzleVerticalResetSpeed, nozzleVerticalResetPos);
        nozzleRotationJoint = new RobotJoint(nozzleRotationResetSpeed, nozzleRotationResetPos);
        nozzleMortarJoint = new RobotJoint(nozzleMortarResetSpeed, nozzleMortarResetPos);

        allJoints.Add(legARailJoint);
        allJoints.Add(legAVerticalJoint);
        allJoints.Add(legARotationJoint);
        allJoints.Add(legBRailJoint);
        allJoints.Add(legBVerticalJoint);
        allJoints.Add(legBRotationJoint);
        allJoints.Add(legCRailJoint);
        allJoints.Add(legCRotationJoint);
        allJoints.Add(legCGripJoint);
        allJoints.Add(nozzleVerticalJoint);
        allJoints.Add(nozzleRotationJoint);
        allJoints.Add(nozzleMortarJoint);

        // set the short 'memory' of previous steps
        steppingDown = false;
        steppingUp = false;
        previousStepLength = _startingStance;

        brickCurrentlyCarried = noBrick; // set the brick


        currentlyAttached = _currentlyAttached; // set the leg currently attached and 'spawn' the robot according to that position and rotation

        if (currentlyAttached == legA)
        {
            leadingLeg = legB;

            legAFootPos = new Vector3(_startingCell.x * gridXZDim, _startingCell.y * gridYDim, _startingCell.z * gridXZDim);
            legAFootRot = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            leadingLeg = legA;

            legBFootPos = new Vector3(_startingCell.x * gridXZDim, _startingCell.y * gridYDim, _startingCell.z * gridXZDim);
            legBFootRot = Quaternion.Euler(0, 180, 0);
        }

    }

    public void UpdateRobot()
    {
        UpdateReferenceTransforms();
        CarryOutMoves();

        // update the average position for the camera target 
        averageRobotPos = new Vector3((legAHipPos.x + legBHipPos.x + mainBeamPos.x) / 3, mainBeamPos.y, (legAHipPos.z + legBHipPos.z + mainBeamPos.z) / 3);
    }

    // handle can be picking or placing - may be easier to separate these out
    public void HandleBrick(int _relativeBrickHeight, int _distanceInFront, int _distanceToSide, int _brickType, int _gripRotation, Brick _brickToMove, bool _pickupMode)
    {
        jointTargetList.Clear();

        SetBrickTypeCurrentlyBeingCarried();

        int brickTypeToBeCarriedAfterHandle = 0;

        if (_pickupMode == true)
        {
            brickTypeToBeCarriedAfterHandle = _brickToMove.brickType;
        }

        brickCurrentlyBeingCarried = _brickToMove;

        /////////////////////////////////////////////////////////////////////////////

        //set over attached leg
        jointTargetList.Add(robotGesture.SetOverLeg(currentlyAttached, previousStepLength, brickCurrentlyCarried));

        if (_relativeBrickHeight == 0)
        {
            //lift leading leg
            if (!steppingDown && !steppingUp)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftNormal));
            }
            else if (steppingDown)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftHigher));
            }
            else if (steppingUp)
            {
                jointTargetList.Add(robotGesture.StepUpLegs(leadingLeg, liftNormal));
            }
        }

        // testNozzle - this is a placeholder
        jointTargetList.Add(robotGesture.ExtendNozzle(nozzleExtendedDistance, 90));

        // outstretch the gripper and rotate
        jointTargetList.Add(robotGesture.OutStretchGripper(currentlyAttached, _distanceInFront, _distanceToSide, previousStepLength, brickCurrentlyCarried));

        float brickfinalRotation = brickCurrentlyBeingCarried.rotation.eulerAngles.y;
        float currentRotation = brickCurrentlyBeingCarried.currentRotation.eulerAngles.y;
        float angleToRotate = currentRotation - brickfinalRotation;
        // rotate the brick
        jointTargetList.Add(robotGesture.RotateGripper(angleToRotate));

        // place brick
        jointTargetList.Add(robotGesture.LowerLegsToPlace(_relativeBrickHeight, currentlyAttached));

        gestureCounter = 0;
        moveInProgress = true;
        stepInProgress = true;

        // open the gripper
        // lift the gripper
        // etc.
    }

    public void TakeStep(int _stepLength, int _stepHeight, int _stepTurnAngle)
    {
        jointTargetList.Clear();

        SetBrickTypeCurrentlyBeingCarried();

        if (_stepHeight == 0)
        {
            //set over attached leg
            jointTargetList.Add(robotGesture.SetOverLeg(currentlyAttached, previousStepLength, brickCurrentlyCarried));

            //lift leading leg
            if (!steppingDown && !steppingUp)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftNormal));
            }
            else if (steppingDown)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftHigher));
            }
            else if (steppingUp)
            {
                jointTargetList.Add(robotGesture.StepUpLegs(leadingLeg, liftNormal));
            }

            //outstretch over leg
            jointTargetList.Add(robotGesture.OutStretchLeg(leadingLeg, _stepLength, _stepTurnAngle, brickCurrentlyCarried));

            //lower leading leg
            jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, 0));

            steppingDown = false;
            steppingUp = false;
        }

        if (_stepHeight == -1)
        {
            //set over attached leg
            jointTargetList.Add(robotGesture.SetOverLeg(currentlyAttached, previousStepLength, brickCurrentlyCarried));

            //lift leading leg
            if (!steppingDown && !steppingUp)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftNormal));
            }
            else if (steppingDown)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftHigher));
            }
            else if (steppingUp)
            {
                jointTargetList.Add(robotGesture.StepUpLegs(leadingLeg, liftNormal));
            }

            //outstretch over leg
            jointTargetList.Add(robotGesture.OutStretchLeg(leadingLeg, _stepLength, _stepTurnAngle, brickCurrentlyCarried));

            //lower leading leg
            jointTargetList.Add(robotGesture.StepDownLegs(leadingLeg, -1));

            steppingDown = true;
            steppingUp = false;
        }

        if (_stepHeight == 1)
        {
            //set over attached leg
            jointTargetList.Add(robotGesture.SetOverLeg(currentlyAttached, previousStepLength, brickCurrentlyCarried));

            //lift leading leg
            if (!steppingDown && !steppingUp)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftHigher));
            }
            else if (steppingDown)
            {
                jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, liftHigher));
            }
            else if (steppingUp)
            {
                jointTargetList.Add(robotGesture.StepUpLegs(leadingLeg, liftHigher));
            }

            //outstretch over leg
            jointTargetList.Add(robotGesture.OutStretchLeg(leadingLeg, _stepLength, _stepTurnAngle, brickCurrentlyCarried));

            //lower leading leg
            jointTargetList.Add(robotGesture.LiftLeg(leadingLeg, 1));

            steppingUp = true;
            steppingDown = false;
        }

        gestureCounter = 0;
        moveInProgress = true;
        stepInProgress = true;
        previousStepLength = _stepLength;
    }

    void CarryOutMoves()
    {
        gestureInProgress = false;

        foreach (RobotJoint joint in allJoints) // check to see if any joints need to move
        {
            if (joint.JointNeedsToMove())
            {
                gestureInProgress = true;
            }

            joint.LerpJointPosition();
        }

        if (!gestureInProgress && moveInProgress)
        {
            MakeNextGesture();
        }

        if (!gestureInProgress && !moveInProgress && stepInProgress) // if the move is a step and it has been completed, the leading legs and currently attached are switched.
        {
            SwitchLeadingLeg();
            SwitchCurrentlyAttached();
            stepInProgress = false;
        }

    }

    void MakeNextGesture() // if the previous gesure is complete, begin the next gesture
    {
        if (gestureCounter == jointTargetList.Count)
        {
            moveInProgress = false;
        }

        if (moveInProgress)
        {
            RobotMove(jointTargetList[gestureCounter]);
            gestureCounter++;
        }
    }

    void RobotMove(float[] _jointTarget) // if each of the joint target positions have been changed, update them.
    {
        //legARail
        if (_jointTarget[0] != -1)
        {
            legARailJoint.targetPos = _jointTarget[0];
        }

        //legAVertical
        if (_jointTarget[1] != -1)
        {
            legAVerticalJoint.targetPos = _jointTarget[1];
        }

        //legARotation
        if (_jointTarget[2] != -1)
        {
            legARotationJoint.targetPos = _jointTarget[2];
        }

        //legBRail
        if (_jointTarget[3] != -1)
        {
            legBRailJoint.targetPos = _jointTarget[3];
        }

        //legBVertical
        if (_jointTarget[4] != -1)
        {
            legBVerticalJoint.targetPos = _jointTarget[4];
        }

        //legBRotation
        if (_jointTarget[5] != -1)
        {
            legBRotationJoint.targetPos = _jointTarget[5];
        }

        //legCRail
        if (_jointTarget[6] != -1)
        {
            legCRailJoint.targetPos = _jointTarget[6];
        }

        //legCRotation
        if (_jointTarget[7] != -1)
        {
            legCRotationJoint.targetPos = _jointTarget[7];
        }

        //legCGrip
        if (_jointTarget[8] != -1)
        {
            legCGripJoint.targetPos = _jointTarget[8];
        }

        //nozzleVertical
        if (_jointTarget[9] != -1)
        {
            nozzleVerticalJoint.targetPos = _jointTarget[9];
        }

        //nozzleRotation
        if (_jointTarget[10] != -1)
        {
            nozzleRotationJoint.targetPos = _jointTarget[10];
        }
        //nozzleMortar
        if (_jointTarget[11] != -1)
        {
            nozzleMortarJoint.targetPos = _jointTarget[11];
        }

        //legARailSpeed
        if (_jointTarget[12] != -1)
        {
            legARailJoint.speed = _jointTarget[12] * railBasicSpeed;
        }

        //legBRailSpeed
        if (_jointTarget[13] != -1)
        {
            legBRailJoint.speed = _jointTarget[13] * railBasicSpeed;
        }

        //legCRailSpeed
        if (_jointTarget[14] != -1)
        {
            legCRailJoint.speed = _jointTarget[14] * railBasicSpeed;
        }

        foreach (RobotJoint joint in allJoints)
        {
            joint.SetLerpValues();
        }
    }

    void SetBrickTypeCurrentlyBeingCarried()
    {
        if (brickCurrentlyBeingCarried != null)
        {
            brickCurrentlyCarried = brickCurrentlyBeingCarried.brickType;
        }
        else
        {
            brickCurrentlyCarried = noBrick;
        }
    }

    void UpdateReferenceTransforms() // this builds a kinematic rig depending on which leg is attached. When the attached leg changes (e.g. when a step is complete), the kinematic rig is inverted. 
    {
        if (currentlyAttached == legA)
        {
            legAPos = legAFootPos;
            legARot = Quaternion.Euler(0, legARotationJoint.currentPos, 0) * legAFootRot;

            legAHipPos = legAPos + new Vector3(0, (legAVerticalJoint.currentPos - legAVerticalResetPos), 0);
            legAHipRot = legARot;

            mainBeamPos = legAHipPos + legARot * new Vector3(0, legAVerticalResetPos, (legARailJoint.currentPos - legCRailResetPos));
            mainBeamRot = legARot;

            legBHipPos = mainBeamPos - legARot * (new Vector3(0, legBVerticalResetPos, legBRailJoint.currentPos - legCRailResetPos));
            legBHipRot = legARot * Quaternion.Euler(0, 180, 0);

            legBPos = legBHipPos - new Vector3(0, legBVerticalJoint.currentPos - legBVerticalResetPos, 0);
            legBRot = legARot * Quaternion.Euler(0, 180, 0); ;

            legBFootPos = legBPos;
            legBFootRot = legARot * Quaternion.Euler(0, legBRotationJoint.currentPos, 0) * Quaternion.Euler(0, 180, 0);

            legCShoulderPos = mainBeamPos + legARot * (new Vector3(0, -0.04125f, (legCRailJoint.currentPos - legCRailResetPos)));
            legCShoulderRot = legARot;

            legCFootPos = legCShoulderPos;
            legCFootRot = legCShoulderRot * Quaternion.Euler(0, -legCRotationJoint.currentPos, 0);

            grip1Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, -(legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, (legCGripJoint.currentPos) * 0.00001f));
            grip2Rot = legCFootRot;

            nozzlePos = mainBeamPos + (mainBeamRot * nozzleOffsetVector) + new Vector3(0, 0.12350f - nozzleVerticalJoint.currentPos, 0);
            nozzleRot = mainBeamRot * Quaternion.Euler(0, nozzleRotationJoint.currentPos, 0);
        }

        if (currentlyAttached == legB)
        {
            legBPos = legBFootPos;
            legBRot = Quaternion.Euler(0, legBRotationJoint.currentPos, 0) * legBFootRot;

            legBHipPos = legBPos + new Vector3(0, (legBVerticalJoint.currentPos - legBVerticalResetPos), 0);
            legBHipRot = legBRot;

            mainBeamPos = legBHipPos + legBRot * new Vector3(0, legBVerticalResetPos, (legCRailResetPos - legBRailJoint.currentPos));
            mainBeamRot = legBRot * Quaternion.Euler(0, 180, 0);

            legAHipPos = mainBeamPos - legBRot * (new Vector3(0, legAVerticalResetPos, legCRailResetPos - legARailJoint.currentPos));
            legAHipRot = legARot * Quaternion.Euler(0, 0, 0);

            legAPos = legAHipPos - new Vector3(0, legAVerticalJoint.currentPos - legAVerticalResetPos, 0);
            legARot = legBRot * Quaternion.Euler(0, 180, 0); ;

            legAFootPos = legAPos;
            legAFootRot = legBRot * Quaternion.Euler(0, legARotationJoint.currentPos, 0) * Quaternion.Euler(0, 180, 0);

            legCShoulderPos = mainBeamPos - legBRot * (new Vector3(0, +0.04125f, (legCRailJoint.currentPos - legCRailResetPos)));
            legCShoulderRot = legBRot;

            legCFootPos = legCShoulderPos;
            legCFootRot = legCShoulderRot * Quaternion.Euler(0, (legCRotationJoint.currentPos) + 180, 0);

            grip1Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, -(legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, (legCGripJoint.currentPos) * 0.00001f));
            grip2Rot = legCFootRot;

            nozzlePos = mainBeamPos + (mainBeamRot * nozzleOffsetVector) + new Vector3(0, 0.12350f - nozzleVerticalJoint.currentPos, 0);
            nozzleRot = mainBeamRot * Quaternion.Euler(0, nozzleRotationJoint.currentPos, 0);
        }

        if (brickCurrentlyBeingCarried != null) // this updates current transform of a brick being carried
        {
            brickCurrentlyBeingCarried.currentPosition = legCFootPos + new Vector3(0, -(gridYDim - 0.01f), 0);
            brickCurrentlyBeingCarried.currentRotation = legCFootRot;
        }
    }

    private void SwitchLeadingLeg()
    {
        if (leadingLeg == legA)
        {
            leadingLeg = legB;
        }
        else
        {
            leadingLeg = legA;
        }
    }

    private void SwitchCurrentlyAttached()
    {
        if (currentlyAttached == legA)
        {
            currentlyAttached = legB;
        }
        else
        {
            currentlyAttached = legA;
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

