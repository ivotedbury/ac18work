using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAction
{
    private RobotState startState;
    private RobotState endState;

    public float actionDuration;                 // How long the action takes to complete

    public RobotState currentState;             // Current state
    private float currentTime;                   // How much time has passed

    public bool actionComplete = false;

    float linearDist;
    float linearTime;
    float angularDist;
    float angularTime;
    float platformRaise;
    float platformTime;

    float timeSinceStart;

    public RobotAction(RobotState _startState, RobotState _endState)
    {
        startState = _startState;
        endState = _endState;

        currentState = startState;

        Setup();
    }

    private void Setup()
    {
        /*
         * Calculates necessary properties
         */

        linearDist = Vector3.Distance(startState.position, endState.position);
        linearTime = linearDist / Constants.ROBOT_LINEAR_SPEED;
        angularDist = Quaternion.Angle(startState.orientation, endState.orientation);
        angularTime = angularDist / Constants.ROBOT_ANGULAR_SPEED;
        platformRaise = endState.platformPosition - startState.platformPosition;
        platformTime = platformRaise / Constants.ROBOT_LIFT_SPEED;

        actionDuration = Mathf.Max(linearTime, angularTime);

        timeSinceStart = 0;
        actionComplete = false;
    }

    public void UpdateRobotState(float deltaTime, bool proceed)
    {
        if (timeSinceStart >= actionDuration)
        {
            actionComplete = true;
        }

        if (proceed)
        {
            timeSinceStart += deltaTime;
        }

        currentState.position = Vector3.Lerp(startState.position, endState.position, timeSinceStart / actionDuration);
        currentState.orientation = Quaternion.Lerp(startState.orientation, endState.orientation, timeSinceStart / actionDuration);
        currentState.platformPosition = Mathf.Lerp(startState.platformPosition, endState.platformPosition, timeSinceStart / actionDuration);
    }

    //public static List<RobotAction> ActionsFromNodes(RobotState currentState, Node currentNode, Node targetNode)
    //{
    //    /*
    //     * Creates a list of robot actions to traverse from currentNode to targetNode
    //     */

    //    List<RobotAction> actions = new List<RobotAction>();
    //    Vector3 linearTranslation = targetNode.transform.position - currentNode.transform.position;
    //    Quaternion newOrientation = Quaternion.LookRotation(linearTranslation);

    //    RobotState midState = currentState;
    //    RobotState finalState = currentState;
    //    midState.orientation = newOrientation;
    //    finalState.orientation = newOrientation;
    //    finalState.position = targetNode.transform.position;

    //    // Adjust orientation
    //    actions.Add(new RobotAction(currentState, midState));

    //    // Perform linear translation
    //    actions.Add(new RobotAction(midState, finalState));

    //    return actions;
    //}
}
