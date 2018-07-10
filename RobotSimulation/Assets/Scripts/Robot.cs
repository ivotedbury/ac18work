using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    const char legARail = 'A';
    const char legAVertical = 'B';
    const char legARotation = 'C';
    const char legBRail = 'D';
    const char legBVertical = 'E';
    const char legCRail = 'F';
    const char legCGrip = 'G';
    const char legCRotation = 'H';

    public int legARailCurrent;
    public int legAVerticalCurrent;
    public int legARotationCurrent;
    public int legBRailCurrent;
    public int legBVerticalCurrent;
    public int legCRailCurrent;
    public int legCGripCurrent;
    public int legCRotationCurrent;

    public int legARailTarget;
    public int legAVerticalTarget;
    public int legARotationTarget;
    public int legBRailTarget;
    public int legBVerticalTarget;
    public int legCRailTarget;
    public int legCGripTarget;
    public int legCRotationTarget;

    public float legARailStartTime;
    public float legAVerticalStartTime;
    public float legARotationStartTime;
    public float legBRailStartTime;
    public float legBVerticalStartTime;
    public float legCRailStartTime;
    public float legCGripStartTime;
    public float legCRotationStartTime;

    int legARailStartValue;
    int legAVerticalStartValue;
    int legARotationStartValue;
    int legBRailStartValue;
    int legBVerticalStartValue;
    int legCRailStartValue;
    int legCGripStartValue;
    int legCRotationStartValue;

    float legARailElapsedTime;
    float legAVerticalElapsedTime;
    float legARotationElapsedTime;
    float legBRailElapsedTime;
    float legBVerticalElapsedTime;
    float legCRailElapsedTime;
    float legCGripElapsedTime;
    float legCRotationElapsedTime;

    float legARailTimeForMove;
    float legAVerticalTimeForMove;
    float legARotationTimeForMove;
    float legBRailTimeForMove;
    float legBVerticalTimeForMove;
    float legCRailTimeForMove;
    float legCGripTimeForMove;
    float legCRotationTimeForMove;

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
    float legCGripSpeed = 2800;
    float legCRotationSpeed = 100;

    int legARailResetPos = 6125;
    int legAVerticalResetPos = 2150;
    int legARotationResetPos = 0;
    int legBRailResetPos = 3875;
    int legBVerticalResetPos = 2150;
    int legCRailResetPos = 5000;
    int legCGripResetPos = 3000;
    int legCRotationResetPos = 0;

    float verticalOffset = 0.124246f;

    public Vector3 legAFootPos;
    public Vector3 legAPos;
    public Vector3 legBFootPos;
    public Vector3 legBPos;
    public Vector3 mainBeamPos;
    public Vector3 legCPos;
    public Vector3 legCFootPos;
    public Vector3 verticalToHorizontalAPos;
    public Vector3 verticalToHorizontalBPos;
    public Vector3 grip1Pos;
    public Vector3 grip2Pos;

    public Quaternion legAFootRot;
    public Quaternion legARot;
    public Quaternion legBFootRot;
    public Quaternion legBRot;
    public Quaternion mainBeamRot;
    public Quaternion legCRot;
    public Quaternion legCFootRot;
    public Quaternion verticalToHorizontalARot;
    public Quaternion verticalToHorizontalBRot;
    public Quaternion grip1Rot;
    public Quaternion grip2Rot;

    float gridDimXZ = 0.05625f;
    float gridDimY = 0.0725f;

    public int grippedPos = 4800;

    public Vector3Int currentLegAGrid;
    public Vector3Int currentLegBGrid;
    public Vector3Int currentLegCGrid;

    public int currentlyAttached; // ABC
    public bool moveInProgress = false;
    public bool stepInProgress = false;
    public int moveCounter = 0;

    public float speedFactor = 10;

    List<int> legARailTargetList = new List<int>();
    List<int> legAVerticalTargetList = new List<int>();
    List<int> legARotationTargetList = new List<int>();
    List<int> legBRailTargetList = new List<int>();
    List<int> legBVerticalTargetList = new List<int>();
    List<int> legCRailTargetList = new List<int>();
    List<int> legCGripTargetList = new List<int>();
    List<int> legCRotationTargetList = new List<int>();

    List<int> legCRailMoveTypeList = new List<int>();
    List<int> currentlyConnectedList = new List<int>();

    public Robot(Vector3Int _startingPos, int _startingStance, int _currentlyAttached)
    {
        legARailCurrent = legARailResetPos;
        legAVerticalCurrent = legAVerticalResetPos;
        legARotationCurrent = legARotationResetPos;
        legBRailCurrent = legBRailResetPos;
        legBVerticalCurrent = legBVerticalResetPos;
        legCRailCurrent = legCRailResetPos;
        legCGripCurrent = legCGripResetPos;
        legCRotationCurrent = legCRotationResetPos;

        legARailTarget = legARailResetPos;
        legAVerticalTarget = legAVerticalResetPos;
        legARotationTarget = legARotationResetPos;
        legBRailTarget = legBRailResetPos;
        legBVerticalTarget = legBVerticalResetPos;
        legCRailTarget = legCRailResetPos;
        legCGripTarget = legCGripResetPos;
        legCRotationTarget = legCRotationResetPos;

        currentlyAttached = _currentlyAttached;

        if (currentlyAttached == 0)
        {
            legAFootPos = new Vector3(_startingPos.x * gridDimXZ, _startingPos.y * gridDimY, _startingPos.z * gridDimXZ);
            legAFootRot = Quaternion.identity;
        }
        if (currentlyAttached == 1)
        {
            legBFootPos = new Vector3(_startingPos.x * gridDimXZ, _startingPos.y * gridDimY, (_startingPos.z + _startingStance) * gridDimXZ);
            legBFootRot = Quaternion.identity;
        }
    }

    public void UpdateRobot()
    {
        CarryOutMoves();
        UpdateReferenceTransforms();
    }

    public void TakeStep(string stepDescription)
    {
        legARailTargetList.Clear();
        legAVerticalTargetList.Clear();
        legARotationTargetList.Clear();
        legBRailTargetList.Clear();
        legBVerticalTargetList.Clear();
        legCRailTargetList.Clear();
        legCGripTargetList.Clear();
        legCRotationTargetList.Clear();
        legCRailMoveTypeList.Clear();
        currentlyConnectedList.Clear();

        if (stepDescription == "Step along 4 lead A")
        {
            int[] legARailTargetValues = { 7250, 7250, 9500, 9500, 5000, 5000, 5000, 5000, 6125 };
            int[] legAVerticalTargetValues = { 2150, 1425, 1425, 2150, 2150, 2150, 2150, 2150, 2150 };
            int[] legARotationTargetValues = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] legBRailTargetValues = { 5000, 5000, 5000, 5000, 500, 500, 2750, 2750, 3875 };
            int[] legBVerticalTargetValues = { 2150, 2150, 2150, 2150, 2150, 1425, 1425, 2150, 2150 };
            int[] legCRailTargetValues = { 2750, 2750, 500, 500, 8870, 8870, 6935, 6935, 5000 };
            int[] legCGripTargetValues = { 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000 };
            int[] legCRotationTargetValues = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] legCRailMoveTypeValues = { 1, 1, 1, 1, 1, 1, 2, 2, 1 };
            int[] currentlyConnectedValues = { 1, 1, 1, 1, 1, 0, 0, 0, 0 };

            legARailTargetList.AddRange(legARailTargetValues);
            legAVerticalTargetList.AddRange(legAVerticalTargetValues);
            legARotationTargetList.AddRange(legARotationTargetValues);
            legBRailTargetList.AddRange(legBRailTargetValues);
            legBVerticalTargetList.AddRange(legBVerticalTargetValues);
            legCRailTargetList.AddRange(legCRailTargetValues);
            legCGripTargetList.AddRange(legCGripTargetValues);
            legCRotationTargetList.AddRange(legCRotationTargetValues);
            legCRailMoveTypeList.AddRange(legCRailMoveTypeValues);
            currentlyConnectedList.AddRange(currentlyConnectedValues);
        }

        else if (stepDescription == "Step along 4 lead B")
        {
            int[] legARailTargetValues = { 5000, 5000, 5000, 5000, 9500, 9500, 7250, 7250, 6125 };
            int[] legAVerticalTargetValues = { 2150, 2150, 2150, 2150, 2150, 1425, 1425, 2150, 2150 };
            int[] legARotationTargetValues = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] legBRailTargetValues = { 2750, 2750, 500, 500, 5000, 5000, 5000, 5000, 3876 };
            int[] legBVerticalTargetValues = { 2150, 1425, 1425, 2150, 2150, 2150, 2150, 2150, 2150 };
            int[] legCRailTargetValues = { 6935, 6935, 8870, 8870, 500, 500, 2750, 2750, 5000 };
            int[] legCGripTargetValues = { 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000 };
            int[] legCRotationTargetValues = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] legCRailMoveTypeValues = { 1, 1, 2, 2, 2, 2, 1, 1, 1 };
            int[] currentlyConnectedValues = { 0, 0, 0, 0, 1, 1, 1, 1, 1 };

            legARailTargetList.AddRange(legARailTargetValues);
            legAVerticalTargetList.AddRange(legAVerticalTargetValues);
            legARotationTargetList.AddRange(legARotationTargetValues);
            legBRailTargetList.AddRange(legBRailTargetValues);
            legBVerticalTargetList.AddRange(legBVerticalTargetValues);
            legCRailTargetList.AddRange(legCRailTargetValues);
            legCGripTargetList.AddRange(legCGripTargetValues);
            legCRotationTargetList.AddRange(legCRotationTargetValues);
            legCRailMoveTypeList.AddRange(legCRailMoveTypeValues);
            currentlyConnectedList.AddRange(currentlyConnectedValues);
        }

        stepInProgress = true;
        moveCounter = 0;
    }

    public void MakeMove()
    {
        RobotMove(legARailTargetList[moveCounter],
            legAVerticalTargetList[moveCounter],
            legARotationTargetList[moveCounter],
            legBRailTargetList[moveCounter],
            legBVerticalTargetList[moveCounter],
            legCRailTargetList[moveCounter],
            legCGripTargetList[moveCounter],
            legCRotationTargetList[moveCounter],
            legCRailMoveTypeList[moveCounter],
            currentlyConnectedList[moveCounter]);
        moveCounter++;

        if (moveCounter == legARailTargetList.Count)
        {
            stepInProgress = false;
        }
    }

    public void RobotMove(int _legARailTarget, int _legAVerticalTarget, int _legARotationTarget, int _legBRailTarget, int _legBVerticalTarget, int _legCRailTarget, int _legCGripTarget, int _legCRotationTarget, int _legCRailMoveType, int _currentlyAttached)
    {
        moveInProgress = true;

        currentlyAttached = _currentlyAttached;

        legARailTarget = _legARailTarget;
        legAVerticalTarget = _legAVerticalTarget;
        legARotationTarget = _legARotationTarget;
        legBRailTarget = _legBRailTarget;
        legBVerticalTarget = _legBVerticalTarget;
        legCRailTarget = _legCRailTarget;
        legCGripTarget = _legCGripTarget;
        legCRotationTarget = _legCRotationTarget;

        if (legARailCurrent != legARailTarget)
        {
            legARailStartValue = legARailCurrent;
            legARailStartTime = Time.time;
            legARailElapsedTime = 0;
            legARailTimeForMove = Mathf.Abs(legARailTarget - legARailStartValue) / (legARailSpeed * speedFactor);
        }

        if (legAVerticalCurrent != legAVerticalTarget)
        {
            legAVerticalStartValue = legAVerticalCurrent;
            legAVerticalStartTime = Time.time;
            legAVerticalElapsedTime = 0;
            legAVerticalTimeForMove = Mathf.Abs(legAVerticalTarget - legAVerticalStartValue) / (legAVerticalSpeed * speedFactor);
        }

        if (legARotationCurrent != legARotationTarget)
        {
            legARotationStartValue = legARotationCurrent;
            legARotationStartTime = Time.time;
            legARotationElapsedTime = 0;
            legARotationTimeForMove = Mathf.Abs(legARotationTarget - legARotationStartValue) / (legARotationSpeed * speedFactor);
        }

        if (legBRailCurrent != legBRailTarget)
        {
            legBRailStartValue = legBRailCurrent;
            legBRailStartTime = Time.time;
            legBRailElapsedTime = 0;
            legBRailTimeForMove = Mathf.Abs(legBRailTarget - legBRailStartValue) / (legBRailSpeed * speedFactor);
        }

        if (legBVerticalCurrent != legBVerticalTarget)
        {
            legBVerticalStartValue = legBVerticalCurrent;
            legBVerticalStartTime = Time.time;
            legBVerticalElapsedTime = 0;
            legBVerticalTimeForMove = Mathf.Abs(legBVerticalTarget - legBVerticalStartValue) / (legBVerticalSpeed * speedFactor);
        }

        if (legCRailCurrent != legCRailTarget)
        {
            legCRailStartValue = legCRailCurrent;
            legCRailStartTime = Time.time;
            legCRailElapsedTime = 0;
            if (_legCRailMoveType == 1)
            {
                legCRailTimeForMove = Mathf.Abs(legCRailTarget - legCRailStartValue) / (legCRailSpeedForLegA * speedFactor);
            }
            if (_legCRailMoveType == 2)
            {
                legCRailTimeForMove = Mathf.Abs(legCRailTarget - legCRailStartValue) / (legCRailSpeedForLegB * speedFactor);
            }
            if (_legCRailMoveType == 3)
            {
                legCRailTimeForMove = Mathf.Abs(legCRailTarget - legCRailStartValue) / (legCRailSpeedWithBrickForLegA * speedFactor);
            }
            if (_legCRailMoveType == 4)
            {
                legCRailTimeForMove = Mathf.Abs(legCRailTarget - legCRailStartValue) / (legCRailSpeedWithBrickForLegB * speedFactor);
            }
            else
            {
                legCRailTimeForMove = Mathf.Abs(legCRailTarget - legCRailStartValue) / (legCRailSpeed * speedFactor);
            }

        }

        if (legCGripCurrent != legCGripTarget)
        {
            legCGripStartValue = legAVerticalCurrent;
            legCGripStartTime = Time.time;
            legCGripElapsedTime = 0;
            legCGripTimeForMove = Mathf.Abs(legCGripTarget - legCGripStartValue) / (legCGripSpeed * speedFactor);
        }

        if (legCRotationCurrent != legCRotationTarget)
        {
            legCRotationStartValue = legCRotationCurrent;
            legCRotationStartTime = Time.time;
            legCRotationElapsedTime = 0;
            legCRotationTimeForMove = Mathf.Abs(legCRotationTarget - legCRotationStartValue) / (legCRotationSpeed * speedFactor);
        }
    }

    public void CarryOutMoves()
    {
        if (legARailCurrent != legARailTarget ||
    legAVerticalCurrent != legAVerticalTarget ||
   legARotationCurrent != legARotationTarget ||
   legBRailCurrent != legBRailTarget ||
   legBVerticalCurrent != legBVerticalTarget ||
   legCRailCurrent != legCRailTarget ||
   legCGripCurrent != legCGripTarget ||
   legCRotationCurrent != legCRotationTarget)
        {
            moveInProgress = true;
        }

        else
        {
            moveInProgress = false;
        }

        if (!moveInProgress && stepInProgress)
        {
            MakeMove();
        }

        if (legARailCurrent != legARailTarget)
        {
            float LegARailPercentage = (legARailElapsedTime * 100 / legARailTimeForMove) / 100;
            legARailCurrent = (int)Mathf.Lerp(legARailStartValue, legARailTarget, LegARailPercentage);
            legARailElapsedTime += Time.deltaTime;
        }

        if (legAVerticalCurrent != legAVerticalTarget)
        {
            float LegAVerticalPercentage = (legAVerticalElapsedTime * 100 / legAVerticalTimeForMove) / 100;
            legAVerticalCurrent = (int)Mathf.Lerp(legAVerticalStartValue, legAVerticalTarget, LegAVerticalPercentage);
            legAVerticalElapsedTime += Time.deltaTime;
        }

        if (legARotationCurrent != legARotationTarget)
        {
            float LegARotationPercentage = (legARotationElapsedTime * 100 / legARotationTimeForMove) / 100;
            legARotationCurrent = (int)Mathf.Lerp(legARotationStartValue, legARotationTarget, LegARotationPercentage);
            legARotationElapsedTime += Time.deltaTime;
        }

        if (legBRailCurrent != legBRailTarget)
        {
            float LegBRailPercentage = (legBRailElapsedTime * 100 / legBRailTimeForMove) / 100;
            legBRailCurrent = (int)Mathf.Lerp(legBRailStartValue, legBRailTarget, LegBRailPercentage);
            legBRailElapsedTime += Time.deltaTime;
        }

        if (legBVerticalCurrent != legBVerticalTarget)
        {
            float LegBVerticalPercentage = (legBVerticalElapsedTime * 100 / legBVerticalTimeForMove) / 100;
            legBVerticalCurrent = (int)Mathf.Lerp(legBVerticalStartValue, legBVerticalTarget, LegBVerticalPercentage);
            legBVerticalElapsedTime += Time.deltaTime;
        }

        if (legCRailCurrent != legCRailTarget)
        {
            float LegCRailPercentage = (legCRailElapsedTime * 100 / legCRailTimeForMove) / 100;
            legCRailCurrent = (int)Mathf.Lerp(legCRailStartValue, legCRailTarget, LegCRailPercentage);
            legCRailElapsedTime += Time.deltaTime;
        }

        if (legCGripCurrent != legCGripTarget)
        {
            float LegCGripPercentage = (legCGripElapsedTime * 100 / legCGripTimeForMove) / 100;
            legCGripCurrent = (int)Mathf.Lerp(legCGripStartValue, legCGripTarget, LegCGripPercentage);
            legCGripElapsedTime += Time.deltaTime;
        }

        if (legCRotationCurrent != legCRotationTarget)
        {
            float LegCRotationPercentage = (legCRotationElapsedTime * 100 / legCRotationTimeForMove) / 100;
            legCRotationCurrent = (int)Mathf.Lerp(legCRotationStartValue, legCRotationTarget, LegCRotationPercentage);
            legCRotationElapsedTime += Time.deltaTime;
        }


    }

    void UpdateReferenceTransforms()
    {
        if (currentlyAttached == 0)
        {
            legAPos = legAFootPos;
            legARot = Quaternion.Euler(0, legARotationCurrent / 10, 0) * legAFootRot;

            verticalToHorizontalAPos = legAPos + new Vector3(0, verticalOffset + (legAVerticalCurrent * 0.0001f), 0);
            verticalToHorizontalARot = legARot;

            mainBeamPos = verticalToHorizontalAPos + legARot * new Vector3(0, 0.06008f, (legARailCurrent * 0.0001f) - 0.5f);
            mainBeamRot = legARot;

            verticalToHorizontalBPos = mainBeamPos - legARot * (new Vector3(0, 0.06008f, legBRailCurrent * 0.0001f - 0.5f));
            verticalToHorizontalBRot = legARot;

            legBPos = verticalToHorizontalBPos - new Vector3(0, verticalOffset + (legBVerticalCurrent * 0.0001f), 0);
            legBRot = legARot;

            legBFootPos = legBPos;
            legBFootRot = legARot;

            legCPos = mainBeamPos + legARot * (new Vector3(0, -0.192f, -(legCRailCurrent * 0.0001f - 0.5f)));
            legCRot = legARot;

            legCFootPos = legCPos;
            legCFootRot = legCRot * Quaternion.Euler(0, legCRotationCurrent / 10, 0);

            grip1Pos = legCFootPos + (legCRot * new Vector3(0, 0, -(grippedPos - legCGripCurrent) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCRot * new Vector3(0, 0, (grippedPos - legCGripCurrent) * 0.00001f));
            grip2Rot = legCFootRot;
        }

        if (currentlyAttached == 1)
        {
            legBPos = legBFootPos;
            legBRot = legBFootRot;

            verticalToHorizontalBPos = legBPos + new Vector3(0, verticalOffset + (legBVerticalCurrent * 0.0001f), 0);
            verticalToHorizontalBRot = legBRot;

            mainBeamPos = verticalToHorizontalBPos + legBRot * new Vector3(0, 0.06008f, (legBRailCurrent * 0.0001f) - 0.5f);
            mainBeamRot = legBRot;

            verticalToHorizontalAPos = mainBeamPos - legBRot * (new Vector3(0, 0.06008f, (legARailCurrent * 0.0001f - 0.5f)));
            verticalToHorizontalARot = legBRot;

            legAPos = verticalToHorizontalAPos - new Vector3(0, verticalOffset + (legAVerticalCurrent * 0.0001f), 0);
            legARot = legBRot;

            legAFootPos = legAPos;
            legAFootRot = legARot * Quaternion.Euler(0, legARotationCurrent / 10, 0) * legAFootRot;

            legCPos = mainBeamPos + legBRot * (new Vector3(0, -0.192f, -(legCRailCurrent * 0.0001f - 0.5f)));
            legCRot = legBRot;

            legCFootPos = legCPos;
            legCFootRot = legCRot * Quaternion.Euler(0, legCRotationCurrent / 10, 0);

            grip1Pos = legCFootPos + (legCRot * new Vector3(0, 0, -(grippedPos - legCGripCurrent) * 0.00001f));
            grip1Rot = legCFootRot;

            grip2Pos = legCFootPos + (legCRot * new Vector3(0, 0, (grippedPos - legCGripCurrent) * 0.00001f));
            grip2Rot = legCFootRot;
        }
    }
}
