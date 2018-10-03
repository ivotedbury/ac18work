using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint
{

    //general
    float speed;
    public float resetPos;
    public float currentPos;
    public float targetPos;

    //for lerp
    float startPos;
    float startTime;
    float elapsedTime;
    public float timeForMove;
    public float distanceToMove;

    float overallTime = 0;

    public RobotJoint(float _jointSpeed, float _jointResetPos)
    {
        SetupJoint(_jointSpeed, _jointResetPos);
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

    public void SetLerpValues(int legCRailMoveType)
    {
        if (targetPos != currentPos)
        {
            distanceToMove = Mathf.Abs(targetPos - currentPos);

            startPos = currentPos;
            startTime = overallTime;//Time.time
            elapsedTime = 0;
            timeForMove = Mathf.Abs(targetPos - startPos) / speed;
        }
    }
    void SetupJoint(float _speed, float _resetPos)
    {
        speed = _speed;
        resetPos = _resetPos;
    }
}
