﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour {

    public Node[,] nodesArray = new Node[Constants.MAIN_STRUCTURE_DIMS.x,Constants.MAIN_STRUCTURE_DIMS.z];
    public Node node;

void Start()
    {
        for (int x = 0; x < Constants.MAIN_STRUCTURE_DIMS.x; x++)
        {
            for (int z = 0; z < Constants.MAIN_STRUCTURE_DIMS.z; z++)
            {
                nodesArray[x, z] = Instantiate (node, new Vector3 (x * Constants.GRID_DIMS.x, 0 , z * Constants.GRID_DIMS.z), node.transform.rotation, this.transform);
                nodesArray[x, z].name = "node (" + x.ToString() + ", " + z.ToString() + ")";
            }

        }
    }
}
