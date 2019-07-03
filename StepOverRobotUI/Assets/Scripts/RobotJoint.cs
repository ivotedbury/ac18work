using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint
{

    //general
    public float speed;
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
    float speedFactor = 1f;

    public RobotJoint(float _jointSpeed, float _jointResetPos)
    {
        SetupJoint(_jointSpeed, _jointResetPos);
    }

    public bool JointNeedsToMove()
    {
        // if the joint needs to move, return true

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
        // interpolate between the current and target joint positions

        if (currentPos != targetPos)
        {
            float progress = elapsedTime / timeForMove;
            currentPos = Mathf.Lerp(startPos, targetPos, progress);
            elapsedTime += Time.deltaTime;
        }
    }

    public void SetLerpValues()
    {
        // if a move is required, find the amount of time it should take. 

        if (targetPos != currentPos)
        {
            distanceToMove = Mathf.Abs(targetPos - currentPos);

            startPos = currentPos;
            startTime = overallTime;
            elapsedTime = 0;
            timeForMove = Mathf.Abs(targetPos - startPos) / (speed * speedFactor);
        }
    }

    void SetupJoint(float _speed, float _resetPos)
    {
        speed = _speed;
        resetPos = _resetPos;
        currentPos = resetPos;
        targetPos = currentPos;
    }
}
