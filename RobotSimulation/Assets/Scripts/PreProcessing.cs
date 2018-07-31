﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreProcessing : MonoBehaviour
{
    public GameObject fullBrickMesh;
    public GameObject halfBrickMesh;
    public GameObject brickContainer;

    public GameObject cellMarkerContainer;
    public GameObject cellMarker;

    public GameObject lineRendererObject;
    public GameObject lineRendererContainer;

    public TextAsset brickImportData;

    public Material gridLineMaterial;

    Material robotMaterialMain;
    Material robotMaterialHighlight;

    List<GameObject> allBrickMeshes = new List<GameObject>();
    List<GameObject> allCellMarkers = new List<GameObject>();
    List<GameObject> allLineRendererObjects = new List<GameObject>();
    List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    BuildSequence buildSequence;

    Vector3Int gridSize = new Vector3Int(75, 20, 75);
    Vector3Int seedPosition = new Vector3Int(20, 1, 20);

    Vector3 brickDisplayOffset = new Vector3(0, -0.0625f, 0);
    float gridXZDim = 0.05625f;
    float gridYDim = 0.0625f;

    float timeScaleFactor = 10f;
    float overallTime = 0;

    void Start()
    {
        SetupUI();

        buildSequence = new BuildSequence(gridSize, seedPosition, brickImportData);

        for (int i = 0; i < buildSequence.completeStructure.Count; i++)
        {

            if (buildSequence.completeStructure[i].brickType == 1)
            {
                allBrickMeshes.Add(Instantiate(fullBrickMesh, buildSequence.completeStructure[i].originCell.actualPosition + brickDisplayOffset, buildSequence.completeStructure[i].rotation));

            }

            else if (buildSequence.completeStructure[i].brickType == 2)
            {
                allBrickMeshes.Add(Instantiate(halfBrickMesh, buildSequence.completeStructure[i].originCell.actualPosition + brickDisplayOffset, buildSequence.completeStructure[i].rotation));
            }

            //make aux bricks red
            if (buildSequence.completeStructure[i].auxBrick)
            {
                allBrickMeshes[i].GetComponent<Renderer>().material.color = Color.red;
            }

            allBrickMeshes[i].transform.SetParent(brickContainer.transform);
        }

        CreateGridLines();

        UpdateCellDisplay();
    }

    private void OnGUI()
    {

    }

    void SetupUI()
    {

    }

    void CreateGridLines()
    {
        for (int x = 0; x < buildSequence.grid.gridSize.x; x++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.grid.cellsArray[x, 1, 0].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.grid.cellsArray[x, 1, buildSequence.grid.gridSize.z - 1].actualPosition + brickDisplayOffset);

        }

        for (int z = 0; z < buildSequence.grid.gridSize.z; z++)
        {
            allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
            allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);

            allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
            allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
            allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = 0.005f;
            allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.grid.cellsArray[0, 1, z].actualPosition + brickDisplayOffset);
            allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.grid.cellsArray[buildSequence.grid.gridSize.x - 1, 1, z].actualPosition + brickDisplayOffset);
        }

        // pickup zone 

        float lineWidth = 0.005f;
        Vector3 lineOffset = new Vector3(0, lineWidth, 0);


        for (int y = 0; y <= 1; y++)
        {
            Vector3 actualLineOffset = lineOffset;

            if (y == 1)
            {
                actualLineOffset = new Vector3(0, 0, 0);
            }

            for (int x = -1; x <= 1; x++)
            {
                if (x == 0)
                {
                    continue;
                }

                allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
                allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
                allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
                allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
                allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
                allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
                allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, gridXZDim) + brickDisplayOffset + actualLineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, y * gridYDim, -gridXZDim) + brickDisplayOffset + actualLineOffset);
            }

            for (int z = -1; z <= 1; z++)
            {
                if (z == 0)
                {
                    continue;
                }

                allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
                allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
                allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
                allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
                allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
                allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
                allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.seedCell.actualPosition + new Vector3(2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.seedCell.actualPosition + new Vector3(-2 * gridXZDim, y * gridYDim, z * gridXZDim) + brickDisplayOffset + actualLineOffset);
            }
        }

        for (int x = -1; x <= 1; x++)
        {
            if (x == 0)
            {
                continue;
            }

            for (int z = -1; z <= 1; z++)
            {
                if (z == 0)
                {
                    continue;
                }

                allLineRendererObjects.Add(Instantiate(lineRendererObject, lineRendererObject.transform.position, lineRendererObject.transform.rotation));
                allLineRendererObjects[allLineRendererObjects.Count - 1].transform.SetParent(lineRendererContainer.transform);
                allLineRenderers.Add(allLineRendererObjects[allLineRendererObjects.Count - 1].AddComponent<LineRenderer>());
                allLineRenderers[allLineRenderers.Count - 1].material = gridLineMaterial;
                allLineRenderers[allLineRenderers.Count - 1].material.color = Color.red;
                allLineRenderers[allLineRenderers.Count - 1].widthMultiplier = lineWidth;
                allLineRenderers[allLineRenderers.Count - 1].positionCount = 2;
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(0, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, 0, z * gridXZDim) + brickDisplayOffset + lineOffset);
                allLineRenderers[allLineRenderers.Count - 1].SetPosition(1, buildSequence.seedCell.actualPosition + new Vector3(x * 2 * gridXZDim, gridYDim, z * gridXZDim) + brickDisplayOffset);
            }
        }
    }

    void UpdateCellDisplay()
    {
        foreach (GameObject cellMarker in allCellMarkers)
        {
            Destroy(cellMarker);
        }

        allCellMarkers.Clear();

        foreach (Cell cell in buildSequence.availableCells)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            allCellMarkers[allCellMarkers.Count - 1].GetComponent<Renderer>().material.color = Color.yellow;

            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }

        foreach (Cell cell in buildSequence.forbiddenCells)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            allCellMarkers[allCellMarkers.Count - 1].GetComponent<Renderer>().material.color = Color.magenta;
            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }

        foreach (Cell cell in buildSequence.desiredPath)
        {
            allCellMarkers.Add(Instantiate(cellMarker, cell.actualPosition + new Vector3(0, 0.001f, 0), Quaternion.identity));
            allCellMarkers[allCellMarkers.Count - 1].GetComponent<Renderer>().material.color = Color.blue;
            allCellMarkers[allCellMarkers.Count - 1].transform.parent = cellMarkerContainer.transform;
        }
    }


    void Update()
    {

    }

}
