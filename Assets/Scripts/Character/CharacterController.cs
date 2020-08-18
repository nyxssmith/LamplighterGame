using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class CharacterController : MonoBehaviour
{

    //Parts of the character
    private Transform CharacterTransform;
    public GameObject CameraTarget;
    private Camera cam;
    private Rigidbody rb;
    private Physics physics;


    // UI

    [SerializeField] GameObject ManaUI;
    [SerializeField] GameObject HealthUI;
    [SerializeField] GameObject StaminaUI;
    [SerializeField] GameObject TargetUI;//healthbar for target
    [SerializeField] GameObject TargetName;//name of target


    //Character save manager
    public string CharacterSaveFileFolder = "Assets/CharacterJson";
    public string CharacterSaveFile = "Player1.json";
    private CharacterDataManager CDM = new CharacterDataManager();

    public CharacterData Character = new CharacterData();

    private GameObject TargetBeaconObject = null; // the actual instance of the target beacon


    // animation parts and locations
    public GameObject AnimationTarget;// TODO point this at self / this.
    private Animator CharacterAnimator;

    public GameObject Hand;

    //TODO implement this with items
    public GameObject Back;
    public GameObject Belt;

    private NavMeshAgent NavAgent;


    // variables that are used for interacting with world but dont matter for save
    private string ItemStatus = "";//action items status, for swapping and dropping
    private bool HasItemInHand = false;
    private bool IsMoving = false;
    private bool IsGrounded = true;

    private bool IsFighting = false;
    private float Action = 0.0f;// actions, 0 is none, 1 is left click, 2 is right click, 3 is belt action
    private float ActionCooldown = 0.0f;// Attack cooldown for npcs = item cooldown*n

    public GameObject TargetBeacon; // prefab of the target beacon

    private bool hasTarget = false; //toggle if a target exists
    public float TargetCoolDown = 0.0f; //cooldown on targeting

    private int rand; //random number used to isolate targets

    public float HealthDamageCoolDown = 0.0f;

    private float JumpCoolDown = 0.0f;
    private string CurrentAnimationState = "";
    private string LastAnimationState = "";

    private float AnimationOverrideTimer = 0.0f;

    private bool isSprinting = false;
    private bool isSprintingCooldown = false;

    private float StaminaLevelBeforeSprintAgain;

    private bool NeedsUIUpdate = false;



    // Targeting and interacting with enemy and squad
    private GameObject FollowTarget = null;
    private GameObject CombatTarget = null;
    private CharacterData TargetCharacter = null;//save info on target character
    private CharacterController TargetCharacterController = null;//save info on target character





    //When character comes online, set vars needed for init
    private void Awake()
    {

        rb = gameObject.GetComponent<Rigidbody>();
        CharacterTransform = gameObject.GetComponent<Transform>();
        NavAgent = this.gameObject.GetComponent<NavMeshAgent>();


        CDM.Init(CharacterSaveFileFolder, CharacterSaveFile);

        Load();
        CharacterAnimator = AnimationTarget.GetComponent<Animator>();


        if (Character.IsPlayer)
        {
            cam = Camera.main;
            this.tag = "player";

        }
        GetFollowTargetFromSquadLeaderId();

        SetNavAgentStateFromIsPlayer();

        // must hit 80 stamina before going again
        StaminaLevelBeforeSprintAgain = Character.MaxStamina * 0.85f;
    }


    private void FixedUpdate()
    {

        // save and load for all chars
        if (Input.GetKeyDown("i"))
        {

            Load();
        }
        if (Input.GetKeyDown("o"))
        {
            Save();
        }


        if (Character.IsPlayer)
        {
            PlayerMove();

            // dont do ui on player, is now on camera
            //DoUI();

            //controls
            // actions for attach and use "r"
            if (Input.GetMouseButtonDown(0))
            {
                Action = 1.0f;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Action = 2.0f;
            }
            if (Input.GetKeyDown("r"))
            {
                Action = 3.0f;
            }







            //Debug save and load functions


            if (Input.GetKeyDown("e"))
            {
                Interact();
            }
            if (Input.GetKeyDown("tab"))
            {
                Target();
            }


            // TODO coordinate this and the above drop system to work for npcs too

            // Item drop controll
            if (Input.GetKey("q"))
            {
                ItemStatus = "Dropping";//applies to habd item
            }
            else if (Input.GetKeyDown("f"))
            {
                ItemStatus = "SwapHandBack";
            }
            else if (Input.GetKeyDown("g"))
            {
                ItemStatus = "SwapHandBelt";
            }
            //TODO use item from belt
            else
            {
                ItemStatus = "";
            }

            CheckIfItemInHand();






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

        // TODO check not fall from world

        SetNavAgentStateFromIsMoving();

        if (Character.IsPlayer)
        {

            SetNavAgentStateFromIsPlayer();

        }

        DoAnimationState();
        if (LastAnimationState != CurrentAnimationState)
        {
            if (LastAnimationState == Character.midair_animation && IsGrounded)
            {
                CurrentAnimationState = Character.landing_animation;
            }
            CharacterAnimator.Play(CurrentAnimationState, 0, 0);
            LastAnimationState = CurrentAnimationState;
        }

        // if the health cooldown was up, lower it
        if (HealthDamageCoolDown >= 0.0f)
        {
            HealthDamageCoolDown -= Time.deltaTime;
        }

        if (Character.CurrentHealth <= 0.0f)
        {
            Destroy(this.gameObject);
        }
        //}
    }


    private void DoAnimationState()
    {
        //TODO redo this all to be based on current status / moving not keys

        if (AnimationOverrideTimer > 0.0f)
        {
            //Debug.Log("should be getting state from item");
            AnimationOverrideTimer -= Time.deltaTime;
        }
        /*
        else if (!IsGrounded && Character.IsPlayer && (JumpCoolDown > 0.0f))
        {
            CurrentAnimationState = Character.midair_animation;
            JumpCoolDown -= Time.deltaTime;
        }
        */
        // sprinting for player
        else if (isSprinting && Character.CurrentStamina > 10)
        {
            if (Character.IsPlayer)
            {
                //TODO check state of character
                if (Input.GetKey("w"))
                {
                    CurrentAnimationState = Character.running_forward_animation;

                }
                else if (Input.GetKey("s"))
                {
                    CurrentAnimationState = Character.running_backward_animation;

                }
            }
            else
            {
                //TODO
            }

        }
        else// not sprinting
        {
            if (IsMoving)
            {
                // player movment by key
                if (Character.IsPlayer)
                {
                    if (Input.GetKey("w"))
                    {
                        CurrentAnimationState = Character.walking_forward_animation;
                    }
                    else if (Input.GetKey("s"))
                    {
                        CurrentAnimationState = Character.walking_backward_animation;

                    }
                    else
                    {//TODO do a walking rightleft etc
                        CurrentAnimationState = Character.walking_forward_animation;

                    }
                }
                else// npc based on state
                {
                    CurrentAnimationState = Character.walking_forward_animation;

                }
            }
            else
            {
                CurrentAnimationState = Character.idle_animation;
            }


        }
    }

    // Movement and npc

    private void NPCMove()
    {
        //TODO if follower
        // todo if enemy
        // make sure if figting, also take control of is moving
        // TODO if enemy follower sees, target instead

        // fight takes priority
        if (IsFighting)
        {
            AttackTarget();
        }
        else if (Character.IsFollowing)
        {
            FollowPlayer();
        }
        else
        {
            IsMoving = false;
        }

    }

    private void AttackTarget()
    {

        //Debug.Log("Attacking", CombatTarget);

        Transform TargetTransform = CombatTarget.GetComponent<Transform>();
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float rotationSpeed = 30f; //speed of turning

        float range = 10f;
        float range2 = 10f;
        float stop = Character.Reach; // this is range to player


        //rotate to look at the player
        var distance = Vector3.Distance(CharacterTransform.position, TargetTransform.position);
        if (distance <= range2 && distance >= range)
        {
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
            Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);
        }
        else if (distance <= range && distance > stop)
        {
            //NavAgent.destination = TargetTransform.position;
            //IsMoving = true;
            NPCGOTOTargetWithSprint(TargetTransform);


        }
        else if ((distance <= stop) && (NavAgent.enabled))
        {


            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
                Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);

            if (ActionCooldown > 0.0f)
            {
                ActionCooldown -= Time.deltaTime;
            }
            else
            {
                Attack();
            }

        }
        else
        {
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
        }
    }

    // pick and do an attack option
    private void Attack()
    {

        if (HasItemInHand)
        {
            Action = 1.0f;
        }
        else
        {
            //TODO if can, do unarmed spells etc
            // else do punch
            Debug.Log("unarmed attack");
        }
        // set attack cooldown
        //ActionCooldown = 2.5f;

    }

    private void SetNavAgentDestination(Vector3 goal_position)
    {
        if (NavAgent.enabled)
        {
            NavAgent.destination = goal_position;
        }
    }

    private void FollowPlayer()
    {
        if (FollowTarget == null)
        {
            GetFollowTargetFromSquadLeaderId();
        }

        Transform TargetTransform = FollowTarget.GetComponent<Transform>();
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float rotationSpeed = 30f; //speed of turning
        float range = 250f;
        float range2 = 250f;
        float stop = 3.8f; // this is range to player

        //rotate to look at the player
        var distance = Vector3.Distance(CharacterTransform.position, TargetTransform.position);
        if (distance <= range2 && distance >= range)
        {
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
            Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);
        }
        else if (distance <= range && distance > stop)
        {

            //NavAgent.destination = TargetTransform.position;
            //IsMoving = true;
            NPCGOTOTargetWithSprint(TargetTransform);


        }
        else if ((distance <= stop) && (NavAgent.enabled))
        {
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
                Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);
        }
        else
        {
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
        }

    }


    private void NPCGOTOTargetWithSprint(Transform TargetPosition)
    {
        IsMoving = true;

        Vector3 Start = CharacterTransform.position;
        Vector3 End = TargetPosition.position;


        SetNavAgentDestination(End);
        float distance_to_go = (Start - End).magnitude;

        // sprinting cooldown for npcs
        if (isSprintingCooldown)
        {
            if (Character.CurrentStamina > StaminaLevelBeforeSprintAgain)
            {
                isSprintingCooldown = false;
            }
        }

        if ((distance_to_go > 20.0f) && (Character.CurrentStamina > 1) && (!isSprintingCooldown))
        {
            Character.CurrentStamina = Character.CurrentStamina - Character.StaminaUseRate;
            Character.CurrentSpeed = Character.BaseMovementSpeed + (Character.StaminaBonusSpeed * (Character.CurrentStamina / Character.MaxStamina * 0.7f));
            isSprinting = true;
        }
        else
        {
            if (isSprinting)
            {
                isSprintingCooldown = true;
            }
            isSprinting = false;
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

        NavAgent.speed = Character.CurrentSpeed;
        //Debug.Log("im " + Character.Name + " and my current speed is" + NavAgent.speed+" im sprinting"+isSprinting+" stamina"+Character.CurrentStamina);


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
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
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
        // TODO rm jumping
        /*
        float DisstanceToTheGround = GetComponent<Collider>().bounds.extents.y;

        //IsGrounded = Physics.Raycast(CharacterTransform.position, Vector3.up, DisstanceToTheGround + 0.1f);

        if ((IsGrounded == true) && (Input.GetButton("Jump")) && (JumpCoolDown <= 0.0f)) //if canjump boolean is true and if the player press the button jump , the player can jump.
        {
            CurrentAnimationState = Character.jump_animation;

            Vector3 up = new Vector3(0f, Character.JumpHeight, 0.0f); // script for jumping
                                                                      //rb.AddForce (up * upper);
            rb.AddForce(up * Character.JumpHeight);
            JumpCoolDown += 3.0f;

        }
        */

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
        else
        {
            if (CombatTarget != null)
            {
                CombatTarget.GetComponent<CharacterController>().DoInteractAction(this.gameObject);
            }
        }
        NeedsUIUpdate = true;
    }

    public void Target()
    {

        if (TargetCoolDown >= 0.0f)
        {
            TargetCoolDown -= Time.deltaTime;
        }
        else
        {
            if (!hasTarget)// if doesnt have a target, find one
            {
                Debug.Log("fiding targt");
                rand = Random.Range(1, 254);

                float radius = Character.TargetRange / 2.0f;
                Vector3 center = CharacterTransform.position + (CharacterTransform.forward * Character.TargetRange / 2.0f);
                Collider[] hitColliders = Physics.OverlapSphere(center, radius);
                int i = 0;
                Vector3 SummonPositon = center;

                while (i < hitColliders.Length)
                {
                    if (hitColliders[i].gameObject.GetComponent<CharacterController>() != null)
                    {
                        Transform TargetTransform = hitColliders[i].gameObject.GetComponent<Transform>();
                        int TargetRand = hitColliders[i].gameObject.GetComponent<CharacterController>().GetRand();
                        if (TargetRand != rand)
                        {
                            SummonPositon = TargetTransform.position + new Vector3(0.0f, 2.0f, 0.0f);
                            TargetBeaconObject = Instantiate(TargetBeacon, SummonPositon, Quaternion.identity);
                            hasTarget = true;
                            CombatTarget = hitColliders[i].gameObject;
                            TargetCharacter = hitColliders[i].gameObject.GetComponent<CharacterController>().GetCharacter();

                            //TargetCharacterController = hitColliders[i].gameObject.GetComponent<CharacterController>().GetCharacterController();

                            //Player = CameraFollowObj.gameObject.GetComponentInParent<CharacterController>();
                            //TargetCharacterController = hitColliders[i].gameObject.GetComponent<CharacterController>().GetCharacterController();
                            //TargetCharacterController = hitColliders[i].GetComponentInParent<CharacterController>();


                            TargetCharacterController = CombatTarget.GetComponent<CharacterController>();

                            Debug.Log("target char controller", TargetCharacterController);
                            //make the target beacon a child of its taret
                            TargetBeaconObject.gameObject.GetComponent<Transform>().parent = CombatTarget.GetComponent<Transform>();
                            break;
                        }
                    }
                    i++;
                }
            }
            else// if does have target, de-target
            {
                Destroy(TargetBeaconObject);
                CombatTarget = null;
                TargetCharacter = null;
                TargetCharacterController = null;
                hasTarget = false;
            }
            TargetCoolDown = 0.05f;
        }
        NeedsUIUpdate = true;

    }

    private void DoInteractAction(GameObject WhoInteracted)
    {
        Debug.Log("I was interacted with by " + WhoInteracted + " my leader is" + Character.squadLeaderId);
        Debug.Log(Character.IsFollower);

        // If a follower, then make then interact toggles follow
        if (Character.IsFollower)
        {

            CharacterController InteractedCharacterController = WhoInteracted.GetComponent<CharacterController>();
            if (InteractedCharacterController != null)
            {
                if (Character.squadLeaderId == "")
                {
                    Debug.Log("Joining squad of " + InteractedCharacterController);
                    JoinSquadOfCharacter(InteractedCharacterController);
                    //JoinSquadLeadBy(InteractedCharacterController.GetUUID());
                }
                else
                {
                    JoinSquadLeadBy("");
                }
            }
            /*
            Debug.Log("im a follwer who was interacted with");
            Character.IsFollowing = !Character.IsFollowing;
            FollowTarget = WhoInteracted;

            //if was told to stop following, then also stop moving
            if (Character.IsFollowing)
            {
                IsMoving = false;
                CurrentAnimationState = Character.idle_animation;
            }
            */
        }
        NeedsUIUpdate = true;

    }

    private void DoTargetedAction()// character will make a beacon above their head
    {
        Debug.Log("I was targetd");
        NeedsUIUpdate = true;

        //TODO reacte to being targeted etc

    }


    private void CheckIfItemInHand()//updates the hand var
    {
        Transform handTransform = Hand.GetComponent<Transform>();
        int i = 0;
        foreach (Transform child in handTransform)
        {
            //Debug.Log("is child of hand" + child);
            i += 1;
        }
        if (i >= 1)
        {
            HasItemInHand = true;
        }
        else
        {
            HasItemInHand = false;
        }
    }

    // Getters and setters for interactions

    public CharacterData GetCharacter()
    {
        return this.Character;
    }

    public CharacterData GetTargetCharacter()
    {
        return this.TargetCharacter;
    }

    public bool GetIsPlayer()
    {
        return this.Character.IsPlayer;
    }

    public Transform GetCharacterTransform()
    {
        return this.CharacterTransform;
    }

    public Transform GetHandTransform()
    {
        return Hand.transform;
    }

    public Transform GetBackTransform()
    {
        return Back.transform;
    }

    public Transform GetBeltTransform()
    {
        return Belt.transform;
    }


    public int GetRand()
    {
        return rand;
    }

    public string GetItemStatus()
    {
        return ItemStatus;
    }

    public float GetItemActionFloat()
    {
        return this.Action;
    }

    public void ResetItemActionFloat()
    {
        Action = 0.0f;
    }


    public bool GetHasItemInHand()
    {
        return this.HasItemInHand;
    }


    // TODO add value to health etc
    // add negative value to reduce
    public void AddValueToStamina(float value)
    {
        Character.CurrentStamina += value;
    }


    public void AddValueToHealth(float value)
    {
        if (!(HealthDamageCoolDown >= 0.0f) && (value < 0))
        {
            Character.CurrentHealth += value;
            HealthDamageCoolDown = 1.0f;
        }
        else
        {
            Character.CurrentHealth += value;
        }


    }

    public float GetCurrentHealth()
    {
        return this.Character.CurrentHealth;
    }

    public bool GetCanFight()
    {
        return this.Character.CanFight;
    }

    public void SetFighting(bool state)
    {
        IsFighting = state;
    }

    public void SetAnimation(string animation, float overrideDuration)
    {
        this.CurrentAnimationState = animation;
        this.AnimationOverrideTimer = overrideDuration;
    }

    // clear the target for when attacking to override target
    public void SetTarget(GameObject TargetToSet)
    {

        Destroy(TargetBeaconObject);
        CombatTarget = null;
        TargetCharacter = null;
        hasTarget = false;

        this.TargetCoolDown = 0.0f;


        Transform TargetTransform = TargetToSet.GetComponent<Transform>();
        Vector3 SummonPositon = TargetTransform.position + new Vector3(0.0f, 2.0f, 0.0f);
        TargetBeaconObject = Instantiate(TargetBeacon, SummonPositon, Quaternion.identity);
        hasTarget = true;
        CombatTarget = TargetToSet;
        TargetCharacter = TargetToSet.gameObject.GetComponent<CharacterController>().GetCharacter();
        //make the target beacon a child of its taret
        TargetBeaconObject.gameObject.GetComponent<Transform>().parent = CombatTarget.GetComponent<Transform>();

    }


    public bool GetHasTarget()
    {
        return this.hasTarget;
    }

    public string GetUUID()
    {
        return this.Character.id;
    }

    public void Save()
    {
        Debug.Log("Saving " + Character.Name);
        Character.x_pos = CharacterTransform.position.x;
        Character.y_pos = CharacterTransform.position.y;
        Character.z_pos = CharacterTransform.position.z;
        CDM.Save(Character);

    }

    public void Load()
    {
        Character = CDM.Load();
        // set world postion
        // TODO move this to recall potion
        //CharacterTransform.position = new Vector3(Character.x_pos, Character.y_pos, Character.z_pos);

        Debug.Log("Loaded data for " + Character.Name + " from " + CharacterSaveFile);

    }


    public void SetIsPlayer(bool NewStatus)
    {
        Character.IsPlayer = NewStatus;
        if (NewStatus)
        {
            cam = Camera.main;
            this.tag = "player";
        }
        else
        {
            cam = null;
            this.tag = "npc";
        }

        SetNavAgentStateFromIsPlayer();
    }

    // swap into target as long as its not self and let camera know
    public bool SwapIntoTarget(CharacterController SwapTargetCharacterController)
    {
        if (SwapTargetCharacterController != this)
        {
            /*
            Character.IsPlayer = false;
            cam = null;
            this.tag = "npc";
            */
            SetIsPlayer(false);
            SwapTargetCharacterController.SetIsPlayer(true);

            /*
            Destroy(TargetBeaconObject);
            CombatTarget = null;
            TargetCharacter = null;
            TargetCharacterController = null;
            hasTarget = false;
            IsFighting = false;
            */

            // TODO cooldown set from being swapped into

            SetNavAgentStateFromIsPlayer();
            return true;
        }
        else
        {
            return false;
        }
    }

    public GameObject GetCameraTarget()
    {
        return CameraTarget;
    }

    /*
    public GameObject GetTargetsCameraTarget()
    {
        return TargetCharacterController.GetCameraTarget();
    }
    */

    private void SetNavAgentStateFromIsPlayer()
    {
        NavAgent.enabled = !Character.IsPlayer;
    }


    private void SetNavAgentStateFromIsMoving()
    {
        NavAgent.enabled = IsMoving;
    }

    public string GetSquadLeaderUUID()
    {
        return Character.squadLeaderId;
    }

    private void GetFollowTargetFromSquadLeaderId()
    {

        // if character is a foller and in a squad find follow target
        if (Character.IsFollower && (Character.squadLeaderId != ""))
        {
            var characterControllersList = FindObjectsOfType<CharacterController>();
            string id;
            foreach (CharacterController controller in characterControllersList)
            {
                id = controller.GetUUID();
                Debug.Log("found id" + id);
                if (id == Character.squadLeaderId)
                {
                    FollowTarget = controller.gameObject;
                    Debug.Log(Character.Name + " found my followtagret " + FollowTarget);
                    break;
                }
            }

        }
    }

    // starts following a leader when given leader uuid
    private void JoinSquadLeadBy(string leader_uuid)
    {
        Character.squadLeaderId = leader_uuid;
        if (leader_uuid != "")
        {
            Character.IsFollowing = true;
        }
        else
        {
            FollowTarget = null;
            Character.IsFollowing = false;
        }
        NeedsUIUpdate = true;
    }

    // either joins a squad that the inviter is in and climbs that tree, or joins their squad owned by them
    private void JoinSquadOfCharacter(CharacterController InviterController)
    {
        string InviterID = InviterController.GetUUID();
        string InviterLeaderID = InviterController.GetSquadLeaderUUID();
        // if inviter is leader or has none, join them
        if (InviterID == InviterLeaderID || InviterLeaderID == "")
        {
            JoinSquadLeadBy(InviterID);
        }
        else
        {
            JoinSquadLeadBy(InviterLeaderID);
        }

    }

    public bool GetNeedsUIUpdate()
    {
        return NeedsUIUpdate;
    }

    public void SetNeedsUIUpdate(bool newState)
    {
        NeedsUIUpdate = newState;
    }

}

