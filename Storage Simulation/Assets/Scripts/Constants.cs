using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    // ROBOT CONSTANTS

    public static Vector3Int[] ROBOT_STARTING_POSITION = new Vector3Int[] {

        new Vector3Int(0,0,0),
        new Vector3Int(1,0,0),
        new Vector3Int(2,0,0),
        new Vector3Int(3,0,0),
        new Vector3Int(4,0,0),
    };

    public static int[] ROBOT_STARTING_DIRECTION = new int[]
    {
        POSITIVE_X,
        POSITIVE_X,
        POSITIVE_X,
        POSITIVE_X,
        POSITIVE_X
    };

    public static int NUMBER_OF_ROBOTS = ROBOT_STARTING_POSITION.Length;

    // ROBOT STATES

    public static int idle = 0;
    public static int busy = 1;

    public static int WAIT = 0;
    public static int GOTO = 1;
    public static int PICKUP = 2;
    public static int DROPOFF = 3;

    public static float ROBOT_LINEAR_SPEED = 3f;
    public static float ROBOT_ANGULAR_SPEED = 500;
    public static float ROBOT_LIFT_SPEED = 0.1f;

    public static int DOWN = 0; // platform positions
    public static int UP = 1; // platform positions

    public static Vector3 CHASSIS_TRANSFORM = new Vector3(0, 0, 0);
    public static Vector3 L_WHEEL_TRANSFORM = new Vector3(-0.128f, 0.037f, 0);
    public static Vector3 R_WHEEL_TRANSFORM = new Vector3(0.128f, 0.037f, 0);
    public static Vector3 PLATFORM_TRANSFORM = new Vector3(0, 0.085f, 0);
    public static Vector3 RAISED_PLATFORM_HEIGHT = new Vector3(0, 0.03f, 0);

    public static float COLLISION_DISTANCE = 0.48f;


    // GRID 
    public static Vector3 GRID_DIMS = new Vector3(0.5f, 0.5f, 0.7f);

    // DIRECTIONS

    public static int POSITIVE_X = 0;
    public static int POSITIVE_Z = 1;
    public static int NEGATIVE_X = 2;
    public static int NEGATIVE_Z = 3;


    // MAIN STRUCTURE
    public static Vector3Int MAIN_STRUCTURE_DIMS = new Vector3Int(10, 1, 10);
    public static Vector2Int MAIN_STRUCTURE_BASE_DIMS = new Vector2Int(MAIN_STRUCTURE_DIMS.x, MAIN_STRUCTURE_DIMS.z);

    // COLOURS

    public static Color NODE_NORMAL = Color.black;
    public static Color NODE_SELECTED = Color.red;

    public static Color TOTE_NORMAL = new Color(0f, 0.4f, 1f, 1f);
    public static Color TOTE_SELECTED = new Color(0.6f, 0.8f, 1f, 1f);

    public static Color COLOUR_RED = new Color(200f / 255, 020f / 255f, 020f / 255f, 1f);
    public static Color COLOUR_RED_ORANGE = new Color(230f / 255, 060f / 255f, 030f / 255f, 1f);
    public static Color COLOUR_ORANGE = new Color(230f / 255, 100f / 255f, 030f / 255f, 1f);
    public static Color COLOUR_ORANGE_YELLOW = new Color(240f / 255, 150f / 255f, 030f / 255f, 1f);
    public static Color COLOUR_YELLOW = new Color(240f / 255, 230f / 255f, 000f / 255f, 1f);
    public static Color COLOUR_YELLOW_GREEN = new Color(076f / 255, 173f / 255f, 051f / 255f, 1f);
    public static Color COLOUR_GREEN = new Color(009f / 255, 100f / 255f, 050f / 255f, 1f);
    public static Color COLOUR_GREEN_BLUE = new Color(004f / 255, 150f / 255f, 150f / 255f, 1f);
    public static Color COLOUR_BLUE = new Color(030f / 255, 060f / 255f, 140f / 255f, 1f);
    public static Color COLOUR_BLUE_VIOLET = new Color(064f / 255, 060f / 255f, 145f / 255f, 1f);
    public static Color COLOUR_VIOLET = new Color(100f / 255, 060f / 255f, 145f / 255f, 1f);
    public static Color COLOUR_VIOLET_RED = new Color(224f / 255, 070f / 255f, 170f / 255f, 1f);



    // INSIDE TOTES

    public static float PRODUCT_SPACING = 0.09f;
    public static float TOTE_OFFSET_X = -1.5f * PRODUCT_SPACING;
    public static float TOTE_OFFSET_Z = -2.5f * PRODUCT_SPACING;
    public static Vector3 TOTE_RAISE = new Vector3(0, 0.75f, 0);

    public static float TOTE_OFFSET_HEIGHT = 0.14f;
    public static Vector3[] PRODUCT_TOTE_STACKING = new Vector3[12] {
      new Vector3((0f * PRODUCT_SPACING) + TOTE_OFFSET_X, (0f * PRODUCT_SPACING) + TOTE_OFFSET_HEIGHT, (0f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((1f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (0f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((2f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (0f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((3f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (0f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((0f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (1f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((1f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (1f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((2f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (1f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((3f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (1f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((0f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (2f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((1f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (2f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((2f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (2f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),
      new Vector3((3f * PRODUCT_SPACING) + TOTE_OFFSET_X, TOTE_OFFSET_HEIGHT, (2f * PRODUCT_SPACING)  + TOTE_OFFSET_Z),

      };

    public Vector3 GET_PRODUCT_POSITION(int _productNumber)
    {
        Vector3 productPosition = new Vector3(0, 0, 0);

        Vector3[] productStackingPositions = new Vector3[PRODUCT_X * PRODUCT_Y * PRODUCT_Z];

        int i = 0;

        for (int x = 0; x < PRODUCT_X; x++)
        {
            for (int z = 0; z < PRODUCT_Y; z++)
            {
                for (int y = 0; y < PRODUCT_Z; y++)
                {
                    productStackingPositions[i] = new Vector3((x * PRODUCT_SPACING) + TOTE_OFFSET_X, (y * PRODUCT_SPACING) + TOTE_OFFSET_HEIGHT, (z * PRODUCT_SPACING) + TOTE_OFFSET_Z);
                    i++;
                }
            }
        }

        productPosition = productStackingPositions[_productNumber];

        return productPosition;
    }

    public static int PRODUCT_X = 4;
    public static int PRODUCT_Y = 3;
    public static int PRODUCT_Z = 6;




    public static int PRODUCT_ID_RED = 0;
    public static int PRODUCT_ID_RED_ORANGE = 1;
    public static int PRODUCT_ID_ORANGE = 2;
    public static int PRODUCT_ID_ORANGE_YELLOW = 3;
    public static int PRODUCT_ID_YELLOW = 4;
    public static int PRODUCT_ID_YELLOW_GREEN = 5;
    public static int PRODUCT_ID_GREEN = 6;
    public static int PRODUCT_ID_GREEN_BLUE = 7;
    public static int PRODUCT_ID_BLUE = 8;
    public static int PRODUCT_ID_BLUE_VIOLET = 9;
    public static int PRODUCT_ID_VIOLET = 10;
    public static int PRODUCT_ID_VIOLET_RED = 11;

}
