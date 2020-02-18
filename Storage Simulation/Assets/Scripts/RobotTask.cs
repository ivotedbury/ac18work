using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTask
{

    public Queue<Node> waypoints = new Queue<Node>();
    public Node destination;
    public Queue<RobotAction> actionsInTask = new Queue<RobotAction>();
    public RobotAction currentAction;
    Queue<RobotState> waypointStates = new Queue<RobotState>();
    RobotState stateUpdate;

    public bool taskComplete;

    public RobotTask(Pathfinder _pathfinder, Node[,,] _nodeArray, RobotState _robotState, int _actionType, Node _current, Node _target)
    {
        taskComplete = false;

        List<Node> waypointsNodes = new List<Node>();
        waypointsNodes = _pathfinder.FindPath(_nodeArray, _current, _target);
        Debug.Log(waypointsNodes.Count);

        destination = waypointsNodes[waypointsNodes.Count - 1];

        Debug.Log("Creating actions");

        Quaternion directionToNextWaypoint;

        RobotState currentState = _robotState;

        for (int i = 0; i < waypointsNodes.Count - 1; i++)
        {
            directionToNextWaypoint = Quaternion.LookRotation(waypointsNodes[i + 1].transform.position - waypointsNodes[i].transform.position); // find direction

            actionsInTask.Enqueue(new RobotAction(currentState, new RobotState(currentState.position, directionToNextWaypoint, currentState.platformPosition))); // turn to that direction

            currentState = new RobotState(currentState.position, directionToNextWaypoint, currentState.platformPosition);

            actionsInTask.Enqueue(new RobotAction(currentState, new RobotState(waypointsNodes[i + 1].transform.position, directionToNextWaypoint, currentState.platformPosition))); // move along that direction

            currentState = new RobotState(waypointsNodes[i + 1].transform.position, directionToNextWaypoint, currentState.platformPosition);

            Debug.Log(actionsInTask.Count + " actions in task");

        }

        if (_actionType == Constants.PICKUP)
        {
            actionsInTask.Enqueue(new RobotAction(currentState, new RobotState(currentState.position, currentState.orientation, Constants.RAISED_PLATFORM_HEIGHT.y))); // move along that direction
        }
        else if (_actionType == Constants.DROPOFF)
        {
            actionsInTask.Enqueue(new RobotAction(currentState, new RobotState(currentState.position, currentState.orientation, 0))); // move along that direction
        }


    }

    public RobotState UpdateTaskState(float _deltaTime, bool _proceed)
    {

        if (actionsInTask.Count > 0)
        {
            currentAction = actionsInTask.Peek();

            if (currentAction.actionComplete)
            {
                actionsInTask.Dequeue();
                currentAction = actionsInTask.Peek();
            }

            currentAction.UpdateRobotState(Time.deltaTime, _proceed);
            stateUpdate = currentAction.currentState;
        }

        else
        {
            taskComplete = true;
        }

        return stateUpdate;
    }

    public bool inProgress()
    {
        if (waypoints.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
