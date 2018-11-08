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
    int stepDownLegs = 4;
    int stepUpLegs = 5;
    int goToStance = 6;

    float legARailResetPos = 0.225f; //0.28125f;
    float legAVerticalResetPos = 0.18125f;
    float legARotationResetPos = 0;
    float legBRailResetPos = 0.45f; //0.50625f;
    float legBVerticalResetPos = 0.18125f;
    float legBRotationResetPos = 0;
    float legCRailResetPos = 0.3375f; //0.39375f;
    float legCRotationResetPos = 0;
    float legCGripResetPos = 0;

    float gridXZDim = 0.05625f;
    float gridYDim = 0.0725f;

    int noBrick = 0;
    int fullBrick = 1;
    int halfBrick = 2;

    float brickFactor;
    float noBrickFactor = 0.5f / (0.5f + 6);
    float fullBrickFactor = 0.5f / (2.5f + 6);
    float halfBrickFactor = 0.5f / (1.5f + 6);

    public float[] GetGesture(int _type, int _leg, float _legHeight, int _legStance, float _rotationAngle, int _brickCurrentlyCarried)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

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
            }
            else if (_leg == legB)
            {
                _output[4] = legBVerticalResetPos - (_legHeight * gridYDim);
            }
        }

        // step down legs type
        if (_type == stepDownLegs)
        {
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
        }

        // step up legs type
        if (_type == stepUpLegs)
        {
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
        }

        // set over leg type
        if (_type == setOverLeg)
        {
            if (_leg == legA)
            {
                _output[0] = (6 + (_legStance * brickFactor)) * gridXZDim;
                _output[3] = (6 + (_legStance + (_legStance * brickFactor))) * gridXZDim;
                _output[6] = 6 * gridXZDim;

                _output[9] = 1;
                _output[10] = 1;
                _output[11] = 1;
            }

            else if (_leg == legB)
            {
                _output[0] = (6 + (_legStance + (_legStance * brickFactor))) * gridXZDim;
                _output[3] = (6 + (_legStance * brickFactor)) * gridXZDim;
                _output[6] = 6 * gridXZDim;

                _output[9] = 1;
                _output[10] = 1;
                _output[11] = 1;
            }
        }

        if (_type == outstretchLeg)
        {
            if (_leg == legB)
            {
                _output[0] = (6 - (_legStance * brickFactor)) * gridXZDim;
                _output[3] = (6 - (_legStance + (_legStance * brickFactor))) * gridXZDim;
                _output[6] = 6 * gridXZDim;

                _output[2] = _rotationAngle;
                _output[5] = 0;

                _output[9] = brickFactor;
                _output[10] = 1;
                _output[11] = brickFactor;
            }
            else if (_leg == legA)
            {
                _output[0] = (6 - (_legStance + (_legStance * brickFactor))) * gridXZDim;
                _output[3] = (6 - (_legStance * brickFactor)) * gridXZDim;
                _output[6] = 6 * gridXZDim;

                _output[2] = 0;
                _output[5] = _rotationAngle;

                _output[9] = 1;
                _output[10] = brickFactor;
                _output[11] = brickFactor;
            }
        }

        // outstretch leg type



        return _output;
    }
}
