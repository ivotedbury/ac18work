using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint
{

    char thisJoint;

    //general
    float speed;
    public float resetPos;
    public float currentPos;
    public float targetPos;

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
    float legARotationSpeed = 500;
    float legBRailSpeed = 2800;
    float legBVerticalSpeed = 500;
    float legCRailSpeed = 2800;
    float legCRailSpeedForLegA; //1
    float legCRailSpeedForLegB; //2
    float legCRailSpeedWithBrickForLegA; //3
    float legCRailSpeedWithBrickForLegB; //4
    float legCRailSpeedWithHalfBrickForLegA; //5
    float legCRailSpeedWithHalfBrickForLegB; //6
    float legCGripSpeed = 2800;
    float legCRotationSpeed = 500;

    //reset positions
    int legARailResetPos = 6125;
    int legAVerticalResetPos = 2150;
    int legARotationResetPos = 0;
    int legBRailResetPos = 3875;
    int legBVerticalResetPos = 2150;
    int legCRailResetPos = 5000;
    int legCGripResetPos = 3000;
    int legCRotationResetPos = 0;

    //relative weights
    float legAWeight = 3700;
    float legBWeight = 3050;
    float legCWeight = 3750;
    float legCWithFullBrickWeight = 5750;
    float legCWithHalfBrickWeight = 4650;

    //relative speed and distance factors
    public float normalLegCFactor;
    public float normalLegAFactor;
    public float normalLegBFactor;
    public float fullBrickLegAFactor;
    public float fullBrickLegBFactor;
    public float halfBrickLegAFactor;
    public float halfBrickLegBFactor;

    float speedFactor = 2f;
    float overallTime = 0;

    public RobotJoint(char _thisJoint)
    {
        thisJoint = _thisJoint;

        SetRelativeFactorsAndSpeeds();
        SetupJoint();
    }

    void SetRelativeFactorsAndSpeeds()
    {
        normalLegCFactor = 1;
        normalLegAFactor = legAWeight / legCWeight;
        normalLegBFactor = legBWeight / legCWeight;
        fullBrickLegAFactor = legAWeight / legCWithFullBrickWeight;
        fullBrickLegBFactor = legBWeight / legCWithFullBrickWeight;
        halfBrickLegAFactor = legAWeight / legCWithHalfBrickWeight;
        halfBrickLegBFactor = legBWeight / legCWithHalfBrickWeight;

        legCRailSpeedForLegA = legCRailSpeed * normalLegAFactor;
        legCRailSpeedForLegB = legCRailSpeed * normalLegBFactor;
        legCRailSpeedWithBrickForLegA = legCRailSpeed * fullBrickLegAFactor;
        legCRailSpeedWithBrickForLegB = legCRailSpeed * fullBrickLegBFactor;
        legCRailSpeedWithHalfBrickForLegA = legCRailSpeed * halfBrickLegAFactor;
        legCRailSpeedWithHalfBrickForLegB = legCRailSpeed * halfBrickLegBFactor;
    }

    public bool JointNeedsToMove()
    {
        overallTime += Time.deltaTime;

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
            currentPos = Mathf.Lerp(startPos, targetPos, progress);
            elapsedTime += Time.deltaTime;
        }
    }

    public float GetTimeForMove()
    {
        return timeForMove;
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
            startTime = overallTime;//Time.time
            elapsedTime = 0;
            timeForMove = Mathf.Abs(targetPos - startPos) / (speed * speedFactor);
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
        targetPos = currentPos;
    }
}
