using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager
{

    public Grid grid;
    public BrickStructure brickStructure;
    public List<Robot> allRobots = new List<Robot>();

    public int startingBricks;

    int nextBrickToPlace;

    List<RobotAction> robotActions = new List<RobotAction>();

    List<Cell> outwardPath;

    public BuildManager(Vector3Int _gridSize, Vector3Int _seedPosition, TextAsset _brickImportData, int _startingBricks, int _numberOfRobots)
    {
        brickStructure = new BrickStructure(_gridSize, _seedPosition, _brickImportData);
        startingBricks = _startingBricks;

        allRobots.Add(new Robot(brickStructure.bricksInTargetStructure[0], 4, 1, true));

        robotActions.Add(new RobotAction(allRobots[0], 2, 0, 8, 0, 4, 180, 0, 0, 0, 0, 0, null, false, 0));
            }

    public void Update()
    {
        if (!allRobots[0].moveInProgress)
        {
            robotActions[0].PerformAction();
        }
    }

    public void GeneratePath()
    {
        Debug.Log(brickStructure.availableCells.Count);
        Debug.Log(brickStructure.bricksInTargetStructure[startingBricks].originCell.position);

        Cell dropOffPoint = brickStructure.FindDropOffCell(brickStructure.bricksInTargetStructure[startingBricks], brickStructure.availableCells);

        Debug.Log(dropOffPoint.position);

        outwardPath = brickStructure.FindPathOneWay(brickStructure.bricksInPlace[1].originCell, dropOffPoint);

       Debug.Log(outwardPath.Count);

        foreach (Cell cellOnPath in outwardPath)
        {
            cellOnPath.currentStatus = 2;
        }
    }

    //public void ConvertPathToMoves(Brick _targetBrick, Cell _pickupCell, Robot _robot, )

}
