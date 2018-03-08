using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    PathFinder pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<PathFinder>();
    }

    public static void RequestPath(Cell pathStart, Cell pathTarget, Action<Cell[], bool> callBack) // input CNodes for start and target, callback action containing the path list and boolean
    {
        PathRequest newRequest = new PathRequest(pathStart, pathTarget, callBack);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathTarget);
        }
    }

    public void FinishedProcessingPath(Cell[] path, bool success)
    {
        currentPathRequest.callBack(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Cell pathStart;
        public Cell pathTarget;
        public Action<Cell[], bool> callBack;

        public PathRequest(Cell _start, Cell _target, Action<Cell[], bool> _callBack)
        {
            pathStart = _start;
            pathTarget = _target;
            callBack = _callBack;
        }
    }
}
