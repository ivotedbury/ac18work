﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;


public class Pathfinder
{

    public List<Node> FindNeighbours(Node[,,] _nodeArray, Node _input)
    {
        List<Node> _neighbours = new List<Node>();

        if (_input.gridPos.x < Constants.MAIN_STRUCTURE_DIMS.x - 1 && _input.flowPosX) // positive X
        {
            _neighbours.Add(_nodeArray[_input.gridPos.x + 1, _input.gridPos.y, _input.gridPos.z]);
        }
        if (_input.gridPos.z < Constants.MAIN_STRUCTURE_DIMS.z - 1 && _input.flowPosZ) // positive Z
        {
            _neighbours.Add(_nodeArray[_input.gridPos.x, _input.gridPos.y, _input.gridPos.z + 1]);
        }
        if (_input.gridPos.x > 0 && _input.flowNegX) // negative X
        {
            _neighbours.Add(_nodeArray[_input.gridPos.x - 1, _input.gridPos.y, _input.gridPos.z]);
        }
        if (_input.gridPos.z > 0 && _input.flowNegZ) // negative Z
        {
            _neighbours.Add(_nodeArray[_input.gridPos.x, _input.gridPos.y, _input.gridPos.z - 1]);
        }

        return _neighbours;
    }

    private int ManhattanDistance(Node _start, Node _end)
    {
        int distance = Mathf.Abs(_end.gridPos.x - _start.gridPos.x) + Mathf.Abs(_end.gridPos.y - _start.gridPos.y) + Mathf.Abs(_end.gridPos.z - _start.gridPos.z);
        return distance;
    }

    public List<Node> FindPath(Node[,,] _nodeRepArray, Node _start, Node _end)
    {
        Debug.Log("Pathfinding started");
        Debug.Log(_start.gridPos);
        Debug.Log(_end.gridPos);

        List<Node> path = new List<Node>();
        bool pathSuccess = false;

        _start.parent = _start;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(_start);

        _start.gCost = 0;
        _start.hCost = ManhattanDistance(_start, _end);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            openSet.Remove(openSet[0]);
            closedSet.Add(current);

            if (current == _end)
            {
                pathSuccess = true;
                break;
            }

            foreach (Node neighbour in FindNeighbours(_nodeRepArray, current))
            {

                if ((neighbour.occupied) || closedSet.Contains(neighbour))
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
                }
            }
        }

        if (pathSuccess)
        {
            path = RetracePath(_start, _end);
          //  path = SimplifyPath(path);

            Debug.Log("Path Found!");
            Debug.Log(path.Count);
        }
        
        return path;
    }

    //List<Node> SimplifyPath(List<Node> _inputPath)
    //{
    //    List<Node> outputpath = new List<Node>();

    //    outputpath = _inputPath;

    //    Quaternion currentDirection;
    //    Quaternion newDirection;

    //    currentDirection = Quaternion.LookRotation(_inputPath[1].transform.position - _inputPath[0].transform.position);

    //    for (int i = 0; i < _inputPath.Count - 1; i++)
    //    {
    //        newDirection = Quaternion.LookRotation(_inputPath[i + 1].transform.position - _inputPath[i].transform.position);
    //        if (newDirection != currentDirection)
    //        {
    //            outputpath.Add(_inputPath[i]);
    //        }
    //    }
    //    return outputpath;
    //}

    List<Node> RetracePath(Node _startPath, Node _endPath)
    {
        List<Node> _path = new List<Node>();
        Node _current = _endPath;

        while (_current != _startPath)
        {
            _path.Add(_current);
            _current = _current.parent;
        }
        _path.Add(_startPath);

        _path.Reverse();
        return _path;
    }

    public Node FindLowestFCost(List<Node> _inputList)
    {
        Node lowestFCost = null;

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



