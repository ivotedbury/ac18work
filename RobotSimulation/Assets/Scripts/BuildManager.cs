using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager
{
    public BrickStructure brickStructure;
    public List<Robot> allRobots = new List<Robot>();

    public int startingBricks;

    int nextBrickToPlace;

    const int changeFootAHeelPosition = 0;
    const int handleBrick = 1;
    const int takeStep = 2;

    int actionCounter = 0;
    public bool readyForNextBrick = true;

    List<RobotAction> robotActions = new List<RobotAction>();

    List<Cell> outwardPath;
    List<Cell> returnPath;


    public BuildManager(Vector3Int _gridSize, Vector3Int _seedPosition, TextAsset _brickImportData, int _startingBricks, int _numberOfRobots)
    {
        brickStructure = new BrickStructure(_gridSize, _seedPosition, _brickImportData);
        startingBricks = _startingBricks;
        nextBrickToPlace = startingBricks;

        for (int i = 0; i < startingBricks; i++)
        {
            brickStructure.bricksInPlace.Add(brickStructure.bricksInTargetStructure[i]);
        }

        allRobots.Add(new Robot(brickStructure.bricksInTargetStructure[0], 4, 1, true));

        // robotActions.Add(new RobotAction(allRobots[0], 2, 0, 8, 0, 4, 180, 0, 0, 0, 0, 0, null, false, 0));

    }

    public void Update()
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
    }

    public void PlaceNextBrick()
    {
        robotActions.Clear();

        brickStructure.UpdateAvailableCells();

        GeneratePath();

        robotActions = ConvertPath(outwardPath, returnPath);
    }

    List<RobotAction> ConvertPath(List<Cell> _outwardPath, List<Cell> _returnPath)
    {
        List<RobotAction> actionsForPath = new List<RobotAction>();
        Vector3Int nextStep;
        Vector3Int nextStepCompanionDirections;
        Vector3Int nextStepCompanionDirectionsPrevious;
        Vector3Int placementDirections;
        int currentDirection;

        actionsForPath.Add(new RobotAction(allRobots[0], handleBrick, 0, 0, 0, 0, 0, -1, 4, 0, 1, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], true, 0));

        for (int i = 0; i < _outwardPath.Count - 1; i++)
        {
            if (i == 0)
            {
                currentDirection = 1;
            }
            else
            {
                currentDirection = brickStructure.GetCurrentDirection(_outwardPath[i + 1], _outwardPath[i]);

                Debug.Log("currentDirecion " + currentDirection);
            }

            nextStep = _outwardPath[i + 1].position - _outwardPath[i].position;
            nextStepCompanionDirections = _outwardPath[i + 1].position - brickStructure.FindCompanionCell(_outwardPath[i + 1], 0, 1, brickStructure.availableCells).position;
            nextStepCompanionDirectionsPrevious = _outwardPath[i].position - brickStructure.FindCompanionCell(_outwardPath[i], 0, 1, brickStructure.availableCells).position;

            actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, Mathf.Abs(nextStep.x) + Mathf.Abs(nextStepCompanionDirectionsPrevious.x), 0, Mathf.Abs(nextStepCompanionDirections.x), 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, 0));
        }

        placementDirections = brickStructure.bricksInTargetStructure[nextBrickToPlace].originCell.position - _outwardPath[_outwardPath.Count - 1].position;

        actionsForPath.Add(new RobotAction(allRobots[0], handleBrick, 0, 0, 0, 0, 0, placementDirections.y - 1, placementDirections.x, placementDirections.z, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, 0));

        for (int i = 0; i < _returnPath.Count - 1; i++)
        {
            if (i == 0)
            {
                currentDirection = 1;
            }
            else
            {
                currentDirection = brickStructure.GetCurrentDirection(_returnPath[i + 1], _returnPath[i]);
            }
            nextStep = _returnPath[i + 1].position - _returnPath[i].position;
            nextStepCompanionDirections = _returnPath[i + 1].position - brickStructure.FindCompanionCell(_returnPath[i + 1], 1, 1, brickStructure.availableCells).position;
            nextStepCompanionDirectionsPrevious = _returnPath[i].position - brickStructure.FindCompanionCell(_returnPath[i], 1, 1, brickStructure.availableCells).position;

            actionsForPath.Add(new RobotAction(allRobots[0], takeStep, nextStep.y, Mathf.Abs(nextStep.x) + Mathf.Abs(nextStepCompanionDirectionsPrevious.x), 1, Mathf.Abs(nextStepCompanionDirections.x), 0, 0, 0, 0, 0, 0, brickStructure.bricksInTargetStructure[nextBrickToPlace], false, 0));
        }

        return actionsForPath;
    }

    public void GeneratePath()
    {
        Cell dropOffPoint = brickStructure.FindDropOffCell(brickStructure.bricksInTargetStructure[nextBrickToPlace], brickStructure.availableCells);
        Cell rearLegDropOffPoint = brickStructure.grid.GetANeighbour(dropOffPoint, new Vector3Int(-4, 0, 0));
        outwardPath = brickStructure.FindPathOneWay(brickStructure.bricksInPlace[1].originCell, dropOffPoint, 1);
        returnPath = brickStructure.FindPathOneWay(rearLegDropOffPoint, brickStructure.bricksInPlace[0].originCell, 3);
        Debug.Log(outwardPath.Count);

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



    //public void ConvertPathToMoves(Brick _targetBrick, Cell _pickupCell, Robot _robot, )

}
