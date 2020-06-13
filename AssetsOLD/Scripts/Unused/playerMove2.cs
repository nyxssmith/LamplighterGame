using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMove2 : MonoBehaviour {

    private Camera cam;

    private void Start ()
    {
    cam = Camera.main;
    }

    private void FixedUpdate ()
    {
    Move();
    }

    private void Move ()
    {
    // Getting the direction to move through player input
    float hMove = Input.GetAxis("Horizontal");
    float vMove = Input.GetAxis("Vertical");
    float speed = 5.0f;

    // Get directions relative to camera
    Vector3 forward = cam.transform.forward;
    Vector3 right = cam.transform.right;

    // Project forward and right direction on the horizontal plane (not up and down), then
    // normalize to get magnitude of 1
    forward.y = 0;
    right.y = 0;
    forward.Normalize();
    right.Normalize();

    // Set the direction for the player to move
    Vector3 dir = right * hMove + forward * vMove;

    // Set the direction's magnitude to 1 so that it does not interfere with the movement speed
    dir.Normalize();

    // Move the player by the direction multiplied by speed and delta time 
    transform.position += dir * speed * Time.deltaTime;

    // Set rotation to direction of movement if moving
    if (dir != Vector3.zero)
    {   
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), 0.2f);
    }

    }

}

