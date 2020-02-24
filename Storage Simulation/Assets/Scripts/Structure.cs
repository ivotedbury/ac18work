using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{

    public Node[,,] nodesArray = new Node[Constants.MAIN_STRUCTURE_DIMS.x, Constants.MAIN_STRUCTURE_DIMS.y, Constants.MAIN_STRUCTURE_DIMS.z];
    public Node node;
    public int structureId;

    void Start()
    {
        List<int> internalCorridors = new List<int>();

        for (int c = 0; c < Constants.MAIN_STRUCTURE_DIMS.x; c += Constants.SLOT_DEPTH + Constants.AISLE_WIDTH)
        {
            internalCorridors.Add(c);
        }

        for (int x = 0; x < Constants.MAIN_STRUCTURE_DIMS.x; x++)
        {
            for (int y = 0; y < Constants.MAIN_STRUCTURE_DIMS.y; y++)
            {
                for (int z = 0; z < Constants.MAIN_STRUCTURE_DIMS.z; z++)
                {
                    nodesArray[x, y, z] = Instantiate(node, new Vector3(x * Constants.GRID_DIMS.x, y * Constants.GRID_DIMS.y, z * Constants.GRID_DIMS.z), node.transform.rotation, this.transform);
                    nodesArray[x, y, z].name = "node (" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
                    nodesArray[x, y, z].gridPos = new Vector3Int(x, y, z);
                    nodesArray[x, y, z].SetupNode();

                    // 

                    if (x == 0
|| x == Constants.MAIN_STRUCTURE_DIMS.x - 1
|| z == 0
|| z == Constants.MAIN_STRUCTURE_DIMS.z - 1
|| internalCorridors.Contains(x))

                    {
                        nodesArray[x, y, z].corridor = true;
                    }

                }
            }
        }
    }
}
