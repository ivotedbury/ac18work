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

    public bool flowPosZ; // North
    public bool flowPosX; // East
    public bool flowNegZ; // South
    public bool flowNegX; // West



    public float fCost()
    {
        return gCost + hCost;
    }

    public void SetupNode()
    {
        gCost = 0;
        hCost = 0;
        walkable = true;

        if (gridPos.x == 0)
        {
            flowPosZ = true;
        }

        else
        {
            flowNegZ = true;
        }
        
        if (gridPos.z == Constants.MAIN_STRUCTURE_DIMS.z - 1)
        {
            flowPosX = true;
        }

        if (gridPos.z == 0)
        {
            flowNegX = true;
        }
    }

    void Start()
    {

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
