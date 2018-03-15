using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public GameObject brickMesh;
    public GameObject brickCellMesh;
    public GameObject lineRendererObject;
    public GameObject gateCellMarkerMesh;

    List<GameObject> bricksInScene = new List<GameObject>();
    List<GameObject> availableCellsInScene = new List<GameObject>();
    List<GameObject> graphBranchesInScene = new List<GameObject>();
    List<GameObject> pathLinesInScene = new List<GameObject>();

    List<LineRenderer> graphLines = new List<LineRenderer>();
    List<LineRenderer> pathLines = new List<LineRenderer>();

    public TextAsset brickData;
    public TextAsset brickStackTemplate;
    public Material graphMaterial;

    BrickArrangement brickArrangement = new BrickArrangement(20, 20, 41);
    BuildManager buildManager;


    public Button showGraphButton;
    public Button hideGraphButton;
    public Button placeNextButton;
    public Button showPathButton;
    public Toggle onDeliveryToggle;

    bool graphBranchesAreShowing = false;

    List<Color> gradientColours = new List<Color>();

    Vector3 lineDisplaySeparator = new Vector3(0, 0.01f, 0); // was 0.01f

    Vector3 gateCellOffset = new Vector3(0, 0.1f, 0);

    void Start()
    {
        buildManager = new BuildManager(brickArrangement);

        GenerateColours(12);

        print(brickArrangement.SetGateCell(new Vector3Int(2, 0, 14)));

        brickArrangement.CreateBricksInArrangment(brickData);
        brickArrangement.CreateStack(brickStackTemplate);

        InstantiateBricks(brickArrangement);


        print(brickArrangement.allBricks.Count);


        print(brickArrangement.allBricks.Count);
        print(brickArrangement.allBricks[0].originCell.position);

        print(brickArrangement.finalBricks.Count);
        print(brickArrangement.finalBricks[0].originCell.position);

        UpdateGateCell(brickArrangement);

        Button showGraphBut = showGraphButton.GetComponent<Button>();
        showGraphBut.onClick.AddListener(ShowGraph);

        Button hideGraphBut = hideGraphButton.GetComponent<Button>();
        hideGraphBut.onClick.AddListener(HideGraph);

        Button placeNextkBut = placeNextButton.GetComponent<Button>();
        placeNextkBut.onClick.AddListener(PlaceNextBrick);

        Button showPathBut = showPathButton.GetComponent<Button>();
        showPathBut.onClick.AddListener(ShowPath);


    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < brickArrangement.allBricks.Count; i++)
        {
            bricksInScene[i].transform.position = brickArrangement.GetRealBrickPosition(brickArrangement.allBricks[i]);
            bricksInScene[i].transform.rotation = brickArrangement.allBricks[i].rotation;
        }

        UpdateCellDisplay(brickArrangement);
        UpdateToggle();
    }

    void UpdateToggle()
    {
        Toggle onDeliveryTog = onDeliveryToggle.GetComponent<Toggle>();

        if (buildManager.onDelivery)
        {
            onDeliveryTog.isOn = true;
        }
        else if (!buildManager.onDelivery)
        {
            onDeliveryTog.isOn = false;
        }
    }

    void ShowPath()
    {
        //  brickArrangement.FindPath();

        DestroyPathLines();
        DrawPathLine(brickArrangement.currentPath);

        print(brickArrangement.arrangementGraph.availableCells.Count);
        print(brickArrangement.arrangementGraph.GetPathFinderNeighbours(brickArrangement.arrangementGraph.availableCells[1]).Count);
    }

    void DrawPathLine(List<Cell> pathCellList)
    {
        for (int i = 0; i < pathCellList.Count - 1; i++)
        {
            pathLinesInScene.Add(Instantiate(lineRendererObject, lineRendererObject.transform.parent));
        }

        for (int i = 0; i < pathLinesInScene.Count; i++)
        {
            var line = pathLinesInScene[i].AddComponent<LineRenderer>();
            line.material = graphMaterial;
            line.widthMultiplier = 0.02f;
            line.positionCount = 2;

            if (buildManager.onDelivery)
            {
                line.material.color = Color.green;
            }
            else
            {
                line.material.color = Color.red;
            }

            pathLines.Add(line);


            line.SetPosition(0, brickArrangement.GetRealCellPosition(pathCellList[i]) + brickArrangement.displayCellOffset + new Vector3(0, 0.05f, 0));
            line.SetPosition(1, brickArrangement.GetRealCellPosition(pathCellList[i + 1]) + brickArrangement.displayCellOffset + new Vector3(0, 0.05f, 0));
        }
    }

    void DestroyPathLines()
    {
        foreach (LineRenderer pathLine in pathLines)
        {
            Destroy(pathLine);
        }

        pathLines.Clear();

        foreach (GameObject pathLine in pathLinesInScene)
        {
            Destroy(pathLine);
        }

        pathLinesInScene.Clear();
    }

    void ShowGraph()
    {
        //brickArrangement.GenerateGraph();
        InstantiateCellDisplay(brickArrangement);
        InstantiateGraphBranches(brickArrangement);
        UpdateLineDisplay(brickArrangement);
    }

    void HideGraph()
    {
        // DestroyCellDisplay(brickArrangement);
        DestroyGraphBranches(brickArrangement);

        graphBranchesAreShowing = false;

    }

    void InstantiateBricks(BrickArrangement inputBrickArrangement)
    {
        foreach (Brick brick in inputBrickArrangement.allBricks)
        {
            bricksInScene.Add(Instantiate(brickMesh, inputBrickArrangement.GetRealBrickPosition(brick), brick.rotation, brickMesh.transform.parent));
        }
        brickArrangement.GenerateGraph();

        InstantiateCellDisplay(inputBrickArrangement);
    }

    void PlaceNextBrick()
    {
        buildManager.NextBuildStep();


        if (graphBranchesAreShowing)
        {
            HideGraph();
            brickArrangement.GenerateGraph();
            ShowGraph();
        }

        brickArrangement.GenerateGraph();
        DestroyCellDisplay(brickArrangement);
        InstantiateCellDisplay(brickArrangement);
        ShowPath();

    }

    void UpdateBricks(BrickArrangement inputBrickArrangement)
    {
        for (int i = 0; i < inputBrickArrangement.finalBricks.Count; i++)
        {
            bricksInScene[i].transform.position = inputBrickArrangement.GetRealBrickPosition(inputBrickArrangement.finalBricks[i]);
            bricksInScene[i].transform.rotation = inputBrickArrangement.finalBricks[i].rotation;
        }
    }

    void InstantiateCellDisplay(BrickArrangement inputBrickArrangement)
    {
        foreach (Cell cell in brickArrangement.arrangementGraph.availableCells)
        {
            availableCellsInScene.Add(Instantiate(brickCellMesh, inputBrickArrangement.GetRealCellPosition(cell) + inputBrickArrangement.displayCellOffset, Quaternion.identity, brickCellMesh.transform.parent));
        }
    }

    void DestroyCellDisplay(BrickArrangement inputBrickArrangement)
    {
        foreach (GameObject cell in availableCellsInScene)
        {
            Destroy(cell);
        }

        availableCellsInScene.Clear();
    }

    void DestroyGraphBranches(BrickArrangement inputBrickArrangement)
    {
        foreach (LineRenderer line in graphLines)
        {
            Destroy(line);
        }

        graphLines.Clear();

        foreach (GameObject graphBranch in graphBranchesInScene)
        {
            Destroy(graphBranch);
        }

        graphBranchesInScene.Clear();
    }

    void InstantiateGraphBranches(BrickArrangement inputBrickArrangement)
    {
        foreach (GraphBranch branch in brickArrangement.arrangementGraph.graphBranches)
        {
            graphBranchesInScene.Add(Instantiate(lineRendererObject, lineRendererObject.transform.parent));
        }

        for (int i = 0; i < graphBranchesInScene.Count; i++)
        {
            var line = graphBranchesInScene[i].AddComponent<LineRenderer>();
            line.material = graphMaterial;
            line.widthMultiplier = 0.005f;
            line.positionCount = 2;

            graphLines.Add(line);
        }

        graphBranchesAreShowing = true;
    }

    void UpdateCellDisplay(BrickArrangement inputBrickArrangement)
    {
        if (graphBranchesAreShowing == true)
        {
            for (int i = 0; i < brickArrangement.arrangementGraph.availableCells.Count; i++)
            {
                // availableCellsInScene[i].transform.position = inputBrickArrangement.GetRealCellPosition(brickArrangement.arrangementGraph.availableCells[i]);
                if (brickArrangement.arrangementGraph.availableCells[i].isPath == true)
                {
                    availableCellsInScene[i].GetComponent<Renderer>().material.color = Color.yellow;
                }
                else if (brickArrangement.arrangementGraph.availableCells[i].isStart == true)
                {
                    availableCellsInScene[i].GetComponent<Renderer>().material.color = Color.green;
                }
                else if (brickArrangement.arrangementGraph.availableCells[i].isEnd == true)
                {
                    availableCellsInScene[i].GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    availableCellsInScene[i].GetComponent<Renderer>().material.color = Color.grey;
                }
            }
        }
    }

    void UpdateLineDisplay(BrickArrangement inputBrickArrangement)
    {
        for (int i = 0; i < graphLines.Count; i++)
        {
            Vector3 lineDisplaySeparatorFactor = new Vector3(0, 0, 0);

            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type >= 2 && inputBrickArrangement.arrangementGraph.graphBranches[i].type <= 5)
            {
                lineDisplaySeparatorFactor = lineDisplaySeparator * 1;
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type >= 12 && inputBrickArrangement.arrangementGraph.graphBranches[i].type <= 15)
            {
                lineDisplaySeparatorFactor = lineDisplaySeparator * 2;
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type >= -5 && inputBrickArrangement.arrangementGraph.graphBranches[i].type <= -2)
            {
                lineDisplaySeparatorFactor = lineDisplaySeparator * 0;
            }

            // horizontalSteps
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 2)
            {
                graphLines[i].material.color = gradientColours[4];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 3)
            {
                graphLines[i].material.color = gradientColours[5];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 4)
            {
                graphLines[i].material.color = gradientColours[6];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 5)
            {
                graphLines[i].material.color = gradientColours[7];
            }

            // upSteps
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 12)
            {
                graphLines[i].material.color = gradientColours[8];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 13)
            {
                graphLines[i].material.color = gradientColours[9];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 14)
            {
                graphLines[i].material.color = gradientColours[10];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 15)
            {
                graphLines[i].material.color = gradientColours[11];
            }

            // downSteps
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -2)
            {
                graphLines[i].material.color = gradientColours[0];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -3)
            {
                graphLines[i].material.color = gradientColours[1];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -4)
            {
                graphLines[i].material.color = gradientColours[2];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -5)
            {
                graphLines[i].material.color = gradientColours[3];
            }

            graphLines[i].SetPosition(0, inputBrickArrangement.GetRealCellPosition(brickArrangement.arrangementGraph.graphBranches[i].start) + inputBrickArrangement.displayCellOffset + lineDisplaySeparatorFactor + new Vector3(0, 0.5f, 0));
            graphLines[i].SetPosition(1, inputBrickArrangement.GetRealCellPosition(brickArrangement.arrangementGraph.graphBranches[i].end) + inputBrickArrangement.displayCellOffset + lineDisplaySeparatorFactor + new Vector3(0, 0.5f, 0));

        }
    }

    void UpdateGateCell(BrickArrangement inputBrickArrangement)
    {
        GameObject gateCellMarker = Instantiate(gateCellMarkerMesh, inputBrickArrangement.GetRealPosition(inputBrickArrangement.gateCell) + gateCellOffset, Quaternion.identity);
    }

    void GenerateColours(float gradientSwatches)
    {
        for (int i = 0; i < gradientSwatches; i++)
        {
            gradientColours.Add(Color.HSVToRGB((i / gradientSwatches), 1, 1));
        }
    }
}
