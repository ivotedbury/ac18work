using UnityEngine;
using System.Collections;

public class CameraTracker : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        transform.LookAt(target);
        transform.position = target.position + new Vector3(0, 2, -1);
    }
}