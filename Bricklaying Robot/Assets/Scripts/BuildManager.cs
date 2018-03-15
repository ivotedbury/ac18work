using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager {

    BrickArrangement brickArrangement;

   public bool onDelivery = true;

    public BuildManager(BrickArrangement _inputBrickArrangment)
    {
        brickArrangement = _inputBrickArrangment;
    }

    public void NextBuildStep()
    {
        
        brickArrangement.FindPath(onDelivery);
        if (!onDelivery)
        {
            brickArrangement.DepositBrick();
        }
       
               onDelivery = !onDelivery;
        brickArrangement.GenerateGraph();
    }

    public void LayNextBrick()
    {
       
    }

    public void PickUpNextBrick()
    {

    }
}
