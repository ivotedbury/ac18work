using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3Int gridPos;

    public Node parent;

    public float gCost;
    public float hCost;
    public bool walkable;

    public float fCost()
    {
        return gCost + hCost;
    }

    void Start()
    {
        gCost = 0;
        hCost = 0;
        walkable = true;
    }

    void Update()
    {

    }

    void OnMouseOver()
    {
        GetComponent<Renderer>().material.color = Constants.NODE_SELECTED;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Constants.NODE_NORMAL;
    }

    private void OnMouseDown()
    {

    }

}
