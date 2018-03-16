using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager
{

    BrickArrangement brickArrangement;

    public bool onDelivery = true;


    public BuildManager(BrickArrangement _inputBrickArrangment)
    {
        brickArrangement = _inputBrickArrangment;
    }

    public void NextBuildStep()
    {
        brickArrangement.GenerateGraph();

        brickArrangement.FindPath(onDelivery);

        if (!onDelivery)
        {
            brickArrangement.DepositBrick();
        }

        onDelivery = !onDelivery;
    }

    public bool CheckPath()
    {
        bool pathFound = false;
        brickArrangement.GenerateGraph();
        
        pathFound = brickArrangement.CheckPath();

        return pathFound;
    }

    public void PickUpNextBrick()
    {
        
    }
}
/* 
 if(!pathComplete)
{
 bestOption = pathFinder.

*/