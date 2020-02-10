using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTask
{

    public Queue<NodeRep> waypoints = new Queue<NodeRep>();
   

    public RobotTask(Pathfinder _pathfinder, NodeRep[,,] _nodeRepArray, int _actionType, NodeRep _current, NodeRep _target)
    {
        if (_actionType == Constants.GO_TO)
        {
            List<NodeRep> waypointsNodes = new List<NodeRep>();
            waypointsNodes = _pathfinder.FindPath(_nodeRepArray, _current, _target);
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
