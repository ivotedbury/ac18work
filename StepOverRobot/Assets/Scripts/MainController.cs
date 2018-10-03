using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

    public GameObject robotMeshes;
    public GameObject robotMeshContainer;

    public GameObject lineRendererObject;
    public GameObject lineRendererContainer;

    public Material gridLineMaterial;

    List<GameObject> allRobotMeshes = new List<GameObject>();
    List<GameObject> allLineRendererObjects = new List<GameObject>();
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    float gridXZDim = 0.05625f;
    float gridYDim = 0.0625f;

    Vector3Int gridSize = new Vector3Int(40, 40, 40);

    int numberOfRobots = 1;

    void Start()
    {


        CreateGridLines();
    }

    void Update()
    {

    }

    void CreateGridLines()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, new Vector3(x * gridXZDim, 0, 0));
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, new Vector3(x * gridXZDim, 0, (gridSize.z - 1) * gridXZDim));

        }

        for (int z = 0; z < gridSize.z; z++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, new Vector3(0, 0, z * gridXZDim));
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, new Vector3((gridSize.x - 1) * gridXZDim, 0, z * gridXZDim));
        }

        // pickup zone 

        //float lineWidth = 0.005f;
        //Vector3 lineOffset = new Vector3(0, lineWidth, 0);


        //for (int y = 0; y <= 1; y++)
        //{
        //    Vector3 actualLineOffset = lineOffset;

        //    if (y == 1)
        //    {
        //        actualLineOffset = new Vector3(0, 0, 0);
        //    }

        //    for (int x = -1; x <= 1; x++)
        //    {
        //        if (x == 0)
        //        {
        //            continue;
        //        }

        //        allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
        //        allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
        //        allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
        //        allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
        //        allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
        //        allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
        //        allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, gridXZDim) + brickDisplayOffset + actualLineOffset);
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, -gridXZDim) + brickDisplayOffset + actualLineOffset);
        //    }

        //    for (int z = -1; z <= 1; z++)
        //    {
        //        if (z == 0)
        //        {
        //            continue;
        //        }

        //        allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
        //        allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
        //        allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
        //        allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
        //        allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
        //        allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
        //        allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(-2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
        //    }
        //}

        //for (int x = -1; x <= 1; x++)
        //{
        //    if (x == 0)
        //    {
        //        continue;
        //    }

        //    for (int z = -1; z <= 1; z++)
        //    {
        //        if (z == 0)
        //        {
        //            continue;
        //        }

        //        allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
        //        allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
        //        allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
        //        allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
        //        allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
        //        allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
        //        allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, 0, z * gridXZDim) + brickDisplayOffset + lineOffset);
        //        allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildManager.brickStructure.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, gridYDim, z * gridXZDim) + brickDisplayOffset);
        //    }
        //}
    }
}
