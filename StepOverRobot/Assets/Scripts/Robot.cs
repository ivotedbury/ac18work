using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    float legARailResetPos = 0.28125f;
    float legAVerticalResetPos = 0.18850f;
    float legARotationResetPos = 0;
    float legBRailResetPos = 0.50625f;
    float legBVerticalResetPos = 0.18850f;
    float legBRotationResetPos = 0;
    float legCRailResetPos = 0.39375f;
    float legCRotationResetPos = 0;
    float legCGripResetPos = 0;


    List<RobotJoint> allJoints = new List<RobotJoint>();

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

    int currentlyAttached;

    public Robot(Vector3Int _startingCell, int _currentlyAttached, int _startingStance)
    {
        RobotJoint legARailJoint = new RobotJoint(100, legARailResetPos);
        RobotJoint legAVerticalJoint = new RobotJoint(100, legAVerticalResetPos);
        RobotJoint legARotationJoint = new RobotJoint(100, legARotationResetPos);
        RobotJoint legBRailJoint = new RobotJoint(100, legBRailResetPos);
        RobotJoint legBVerticalJoint = new RobotJoint(100, legBVerticalResetPos);
        RobotJoint legBRotationJoint = new RobotJoint(100, legBRotationResetPos);
        RobotJoint legCRailJoint = new RobotJoint(100, legCRailResetPos);
        RobotJoint legCRotationJoint = new RobotJoint(100, legCRotationResetPos);
        RobotJoint legCGripJoint = new RobotJoint(100, legCGripResetPos);

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

        if (currentlyAttached == 0)
        {
            legAFootPos = new Vector3Int(0, 0, 0);
        }
        else
        {
            legBFootPos = new Vector3Int(0, 0, 0);
        }
    }

    public void UpdateRobot()
    {
       // CarryOutMoves();
        UpdateReferenceTransforms();
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

