using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {

    public int direction;

    public Node currentNode;
    public Node targetNode;

    public Pathfinder pathfinder;

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
        this.transform.position = currentNode.transform.position;

        chassis.transform.position = this.transform.position + (this.transform.rotation * Constants.CHASSIS_TRANSFORM);
        lWheel.transform.position = this.transform.position + (this.transform.rotation * Constants.L_WHEEL_TRANSFORM);
        rWheel.transform.position = this.transform.position + (this.transform.rotation * Constants.R_WHEEL_TRANSFORM);
        platform.transform.position = this.transform.position + (this.transform.rotation * Constants.PLATFORM_TRANSFORM) + (platformPosition * Constants.RAISED_PLATFORM_HEIGHT);
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
