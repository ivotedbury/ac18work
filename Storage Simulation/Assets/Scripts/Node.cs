using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3Int gridPos;

    public Node(Vector3Int _nodePos)
    {
        gridPos = _nodePos;
        this.transform.position = new Vector3(gridPos.x * Constants.GRID_DIMS.x, gridPos.y * Constants.GRID_DIMS.y, gridPos.z * Constants.GRID_DIMS.z);
    }

    void Start()
    {

    }

    // Update is called once per frame
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
