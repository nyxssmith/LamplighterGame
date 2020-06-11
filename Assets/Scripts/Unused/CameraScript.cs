

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
// Angle to clamp the camera at on the up/down axis
private const float Y_ANGLE_MIN = -50.0f;
private const float Y_ANGLE_MAX = 50.0f;

// The game object's transform for the camera to look at
public Transform target;

private Transform camTransform;

// Distance between the player and the camera
private float distanceBetween = 2f;
private float currentX = 0.0f;
private float currentY = 0.0f;

// How much the camera reacts to movement of the mouse
private float sensitivityX = 6.0f;
private float sensitivityY = 3.0f;

private void Start()
{
    camTransform = transform;
}

private void Update()
{
    currentX += Input.GetAxis("Mouse X") * sensitivityX;
    currentY -= Input.GetAxis("Mouse Y") * sensitivityY;

    currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
}

private void LateUpdate()
{
    int layerMask = 1 << 9;
    layerMask = ~layerMask;

    Vector3 dir = new Vector3(0.5f, 1.6f, -distanceBetween);
    Quaternion rot = Quaternion.Euler(currentY, currentX, 0);
    camTransform.position = target.position + rot * dir;


}
}