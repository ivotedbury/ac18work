using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    float legARailResetPos = 0.28125f;
    float legAVerticalResetPos = 0.18125f;
    float legARotationResetPos = 0;
    float legBRailResetPos = 0.50625f;
    float legBVerticalResetPos = 0.18125f;
    float legBRotationResetPos = 0;
    float legCRailResetPos = 0.39375f;
    float legCRotationResetPos = 0;
    float legCGripResetPos = 0;

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

    float gridXZDim = 0.05625f;
    float gridYDim = 0.0625f;

    List<RobotJoint> allJoints = new List<RobotJoint>();
    RobotGesture robotGesture = new RobotGesture();
    List<float[]> jointTargetList = new List<float[]>();
    bool gestureInProgress = false;
    bool stepInProgress = false;
    int gestureCounter;
    public bool moveInProgress;

    bool steppingDown;
    bool steppingUp;

    RobotJoint legARailJoint;
    RobotJoint legAVerticalJoint;
    RobotJoint legARotationJoint;
    RobotJoint legBRailJoint;
    RobotJoint legBVerticalJoint;
    RobotJoint legBRotationJoint;
    RobotJoint legCRailJoint;
    RobotJoint legCRotationJoint;
    RobotJoint legCGripJoint;

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

    // leg states
    int currentlyAttached;
    int leadingLeg;
    // leg types
    int legA = 0;
    int legB = 1;

    //gesture types
    int rotateLeg = 0;
    int liftLeg = 1;
    int setOverLeg = 2;
    int outstretchLeg = 3;
    int stepDownLegs = 4;
    int stepUpLegs = 5;
    int goToStance = 6;

    //brick type being carried
    int brickCurrentlyCarried;
    int noBrick = 0;
    int fullBrick = 1;
    int halfBrick = 2;

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

        allJoints.Add(legARailJoint);
        allJoints.Add(legAVerticalJoint);
        allJoints.Add(legARotationJoint);
        allJoints.Add(legBRailJoint);
        allJoints.Add(legBVerticalJoint);
        allJoints.Add(legBRotationJoint);
        allJoints.Add(legCRailJoint);
        allJoints.Add(legCRotationJoint);
        allJoints.Add(legCGripJoint);

        currentlyAttached = _currentlyAttached;
        brickCurrentlyCarried = noBrick;
        steppingDown = false;
        steppingUp = false;

        if (currentlyAttached == legA)
        {
            leadingLeg = legB;
        }
        else
        {
            leadingLeg = legA;
        }

        if (currentlyAttached == 0)
        {
            legAFootPos = new Vector3(_startingCell.x * gridXZDim, _startingCell.y * gridYDim, _startingCell.z * gridXZDim);
            legAFootRot = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            legBFootPos = new Vector3(_startingCell.x * gridXZDim, _startingCell.y * gridYDim, _startingCell.z * gridXZDim);
            legBFootRot = Quaternion.Euler(0, 180, 0);
        }
    }

    public void UpdateRobot()
    {
        UpdateReferenceTransforms();
        CarryOutMoves();
    }

    public void Rotate()
    {
        jointTargetList.Clear();
        jointTargetList.Add(robotGesture.GetGesture(rotateLeg, 0, 0, 4, 90, brickCurrentlyCarried));
    }

    public void TakeStep(int _stepLength, int _stepHeight, int _stepTurnAngle)
    {
        jointTargetList.Clear();

        if (_stepHeight == 0)
        {
            //set over attached leg
            jointTargetList.Add(robotGesture.GetGesture(setOverLeg, currentlyAttached, 0, _stepLength, 0, brickCurrentlyCarried));

            //lift leading leg
            if (!steppingDown && !steppingUp)
            {
                jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 0.88f, _stepLength, 0, brickCurrentlyCarried));
            }
            else if (steppingDown)
            {
                jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 1.25f, _stepLength, 0, brickCurrentlyCarried));
            }
            else if (steppingUp)
            {
                jointTargetList.Add(robotGesture.GetGesture(stepUpLegs, leadingLeg, 0.88f, _stepLength, 0, brickCurrentlyCarried));
            }

            //outstretch over leg
            jointTargetList.Add(robotGesture.GetGesture(outstretchLeg, leadingLeg, 1, _stepLength, _stepTurnAngle, brickCurrentlyCarried));

            //lower leading leg
            jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 0, _stepLength, 0, brickCurrentlyCarried));
                       
            steppingDown = false;
            steppingUp = false;
        }

        if (_stepHeight == -1)
        {
            //set over attached leg
            jointTargetList.Add(robotGesture.GetGesture(setOverLeg, currentlyAttached, 0, _stepLength, 0, brickCurrentlyCarried));

            //lift leading leg
            if (!steppingDown && !steppingUp)
            {
                jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 0.88f, _stepLength, 0, brickCurrentlyCarried));
            }
            else if (steppingDown)
            {
                jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 1.25f, _stepLength, 0, brickCurrentlyCarried));
            }
            else if (steppingUp)
            {
                jointTargetList.Add(robotGesture.GetGesture(stepUpLegs, leadingLeg, 0.88f, _stepLength, 0, brickCurrentlyCarried));
            }

            //outstretch over leg
            jointTargetList.Add(robotGesture.GetGesture(outstretchLeg, leadingLeg, 1, _stepLength, _stepTurnAngle, brickCurrentlyCarried));

            //lower leading leg
            jointTargetList.Add(robotGesture.GetGesture(stepDownLegs, leadingLeg, -1, _stepLength, 0, brickCurrentlyCarried));

            steppingDown = true;
            steppingUp = false;
        }

        if (_stepHeight == 1)
        {
            //set over attached leg
            jointTargetList.Add(robotGesture.GetGesture(setOverLeg, currentlyAttached, 0, _stepLength, 0, brickCurrentlyCarried));

            //lift leading leg
            if (!steppingDown && !steppingUp)
            {
                jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 1.25f, _stepLength, 0, brickCurrentlyCarried));
            }
            else if (steppingDown)
            {
                jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 1.25f, _stepLength, 0, brickCurrentlyCarried));
            }
            else if (steppingUp)
            {
                jointTargetList.Add(robotGesture.GetGesture(stepUpLegs, leadingLeg, 1.25f, _stepLength, 0, brickCurrentlyCarried));
            }

            //outstretch over leg
            jointTargetList.Add(robotGesture.GetGesture(outstretchLeg, leadingLeg, 1, _stepLength, _stepTurnAngle, brickCurrentlyCarried));

            //lower leading leg
            jointTargetList.Add(robotGesture.GetGesture(liftLeg, leadingLeg, 1, _stepLength, 0, brickCurrentlyCarried));

            steppingUp = true;
            steppingDown = false;
        }

        gestureCounter = 0;
        moveInProgress = true;
        stepInProgress = true;
    }

    void CarryOutMoves()
    {
        gestureInProgress = false;

        foreach (RobotJoint joint in allJoints)
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

        if (!gestureInProgress && !moveInProgress && stepInProgress)
        {
            SwitchLeadingLeg();
            SwitchCurrentlyAttached();
            stepInProgress = false;
        }

    }

    void MakeNextGesture()
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

    void RobotMove(float[] _jointTarget)
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

        //legARailSpeed
        if (_jointTarget[9] != -1)
        {
            legARailJoint.speed = _jointTarget[9] * railBasicSpeed;
        }

        //legBRailSpeed
        if (_jointTarget[10] != -1)
        {
            legBRailJoint.speed = _jointTarget[10] * railBasicSpeed;
        }

        //legCRailSpeed
        if (_jointTarget[11] != -1)
        {
            legCRailJoint.speed = _jointTarget[11] * railBasicSpeed;
        }

        foreach (RobotJoint joint in allJoints)
        {
            joint.SetLerpValues();
        }
    }

    void UpdateReferenceTransforms()
    {
        if (currentlyAttached == legA)
        {
            legAPos = legAFootPos;
            legARot = Quaternion.Euler(0, legARotationJoint.currentPos, 0) * legAFootRot;

            legAHipPos = legAPos + new Vector3(0, (legAVerticalJoint.currentPos - legAVerticalResetPos), 0);
            legAHipRot = legARot;

            mainBeamPos = legAHipPos + legARot * new Vector3(0, legAVerticalResetPos + 0.04f, (legARailJoint.currentPos - legCRailResetPos));
            mainBeamRot = legARot;

            legBHipPos = mainBeamPos - legARot * (new Vector3(0, legBVerticalResetPos + 0.04f, legBRailJoint.currentPos - legCRailResetPos));
            legBHipRot = legARot * Quaternion.Euler(0, 180, 0);

            legBPos = legBHipPos - new Vector3(0, legBVerticalJoint.currentPos - legBVerticalResetPos, 0);
            legBRot = legARot * Quaternion.Euler(0, 180, 0); ;

            legBFootPos = legBPos;
            legBFootRot = legARot * Quaternion.Euler(0, legBRotationJoint.currentPos, 0) * Quaternion.Euler(0, 180, 0);

            legCShoulderPos = mainBeamPos + legARot * (new Vector3(0, -0.04f, (legCRailJoint.currentPos - legCRailResetPos)));
            legCShoulderRot = legARot;

            legCFootPos = legCShoulderPos;
            legCFootRot = legCShoulderRot * Quaternion.Euler(0, -legCRotationJoint.currentPos / 10, 0);

            grip1Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, -(legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, (legCGripJoint.currentPos) * 0.00001f));
            grip2Rot = legCFootRot;
        }

        if (currentlyAttached == legB)
        {
            legBPos = legBFootPos;
            legBRot = Quaternion.Euler(0, legBRotationJoint.currentPos, 0) * legBFootRot;

            legBHipPos = legBPos + new Vector3(0, (legBVerticalJoint.currentPos - legBVerticalResetPos), 0);
            legBHipRot = legBRot;

            mainBeamPos = legBHipPos + legBRot * new Vector3(0, legBVerticalResetPos + 0.04f, (legCRailResetPos - legBRailJoint.currentPos));
            mainBeamRot = legBRot;

            legAHipPos = mainBeamPos - legBRot * (new Vector3(0, legAVerticalResetPos + 0.04f, legCRailResetPos - legARailJoint.currentPos));
            legAHipRot = legARot * Quaternion.Euler(0, 0, 0);

            legAPos = legAHipPos - new Vector3(0, legAVerticalJoint.currentPos - legAVerticalResetPos, 0);
            legARot = legBRot * Quaternion.Euler(0, 180, 0); ;

            legAFootPos = legAPos;
            legAFootRot = legBRot * Quaternion.Euler(0, legARotationJoint.currentPos, 0) * Quaternion.Euler(0, 180, 0);

            legCShoulderPos = mainBeamPos - legBRot * (new Vector3(0, +0.04f, (legCRailJoint.currentPos - legCRailResetPos)));
            legCShoulderRot = legBRot;

            legCFootPos = legCShoulderPos;
            legCFootRot = legCShoulderRot * Quaternion.Euler(0, (legCRotationJoint.currentPos / 10) + 180, 0);

            grip1Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, -(legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, (legCGripJoint.currentPos) * 0.00001f));
            grip2Rot = legCFootRot;
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

    //    void CarryOutMoves()
    //    {
    //        if (legARailJoint.JointNeedsToMove() ||
    //legAVerticalJoint.JointNeedsToMove() ||
    //legARotationJoint.JointNeedsToMove() ||
    //legBRailJoint.JointNeedsToMove() ||
    //legBVerticalJoint.JointNeedsToMove() ||
    //legCRailJoint.JointNeedsToMove() ||
    //legCGripJoint.JointNeedsToMove() ||
    //legCRotationJoint.JointNeedsToMove())
    //        {
    //            moveInProgress = true;
    //        }

    //        else
    //        {
    //            moveInProgress = false;
    //        }

    //        if (!moveInProgress && stepInProgress)
    //        {
    //            MakeMove();

    //        }

    //        foreach (RobotJoint joint in allJoints)
    //        {
    //            if (_withInterpolation)
    //            {
    //                joint.LerpJointPosition();
    //            }
    //            else
    //            {
    //                joint.currentPos = joint.targetPos;
    //            }
    //        }
    //    }
}

