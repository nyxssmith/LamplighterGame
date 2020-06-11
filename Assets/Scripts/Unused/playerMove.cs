using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMove : MonoBehaviour {

    public Rigidbody rb;
    public float forwardForce = 2f;
    public float sidewayForce = 1f;
    public float jumpForce = 1f;
    public float DashDistance = 2f;

    private string forard_key = "w";
    private string backward_key = "s";
    private string left_key = "a";
    private string right_key = "d";
    private string jump_key = "space";
    //private string dash_key = "shift";
    private bool _isGrounded = true;


    public float GroundDistance = 0.2f;
    public LayerMask Ground;


 public float mouseSpeed = 3;
     public Transform player;
     public Camera yourCam;

	// Use this for initialization
	void Start () {

       // rb.AddForce(0, 200, 500);
	}
	
	// Update is called once per frame
	
    private void FixedUpdate()
    {
        if(Input.GetKey(forard_key)){
            rb.AddForce(0, 0, forwardForce * Time.deltaTime, ForceMode.VelocityChange);
            //rb.AddForce(rb.forward * forwardForce);
            //rb.transform.forward(0, 0, forwardForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        if(Input.GetKey(backward_key)){
            rb.AddForce(0, 0, -forwardForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        
        if(Input.GetKey(right_key))
        {
            rb.AddForce(sidewayForce * Time.deltaTime, 0, 0 , ForceMode.VelocityChange);
        }
        if(Input.GetKey(left_key))
        {
            rb.AddForce(-sidewayForce * Time.deltaTime, 0, 0 , ForceMode.VelocityChange);
         }

        _isGrounded = Physics.CheckSphere(rb.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        // TODO make this work
        if(Input.GetKey(jump_key) && _isGrounded)
        {
            rb.AddForce(0 , jumpForce, 0 , ForceMode.VelocityChange);
         }


        //mouse look rotation
        float X = Input.GetAxis("Mouse X") * mouseSpeed;
         float Y = Input.GetAxis("Mouse Y") * mouseSpeed;
 
         player.Rotate(0, X, 0); // Player rotates on Y axis, your Cam is child, then rotates too
 
 
         // To scurity check to not rotate 360º 
         if (yourCam.transform.eulerAngles.x + (-Y) > 80 && yourCam.transform.eulerAngles.x + (-Y) < 280)
         { }
         else
         {
 
             yourCam.transform.RotateAround(player.position, yourCam.transform.right, -Y);
         }



    /*
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
    */


        // if falls out of world end
        if (rb.position.y < -1f)
        {
            FindObjectOfType<GameManager>().endGame();
        }

    }
}
/*
public class playerMove2  : MonoBehaviour
{

    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;

    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;

    private string forard_key = "w";
    private string backward_key = "s";
    private string left_key = "a";
    private string right_key = "d";
    private string jump_key = "space";
    
    
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetChild(0);
    }

    void Update()
    {


        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");

        /*
         if(Input.GetKey("d"))
        {
            rb.AddForce(sidewayForce * Time.deltaTime, 0, 0 , ForceMode.VelocityChange);
        }
        if(Input.GetKey("a"))
        {
            rb.AddForce(-sidewayForce * Time.deltaTime, 0, 0 , ForceMode.VelocityChange);
         }
         



        if (_inputs != Vector3.zero)
            transform.forward = _inputs;

        if (Input.GetKey(jump_key))// && _isGrounded)
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
            _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        }
    }


    void FixedUpdate()
    {

        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        // jumping
        if(Input.GetKey(jump_key) & _isGrounded)
        {
            _body.AddForce(0, JumpHeight, 0 , ForceMode.VelocityChange);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
         if (Input.GetKey(left_key))
             _body.AddForce(Vector3.left);
         if (Input.GetKey(right_key))
             _body.AddForce(Vector3.right);
         if (Input.GetKey(forard_key))
             _body.AddForce(Vector3.up);
         if (Input.GetKey(backward_key))
             _body.AddForce(Vector3.down);
 

        //    if(Input.GetKey(forard_key))
        
    }
}
*/