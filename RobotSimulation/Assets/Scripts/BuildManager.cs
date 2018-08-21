using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager
{
    public BrickStructure brickStructure;
    public List<Robot> allRobots = new List<Robot>();

    public int startingBricks;

    public int nextBrickToPlace;

    const int changeFootAHeelPosition = 0;
    const int handleBrick = 1;
    const int takeStep = 2;

    int actionCounter = 0;
    public bool readyForNextBrick = true;
    public bool buildComplete = false;

    List<RobotAction> robotActions = new List<RobotAction>();

    public List<Cell> outwardPath;
    public List<Cell> returnPath;

    //data for the build
    public List<float> tripTimeOut = new List<float>();
    public List<float> tripTimeBack = new List<float>();

    public List<float> tripCostOut = new List<float>();
    public List<float> tripCostReturn = new List<float>();

    public List<float> pathCountOut = new List<float>();
    public List<float> pathCountBack = new List<float>();
    public List<float> climbingOut = new List<float>();
    public List<float> climbingBack = new List<float>();
    public List<float> climbingOutAverage = new List<float>();
    public List<float> climbingBackAverage = new List<float>();
    public List<float> distanceOut = new List<float>();
    public List<float> distanceBack = new List<float>();
    public List<float> distanceOutAverage = new List<float>();
    public List<float> distanceBackAverage = new List<float>();

    public bool analysisMode = false;
    public List<RobotAction> allRobotActions = new List<RobotAction>();

    public BuildManager(Vector3Int _gridSize, Vector3Int _seedPosition, TextAsset _brickImportData, int _startingBricks, int _numberOfRobots)
    {
        brickStructure = new BrickStructure(_gridSize, _seedPosition, _brickImportData);
        startingBricks = _startingBricks;
        nextBrickToPlace = startingBricks;
        // was bricksInTargetStructure
        for (int i = 0; i < startingBricks; i++)
        {
            brickStructure.bricksInPlace.Add(brickStructure.bricksInTargetStructure[i]);
        }

        allRobots.Add(new Robot(brickStructure.bricksInTargetStructure[0], 4, 0, true));

        // robotActions.Add(new RobotAction(allRobots[0], 2, 0, 8, 0, 4, 180, 0, 0, 0, 0, 0, null, false, 0));

    }

    public void Update()
    {
        if (buildComplete == false)
        {
            if (readyForNextBrick)
            {
                actionCounter = 0;
                PlaceNextBrick();
                readyForNextBrick = false;
            }

            if (actionCounter < robotActions.Count)
            {
                if (!allRobots[0].moveInProgress)
                {
                    robotActions[actionCounter].PerformAction();
                    actionCounter++;
                }
            }
            else
            {
                brickStructure.bricksInPlace.Add(brickStructure.bricksInTargetStructure[nextBrickToPlace]);
                nextBrickToPlace++;
                readyForNextBrick = true;
            }

            if (nextBrickToPlace == brickStructure.bricksInTargetStructure.Count)
            {
                buildComplete = true;
            }
        }
    }

    public void PlaceNextBrick()
    {
        robotActions.Clear();

        brickStructure.UpdateAvailableCells();

        GeneratePath();

        robotActions = ConvertPath(outwardPath, returnPath);

        if (analysisMode)
        {
            foreach (RobotAction action in robotActions)
            {
                allRobots[0].GetTimeForAction();
            }
        }

        allRobotActions.AddRange(robotActions);
    }

    void ProcessPath(List<Cell> _pathToProcess, bool _isOutward)
    {
        float pathLength = _pathToProcess.Count - 1;
        float averageStepDistance = 0;
        float totalDistance = 0;
        float averageHeightChange = 0;
        float totalHeightChange = 0;

        for (int i = 1; i < _pathToProcess.Count; i++)
        {
            totalDistance += Mathf.Abs((_pathToProcess[i].position.x - _pathToProcess[i - 1].position.x) + (_pathToProcess[i].position.z - _pathToProcess[i - 1].position.z));
            totalHeightChange += _pathToProcess[i].position.y - _pathToProcess[i - 1].position.y;
        }

        averageStepDistance = totalDistance / pathLength;
        averageHeightChange = totalHeightChange / pathLength;

        if (_isOutward)
        {
            pathCountOut.Add(pathLength);
            distanceOut.Add(totalDistance);
            distanceOutAverage.Add(averageStepDistance);
            climbingOut.Add(totalHeightChange);
            climbingOutAverage.Add(averageHeightChange);
        }
        else
        {
            pathCountBack.Add(pathLength);
            distanceBack.Add(totalDistance);
            distanceBackAverage.Add(averageStepDistance);
            climbingBack.Add(totalHeightChange);
            climbingBackAverage.Add(averageHeightChange);
        }
    }

    public void GeneratePath()
    {
        Cell dropOffPoint = brickStructure.FindDropOffCell(brickStructure.bricksInTargetStructure[nextBrickToPlace], brickStructure.availableCells);

        outwardPath = brickStructure.FindPathOneWay(brickStructure.bricksInPlace[1].originCell, dropOffPoint, 1);
        tripCostOut.Add(brickStructure.pathFinder.totalCostOfTrip);
        ProcessPath(outwardPath, true);

        returnPath = brickStructure.FindPathOneWay(dropOffPoint, brickStructure.bricksInPlace[0].originCell, 3);
        tripCostReturn.Add(brickStructure.pathFinder.totalCostOfTrip);
        ProcessPath(returnPath, false);

        foreach (Cell anyCell in brickStructure.grid.cellsList)
        {
            anyCell.currentStatus = 0;
        }

        foreach (Cell cellOnPath in outwardPath)
        {
            cellOnPath.currentStatus = 2;
        }
        foreach (Cell cellOnPath in returnPath)
        {
            cellOnPath.currentStatus = 2;
        }
    }

    List<RobotAction> ConvertPath(List<Cell> _outwardPath, List<Cell> _returnPath)
    {
        List<RobotAction> actionsForPath = new List<RobotAction>();
        Vector3Int nextStep;
        Vector3Int nextStepCompanionDirections;
        Vector3Int nextStepCompanionDirectionsPrevious;
        Vector3Int placementDirections;
        Cell nextCompanionCell;
        int leadLeg;
        int heelPosition;
        bool backWeight = true;
        int currentDirectionOfTravel;
        int previousDirectionOfTravel = 0;
        int robotOrientation = 0;
        int previousRobotOrientation;
        int turnAngle = 0;
        bool immediateTurnRequired = false;
        bool afterImmediateTurn = false;
        int lateTurnAngle = 0;

        // PICKUP
        leadLeg = 0;
        heelPosition = 0;
        actionsForPath.Add(new RobotAction(allRobots[0], handleBrick, 0, 0, 0, 0, 0, -1, 4, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], true, backWeight, 0));
        backWeight = false;
        Debug.Log("pickup");

        // TURN AROUND

        actionsForPath.Add(new RobotAction(allRobots[0], takeStep, 0, 8, 1, 4, 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, 0));
        actionsForPath.Add(new RobotAction(allRobots[0], takeStep, 0, 4, 1, 4, 180, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, 0));
        Debug.Log("turnaround");

        //OUTWARD PATH
        leadLeg = 0;
        for (int i = 0; i < _outwardPath.Count - 1; i++)
        {
            if (i == 0)
            {
                currentDirectionOfTravel = 1;
                previousDirectionOfTravel = 1;
            }
            else
            {
                currentDirectionOfTravel = brickStructure.GetCurrentDirectionOfTravel(_outwardPath[i], _outwardPath[i + 1]);//

                Debug.Log("currentDirectionOutward " + currentDirectionOfTravel);
            }

            if (currentDirectionOfTravel != previousDirectionOfTravel)
            {
                turnAngle = DetermineTurnAngle(previousDirectionOfTravel, currentDirectionOfTravel);
            }

            robotOrientation = FindCurrentRobotOrientation(leadLeg, currentDirectionOfTravel);
            previousRobotOrientation = FindCurrentRobotOrientation(leadLeg, previousDirectionOfTravel);

            nextStep = _outwardPath[i + 1].position - _outwardPath[i].position;
            nextCompanionCell = brickStructure.FindCompanionCell(_outwardPath[i + 1], leadLeg, robotOrientation, brickStructure.availableCells);

            nextStepCompanionDirections = _outwardPath[i + 1].position - nextCompanionCell.position;
            nextStepCompanionDirectionsPrevious = _outwardPath[i].position - brickStructure.FindCompanionCell(_outwardPath[i], leadLeg, previousRobotOrientation, brickStructure.availableCells).position;

            //sort heel position for close footwork
            if (nextStepCompanionDirections.magnitude < 3 && heelPosition == 0)
            {
                heelPosition = 180;
            }
            else
            {
                heelPosition = 0;
            }

            if (turnAngle != 0)////////////////////////////////////////////////////////////
            {
                if (i == _outwardPath.Count - 2)
                {
                    leadLeg = 1;
                    lateTurnAngle = turnAngle;
                    actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, turnAngle, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                    turnAngle = 0;
                    immediateTurnRequired = true;
                }

                else
                {
                    leadLeg = 1;
                    actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, turnAngle, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                    turnAngle = 0;
                    actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude + (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                    actionsForPath.Add(new RobotAction(allRobots[0], takeStep, 0, (int)nextStepCompanionDirections.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, 180, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                    leadLeg = 0;
                }
            }

            else
            {
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude + (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, turnAngle, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
            }

            previousDirectionOfTravel = currentDirectionOfTravel;
        }
        Debug.Log("outwardpath");

        //PLACEMENT
        placementDirections = brickStructure.bricksInTargetStructure[nextBrickToPlace].originCell.position - _outwardPath[_outwardPath.Count - 1].position;

        int placementRelativeBrickHeight = 0;
        int placementDistanceInFront = 0;
        int placementDistanceToSide = 0;
        int placementAngle = 0;

        if (BackWeightPossible(_outwardPath[_outwardPath.Count - 1], robotOrientation))
        {
            backWeight = true;
        }

        if (robotOrientation == 0)
        {
            placementRelativeBrickHeight = placementDirections.y;
            placementDistanceInFront = placementDirections.z;
            placementDistanceToSide = placementDirections.x;

            placementAngle = (int)brickStructure.bricksInTargetStructure[nextBrickToPlace].rotation.eulerAngles.y;
        }
        if (robotOrientation == 1)
        {
            placementRelativeBrickHeight = placementDirections.y;
            placementDistanceInFront = placementDirections.x;
            placementDistanceToSide = -placementDirections.z;

            placementAngle = (int)brickStructure.bricksInTargetStructure[nextBrickToPlace].rotation.eulerAngles.y - 90;

        }
        if (robotOrientation == 2)
        {
            placementRelativeBrickHeight = placementDirections.y;
            placementDistanceInFront = -placementDirections.z;
            placementDistanceToSide = -placementDirections.x;

            placementAngle = (int)brickStructure.bricksInTargetStructure[nextBrickToPlace].rotation.eulerAngles.y;

        }
        if (robotOrientation == 3)
        {
            placementRelativeBrickHeight = placementDirections.y;
            placementDistanceInFront = -placementDirections.x;
            placementDistanceToSide = placementDirections.z;

            placementAngle = (int)brickStructure.bricksInTargetStructure[nextBrickToPlace].rotation.eulerAngles.y - 90;
        }

        actionsForPath.Add(new RobotAction(allRobots[0], handleBrick, 0, 0, 0, 0, 0, placementRelativeBrickHeight - 1, placementDistanceInFront, placementDistanceToSide, leadLeg, placementAngle, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, 0));
        Debug.Log("placement");

        backWeight = false;

        //RETURN PATH
        leadLeg = 1;
        for (int i = 0; i < _returnPath.Count - 1; i++)
        {
            if (i == 0)
            {
                currentDirectionOfTravel = InvertDirectionOfTravel(previousDirectionOfTravel);
                previousDirectionOfTravel = currentDirectionOfTravel;
                leadLeg = 1;
            }
            else
            {

                currentDirectionOfTravel = brickStructure.GetCurrentDirectionOfTravel(_returnPath[i], _returnPath[i + 1]);

                if (afterImmediateTurn)
                {
                    previousDirectionOfTravel = currentDirectionOfTravel;
                }

                Debug.Log("currentDirectionBack " + currentDirectionOfTravel);

                leadLeg = 0;
            }

            if (afterImmediateTurn)
            {
                afterImmediateTurn = false;
                continue;
            }

            if (currentDirectionOfTravel != previousDirectionOfTravel)
            {
                turnAngle = DetermineTurnAngle(previousDirectionOfTravel, currentDirectionOfTravel);
            }

            robotOrientation = FindCurrentRobotOrientation(leadLeg, currentDirectionOfTravel);
            previousRobotOrientation = FindCurrentRobotOrientation(leadLeg, previousDirectionOfTravel);
            nextStep = _returnPath[i + 1].position - _returnPath[i].position;
            nextCompanionCell = brickStructure.FindCompanionCell(_returnPath[i + 1], leadLeg, robotOrientation, brickStructure.availableCells);

            nextStepCompanionDirections = _returnPath[i + 1].position - nextCompanionCell.position;
            if (i == 0)
            {
                nextStepCompanionDirectionsPrevious = _returnPath[i].position - brickStructure.FindCompanionCell(_returnPath[i], 0, robotOrientation, brickStructure.availableCells).position;
            }
            else
            {
                nextStepCompanionDirectionsPrevious = _returnPath[i].position - brickStructure.FindCompanionCell(_returnPath[i], leadLeg, previousDirectionOfTravel, brickStructure.availableCells).position;
            }

            //robotOrientation = FindCurrentRobotOrientation(leadLeg, currentDirectionOfTravel);

            //nextStep = _returnPath[i + 1].position - _returnPath[i].position;

            //nextStepCompanionDirections = _returnPath[i + 1].position - brickStructure.FindCompanionCell(_returnPath[i + 1], leadLeg, robotOrientation, brickStructure.availableCells).position;
            //nextStepCompanionDirectionsPrevious = _returnPath[i].position - brickStructure.FindCompanionCell(_returnPath[i], leadLeg, robotOrientation, brickStructure.availableCells).position;

            //sort heel position for close footwork
            if (nextStepCompanionDirections.magnitude < 3 && heelPosition == 0)
            {
                heelPosition = 180;
            }
            else
            {
                heelPosition = 0;
            }

            if (immediateTurnRequired)
            {
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude, 1, (int)nextStep.magnitude, -lateTurnAngle, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                immediateTurnRequired = false;
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude + (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)nextStepCompanionDirectionsPrevious.magnitude, 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, 0, (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, 180, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                afterImmediateTurn = true;
                leadLeg = 0;
                continue;
            }

            if (i == 0)
            {
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude + (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, 0, (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, 180, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
            }

            else if (turnAngle != 0)////////////////////////////////////////////////////////////
            {
                leadLeg = 1;
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, turnAngle, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                turnAngle = 0;
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude + (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, 0, (int)nextStepCompanionDirections.magnitude, leadLeg, (int)nextStepCompanionDirections.magnitude, 180, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
                leadLeg = 0;
            }

            else
            {
                actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, (int)nextStep.magnitude + (int)nextStepCompanionDirectionsPrevious.magnitude, leadLeg, (int)Mathf.Abs(nextStepCompanionDirections.magnitude), 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, backWeight, heelPosition));
            }

            previousDirectionOfTravel = currentDirectionOfTravel;

        }

        Debug.Log("journeyback");


        return actionsForPath;
    }

    private bool BackWeightPossible(Cell _leadFootCell, int _robotOrientation)
    {
        bool backWeightIsPossible = false;

        Cell candidateCell = null;

        if (_robotOrientation == 0)
        {
            candidateCell = brickStructure.grid.GetANeighbour(_leadFootCell, new Vector3Int(0, 0, -8));
        }
        else if (_robotOrientation == 1)
        {
            candidateCell = brickStructure.grid.GetANeighbour(_leadFootCell, new Vector3Int(-8, 0, 0));
        }
        else if (_robotOrientation == 2)
        {
            candidateCell = brickStructure.grid.GetANeighbour(_leadFootCell, new Vector3Int(8, 0, 0));
        }
        else if (_robotOrientation == 3)
        {
            candidateCell = brickStructure.grid.GetANeighbour(_leadFootCell, new Vector3Int(0, 0, 8));
        }

        if (brickStructure.availableCells.Contains(candidateCell))
        {
            backWeightIsPossible = true;
        }

        return backWeightIsPossible;
    }

    private int InvertDirectionOfTravel(int _inputDirection)
    {
        int invertedDirection = 0;

        if (_inputDirection == 0)
        {
            invertedDirection = 2;
        }
        else if (_inputDirection == 1)
        {
            invertedDirection = 3;
        }
        else if (_inputDirection == 2)
        {
            invertedDirection = 0;
        }
        else if (_inputDirection == 3)
        {
            invertedDirection = 1;
        }

        return invertedDirection;
    }

    private int DetermineTurnAngle(int _previousDirection, int _newDirection)
    {
        int turnAngle = 0;

        if (_previousDirection == 0)
        {
            if (_newDirection == 1)
            {
                turnAngle = -90;
            }
            else if (_newDirection == 2)
            {
                turnAngle = 180;
            }
            else if (_newDirection == 3)
            {
                turnAngle = 90;
            }
        }
        else if (_previousDirection == 1)
        {
            if (_newDirection == 0)
            {
                turnAngle = 90;
            }
            else if (_newDirection == 2)
            {
                turnAngle = -90;
            }
            else if (_newDirection == 3)
            {
                turnAngle = 180;
            }
        }
        else if (_previousDirection == 2)
        {
            if (_newDirection == 0)
            {
                turnAngle = 180;
            }
            else if (_newDirection == 1)
            {
                turnAngle = 90;
            }
            else if (_newDirection == 3)
            {
                turnAngle = -90;
            }
        }
        else if (_previousDirection == 3)
        {
            if (_newDirection == 0)
            {
                turnAngle = -90;
            }
            else if (_newDirection == 1)
            {
                turnAngle = 180;
            }
            else if (_newDirection == 2)
            {
                turnAngle = 90;
            }
        }


        return turnAngle;
    }

    private int FindCurrentRobotOrientation(int _leadLeg, int _currentDirectionOfTravel)
    {
        int _robotOrientation = 0;

        if (_leadLeg == 0)
        {
            _robotOrientation = _currentDirectionOfTravel;
        }
        else if (_leadLeg == 1)
        {
            if (_currentDirectionOfTravel == 0)
            {
                _robotOrientation = 2;
            }
            else if (_currentDirectionOfTravel == 1)
            {
                _robotOrientation = 3;
            }
            else if (_currentDirectionOfTravel == 2)
            {
                _robotOrientation = 0;
            }
            else if (_currentDirectionOfTravel == 3)
            {
                _robotOrientation = 1;
            }
        }
        return _robotOrientation;
    }


}
