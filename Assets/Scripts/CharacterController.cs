using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    //Control settings
    public float mouseSpeed = 3;
    public bool isPlayer = false;

    //Objects and vars not loaded from save file
    private Transform player;
    private Camera cam;
    private Rigidbody rb;

    private Physics physics;

    //Character save manager
    public string CharacterSaveFileFolder = "Assets/CharacterJson";
    public string CharacterSaveFile = "Player1.json";
    private CharacterDataManager CDM = new CharacterDataManager ();

    private CharacterData Character = new CharacterData ();

    //When character comes online, set vars needed for init
    private void Awake () {
        cam = Camera.main;
        rb = gameObject.GetComponent<Rigidbody> ();
        player = gameObject.GetComponent<Transform> ();

        Debug.Log ("Starting");
        CDM.Init (CharacterSaveFileFolder, CharacterSaveFile);
        Character = CDM.Load ();
        this.tag = "player";
        //TODO load on init

    }

    private void FixedUpdate () {
        if (isPlayer) {
            PlayerMove ();

            //Debug save and load functions
            if (Input.GetKey ("i")) {
                Character = CDM.Load ();
            }

            if (Input.GetKey ("o")) {
                CDM.Save (Character);
            }
            if (Input.GetKey ("e")) {
                Interact ();
            }
            if (Input.GetKey ("tab")) {
                Target();

            }

        } else {
            //NPCMove();
        }
    }

    private void NPCMove () {
        float maxForce = 50f;
        Vector3 position = new Vector3 (Random.Range (-1f * maxForce, maxForce), Random.Range (-1f * maxForce, maxForce), Random.Range (-1f * maxForce, maxForce));
        //Debug.Log(position);
        rb.AddForce (position);

    }

    private void PlayerMove () {

        // Getting the direction to move through player input
        float hMove = Input.GetAxis ("Horizontal");
        float vMove = Input.GetAxis ("Vertical");

        //if moving forward, slightly fasater
        if (Input.GetKey ("w")) {
            Character.CurrentSpeed = Character.BaseMovementSpeed * 1.15f;
        }

        //sprinting
        if (Input.GetKey (KeyCode.LeftShift) && Character.CurrentStamina > 1) {
            Character.CurrentStamina = Character.CurrentStamina - Character.StaminaUseRate;
            Character.CurrentSpeed = Character.BaseMovementSpeed + (Character.StaminaBonusSpeed * (Character.CurrentStamina / Character.MaxStamina * 0.7f));
        } else {
            if (Character.CurrentStamina < Character.MaxStamina) {
                if (Character.CurrentStamina < 0) {
                    Character.CurrentStamina = 1;
                } else {
                    Character.CurrentStamina = Character.CurrentStamina + Character.CurrentStamina * Character.StaminaRechargeRate;
                }
            }
            Character.CurrentSpeed = Character.BaseMovementSpeed;
        }

        //Actual movemment

        // Get directions relative to camera
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        // Project forward and right direction on the horizontal plane (not up and down), then
        // normalize to get magnitude of 1
        forward.y = 0;
        right.y = 0;
        forward.Normalize ();
        right.Normalize ();

        // Set the direction for the player to move
        Vector3 dir = right * hMove + forward * vMove;

        // Set the direction's magnitude to 1 so that it does not interfere with the movement speed
        dir.Normalize ();

        // Move the player by the direction multiplied by speed and delta time 
        transform.position += dir * Character.CurrentSpeed * Time.deltaTime;

        // Set rotation to direction of movement if moving
        if (dir != Vector3.zero) {
            transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (forward), 0.2f);
        }

        //Jumping
        float DisstanceToTheGround = GetComponent<Collider> ().bounds.extents.y;

        bool IsGrounded = Physics.Raycast (transform.position, Vector3.down, DisstanceToTheGround + 0.1f);

        if ((IsGrounded == true) && (Input.GetButton ("Jump"))) //if canjump boolean is true and if the player press the button jump , the player can jump.
        {
            Vector3 up = new Vector3 (0.0f, Character.JumpHeight, 0.0f); // script for jumping
            //rb.AddForce (up * upper);
            rb.AddForce (up * Character.JumpHeight);
        }

        //Camera focus

        //mouse look rotation
        float X = Input.GetAxis ("Mouse X") * mouseSpeed;
        float Y = Input.GetAxis ("Mouse Y") * mouseSpeed;

        player.Rotate (0, X, 0); // Player rotates on Y axis, your Cam is child, then rotates too

        // To scurity check to not rotate 360º 
        if (cam.transform.eulerAngles.x + (-Y) > 80 && cam.transform.eulerAngles.x + (-Y) < 280) {

        } else {
            cam.transform.RotateAround (player.position, cam.transform.right, -Y);
        }

    }

    private void Interact () {

        RaycastHit hit;
        if (Physics.Raycast (transform.position, transform.forward, out hit, Character.Reach)) {
            Debug.Log (hit);
            Debug.Log (hit.collider);

            //IInteractable interactable = hit.collider.GetComponent<IInteractable> ();

            //if (interactable != null) {
            //    interactable.ShowInteractability ();
            //    interactable.Interact ();
            //}
        }
    }

    private void Target () {

        float radius = 1f;
        float depth = 0.1f;
        float angle = 100f;

        RaycastHit[] coneHits = physics.ConeCastAll (player.position, radius, player.forward, depth, angle);

        //TODO dedup this
        //var coneHits = new HashSet<RaycastHit>(allConeHits);
        Debug.Log("Targeting");

        if (coneHits.Length > 0) {
            for (int i = 0; i < coneHits.Length; i++) {
                //do something with collider information
                Debug.Log (coneHits[i].collider.gameObject);
                coneHits[i].collider.gameObject.GetComponent<Renderer> ().material.color = new Color (0, 0, 1f);
            }
        }
    }
}