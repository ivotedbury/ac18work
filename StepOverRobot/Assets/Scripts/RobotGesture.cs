using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGesture
{
      int legA = 0;
    int legB = 1;
    int legC = 2;

    string rotateLeg = "rotateLeg";

    public float[] GetGesture(string _type, int _leg, float _rotationAngle)
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

        return _output;
    }
}
