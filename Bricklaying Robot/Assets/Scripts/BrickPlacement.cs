using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class BrickPlacement : MonoBehaviour
{

    public GameObject actualBrickMesh;
    public GameObject actualBrickPlane;
    public GameObject lineRendererObject;
    public GameObject seedMarker;

    public Material lineMaterial1;
    public Material lineMaterial2;
    public Material lineMaterial3;
    public Material lineMaterial4;
    public Material lineMaterial5;
    public Material lineMaterial6;

    List<Brick> brickList = new List<Brick>();
    List<BrickPlane> planeList = new List<BrickPlane>();
    List<BrickPlane> availablePlaneList = new List<BrickPlane>();
    List<GraphBranch> branchList = new List<GraphBranch>();

    List<DataImport> importList = new List<DataImport>();

    // brick stack for instantiation
    int width = 4;
    int height = 3;
    int length = 5;

    int numOfBricks;

    // size of coordinate space
    int gridXCount = 20;
    int gridYCount = 30;
    int gridZCount = 41;


    Vector3[,,] gridLocation;

    //brick measurements
    float lengthSpacing = 0.225f;
    float widthSpacing = 0.1125f;
    float heightSpacing = 0.0725f;

    float horizontalGridSpacing = 0.05625f;
    float verticalGridSpacing = 0.0725f;

    Quaternion pointZ = Quaternion.Euler(0, 0, 0);
    Quaternion pointX = Quaternion.Euler(0, 90, 0);

    IEnumerator startBricks;

    bool move = false;
    bool graphIsGenerated = false;

    public Button generateGraphButton;
    public Button hideGraphButton;
    public Button createStackButton;

    public string brickData;

    Vector3 seedLocation;

    int counter = 1;

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

        Button createStackBut = createStackButton.GetComponent<Button>();
        createStackBut.onClick.AddListener(testBrickOrder);

        Load(brickData);

        //generate grid locations
        generateGrid();

        seedLocation = gridLocation[5, 0, 15]; //+ new Vector3(0.001f,0,0.02f);
        GameObject seedPoint = Instantiate(seedMarker, seedLocation, Quaternion.identity);

        //place bricks
        startBricks = depositBricks();
        StartCoroutine(startBricks);


    }

    void testBrickOrder()
    {

        brickList[brickList.Count - counter].brickObject.transform.position += new Vector3(2, 0, 0);
        counter += 1;
    }

    List<Brick> reorderBricks(List<Brick> bricksInStructure)
    {
        List<Brick> reorderedBrickList = new List<Brick>();
        float currentClosestDistance;
        float testDistance;
        int listLength = bricksInStructure.Count;

        print(listLength);

        for (int currentSearchLayer = 0; currentSearchLayer < 5; currentSearchLayer++)
        {
            for (int j = 0; j < listLength; j++)
            {
                currentClosestDistance = 100;

                for (int i = 0; i < bricksInStructure.Count; i++) // current closest brick 
                {

                    if (bricksInStructure[i].idealCurrentPosition.y == seedLocation.y + (currentSearchLayer * verticalGridSpacing)) // if Y is on current search layer
                    {

                        testDistance = Vector3.Magnitude(seedLocation - bricksInStructure[i].idealCurrentPosition);

                        if (testDistance < currentClosestDistance)
                        {
                            currentClosestDistance = testDistance;
                            print(testDistance);
                        }
                    }
                }

                for (int k = 0; k < bricksInStructure.Count; k++)
                {
                    if (currentClosestDistance == Vector3.Magnitude(seedLocation - bricksInStructure[k].idealCurrentPosition))
                    {
                        reorderedBrickList.Add(bricksInStructure[k]);

                        for (int h = 0; h < bricksInStructure[k].localPlaneList.Length; h++) // update parent of all brick planes
                        {
                            brickList[k].localPlaneList[h].parentBrick = reorderedBrickList[reorderedBrickList.Count - 1];
                        }

                    }

                }
                bricksInStructure.Remove(reorderedBrickList[reorderedBrickList.Count - 1]);
            }
        }

        print("brickListReordered");
        print(reorderedBrickList.Count);
        return reorderedBrickList;

    }


    bool Load(String textInput)
    {
        string temp_x;
        string temp_y;
        string temp_z;
        string temp_orientation;

        string line;

        // Create a new StreamReader, tell it which file to read and what encoding the file was saved as

        StreamReader theReader = new StreamReader(textInput, Encoding.Default);

        using (theReader)
        {
            // While there's lines left in the text file, do this:
            do
            {
                line = theReader.ReadLine();

                if (line != null)
                {
                    // split into arguments based on comma

                    string[] entries = line.Split(',');
                    if (entries.Length > 0)
                    {
                        temp_x = entries[0];
                        temp_y = entries[1];
                        temp_z = entries[2];
                        temp_orientation = entries[3];

                        importList.Add(new DataImport(temp_x, temp_y, temp_z, temp_orientation));
                    }
                }
            }
            while (line != null);

            // Done reading, close the reader and return true to broadcast success  
            theReader.Close();
            return true;
        }


    }


    IEnumerator depositBricks()
    {
        //create bricks
        int originX = 2;
        int originY = 0;
        int originZ = 30;

        WaitForSeconds time = new WaitForSeconds(0.05f);

        for (int i = 0; i < importList.Count; i++)
        {
            brickList.Add(new Brick(actualBrickMesh, actualBrickPlane, gridLocation[importList[i].xCoord + originX, importList[i].yCoord + originY, importList[i].zCoord + originZ], convertOrientation(importList[i].orientation)));

            for (int k = 0; k < brickList.Count; k++)
            {
                //add planes to overall plane list and assign their parent brick
                for (int j = 0; j < brickList[k].localPlaneList.Length; j++)
                {
                    planeList.Add(brickList[k].localPlaneList[j]);
                    brickList[k].localPlaneList[j].parentBrick = brickList[k];
                }
            }

            yield return time;

        }
        brickList = reorderBricks(brickList);
    }

    Quaternion convertOrientation(Boolean _input)
    {
        Quaternion output;
        if (_input == true)
        {
            output = pointZ;
        }
        else
        {
            output = pointX;
        }
        return output;
    }

    void Update()
    {
        MoveBricks();

        //update all positions
        for (int i = 0; i < brickList.Count; i++)
        {
            brickList[i].updatePosition();

            for (int j = 0; j < brickList[i].localPlaneList.Length; j++)
            {
                brickList[i].localPlaneList[j].updatePosition();
            }
        }

        UpdateGraph();
    }

    void generateGrid()
    {
        gridLocation = new Vector3[gridXCount, gridYCount, gridZCount];

        for (int z = 0; z < gridZCount; z++)
        {
            for (int y = 0; y < gridYCount; y++)
            {
                for (int x = 0; x < gridXCount; x++)
                {
                    gridLocation[x, y, z] = new Vector3(x * horizontalGridSpacing, y * verticalGridSpacing, z * horizontalGridSpacing);
                    //print(gridLocation[x, y, z].ToString("F5"));
                }
            }
        }
    }

    void MoveBricks()
    {

        // move bricks on mouse click
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && move == false)
            {
                for (int i = 0; i < brickList.Count; i++)
                {

                    if (hit.transform.position == brickList[i].position)
                    {
                        brickList[i].brickObject.transform.position = new Vector3(brickList[i].brickObject.transform.position.x, (brickList[i].spawnHeight * 0.12f), brickList[i].brickObject.transform.position.z) + new Vector3(0, 0.05f, 1.125f);
                        print(i + " hit");
                        move = true;
                    }
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

    public class DataImport
    {
        public int xCoord;
        public int yCoord;
        public int zCoord;
        public Boolean orientation;

        public DataImport(string _x, string _y, string _z, string _orientation)
        {
            xCoord = System.Int32.Parse(_x);
            yCoord = System.Int32.Parse(_y);
            zCoord = System.Int32.Parse(_z);
            orientation = _orientation.Equals("True") ? true : false;
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

        public Vector3 idealCurrentPosition;

        public BrickPlane[] localPlaneList = new BrickPlane[3];

        int brickNumber;
        public int spawnHeight;

        public Brick(GameObject _brickMesh, GameObject _brickPlaneMesh, Vector3 _position, Quaternion _rotation)
        {
            position = _position;
            rotation = _rotation;

            idealCurrentPosition = _position;

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


