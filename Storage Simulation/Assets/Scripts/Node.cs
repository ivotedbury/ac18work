using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public Node(Vector3Int _nodePos)
    {
        this.transform.position = new Vector3(_nodePos.x * Constants.GRID_DIMS.x, _nodePos.y * Constants.GRID_DIMS.y, _nodePos.z * Constants.GRID_DIMS.z);
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
