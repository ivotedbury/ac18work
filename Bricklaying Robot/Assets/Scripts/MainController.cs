using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public TextAsset brickData;

    BrickArrangment _brickArrangement =  new BrickArrangment(20,20,41);

    void Start()
    {
        _brickArrangement.CreateBricksInArrangment(brickData);
       
        print(_brickArrangement.bricks.Count);
        print(_brickArrangement.bricks[2].originCell.position);
        print(_brickArrangement.bricks[2].childCells.Count);

        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
