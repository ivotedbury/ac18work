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

    float gridXZDim = 0.05625f;
    float gridYDim = 0.0625f;

    List<RobotJoint> allJoints = new List<RobotJoint>();
    RobotGesture robotGesture = new RobotGesture();
    List<float[]> jointTargetList = new List<float[]>();
    bool moveInProgress = false;

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

    int currentlyAttached;
    int leadingLeg;

    public Robot(Vector3Int _startingCell, int _currentlyAttached, int _leadingLeg, int _startingStance)
    {
        legARailJoint = new RobotJoint(100, legARailResetPos);
        legAVerticalJoint = new RobotJoint(100, legAVerticalResetPos);
        legARotationJoint = new RobotJoint(100, legARotationResetPos);
        legBRailJoint = new RobotJoint(100, legBRailResetPos);
        legBVerticalJoint = new RobotJoint(100, legBVerticalResetPos);
        legBRotationJoint = new RobotJoint(100, legBRotationResetPos);
        legCRailJoint = new RobotJoint(100, legCRailResetPos);
        legCRotationJoint = new RobotJoint(100, legCRotationResetPos);
        legCGripJoint = new RobotJoint(100, legCGripResetPos);

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
        leadingLeg = _leadingLeg;

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

        Rotate();
    }

    public void UpdateRobot()
    {
        UpdateReferenceTransforms();
        CarryOutMoves();
    }

    public void Rotate()
    {
        jointTargetList.Clear();
        jointTargetList.Add(robotGesture.GetGesture("rotateLeg", 1, 90));
        RobotMove(jointTargetList[0]);
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

        foreach (RobotJoint joint in allJoints)
        {
            joint.SetLerpValues();
        }
    }

    void CarryOutMoves()
    {
        foreach (RobotJoint joint in allJoints)
        {
            if (joint.JointNeedsToMove())
            {
                moveInProgress = true;
            }
            else
            {
                moveInProgress = false;
            }

            joint.LerpJointPosition();
        }
    }

    void UpdateReferenceTransforms()
    {
        if (currentlyAttached == 0)
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

        if (currentlyAttached == 1)
        {
            legBPos = legBFootPos;
            legBRot = Quaternion.Euler(0, legBRotationJoint.currentPos, 0) * legBFootRot;

            legBHipPos = legBPos + new Vector3(0, (legBVerticalJoint.currentPos - legBVerticalResetPos), 0);
            legBHipRot = legBRot;

            mainBeamPos = legBHipPos + legBRot * new Vector3(0, legBVerticalResetPos + 0.04f, (legCRailResetPos - legBRailJoint.currentPos));
            mainBeamRot = legBRot;

            legAHipPos = mainBeamPos - legBRot * (new Vector3(0, legAVerticalResetPos + 0.04f, legCRailResetPos - legARailJoint.currentPos));
            legAHipRot = legARot * Quaternion.Euler(0, 180, 0);

            legAPos = legAHipPos - new Vector3(0, legAVerticalJoint.currentPos - legAVerticalResetPos, 0);
            legARot = legBRot * Quaternion.Euler(0, 180, 0); ;

            legAFootPos = legAPos;
            legAFootRot = legBRot * Quaternion.Euler(0, legBRotationJoint.currentPos, 0) * Quaternion.Euler(0, 180, 0);

            legCShoulderPos = mainBeamPos + legBRot * (new Vector3(0, -0.04f, (legCRailJoint.currentPos - legCRailResetPos)));
            legCShoulderRot = legBRot;

            legCFootPos = legCShoulderPos;
            legCFootRot = legCShoulderRot * Quaternion.Euler(0, -legCRotationJoint.currentPos / 10, 0);

            grip1Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, -(legCGripJoint.currentPos) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCShoulderRot * new Vector3(0, 0, (legCGripJoint.currentPos) * 0.00001f));
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

