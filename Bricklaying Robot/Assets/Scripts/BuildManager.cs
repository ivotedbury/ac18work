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
        bool pathIsPossible = false;
        brickArrangement.GenerateGraph();

        pathIsPossible = brickArrangement.CheckPath();

        return pathIsPossible;
    }

    //public void TryAddingBricks()
    //{
    //    brickArrangement.GenerateBrickListToTry();
    //}
}
/* 
 if(!pathComplete)
{
 bestOption = pathFinder.

*/