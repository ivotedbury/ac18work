using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;


public class Pathfinder
{

    public List<NodeRep> FindNeighbours(NodeRep[,,] _nodeRepArray, NodeRep _input)
    {
        List<NodeRep> _neighbours = new List<NodeRep>();

        if (_input.pos.x < Constants.MAIN_STRUCTURE_DIMS.x - 1) // positive X
        {
            _neighbours.Add(_nodeRepArray[_input.pos.x + 1, _input.pos.y, _input.pos.z]);
       }
        if (_input.pos.z < Constants.MAIN_STRUCTURE_DIMS.z - 1) // positive Z
        {
            _neighbours.Add(_nodeRepArray[_input.pos.x, _input.pos.y, _input.pos.z + 1]);
        }
        if (_input.pos.x > 0) // negative X
        {
            _neighbours.Add(_nodeRepArray[_input.pos.x - 1, _input.pos.y, _input.pos.z]);
        }
        if (_input.pos.z > 0) // negative Z
        {
            _neighbours.Add(_nodeRepArray[_input.pos.x, _input.pos.y, _input.pos.z - 1]);
        }

        return _neighbours;
    }

    private int ManhattanDistance(NodeRep _start, NodeRep _end)
    {
        int distance = Mathf.Abs(_end.pos.x - _start.pos.x) + Mathf.Abs(_end.pos.y - _start.pos.y) + Mathf.Abs(_end.pos.z - _start.pos.z);
        return distance;
    }

    public List<NodeRep> FindPath(NodeRep[,,] _nodeRepArray, NodeRep _start, NodeRep _end)
    {

        List<NodeRep> path = new List<NodeRep>();
        bool pathSuccess = false;

        _start.parent = _start;

        List<NodeRep> openSet = new List<NodeRep>();
        HashSet<NodeRep> closedSet = new HashSet<NodeRep>();
        openSet.Add(_start);

        _start.gCost = 0;
        _start.hCost = ManhattanDistance(_start, _end);

        while (openSet.Count > 0)
        {
            NodeRep current = openSet[0];
            openSet.Remove(openSet[0]);
            closedSet.Add(current);

            if (current == _end)
            {
                pathSuccess = true;
                break;
            }

            foreach (NodeRep neighbour in FindNeighbours(_nodeRepArray, current))
            {

                if ((!neighbour.walkable) || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = ((int)current.gCost + ManhattanDistance(current, neighbour));
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = ManhattanDistance(neighbour, _end);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    //else
                    //    openSet.UpdateItem(neighbour);
                }
            }
        }

        if (pathSuccess)
        {
            path = RetracePath(_start, _end);
            Debug.Log("Path Found!");
            Debug.Log(path.Count);
        }
        return path;

        //requestManager.FinishedProcessingPath(path, pathSuccess);

    }

    List<NodeRep> RetracePath(NodeRep _startPath, NodeRep _endPath)
    {
        List<NodeRep> _path = new List<NodeRep>();
        NodeRep _current = _endPath;

        while (_current != _startPath)
        {
            _path.Add(_current);
            _current = _current.parent;
        }
        // _path.Add(_startPath);

        _path.Reverse();
        return _path;
    }

    public NodeRep FindLowestFCost(List<NodeRep> _inputList)
    {
        NodeRep lowestFCost = null;

        float currentLowestFCost = Mathf.Infinity;

        for (int i = 0; i < _inputList.Count; i++)
        {
            if (_inputList[i].fCost() < currentLowestFCost)
            {
                lowestFCost = _inputList[i];
                currentLowestFCost = _inputList[i].fCost();
            }
        }
        return lowestFCost;
    }
}



