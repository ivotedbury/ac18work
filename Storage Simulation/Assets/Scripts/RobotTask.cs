using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTask
{

    public Queue<Node> waypoints = new Queue<Node>();
   

    public RobotTask(Pathfinder _pathfinder, Node[,,] _nodeArray, int _actionType, Node _current, Node _target)
    {
        if (_actionType == Constants.GO_TO)
        {
            List<Node> waypointsNodes = new List<Node>();
            waypointsNodes = _pathfinder.FindPath(_nodeArray, _current, _target);
            Debug.Log(waypointsNodes.Count);

            for (int i = 0; i < waypointsNodes.Count; i++)
            {
                waypoints.Enqueue(waypointsNodes[i]);
            }
        }
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
