using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{

    public Robot robotReference;
    Structure structure;
    List<Robot> allRobots = new List<Robot>();

    Queue<Tote> pickingQueue = new Queue<Tote>();
    Queue<Tote> replenishingQueue = new Queue<Tote>();

    int numberOfRobots = Constants.NUMBER_OF_ROBOTS;

    public void InitialiseRobots(Structure _structure)
    {
        structure = _structure;

        for (int i = 0; i < numberOfRobots; i++)
        {
            Node startingNode = structure.nodesArray[Constants.ROBOT_STARTING_POSITION[i].x, Constants.ROBOT_STARTING_POSITION[i].y, Constants.ROBOT_STARTING_POSITION[i].z];
            Robot newRobot = Instantiate(robotReference, startingNode.transform.position, Quaternion.identity, transform);
            newRobot.currentNode = startingNode;
            newRobot.direction = Constants.ROBOT_STARTING_DIRECTION[i];
            newRobot.pathfinder = new Pathfinder();
            Debug.Log(newRobot.pathfinder.nodeRepArray.Length);
        }

        // allRobots[0].pathfinder.InitialisePathfinder(structure.nodesArray);
        List<NodeRep> testPath = new List<NodeRep>();
        Debug.Log(testPath.Count);
        testPath = allRobots[0].pathfinder.FindPath(allRobots[0].pathfinder.nodeRepArray[0, 0, 0], allRobots[0].pathfinder.nodeRepArray[0, 0, 0]);
        Debug.Log(testPath.Count);
    }

}

