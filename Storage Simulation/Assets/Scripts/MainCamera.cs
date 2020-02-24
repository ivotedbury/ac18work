using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
    // camera mode
    int cameraMode;
    int robotToFollow = 0;

    public RobotManager robotManager;

    public float cameraSensitivity = 80;
    public float climbSpeed = 10;
    public float normalMoveSpeed = 20;
    public float slowMoveFactor = 10;
    public float fastMoveFactor = 15;

    private float rotationX;
    private float rotationY;

    private static float heightLimit = 0.1f;

    void Start()
    {
        Screen.lockCursor = false;

        rotationY = -transform.rotation.eulerAngles.x;
        rotationX = transform.rotation.eulerAngles.y;
        rotationY = Mathf.Clamp(rotationY, -90, 90);
        
        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        cameraMode = Constants.CAMERA_3D_VIEW;
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (cameraMode < Constants.CAMERA_MODE_LIMIT)
            {
                cameraMode++;
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (cameraMode > 0)
            {
                cameraMode--;
            }
        }

        if (cameraMode == Constants.CAMERA_ROBOT_FOLLOW)
        {
            transform.position = robotManager.allRobots[robotToFollow].transform.position + robotManager.allRobots[robotToFollow].transform.rotation * Constants.CAMERA_ROBOT_VIEW_OFFSET;
            transform.rotation = robotManager.allRobots[robotToFollow].transform.rotation;
        }

            if (Input.GetMouseButton(1))
            {
                rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
                rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, -90, 90);

                transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
            }

        transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor * Input.GetAxis("Mouse ScrollWheel")) * Time.deltaTime;

        if (Input.GetMouseButton(2))
        {
            transform.position -= transform.right * normalMoveSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            transform.position -= transform.up * normalMoveSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E))
        {
            if (transform.position.y >= heightLimit)
            {
                transform.position -= transform.up * climbSpeed * Time.deltaTime;
            }

        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Screen.lockCursor = (Screen.lockCursor == false) ? true : false;
        }
    }
}