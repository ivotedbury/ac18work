using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint
{

    char thisJoint;

    //general
    float speed;
    int resetPos;
    public int currentPos;
    public int targetPos;
    public int speedMode = 0;

    //for lerp
    float startPos;
    float startTime;
    float elapsedTime;
    float timeForMove;

    //overall constants
    const char legARail = 'A';
    const char legAVertical = 'B';
    const char legARotation = 'C';
    const char legBRail = 'D';
    const char legBVertical = 'E';
    const char legCRail = 'F';
    const char legCGrip = 'G';
    const char legCRotation = 'H';

    //speeds
    float legARailSpeed = 2800;
    float legAVerticalSpeed = 500;
    float legARotationSpeed = 100;
    float legBRailSpeed = 2800;
    float legBVerticalSpeed = 500;
    float legCRailSpeed = 2800;
    float legCRailSpeedForLegA = 2800;
    float legCRailSpeedForLegB = 2408;
    float legCRailSpeedWithBrickForLegA = 2800;/////////////////not set!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    float legCRailSpeedWithBrickForLegB = 2800;/////////////////not set!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    float legCRailSpeedWithHalfBrickForLegA = 2800;/////////////////not set!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    float legCRailSpeedWithHalfBrickForLegB = 2800;/////////////////not set!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    float legCGripSpeed = 2800;
    float legCRotationSpeed = 100;

    //reset positions
    int legARailResetPos = 6125;
    int legAVerticalResetPos = 2150;
    int legARotationResetPos = 0;
    int legBRailResetPos = 3875;
    int legBVerticalResetPos = 2150;
    int legCRailResetPos = 5000;
    int legCGripResetPos = 3000;
    int legCRotationResetPos = 0;

    public RobotJoint(char _thisJoint)
    {
        thisJoint = _thisJoint;

        SetupJoint();
    }

    public bool JointNeedsToMove()
    {
        bool jointNeedsToMove;

        if (targetPos != currentPos)
        {
            jointNeedsToMove = true;
        }
        else
        {
            jointNeedsToMove = false;
        }

        return jointNeedsToMove;
    }

    public void LerpJointPosition()
    {
        if (currentPos != targetPos)
        {
            float progress = elapsedTime / timeForMove;
            currentPos = (int)Mathf.Lerp(startPos, targetPos, progress);
            elapsedTime += Time.deltaTime;
        }
    }

    public void SetLerpValues(int legCRailMoveType)
    {
        if (thisJoint == legCRail)
        {
            if (legCRailMoveType == 0)
            {
                speed = legCRailSpeed;
            }
            else if (legCRailMoveType == 1)
            {
                speed = legCRailSpeedForLegA;
            }
            else if (legCRailMoveType == 2)
            {
                speed = legCRailSpeedForLegB;
            }
            else if (legCRailMoveType == 3)
            {
                speed = legCRailSpeedWithBrickForLegA;
            }
            else if (legCRailMoveType == 4)
            {
                speed = legCRailSpeedWithBrickForLegB;
            }
            else if (legCRailMoveType == 5)
            {
                speed = legCRailSpeedWithHalfBrickForLegA;
            }
            else if (legCRailMoveType == 6)
            {
                speed = legCRailSpeedWithHalfBrickForLegB;
            }
        }

        if (targetPos != currentPos)
        {
            startPos = currentPos;
            startTime = Time.time;
            elapsedTime = 0;
            timeForMove = Mathf.Abs(targetPos - startPos) / speed;
        }
    }

    void SetupJoint()
    {
        if (thisJoint == legARail)
        {
            speed = legARailSpeed;
            resetPos = legARailResetPos;
        }
        if (thisJoint == legAVertical)
        {
            speed = legAVerticalSpeed;
            resetPos = legAVerticalResetPos;
        }
        if (thisJoint == legARotation)
        {
            speed = legARotationSpeed;
            resetPos = legARotationResetPos;
        }
        if (thisJoint == legBRail)
        {
            speed = legBRailSpeed;
            resetPos = legBRailResetPos;
        }
        if (thisJoint == legBVertical)
        {
            speed = legBVerticalSpeed;
            resetPos = legBVerticalResetPos;
        }
        if (thisJoint == legCRail)
        {
            speed = legCRailSpeed;
            resetPos = legCRailResetPos;
        }
        if (thisJoint == legCGrip)
        {
            speed = legCGripSpeed;
            resetPos = legCGripResetPos;
        }
        if (thisJoint == legCRotation)
        {
            speed = legCRotationSpeed;
            resetPos = legCRotationResetPos;
        }

        currentPos = resetPos;
    }
}
