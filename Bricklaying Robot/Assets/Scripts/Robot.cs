using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{

   public Cell position;
   public Quaternion rotation;
    BrickArrangement brickArrangement;

    public Robot(Cell startPosition, BrickArrangement inputBrickArrangement)
    {
        position = startPosition;
        brickArrangement = inputBrickArrangement;
        rotation = Quaternion.Euler(0,0,0);
    }

    public void FollowPath(List<Cell> inputPathToFollow)
    {

    }
}
