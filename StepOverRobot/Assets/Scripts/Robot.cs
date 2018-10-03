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

    public Robot()
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
    }
}
