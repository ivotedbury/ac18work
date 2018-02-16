using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public GameObject brickMesh;
    public GameObject brickCellMesh;
    public GameObject lineRendererObject;
    public GameObject gateCellMarkerMesh;

    List<GameObject> bricksInScene = new List<GameObject>();

    public TextAsset brickData;

    BrickArrangement _brickArrangement =  new BrickArrangement(20,20,41);



    void Start()
    {
        _brickArrangement.CreateBricksInArrangment(brickData);
        print(_brickArrangement.SetGateCell(new Vector3Int(10, 0, 10)));

        InstantateBricks(_brickArrangement);
       
        print(_brickArrangement.bricks.Count);
        print(_brickArrangement.bricks[2].originCell.position);
        print(_brickArrangement.bricks[2].childCells.Count);
        UpdateGateCell(_brickArrangement);
    }

    // Update is called once per frame
    void Update()
    {
       // UpdateBricks(_brickArrangement);
    }

    void InstantateBricks(BrickArrangement inputBrickArrangement)
    {
        foreach (Brick brick in inputBrickArrangement.bricks)
        {
            bricksInScene.Add(Instantiate(brickMesh, inputBrickArrangement.GetRealPosition(brick), brick.rotation));
        }
    }

    void UpdateBricks(BrickArrangement inputBrickArrangement)
    {
        for (int i = 0; i < inputBrickArrangement.bricks.Count; i ++)
        {
            bricksInScene[i].transform.position = inputBrickArrangement.GetRealPosition(inputBrickArrangement.bricks[i]);
            bricksInScene[i].transform.rotation = inputBrickArrangement.bricks[i].rotation;
        }
    }

    void UpdateGateCell(BrickArrangement inputBrickArrangement)
    {
        GameObject gateCellMarker = Instantiate(gateCellMarkerMesh, inputBrickArrangement.GetRealPosition(inputBrickArrangement.gateCell), Quaternion.identity);
    }

    }
