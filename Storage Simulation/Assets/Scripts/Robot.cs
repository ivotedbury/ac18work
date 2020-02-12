using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{

    public int direction;

    public Node currentNode;
    public Node targetNode;
    public Structure structure;
    public RobotState robotState;

    public Pathfinder pathfinder;
    public RobotTask currentTask;

    public Queue<RobotTask> taskQueue = new Queue<RobotTask>();
    public int currentState = Constants.idle;

    public GameObject chassis;
    public GameObject lWheel;
    public GameObject rWheel;
    public GameObject platform;
    public int robotID;

    Tote passengerTote;

    // STATES
    private int platformState = Constants.DOWN;
    private float platformPosition = 0f;
    private bool isMoving = false;

    public bool taskComplete = true;
    public bool proceed = true;
    // public RobotAction currentAction;

    void Update()
    {
            BroadcastMessage("CheckCollisions", transform.position);

            if (!currentTask.taskComplete && proceed)
            {
                robotState = currentTask.UpdateTaskState(Time.deltaTime);
            }
      
        UpdateTransforms();


        //if (taskComplete)
        //{
        //    //   ProcessNextTask();
        //}

        //if (currentState == Constants.idle)
        //{
        //    if (taskQueue.Count > 0)
        //    {
        //        currentTask = taskQueue.Dequeue();
        //        CarryOutTask(currentTask);
        //        currentState = Constants.busy;
        //    }
        //    else
        //    {
        //        currentState = Constants.idle;
        //    }
        //}
    }

    void UpdateTransforms()
    {
        transform.position = robotState.position;
        transform.rotation = robotState.orientation;
        platformPosition = robotState.platformPosition;
        UpdateComponents();
    }

    void CheckCollisions(Vector3 _position)
    {
        if (this.transform.position != _position && Vector3.Distance(transform.position, _position) < Constants.COLLISION_DISTANCE)
        {
            proceed = false;
        }
        else
        {
            proceed = true;
        }
    }

    public void AssignTask(int _type, Node _destination)
    {
        Node _startingNode;

        if (taskQueue.Count > 0)
        {
            _startingNode = taskQueue.Peek().destination;
        }
        else
        {
            _startingNode = currentNode;
        }

        currentTask = new RobotTask(pathfinder, structure.nodesArray, new RobotState(transform.position, transform.rotation, platformPosition), _type, _startingNode, _destination);

        Debug.Log("Task assigned to " + robotID);
    }

    void CarryOutTask(RobotTask _task)
    {
        if (_task.inProgress())
        {
            targetNode = _task.waypoints.Peek();
            transform.position = new Vector3(targetNode.gridPos.x * Constants.GRID_DIMS.x, targetNode.gridPos.y * Constants.GRID_DIMS.y, targetNode.gridPos.z * Constants.GRID_DIMS.z);
            // currentNode = _task.waypoints.Dequeue();
        }

    }

    void UpdateComponents()
    {
        chassis.transform.position = this.transform.position + (this.transform.rotation * Constants.CHASSIS_TRANSFORM);
        lWheel.transform.position = this.transform.position + (this.transform.rotation * Constants.L_WHEEL_TRANSFORM);
        rWheel.transform.position = this.transform.position + (this.transform.rotation * Constants.R_WHEEL_TRANSFORM);
        platform.transform.position = this.transform.position + (this.transform.rotation * Constants.PLATFORM_TRANSFORM) + (platformPosition * Constants.RAISED_PLATFORM_HEIGHT);
    }

    public void InitialiseRobot(Structure _structure, Node _node, int _robotID)
    {
        structure = _structure;
        robotState = new RobotState(transform.position, transform.rotation, platformPosition);
        currentNode = _node;
        robotID = _robotID;
    }

}




public struct RobotState
{
    /*
     *  Completely describes the state of the robot
     */

    public Vector3 position;
    public Quaternion orientation;
    public float platformPosition;


    public RobotState(Vector3 _position, Quaternion _orientation, float _platformPosition)
    {
        position = _position;
        orientation = _orientation;
        platformPosition = _platformPosition;
    }
}

public struct RobotStateUpdate
{
    /*
     *  Describes an update to the robot state
     */

    public RobotState robotState;
    public float timeTaken;

    public RobotStateUpdate(RobotState _robotState, float _timeTaken)
    {
        robotState = _robotState;
        timeTaken = _timeTaken;
    }
}
