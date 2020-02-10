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
            newRobot.InitialiseRobot();
            newRobot.currentNode = newRobot.nodeRepArray[startingNode.gridPos.x, startingNode.gridPos.y, startingNode.gridPos.z];
            newRobot.direction = Constants.ROBOT_STARTING_DIRECTION[i];
            newRobot.pathfinder = new Pathfinder();
            allRobots.Add(newRobot);
            newRobot.taskQueue.Enqueue(new RobotTask(newRobot.pathfinder, newRobot.nodeRepArray, Constants.GO_TO, newRobot.currentNode, newRobot.nodeRepArray[Random.Range(1,Constants.MAIN_STRUCTURE_DIMS.x), 0, Random.Range(1, Constants.MAIN_STRUCTURE_DIMS.z)]));
        }

       // List<NodeRep> testPath = new List<NodeRep>();
       // Debug.Log(allRobots[0].pathfinder.FindNeighbours(allRobots[0].pathfinder.nodeRepArray[0, 0, 0]).Count);
       //allRobots[0].pathfinder.FindPath(allRobots[0].pathfinder.nodeRepArray[0, 0, 0], allRobots[0].pathfinder.nodeRepArray[0, 0, 5]);
       // Debug.Log(allRobots[0].pathfinder.result.Count);
    }

}

