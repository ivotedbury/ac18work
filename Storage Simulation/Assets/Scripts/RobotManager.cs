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
            newRobot.InitialiseRobot(structure, startingNode, i);
            //  newRobot.currentNode = newRobot.structure.nodesArray[startingNode.gridPos.x, startingNode.gridPos.y, startingNode.gridPos.z];
            newRobot.direction = Constants.ROBOT_STARTING_DIRECTION[i];
            newRobot.pathfinder = new Pathfinder();
            newRobot.AssignTask(Constants.PICKUP, structure.nodesArray[Random.Range(1, 9), 0, Random.Range(1, 9)]);
            //  newRobot.currentAction = new RobotAction(new RobotState(newRobot.transform.position, newRobot.transform.rotation, 0), new RobotState(structure.nodesArray[newRobot.currentNode.gridPos.x, 0, Random.Range(0, 9)].transform.position, Quaternion.Euler(0, 0, 0), Constants.RAISED_PLATFORM_HEIGHT.y), newRobot.currentNode);
            allRobots.Add(newRobot);
            // newRobot.taskQueue.Enqueue(new RobotTask(newRobot.pathfinder, newRobot.structure.nodesArray, Constants.GO_TO, newRobot.currentNode, newRobot.structure.nodesArray[Random.Range(1, Constants.MAIN_STRUCTURE_DIMS.x), 0, Random.Range(1, Constants.MAIN_STRUCTURE_DIMS.z)]));
        }
    }

    //void Update()
    //{
    //    for (int i = 0; i < allRobots.Count; i++)
    //    {
    //        for (int j = 0; j < allRobots.Count; j++)
    //        {
    //            if (i == j)
    //            {
    //                continue;
    //            }
    //            if (Vector3.Distance(allRobots[i].transform.position, allRobots[j].transform.position) < Constants.COLLISION_DISTANCE)
    //            {
    //                allRobots[i].proceed = false;
    //                Debug.Log("Collision detected between " + allRobots[i].robotID + " and " + allRobots[j].robotID);
    //            }
    //            else
    //            {
    //                allRobots[i].proceed = true;
    //            }
    //        }
    //    }
    //}

}

