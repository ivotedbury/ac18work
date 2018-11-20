using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGesture
{
    //reset positions
    float legARailResetPos = 0.225f; //0.28125f;
    float legAVerticalResetPos = 0.2125f;
    float legARotationResetPos = 0;
    float legBRailResetPos = 0.45f; //0.50625f;
    float legBVerticalResetPos = 0.2125f;
    float legBRotationResetPos = 0;
    float legCRailResetPos = 0.3375f; //0.39375f;
    float legCRotationResetPos = 0;
    float legCGripResetPos = 0;

    float gripOpenDistance = 0.0125f;

    // grid dimensions
    float gridXZDim = 0.05625f;
    float gridYDim = 0.0725f;

    // leg types
    int legA = 0;
    int legB = 1;

    //brick type being carried
    int noBrick = 0;
    int fullBrick = 1;
    int halfBrick = 2;

    // speed factor between body and leg
    float brickFactor;
    float noBrickFactor = 0.5f / (0.5f + 6);
    float fullBrickFactor = 0.5f / (2.5f + 6);
    float halfBrickFactor = 0.5f / (1.5f + 6);

    // speed factor between body and gripper
    float brickFactorForBody;
    float noBrickFactorForBody = 6 / 0.5f;
    float fullBrickactorForBody = 6 / 2.5f;
    float halfBrickFactorForBody = 6 / 1.5f;

    void SetBrickFactor(int _brickType)
    {
        //set the brick factor to change the relative speeds between elements depending on which brick is being carried
        if (_brickType == noBrick)
        {
            brickFactor = noBrickFactor;
            brickFactorForBody = noBrickFactorForBody;
        }
        else if (_brickType == fullBrick)
        {
            brickFactor = fullBrickFactor;
            brickFactorForBody = fullBrickactorForBody;
        }
        else if (_brickType == halfBrick)
        {
            brickFactor = halfBrickFactor;
            brickFactorForBody = halfBrickFactorForBody;
        }
    }

    // here are all the gestures - return a list of joint positions for each set of inputs

    public float[] OutStretchGripper(int _legCurrentlyAttached, int _distanceInFront, int _distanceToSide, int _previousStepLength, int _brickCurrentlyCarried)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        SetBrickFactor(_brickCurrentlyCarried);

        float rotationAngle = RadianToDegree(Mathf.Atan2(_distanceToSide, _distanceInFront));
        float outstretchDistance = Mathf.Sqrt(Mathf.Pow(_distanceInFront, 2) + Mathf.Pow(_distanceToSide, 2));

        float gripperDisplacementFromEnd = 6 - ((1 + brickFactor) * outstretchDistance);

        if (_legCurrentlyAttached == legA)
        {
            _output[0] = (gripperDisplacementFromEnd + outstretchDistance) * gridXZDim;
            _output[3] = (gripperDisplacementFromEnd + outstretchDistance + _previousStepLength) * gridXZDim;
            _output[6] = (12 - gripperDisplacementFromEnd) * gridXZDim;

            _output[2] = rotationAngle;
            Debug.Log(rotationAngle);
        }

        return _output;
    }

    public float[] OpenGrip()
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        _output[8] = gripOpenDistance;

        return _output;
    }

    public float[] CloseGrip()
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        _output[8] = legCGripResetPos;

        return _output;
    }

    public float[] ExtendNozzle(float _distance, float _rotation)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        _output[9] = _distance;
        _output[10] = _rotation;

        return _output;
    }

    public float[] RetractNozzle()
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        _output[9] = 0;
        _output[10] = 0;

        return _output;
    }

    public float[] RotateGripper(float _angleToRotate)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        _output[7] = _angleToRotate;

        return _output;
    }

    public float[] LowerLegsToPlace(int _relativeBrickHeight, int _legCurrentlyAttached)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_legCurrentlyAttached == legA)
        {
            _output[1] = legAVerticalResetPos + ((_relativeBrickHeight - 1) * gridYDim) - 0.02675f; //
            _output[4] = legBVerticalResetPos + ((_relativeBrickHeight - 1.88f) * gridYDim) - 0.02675f; //
        }

        return _output;
    }

    public float[] LiftLegsAfterPlace(int _relativeBrickHeight, int _legCurrentlyAttached)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_legCurrentlyAttached == legA)
        {
            _output[1] = legAVerticalResetPos; //
            _output[4] = legBVerticalResetPos - (0.88f * gridYDim); //
        }

        return _output;
    }

    public float[] SetOverLeg(int _leg, int _legStance, int _brickCurrentlyCarried)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        SetBrickFactor(_brickCurrentlyCarried);

        if (_leg == legA)
        {
            _output[0] = (6 + (_legStance * brickFactor)) * gridXZDim;
            _output[3] = (6 + (_legStance + (_legStance * brickFactor))) * gridXZDim;
            _output[6] = 6 * gridXZDim;

            _output[12] = 1;
            _output[13] = 1;
            _output[14] = 1;
        }

        else if (_leg == legB)
        {
            _output[0] = (6 + (_legStance + (_legStance * brickFactor))) * gridXZDim;
            _output[3] = (6 + (_legStance * brickFactor)) * gridXZDim;
            _output[6] = 6 * gridXZDim;

            _output[12] = 1;
            _output[13] = 1;
            _output[14] = 1;
        }

        return _output;
    }

    public float[] LiftLeg(int _leg, float _legHeight)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_leg == legA)
        {
            _output[1] = legAVerticalResetPos - (_legHeight * gridYDim);
        }
        else if (_leg == legB)
        {
            _output[4] = legBVerticalResetPos - (_legHeight * gridYDim);
        }

        return _output;
    }

    public float[] OutStretchLeg(int _leg, int _legStretch, float _rotationAngle, int _brickCurrentlyCarried)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        SetBrickFactor(_brickCurrentlyCarried);

        if (_leg == legB)
        {
            _output[0] = (6 - (_legStretch * brickFactor)) * gridXZDim;
            _output[3] = (6 - (_legStretch + (_legStretch * brickFactor))) * gridXZDim;
            _output[6] = 6 * gridXZDim;

            _output[2] = _rotationAngle;
            _output[5] = 0;

            _output[12] = brickFactor;
            _output[13] = 1;
            _output[14] = brickFactor;
        }
        else if (_leg == legA)
        {
            _output[0] = (6 - (_legStretch + (_legStretch * brickFactor))) * gridXZDim;
            _output[3] = (6 - (_legStretch * brickFactor)) * gridXZDim;
            _output[6] = 6 * gridXZDim;

            _output[2] = 0;
            _output[5] = _rotationAngle;

            _output[12] = 1;
            _output[13] = brickFactor;
            _output[14] = brickFactor;
        }

        return _output;
    }

    public float[] StepDownLegs(int _leg, float _legHeight)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_leg == legA)
        {
            _output[1] = legAVerticalResetPos + ((_legHeight + 1) * gridYDim);
            _output[4] = legBVerticalResetPos + (_legHeight * gridYDim);

        }
        else if (_leg == legB)
        {
            _output[1] = legAVerticalResetPos + (_legHeight * gridYDim);
            _output[4] = legBVerticalResetPos + ((_legHeight + 1) * gridYDim);
        }

        return _output;
    }

    public float[] StepUpLegs(int _leg, float _legHeight)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        if (_leg == legA)
        {
            _output[1] = legAVerticalResetPos - (_legHeight * gridYDim);
            _output[4] = legBVerticalResetPos;

        }
        else if (_leg == legB)
        {
            _output[1] = legAVerticalResetPos;
            _output[4] = legBVerticalResetPos - (_legHeight * gridYDim);
        }

        return _output;
    }

    // extra functions

    private float RadianToDegree(float angle)
    {
        return (angle * (180 / Mathf.PI));
    }

    private float DegreeToRadian(float angle)
    {
        return (Mathf.PI * angle / 180);
    }
}
