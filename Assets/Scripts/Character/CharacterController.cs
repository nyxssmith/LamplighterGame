using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    //Control settings
    public bool isPlayer = false;

    //Objects and vars not loaded from save file
    private Transform CharacterTransform;
    private Camera cam;
    private Rigidbody rb;
    private Physics physics;

    [SerializeField] GameObject ManaUI;
    [SerializeField] GameObject HealthUI;
    [SerializeField] GameObject StaminaUI;

    //Character save manager
    public string CharacterSaveFileFolder = "Assets/CharacterJson";
    public string CharacterSaveFile = "Player1.json";
    private CharacterDataManager CDM = new CharacterDataManager();

    private CharacterData Character = new CharacterData();

    // variables that are used for interacting with world but dont matter for save
    private bool IsDroppingItem = false;
    private bool IsMoving = false;
    private bool IsGrounded = true;

    // Targeting and interacting
    private GameObject FollowTarget;
    private GameObject CombatTarget;


    // animation parts and locations
    public GameObject AnimationTarget;// TODO point this at self / this.
    private Animator CharacterAnimator;

    public GameObject Hand;

    //TODO implement this with items
    public GameObject Back;
    public GameObject Belt;



    private string CurrentAnimationState = "Idle01";
    private string LastAnimationState = "Idle01";


    //When character comes online, set vars needed for init
    private void Awake()
    {

        rb = gameObject.GetComponent<Rigidbody>();
        CharacterTransform = gameObject.GetComponent<Transform>();



        CDM.Init(CharacterSaveFileFolder, CharacterSaveFile);
        Character = CDM.Load();
        Debug.Log("Loaded data for " + Character.Name + " from " + CharacterSaveFile);

        if (isPlayer)
        {
            cam = Camera.main;
            this.tag = "player";

            //TODO make it so all character can be animated later
            //TODO replace target with this
            CharacterAnimator = AnimationTarget.GetComponent<Animator>();


        }
    }

    private void FixedUpdate()
    {
        if (isPlayer)
        {
            PlayerMove();
            DoUI();

            //Debug save and load functions
            if (Input.GetKeyDown("i"))
            {
                Character = CDM.Load();
            }

            if (Input.GetKeyDown("o"))
            {
                CDM.Save(Character);
            }
            if (Input.GetKeyDown("e"))
            {
                Interact();
            }
            if (Input.GetKeyDown("tab"))
            {
                Target();

            }

            if (Input.GetKey("q"))
            {
                IsDroppingItem = true;
            }
            else
            {
                IsDroppingItem = false;
            }

            /*
            if (Input.GetKeyDown("g")) {

            }*/

        }
        else
        {
            NPCMove();
        }
    }

    // things do to at frame time
    private void Update()
    {
        //TODO rm this check so all charactes are animated
        if (isPlayer)
        {

            DoAnimationState();
            if (LastAnimationState != CurrentAnimationState)
            {
                if (LastAnimationState == "MidAir" && IsGrounded)
                {
                    CurrentAnimationState = "Landing";
                }
                CharacterAnimator.Play(CurrentAnimationState, 0, 0);
                LastAnimationState = CurrentAnimationState;
            }
        }
    }


    private void DoAnimationState()
    {
        /*
        if ((IsGrounded == true) && (Input.GetButton("Jump")))
        {
            CurrentAnimationState = "Jump";
            Debug.Log("Juumping");
        }
        else if (!IsGrounded && LastAnimationState == "MidAir")
        {
            CurrentAnimationState = "MidAir";
            Debug.Log("airing");
        }
        else */
        if (!IsGrounded){
            CurrentAnimationState = "MidAir";
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Character.CurrentStamina > 10)
        {
            if (Input.GetKey("w"))
            {
                CurrentAnimationState = "Running";

            }
            else if (Input.GetKey("s"))
            {
                CurrentAnimationState = "RunningBackwards";

            }

        }
        else
        {
            if (IsMoving)
            {
                CurrentAnimationState = "Walking";
            }
            else
            {
                CurrentAnimationState = "Idle01";
            }


        }
    }

    private void DoUI()
    {
        DoHealthUI();
        DoStaminaUI();
        DoManaUI();
    }

    private void DoHealthUI()
    {
        //TODO
    }
    private void DoStaminaUI()
    {
        StaminaUI.GetComponent<FillUI>().SetTo(Character.CurrentStamina / Character.MaxStamina);
        //TODO
    }

    private void DoManaUI()
    {
        //TODO
    }

    // Movement and npc

    private void NPCMove()
    {
        //TODO if follower
        // todo if enemy
        // TODO if enemy follower sees, target instead
        if (Character.IsFollowing)
        {
            FollowPlayer();
        }

    }

    private void FollowPlayer()
    {

        float rotationSpeed = 6f; //speed of turning
        float range = 10f;
        float range2 = 10f;
        float stop = 1f; // this is range to player

        Transform TargetTransform = FollowTarget.GetComponent<Transform>();
        //rotate to look at the player
        var distance = Vector3.Distance(CharacterTransform.position, TargetTransform.position);
        if (distance <= range2 && distance >= range)
        {
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
                Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);
        }
        else if (distance <= range && distance > stop)
        {

            //move towards the player
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
                Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);
            CharacterTransform.position += CharacterTransform.forward * Character.CurrentSpeed * Time.deltaTime;
        }
        else if (distance <= stop)
        {
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
                Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);
        }

    }

    private void RandomMove()
    {
        float maxForce = 50f;
        Vector3 position = new Vector3(Random.Range(-1f * maxForce, maxForce), Random.Range(-1f * maxForce, maxForce), Random.Range(-1f * maxForce, maxForce));
        //Debug.Log(position);
        rb.AddForce(position);

    }

    // Player movement

    private void PlayerMove()
    {

        // Getting the direction to move through player input
        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");

        //if moving forward, slightly fasater
        if (Input.GetKey("w"))
        {
            Character.CurrentSpeed = Character.BaseMovementSpeed * 1.15f;
        }

        //sprinting
        if (Input.GetKey(KeyCode.LeftShift) && Character.CurrentStamina > 1)
        {
            Character.CurrentStamina = Character.CurrentStamina - Character.StaminaUseRate;
            Character.CurrentSpeed = Character.BaseMovementSpeed + (Character.StaminaBonusSpeed * (Character.CurrentStamina / Character.MaxStamina * 0.7f));
        }
        else
        {

            if (Character.CurrentStamina < Character.MaxStamina)
            {
                if (Character.CurrentStamina <= 1)
                {
                    Character.CurrentStamina = 2;
                }
                else
                {
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
        forward.Normalize();
        right.Normalize();

        // Set the direction for the player to move
        Vector3 dir = right * hMove + forward * vMove;

        IsMoving = dir != Vector3.zero;

        // Set the direction's magnitude to 1 so that it does not interfere with the movement speed
        dir.Normalize();

        // Move the player by the direction multiplied by speed and delta time 
        transform.position += dir * Character.CurrentSpeed * Time.deltaTime;

        // Set rotation to direction of movement if moving
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), 0.2f);
        }

        //Jumping
        float DisstanceToTheGround = GetComponent<Collider>().bounds.extents.y;

        //IsGrounded = Physics.Raycast(CharacterTransform.position, Vector3.up, DisstanceToTheGround + 0.1f);

        if ((IsGrounded == true) && (Input.GetButton("Jump"))) //if canjump boolean is true and if the player press the button jump , the player can jump.
        {
            CurrentAnimationState = "Jump";

            Vector3 up = new Vector3(0f, Character.JumpHeight, 0.0f); // script for jumping
                                                                      //rb.AddForce (up * upper);
            rb.AddForce(up * Character.JumpHeight);

        }

    }


    void OnCollisionExit(Collision hit)
    {
        if (hit.gameObject.tag == "Ground")
        {
            IsGrounded = false;
        }
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.tag == "Ground")
        {
            IsGrounded = true;
        }
    }

    // Targeting and interacting

    private void Interact()
    {

        RaycastHit hit;
        if (Physics.Raycast(CharacterTransform.position, CharacterTransform.forward, out hit, Character.Reach))
        {
            //Debug.Log (hit);
            Debug.Log("Interacted with" + hit.collider.gameObject);
            hit.collider.gameObject.GetComponent<CharacterController>().DoInteractAction(this.gameObject);
            //hit.collider.gameObject.target = this;

            //IInteractable interactable = hit.collider.GetComponent<IInteractable> ();

            //if (interactable != null) {
            //    interactable.ShowInteractability ();
            //    interactable.Interact ();
            //}
        }
    }

    private void Target()
    {

        RaycastHit hit;
        if (Physics.Raycast(CharacterTransform.position, CharacterTransform.forward, out hit, Character.TargetRange))
        {
            Debug.Log(hit);
            Debug.Log(hit.collider.gameObject);
            hit.collider.gameObject.GetComponent<CharacterController>().DoTargetedAction();

        }

    }

    private void DoInteractAction(GameObject WhoInteracted)
    {
        Debug.Log("I was interacted with by " + WhoInteracted);
        Debug.Log(Character.IsFollower);

        // If a follower, then make then interact toggles follow
        if (Character.IsFollower)
        {
            Debug.Log("im a follwer who was interacted with");
            Character.IsFollowing = !Character.IsFollowing;
            FollowTarget = WhoInteracted;
        }
    }

    private void DoTargetedAction()
    {
        Debug.Log("I was targetd");
    }

    // moves the charactesr pointer to their target to above the target
    private void MoveTargetPointer()
    {

    }

    // Getters and setters for interactions

    public CharacterData GetCharacter()
    {
        return this.Character;
    }

    public bool GetIsPlayer()
    {
        return this.isPlayer;
    }

    public Transform GetCharacterTransform()
    {
        return this.CharacterTransform;
    }

    public Transform GetHandTransform()
    {
        return Hand.transform;
    }

    public bool GetIsDroppingItem()
    {
        return this.IsDroppingItem;
    }

    // TODO add value to health etc
    // add negative value to reduce
    public void AddValueToStamina(float value)
    {
        Character.CurrentStamina += value;
    }


}