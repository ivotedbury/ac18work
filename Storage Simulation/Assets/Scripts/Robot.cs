using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{

    public int direction;

    public NodeRep currentNode;
    public NodeRep targetNode;
    public NodeRep[,,] nodeRepArray = new NodeRep[Constants.MAIN_STRUCTURE_DIMS.x, Constants.MAIN_STRUCTURE_DIMS.y, Constants.MAIN_STRUCTURE_DIMS.z];



    public Pathfinder pathfinder;
    public RobotTask currentTask;

    public Queue<RobotTask> taskQueue = new Queue<RobotTask>();
    public int currentState = Constants.idle;

    public GameObject chassis;
    public GameObject lWheel;
    public GameObject rWheel;
    public GameObject platform;

    Tote passengerTote;

    // STATES
    private int platformState = Constants.DOWN;
    private float platformPosition = 0f;
    private bool isMoving = false;



    void Update()
    {
        chassis.transform.position = this.transform.position + (this.transform.rotation * Constants.CHASSIS_TRANSFORM);
        lWheel.transform.position = this.transform.position + (this.transform.rotation * Constants.L_WHEEL_TRANSFORM);
        rWheel.transform.position = this.transform.position + (this.transform.rotation * Constants.R_WHEEL_TRANSFORM);
        platform.transform.position = this.transform.position + (this.transform.rotation * Constants.PLATFORM_TRANSFORM) + (platformPosition * Constants.RAISED_PLATFORM_HEIGHT);

        if (currentState == Constants.idle)
        {
            if (taskQueue.Count > 0)
            {
                currentTask = taskQueue.Dequeue();
                CarryOutTask(currentTask);
                currentState = Constants.busy;
            }
            else
            {
                currentState = Constants.idle;
            }
        }
    }

    void CarryOutTask(RobotTask _task)
    {
        if (_task.inProgress())
        {
            targetNode = _task.waypoints.Peek();
            transform.position = new Vector3(targetNode.pos.x * Constants.GRID_DIMS.x, targetNode.pos.y * Constants.GRID_DIMS.y, targetNode.pos.z * Constants.GRID_DIMS.z);
            currentNode = _task.waypoints.Dequeue();
        }

    }

    public void InitialiseRobot()
    {
        for (int x = 0; x < Constants.MAIN_STRUCTURE_DIMS.x; x++)
        {
            for (int y = 0; y < Constants.MAIN_STRUCTURE_DIMS.y; y++)
            {
                for (int z = 0; z < Constants.MAIN_STRUCTURE_DIMS.z; z++)
                {
                    nodeRepArray[x, y, z] = new NodeRep(new Vector3Int(x, y, z));
                }
            }
        }
    }
}




public struct RobotState
{
    /*
     *  Completely describes the state of the robot
     */

    public Vector3 position;
    public Quaternion orientation;

    public RobotState(Vector3 _position, Quaternion _orientation)
    {
        position = _position;
        orientation = _orientation;
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
