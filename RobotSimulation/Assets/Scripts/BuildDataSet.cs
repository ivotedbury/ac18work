using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDataSet
{
    public int totalBricks;
    public int auxBricks;
    public int targetBricks;
    public int targetBricksFull;
    public int targetBricksHalf;
    public int auxBricksFull;
    public int auxBricksHalf;

    public List<Brick> bricksInStructure = new List<Brick>();

    public List<float> pathCountOut = new List<float>();
    public List<float> pathCountBack = new List<float>();
    public List<float> climbingOut = new List<float>();
    public List<float> climbingBack = new List<float>();
    public List<float> climbingOutAverage = new List<float>();
    public List<float> climbingBackAverage = new List<float>();
    public List<float> distanceOut = new List<float>();
    public List<float> distanceBack = new List<float>();
    public List<float> distanceOutAverage = new List<float>();
    public List<float> distanceBackAverage = new List<float>();


    public BuildDataSet(List<Brick> _bricksInStructure)
    {
        bricksInStructure = _bricksInStructure;

        foreach (Brick brick in bricksInStructure)
        {
            if (brick.auxBrick)
            {
                if (brick.brickType == 1)
                {
                    auxBricksFull++;
                }
                else if (brick.brickType == 2)
                {
                    auxBricksHalf++;
                }
            }

            else if (!brick.auxBrick)
            {
                if (brick.brickType == 1)
                {
                    targetBricksFull++;
                }
                else if (brick.brickType == 2)
                {
                    targetBricksHalf++;
                }
            }
        }

        auxBricks = auxBricksFull + auxBricksHalf;
        targetBricks = targetBricksFull + targetBricksHalf;
        totalBricks = auxBricks + targetBricks;
    }
}
