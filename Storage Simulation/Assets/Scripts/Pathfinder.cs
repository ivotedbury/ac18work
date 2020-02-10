using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinder
{

    public List<NodeRep> result = new List<NodeRep>();

    public NodeRep[,,] nodeRepArray = new NodeRep[Constants.MAIN_STRUCTURE_DIMS.x, Constants.MAIN_STRUCTURE_DIMS.y, Constants.MAIN_STRUCTURE_DIMS.z];

    //

    public Pathfinder()
    {
        InitialisePathfinder();
    }

    public void InitialisePathfinder()
    {
        for (int x = 0; x < Constants.MAIN_STRUCTURE_DIMS.x; x++)
        {
            for (int y = 0; y < Constants.MAIN_STRUCTURE_DIMS.y; y++)
            {
                for (int z = 0; z < Constants.MAIN_STRUCTURE_DIMS.z; z++)
                {
                    nodeRepArray[x, y, z] = new NodeRep(new Vector3Int(x,y,z));
                }
            }
        }
    }

    private List<NodeRep> FindNeighbours(NodeRep _input)
    {
        List<NodeRep> _neighbours = new List<NodeRep>();

        if (_input.pos.x < Constants.GRID_DIMS.x - 1) // positive X
        {
            _neighbours.Add(nodeRepArray[_input.pos.x + 1, _input.pos.y, _input.pos.z]);
            Debug.Log("Found neighbour +X");
        }
        if (_input.pos.z < Constants.GRID_DIMS.z - 1) // positive Z
        {
            _neighbours.Add(nodeRepArray[_input.pos.x, _input.pos.y, _input.pos.z + 1]);
            Debug.Log("Found neighbour +Z");
        }
        if (_input.pos.x > 0) // negative X
        {
            _neighbours.Add(nodeRepArray[_input.pos.x - 1, _input.pos.y, _input.pos.z]);
            Debug.Log("Found neighbour -X");
        }
        if (_input.pos.z > 0) // negative Z
        {
            _neighbours.Add(nodeRepArray[_input.pos.x, _input.pos.y, _input.pos.z - 1]);
            Debug.Log("Found neighbour -Z");
        }

        return _neighbours;
    }

    private int ManhattanDistance(NodeRep _start, NodeRep _end)
    {
        int distance = Mathf.Abs(_start.pos.x - _end.pos.x) + Mathf.Abs(_start.pos.y - _end.pos.y) + Mathf.Abs(_start.pos.z - _end.pos.z);
        return distance;
    }

    public List<NodeRep> FindPath(NodeRep _start, NodeRep _end)
    {
        List<NodeRep> path = new List<NodeRep>();
        List<NodeRep> open = new List<NodeRep>();
        List<NodeRep> closed = new List<NodeRep>();
        List<NodeRep> neighbours = new List<NodeRep>();

        NodeRep current;

        bool pathSuccess = false;

        Debug.Log("Pathfinding started...");


        open.Add(_start);
        _start.hCost = ManhattanDistance(_start, _end);
        _start.gCost = 0;


        while (open.Count < 0)
        {
            current = FindLowestFCost(open);

            Debug.Log(current.fCost());

            if (current == _end)
            {
                pathSuccess = true;
                break;
            }

            neighbours = FindNeighbours(current);

            Debug.Log(neighbours.Count);

            foreach (NodeRep neighbour in neighbours)
            {
                float currentNeighbourCost = current.gCost + ManhattanDistance(current, neighbour);
                neighbour.gCost = ManhattanDistance(_start, neighbour);

                if (open.Contains(neighbour))
                {
                    if (neighbour.gCost <= currentNeighbourCost)
                    {
                        break;
                    }
                }
                else if (closed.Contains(neighbour))
                {
                    if (neighbour.gCost <= currentNeighbourCost)
                    {
                        break;
                    }
                    closed.Remove(neighbour);
                    open.Add(neighbour);
                }
                else
                {
                    open.Add(neighbour);
                    neighbour.hCost = ManhattanDistance(neighbour, _end);
                }
                neighbour.gCost = currentNeighbourCost;
                neighbour.parent = current;
            }


            closed.Add(current);
        }

        if (pathSuccess)
        {
            path = RetracePath(_start, _end);
        }

        return path;
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


public class NodeRep
{
    public Vector3Int pos;
    public NodeRep parent;

    public float gCost;
    public float hCost;

    public float fCost()
    {
        return gCost + hCost;
    }

    public NodeRep(Vector3Int _inputPos)
    {
        pos = _inputPos;
        gCost = 0;
        hCost = 0;
    }
}
