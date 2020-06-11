using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO load vars from json
public class playerMove3 : MonoBehaviour {

    
 public float mouseSpeed = 3;
     public Transform player;
     //public Camera yourCam;

    private Camera cam;

    public float upper = 9.0F;

    public bool IsGrounded = false; // boolean for jump

    public Rigidbody rb;


    //TODO stamina rate
    public float stamina = 1000f;
    public float maxStamina = 1000f;
    //speed added to the base speed
    public float staminaBonusSpeed = 4f;

    //stamina percent gained back per update
    public float staminaRechargeRate = .001f;

    // number of stamina used per per update
    public float staminaUseRate = 6f;

    //base movement speed
    public float speed = 2.5f; //actual speed TODO doesnt need be public
    public float baseMovementSpeed = 2.5f;


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


    //if moving forward, slightly fasater
    if(Input.GetKey("w")){
        speed = baseMovementSpeed *1.15f;
    }
    
    //sprinting

    if(Input.GetKey(KeyCode.LeftShift) && stamina > 1f){
        stamina = stamina - staminaUseRate;
        speed = baseMovementSpeed + (staminaBonusSpeed *(stamina/maxStamina*.7f));
    }
    else
    {
        if(stamina < maxStamina){
            if(stamina < 0){
                stamina = 1;
            }else{
                stamina = stamina + stamina*staminaRechargeRate;
            }
        }
        speed = baseMovementSpeed;
    }
    

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


        rb = GetComponent<Rigidbody> ();

float DisstanceToTheGround = GetComponent<Collider>().bounds.extents.y;
 
         bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DisstanceToTheGround + 0.1f);

    if ((IsGrounded == true) && (Input.GetButton("Jump"))) //if canjump boolean is true and if the player press the button jump , the player can jump.
            {
                Vector3 up = new Vector3 (0.0f, upper, 0.0f); // script for jumping
                //rb.AddForce (up * upper);
                rb.AddForce(up*upper);
            }



        //mouse look rotation
        float X = Input.GetAxis("Mouse X") * mouseSpeed;
         float Y = Input.GetAxis("Mouse Y") * mouseSpeed;
 
         player.Rotate(0, X, 0); // Player rotates on Y axis, your Cam is child, then rotates too
 
 
         // To scurity check to not rotate 360º 
         if (cam.transform.eulerAngles.x + (-Y) > 80 && cam.transform.eulerAngles.x + (-Y) < 280)
         { }
         else
         {
 
             cam.transform.RotateAround(player.position, cam.transform.right, -Y);
         }


    }
 
}

