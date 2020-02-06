using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    // ROBOT CONSTANTS
    public static float ROBOT_LINEAR_SPEED = 0.5f;
    public static float ROBOT_ANGULAR_SPEED = 1000f;
    public static float ROBOT_LIFT_SPEED = 0.1f;

    public static int DOWN = 0; // platform positions
    public static int UP = 1; // platform positions

    public static Vector3 CHASSIS_TRANSFORM = new Vector3(0, 0, 0);
    public static Vector3 L_WHEEL_TRANSFORM = new Vector3(-0.128f, 0.037f, 0);
    public static Vector3 R_WHEEL_TRANSFORM = new Vector3(0.128f, 0.037f, 0);
    public static Vector3 PLATFORM_TRANSFORM = new Vector3(0, 0.085f, 0);
    public static Vector3 RAISED_PLATFORM_HEIGHT = new Vector3(0, 0.03f, 0);


    // GRID 
    public static Vector3 GRID_DIMS = new Vector3(0.5f,0.5f,0.7f);

    // DIRECTIONS

    public static int POSITIVE_X = 0;
    public static int POSITIVE_Z = 1;
    public static int NEGATIVE_X = 2;
    public static int NEGATIVE_Z = 3;

    // MAIN STRUCTURE
    public static Vector3Int MAIN_STRUCTURE_DIMS = new Vector3Int(10, 10, 10);
    public static Vector2Int MAIN_STRUCTURE_BASE_DIMS = new Vector2Int(MAIN_STRUCTURE_DIMS.x, MAIN_STRUCTURE_DIMS.z);

    // COLOURS

    public static Color NODE_NORMAL = Color.black;
    public static Color NODE_SELECTED = Color.red;

    public static Color TOTE_NORMAL = new Color(0f, 0.4f, 1f, 1f);
    public static Color TOTE_SELECTED = new Color(0.6f,0.8f,1f,1f);



}
