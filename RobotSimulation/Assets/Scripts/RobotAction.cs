using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAction
{


    Robot robot;
    int action;
    //step
    int stepGradient;
    int stepSize;
    int leadLegForStep;
    int endStance;
    int turnAngle;
    //handling
    int relativeBrickHeight;
    int distanceInFront;
    int distanceToSide;
    int leadLegForPlacement;
    int gripRotation;
    Brick brickToMove;
    bool pickupMode;
    //heel posiiton
    int heelPosition;


    int changeFootAHeelPosition = 0;
    int handleBrick = 1;
    int takeStep = 2;

    public RobotAction(
        Robot _robot,
        int _action,
        //step
        int _stepGradient,
        int _stepSize,
        int _leadLegForStep,
        int _endStance,
        int _turnAngle,
        //handling
        int _relativeBrickHeight,
        int _distanceInFront,
        int _distanceToSide,
        int _leadLegForPlacement,
        int _gripRotation,
        Brick _brickToMove,
        bool _pickupMode,
        //heel posiiton
        int _heelPosition
       )
    {
        robot = _robot;
        action = _action;
        //step
        stepGradient = _stepGradient;
        stepSize = _stepSize;
        leadLegForStep = _leadLegForStep;
        endStance = _endStance;
        turnAngle = _turnAngle;
        //handling
        relativeBrickHeight = _relativeBrickHeight;
        distanceInFront = _distanceInFront;
        distanceToSide = _distanceToSide;
        leadLegForPlacement = _leadLegForPlacement;
        gripRotation = _gripRotation;
        brickToMove = _brickToMove;
        pickupMode = _pickupMode;
        //heel posiiton
        heelPosition = _heelPosition;
    }

    public void PerformAction()
    {
        if (action == changeFootAHeelPosition)
        {
            robot.ChangeFootAHeelPosition(heelPosition);
        }

        if (action == handleBrick)
        {
            robot.HandleBrick(relativeBrickHeight, distanceInFront, distanceToSide, leadLegForPlacement, gripRotation, brickToMove, pickupMode);
        }

        if (action == takeStep)
        {
            robot.TakeStep(stepGradient, stepSize, leadLegForStep, endStance, turnAngle);
        }

    }
}
