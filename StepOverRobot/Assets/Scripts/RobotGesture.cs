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

    public float[] GetGesture(int _type, int _leg, int _legHeight, float _rotationAngle)
    {
        float[] _output = { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

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

        return _output;
    }
}
