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

    List<LineRenderer> lines = new List<LineRenderer>();

    public TextAsset brickData;
    public TextAsset brickStackTemplate;
    public Material graphMaterial;

    BrickArrangement brickArrangement = new BrickArrangement(20, 20, 41);

    public Button generateGraphButton;
    public Button hideGraphButton;
    public Button createStackButton;
    public Button showPathButton;

    bool graphBranchesAreShowing = false;

    List<Color> gradientColours = new List<Color>();

    Vector3 lineDisplaySeparator = new Vector3(0, 0.3f, 0); // was 0.01f

    Vector3 gateCellOffset = new Vector3(0, 0.1f, 0);

    void Start()
    {
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

        Button generateGraphBut = generateGraphButton.GetComponent<Button>();
        generateGraphBut.onClick.AddListener(ShowGraph);

        Button hideGraphBut = hideGraphButton.GetComponent<Button>();
        hideGraphBut.onClick.AddListener(HideGraph);

        Button createStackBut = createStackButton.GetComponent<Button>();
        createStackBut.onClick.AddListener(CreateStack);

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
    }

    void ShowPath()
    {
        brickArrangement.FindPath();
        print(brickArrangement.arrangementGraph.availableCells.Count);
        print(brickArrangement.arrangementGraph.GetPathFinderNeighbours(brickArrangement.arrangementGraph.availableCells[20]));
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
        DestroyCellDisplay(brickArrangement);
        DestroyGraphBranches(brickArrangement);

        graphBranchesAreShowing = false;
        //brickArrangement.graphIsGenerated = false;
    }

    void InstantiateBricks(BrickArrangement inputBrickArrangement)
    {
        foreach (Brick brick in inputBrickArrangement.allBricks)
        {
            bricksInScene.Add(Instantiate(brickMesh, inputBrickArrangement.GetRealBrickPosition(brick), brick.rotation, brickMesh.transform.parent));
        }
    }

    void CreateStack()
    {
        brickArrangement.DepositBrick();

        if (graphBranchesAreShowing)
        {
            HideGraph();
            brickArrangement.GenerateGraph();
            ShowGraph();
        }
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
        foreach (LineRenderer line in lines)
        {
            Destroy(line);
        }

        lines.Clear();

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
            line.widthMultiplier = 0.01f;
            line.positionCount = 2;

            lines.Add(line);
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
                else if(brickArrangement.arrangementGraph.availableCells[i].isStart == true)
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
        for (int i = 0; i < lines.Count; i++)
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
                lines[i].material.color = gradientColours[4];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 3)
            {
                lines[i].material.color = gradientColours[5];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 4)
            {
                lines[i].material.color = gradientColours[6];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 5)
            {
                lines[i].material.color = gradientColours[7];
            }

            // upSteps
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 12)
            {
                lines[i].material.color = gradientColours[8];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 13)
            {
                lines[i].material.color = gradientColours[9];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 14)
            {
                lines[i].material.color = gradientColours[10];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == 15)
            {
                lines[i].material.color = gradientColours[11];
            }

            // downSteps
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -2)
            {
                lines[i].material.color = gradientColours[0];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -3)
            {
                lines[i].material.color = gradientColours[1];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -4)
            {
                lines[i].material.color = gradientColours[2];
            }
            if (inputBrickArrangement.arrangementGraph.graphBranches[i].type == -5)
            {
                lines[i].material.color = gradientColours[3];
            }

            lines[i].SetPosition(0, inputBrickArrangement.GetRealCellPosition(brickArrangement.arrangementGraph.graphBranches[i].start) + inputBrickArrangement.displayCellOffset + lineDisplaySeparatorFactor      + new Vector3(0,0.5f,0));
            lines[i].SetPosition(1, inputBrickArrangement.GetRealCellPosition(brickArrangement.arrangementGraph.graphBranches[i].end) + inputBrickArrangement.displayCellOffset + lineDisplaySeparatorFactor + new Vector3(0, 0.5f, 0));

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
