using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3Int gridPos;

    public Node parent;

    public float gCost;
    public float hCost;
    public bool occupied;
    public bool corridor;

    public bool flowPosZ; // North
    public bool flowPosX; // East
    public bool flowNegZ; // South
    public bool flowNegX; // West

    GameObject linePosZ;
    GameObject linePosX;
    GameObject lineNegZ;
    GameObject lineNegX;

    LineRenderer posZ;
    LineRenderer posX;
    LineRenderer negZ;
    LineRenderer negX;

    void SetupFlowLines()
    {
        if (flowPosZ)
        {
            linePosZ = new GameObject();
            Instantiate(linePosZ, transform);
            posZ = linePosZ.AddComponent<LineRenderer>();
            posZ.positionCount = 2;
            posZ.startWidth = Constants.FLOW_LINE_WIDTH;
            posZ.SetPosition(0, transform.position + Constants.FLOW_LINE_OFFSET);
            posZ.SetPosition(1, transform.position + new Vector3(0, 0, Constants.FLOW_LINE_LENGTH) + Constants.FLOW_LINE_OFFSET);
            posZ.material = new Material(Shader.Find("Sprites/Default"));
            posZ.startColor = Constants.FLOW_LINE_COLOUR_POSZ;
            posZ.endColor = Constants.FLOW_LINE_COLOUR_POSZ;
        }
        if (flowPosX)
        {
            linePosX = new GameObject();
            Instantiate(linePosX, transform);
            posX = linePosX.AddComponent<LineRenderer>();
            posX.positionCount = 2;
            posX.startWidth = Constants.FLOW_LINE_WIDTH;
            posX.SetPosition(0, transform.position + Constants.FLOW_LINE_OFFSET);
            posX.SetPosition(1, transform.position + new Vector3(Constants.FLOW_LINE_LENGTH, 0, 0) + Constants.FLOW_LINE_OFFSET);
            posX.material = new Material(Shader.Find("Sprites/Default"));
            posX.startColor = Constants.FLOW_LINE_COLOUR_POSX;
            posX.endColor = Constants.FLOW_LINE_COLOUR_POSX;
        }
        if (flowNegZ)
        {
            lineNegZ = new GameObject();
            Instantiate(lineNegZ, transform);
            negZ = lineNegZ.AddComponent<LineRenderer>();
            negZ.positionCount = 2;
            negZ.startWidth = Constants.FLOW_LINE_WIDTH;
            negZ.SetPosition(0, transform.position + Constants.FLOW_LINE_OFFSET);
            negZ.SetPosition(1, transform.position + new Vector3(0, 0, -1 * Constants.FLOW_LINE_LENGTH) + Constants.FLOW_LINE_OFFSET);
            negZ.material = new Material(Shader.Find("Sprites/Default"));
            negZ.startColor = Constants.FLOW_LINE_COLOUR_NEGZ;
            negZ.endColor = Constants.FLOW_LINE_COLOUR_NEGZ;
        }
        if (flowNegX)
        {
            lineNegX = new GameObject();
            Instantiate(lineNegX, transform);
            negX = lineNegX.AddComponent<LineRenderer>();
            negX.positionCount = 2;
            negX.startWidth = Constants.FLOW_LINE_WIDTH;
            negX.SetPosition(0, transform.position + Constants.FLOW_LINE_OFFSET);
            negX.SetPosition(1, transform.position + new Vector3(-1 * Constants.FLOW_LINE_LENGTH, 0, 0) + Constants.FLOW_LINE_OFFSET);
            negX.material = new Material(Shader.Find("Sprites/Default"));
            negX.startColor = Constants.FLOW_LINE_COLOUR_NEGX;
            negX.endColor = Constants.FLOW_LINE_COLOUR_NEGX;
        }
    }

    public float fCost()
    {
        return gCost + hCost;
    }

    public void SetupNode()
    {
        gCost = 0;
        hCost = 0;
        corridor = false;
        occupied = false;

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

        SetupFlowLines();
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
