using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGesture
{
    int legA = 0;
    int legB = 1;
    int legC = 2;

    int rotateLeg = 0;
    int liftLeg = 1;
    int setOverLeg = 2;
    int outstretchLeg = 3;

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
    float gridYDim = 0.0725f;

    int noBrick = 0;
    int fullBrick = 1;
    int halfBrick = 2;

    float brickFactor;
    float noBrickFactor = 0.5f / 0.5f;
    float fullBrickFactor = 0.5f / 2.5f;
    float halfBrickFactor = 0.5f / 1.5f;

    public float[] GetGesture(int _type, int _leg, int _legHeight, int _legStance, float _rotationAngle, int _brickCurrentlyCarried)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        //set the brick factor
        if (_brickCurrentlyCarried == noBrick)
        {
            brickFactor = noBrickFactor;
        }
        else if (_brickCurrentlyCarried == fullBrick)
        {
            brickFactor = fullBrickFactor;
        }
        else if (_brickCurrentlyCarried == halfBrick)
        {
            brickFactor = halfBrickFactor;
        }

        //rotate type
        if (_type == rotateLeg)
        {
            if (_leg == legA)
            {
                _output[2] = _rotationAngle;
            }
            else if (_leg == legB)
            {
                _output[5] = _rotationAngle;
            }
            else
            {
                _output[7] = _rotationAngle;
            }
        }

        // lift leg type
        if (_type == liftLeg)
        {
            if (_leg == legA)
            {
                _output[1] = legAVerticalResetPos - (_legHeight * gridYDim);
                _output[2] = 0;
            }
            else if (_leg == legB)
            {
                _output[4] = legBVerticalResetPos - (_legHeight * gridYDim);
               _output[5] = 0;
            }
        }

        // set over leg type
        if (_type == setOverLeg)
        {
            if (_leg == legA)
            {
                _output[0] = 7 * gridXZDim;
                _output[3] = (7 + _legStance) * gridXZDim;
                _output[6] = (7 + (_legStance * brickFactor)) * gridXZDim;
            }
            else if (_leg == legB)
            {
                _output[0] = (7 + _legStance) * gridXZDim;
                _output[3] = 7 * gridXZDim;
                _output[6] = (7 + (_legStance * brickFactor)) * gridXZDim;
            }
        }

        if (_type == outstretchLeg)
        {
            if (_leg == legB)
            {
                _output[0] = 7 * gridXZDim;
                _output[3] = (7 - _legStance) * gridXZDim;
                _output[6] = (7 - (_legStance * brickFactor)) * gridXZDim;

                _output[2] = _rotationAngle;
            }
            else if (_leg == legA)
            {
                _output[0] = (7 - _legStance) * gridXZDim;
                _output[3] = 7 * gridXZDim;
                _output[6] = (7 - (_legStance * brickFactor)) * gridXZDim;

                _output[5] = _rotationAngle;
            }
        }

        // outstretch leg type



        return _output;
    }
}
