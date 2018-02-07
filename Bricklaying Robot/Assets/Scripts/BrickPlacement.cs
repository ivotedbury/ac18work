using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BrickPlacement : MonoBehaviour
{

    public GameObject actualBrickMesh;
    public GameObject actualBrickPlane;
    public GameObject lineRendererObject;

    public Material lineMaterial1;
    public Material lineMaterial2;
    public Material lineMaterial3;
    public Material lineMaterial4;
    public Material lineMaterial5;
    public Material lineMaterial6;

    public List<Brick> brickList = new List<Brick>();
    public List<BrickPlane> planeList = new List<BrickPlane>();
    public List<BrickPlane> availablePlaneList = new List<BrickPlane>();
    public List<GraphBranch> branchList = new List<GraphBranch>();

    // brick stack for instantiation
    int width = 4;
    int height = 3;
    int length = 5;

    int numOfBricks;

    IEnumerator startBricks;

    bool move = false;
    bool graphIsGenerated = false;

    public Button generateGraphButton;
    public Button hideGraphButton;

    void Awake()
    {
        numOfBricks = width * height * length;
    }

    void Start()
    {
        //GUI
        Button generateGraphBut = generateGraphButton.GetComponent<Button>();
        generateGraphBut.onClick.AddListener(GenerateGraph);

        Button hideGraphBut = hideGraphButton.GetComponent<Button>();
        hideGraphBut.onClick.AddListener(HideGraph);

        //place bricks
        startBricks = depositBricks();
        StartCoroutine(startBricks);
    }

    IEnumerator depositBricks()
    {
        //create bricks
        for (int h = 0; h < height; h++)
        {
            for (int i = 0; i < (numOfBricks / height); i++)
            {
                brickList.Add(new Brick(actualBrickMesh, actualBrickPlane, new Vector3((i % width) * 0.1125f, h * 0.0725f, (i - (i % width)) / width * 0.225f), Quaternion.Euler(0, 0, 0)));
                brickList[i].spawnHeight = h;

                yield return new WaitForSeconds(0.02f);
            }
        }

        //add planes to overall plane list and assign their parent brick
        for (int k = 0; k < brickList.Count; k++)
        {
            for (int j = 0; j < brickList[k].localPlaneList.Length; j++)
            {
                planeList.Add(brickList[k].localPlaneList[j]);
                brickList[k].localPlaneList[j].parentBrick = brickList[k];
            }
        }
    }

    void Update()
    {
        MoveBricks();

        //update all positions
        for (int i = 0; i < numOfBricks; i++)
        {
            brickList[i].updatePosition();

            for (int j = 0; j < brickList[i].localPlaneList.Length; j++)
            {
                brickList[i].localPlaneList[j].updatePosition();
            }
        }

        UpdateGraph();
    }

    private void OnMouseDown()
    {
        
    }

    void MoveBricks()
    {

        // move bricks on mouse click
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            for (int i = 0; i < numOfBricks; i++)
            {

                if (Physics.Raycast(ray, out hit) && hit.collider.transform.position == brickList[i].position && move == false)
                {
                    hit.transform.position += Vector3.up;
                    brickList[i].brickObject.transform.position = new Vector3(brickList[i].brickObject.transform.position.x, (brickList[i].spawnHeight * 0.12f), brickList[i].brickObject.transform.position.z) + new Vector3(0, 0.05f, 1.125f);
                    print(i);
                    move = true;
                }

            }

        }
        else
        {
            move = false;
        }
    }

    Vector3 RoundPosition(Vector3 inputPosition)
    {
        //rounds location measurements to nearest snap value

        Vector3 roundedPosition = new Vector3(0, 0, 0);
        float snapValueX = 0.05625f;
        float snapValueY = 0.07250f;
        float snapValueZ = 0.05625f;

        roundedPosition.x = Mathf.Round(inputPosition.x / snapValueX) * snapValueX;
        roundedPosition.y = Mathf.Round((inputPosition.y - 0.03625f) / snapValueY) * snapValueY;
        roundedPosition.z = Mathf.Round(inputPosition.z / snapValueZ) * snapValueZ;

        return roundedPosition;
    }

    void GenerateGraph()
    {
        GenerateGraphList();
        GenerateGraphBranches();
    }

    void GenerateGraphList()
    {
        List<BrickPlane> coveredPlaneList = new List<BrickPlane>();
        availablePlaneList.Clear();

        HideGraph();

        for (int h = 0; h < planeList.Count; h++)
        {
            planeList[h].isAvailable = false;
            availablePlaneList.Add(planeList[h]);
        }

        for (int i = 0; i < availablePlaneList.Count; i++)
        {
            for (int j = 0; j < availablePlaneList.Count; j++)
            {

                if (RoundPosition(availablePlaneList[i].position) + new Vector3(0, 0.0725f, 0) == RoundPosition(availablePlaneList[j].position))
                {
                    coveredPlaneList.Add(availablePlaneList[i]);
                }
            }
        }

        for (int k = 0; k < coveredPlaneList.Count; k++)
        {
            if (availablePlaneList.Contains(coveredPlaneList[k]))
            {
                availablePlaneList.Remove(coveredPlaneList[k]);
            }
        }

        for (int m = 0; m < availablePlaneList.Count; m++)
        {
            availablePlaneList[m].isAvailable = true;
        }

        print("Graph2");
        print(availablePlaneList.Count);
    }

    void GenerateGraphBranches()
    {
        for (int i = 0; i < availablePlaneList.Count; i++)
        {
            for (int j = 0; j < availablePlaneList.Count; j++)
            {

                if (RoundPosition(availablePlaneList[i].position) + new Vector3(0.05625f, 0, 0) == RoundPosition(availablePlaneList[j].position) ||
                    RoundPosition(availablePlaneList[i].position) + new Vector3(-0.05625f, 0, 0) == RoundPosition(availablePlaneList[j].position) ||
                    RoundPosition(availablePlaneList[i].position) + new Vector3(0, 0, 0.05625f) == RoundPosition(availablePlaneList[j].position) ||
                    RoundPosition(availablePlaneList[i].position) + new Vector3(0, 0, -0.05625f) == RoundPosition(availablePlaneList[j].position))
                {
                    branchList.Add(new GraphBranch(availablePlaneList[i], availablePlaneList[j], lineMaterial1, "Horizontal 1", lineRendererObject));
                }

                if (RoundPosition(availablePlaneList[i].position) + new Vector3(0.1125f, 0, 0) == RoundPosition(availablePlaneList[j].position) ||
                    RoundPosition(availablePlaneList[i].position) + new Vector3(-0.1125f, 0, 0) == RoundPosition(availablePlaneList[j].position) ||
                    RoundPosition(availablePlaneList[i].position) + new Vector3(0, 0, 0.1125f) == RoundPosition(availablePlaneList[j].position) ||
                    RoundPosition(availablePlaneList[i].position) + new Vector3(0, 0, -0.1125f) == RoundPosition(availablePlaneList[j].position))
                {
                    branchList.Add(new GraphBranch(availablePlaneList[i], availablePlaneList[j], lineMaterial2, "Horizontal 2", lineRendererObject));
                }

                if (RoundPosition(availablePlaneList[i].position) + new Vector3(0.1125f, 0.07250f, 0) == RoundPosition(availablePlaneList[j].position) ||
                   RoundPosition(availablePlaneList[i].position) + new Vector3(-0.1125f, 0.07250f, 0) == RoundPosition(availablePlaneList[j].position) ||
                   RoundPosition(availablePlaneList[i].position) + new Vector3(0, 0.07250f, 0.1125f) == RoundPosition(availablePlaneList[j].position) ||
                   RoundPosition(availablePlaneList[i].position) + new Vector3(0, 0.07250f, -0.1125f) == RoundPosition(availablePlaneList[j].position))
                {
                    branchList.Add(new GraphBranch(availablePlaneList[i], availablePlaneList[j], lineMaterial3, "Stepdown 1", lineRendererObject));
                }

                if (RoundPosition(availablePlaneList[i].position) + new Vector3(0.1125f, 0, 0.1125f) == RoundPosition(availablePlaneList[j].position) ||
                  RoundPosition(availablePlaneList[i].position) + new Vector3(-0.1125f, 0, 0.1125f) == RoundPosition(availablePlaneList[j].position) ||
                  RoundPosition(availablePlaneList[i].position) + new Vector3(-0.1125f, 0, -0.1125f) == RoundPosition(availablePlaneList[j].position) ||
                  RoundPosition(availablePlaneList[i].position) + new Vector3(0.1125f, 0, -0.1125f) == RoundPosition(availablePlaneList[j].position))
                {
                    branchList.Add(new GraphBranch(availablePlaneList[i], availablePlaneList[j], lineMaterial4, "Diagonal 2", lineRendererObject));
                }

                if (RoundPosition(availablePlaneList[i].position) + new Vector3(0.1125f, 0.07250f, 0.1125f) == RoundPosition(availablePlaneList[j].position) ||
               RoundPosition(availablePlaneList[i].position) + new Vector3(-0.1125f, 0.07250f, 0.1125f) == RoundPosition(availablePlaneList[j].position) ||
               RoundPosition(availablePlaneList[i].position) + new Vector3(-0.1125f, 0.07250f, -0.1125f) == RoundPosition(availablePlaneList[j].position) ||
               RoundPosition(availablePlaneList[i].position) + new Vector3(0.1125f, 0.07250f, -0.1125f) == RoundPosition(availablePlaneList[j].position))
                {
                    branchList.Add(new GraphBranch(availablePlaneList[i], availablePlaneList[j], lineMaterial5, "Diagonal Stepdown", lineRendererObject));
                }
            }
        }
    }

    void HideGraph()
    {
        for (int g = 0; g < branchList.Count; g++)
        {
            Destroy(branchList[g].lineRendererObject);
        }

        branchList.Clear();
    }

    void UpdateGraph()
    {
        for (int i = 0; i < branchList.Count; i++)
        {
            branchList[i].UpdateGraph();
        }
    }

    public class GraphBranch
    {
        public BrickPlane start;
        public BrickPlane end;
        public GameObject lineRendererObject;

        string type;
        Material material;

        LineRenderer line;

        Vector3 offsetVector = new Vector3(0, 0.0025f, 0);

        public GraphBranch(BrickPlane _start, BrickPlane _end, Material _material, string _type, GameObject _lineRendererObject)
        {
            start = _start;
            end = _end;
            material = _material;
            type = _type;

            lineRendererObject = Instantiate(_lineRendererObject, new Vector3(0, 0, 0), Quaternion.identity);

            line = lineRendererObject.AddComponent<LineRenderer>();
            line.material = material;
            line.widthMultiplier = 0.01f;
            line.positionCount = 2;

        }

        public void UpdateGraph()
        {
            if (type == "Horizontal 1")
            {
                line.SetPosition(0, start.position + offsetVector);
                line.SetPosition(1, end.position + offsetVector);
            }

            else
            {
                line.SetPosition(0, start.position);
                line.SetPosition(1, end.position);
            }
        }
    }

    public class Brick
    {
        public GameObject brickObject;
        public Vector3 position;
        public Quaternion rotation;
        Vector3 offsetVector;

        public BrickPlane[] localPlaneList = new BrickPlane[3];

        int brickNumber;
        public int spawnHeight;

        public Brick(GameObject _brickMesh, GameObject _brickPlaneMesh, Vector3 _position, Quaternion _rotation)
        {
            position = _position;
            rotation = _rotation;

            brickObject = Instantiate(_brickMesh, position, rotation);

            for (int i = 0; i < localPlaneList.Length; i++)
            {
                localPlaneList[i] = new BrickPlane(_brickPlaneMesh, position, rotation, true, i);

            }
        }

        public void updatePosition()
        {
            position = brickObject.transform.position;
            rotation = brickObject.transform.rotation;
        }
    }

    public class BrickPlane
    {
        public GameObject brickPlane;
        public Vector3 position;
        public Quaternion rotation;

        Vector3 offsetVector;

        Color normalColour = new Color(0, 0, 1);
        Color availableColour = new Color(0, 1, 0);

        public Brick parentBrick;
        int planeNumber;

        public bool isAvailable = false;

        public bool onBrick;

        public BrickPlane(GameObject _brickPlaneMesh, Vector3 _position, Quaternion _rotation, bool _onBrick, int _planeNumber)
        {
            position = _position;
            rotation = _rotation;
            onBrick = _onBrick;

            planeNumber = _planeNumber;

            if (planeNumber == 0)
            {
                offsetVector = new Vector3(0, 0.03625f, -0.05625f);
            }

            if (planeNumber == 1)
            {
                offsetVector = new Vector3(0, 0.03625f, 0);
            }

            if (planeNumber == 2)
            {
                offsetVector = new Vector3(0, 0.03625f, 0.05625f);
            }

            brickPlane = Instantiate(_brickPlaneMesh, position + offsetVector, rotation);
            brickPlane.GetComponent<Renderer>().material.color = normalColour;
        }

        public void updatePosition()
        {
            position = parentBrick.position + parentBrick.rotation * offsetVector;
            rotation = parentBrick.rotation;
            brickPlane.transform.position = position;
            brickPlane.transform.rotation = rotation;

            if (isAvailable == true)
            {
                brickPlane.GetComponent<Renderer>().material.color = availableColour;
            }
            else
            {
                brickPlane.GetComponent<Renderer>().material.color = normalColour;
            }
        }
    }
}


