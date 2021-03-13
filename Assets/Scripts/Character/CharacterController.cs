using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    //Parts of the character
    private Transform CharacterTransform;

    public GameObject CameraTarget;

    private Camera cam;

    public Rigidbody rb;

    private Physics physics;

    // UI
    [SerializeField]
    GameObject ManaUI;

    [SerializeField]
    GameObject HealthUI;

    [SerializeField]
    GameObject StaminaUI;

    [SerializeField]
    GameObject TargetUI; //healthbar for target

    [SerializeField]
    GameObject TargetName; //name of target

    public SpriteRenderer Circle;

    public GameObject SpeechBubblePreFab;

    public GameObject DialogManagerPreFab;

    private Color UIColor;

    //Character save manager
    //public string CharacterSaveFileFolder = "Assets/CharacterJson";
    //public string CharacterSaveFile = "Player1.json";
    private CharacterDataManager CDM = new CharacterDataManager();

    public CharacterData Character = new CharacterData();

    private GameObject TargetBeaconObject = null; // the actual instance of the target beacon

    // animation parts and locations
    public GameObject AnimationTarget; // TODO point this at self / this.

    //private Animator CharacterAnimator;
    public GameObject Hand;

    public GameObject Back;

    public GameObject Belt;

    private NavMeshAgent NavAgent;

    public GameObject AnimationControlManagerChild;

    private DM.ControlManager AnimationControlManager;

    private IsLoadedController LoadedController;

    // variables that are used for interacting with world but dont matter for save
    private string ItemStatus = ""; //action items status, for swapping and dropping

    private bool HasItemInHand = false;

    private bool IsMoving = false;

    private bool IsGrounded = true;

    private bool IsFighting = false;

    private float Action = 0.0f; // actions, 0 is none, 1 is left click, 2 is right click, 3 is belt action

    private float ActionCooldown = 0.0f; // Attack cooldown for npcs = item cooldown*n

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

    private string myFaction;

    private float selfDestructTimer = 1.0f;

    private bool selfDestuctStarted = false;

    private GameObject SpeechBubbleObject = null;

    private List<GameObject> SpeechBubbles = new List<GameObject>();

    private Vector3 WanderPointCenter = new Vector3(0.0f, -1.0f, 0.0f);

    private Vector3 WanderPointGoal = new Vector3(0.0f, -1.0f, 0.0f);

    private bool inBuildMode = false;

    // default wandering range radius
    float wanderRange = 3.0f;

    private float StandingStillTimer = 0.0f;

    private ItemController HeldItemController;

    private bool WentHomeToSleep = false;

    // dialog stuff
    private bool CanJoinDialog = true;

    public bool IsInDialog = false;

    private DialogManager CurrentDialogManager = null;

    private GameObject DialogManagerObject = null;

    private string DialogText = "";

    // buildings relevant to character like home and shops
    public List<BuildingController> Buildings = new List<BuildingController>();

    private bool isInShop;

    private BuildingController CurrentShopController;

    // town they own/belong to
    private string TownUUID;

    // town they are in
    private TownController Town;

    // schedule and task stuff
    // will do current task, when done, if no next task will do last task, current task can set next task
    private string CurrentTask = "";

    private string NextTask = "";

    private string LastTask = "";

    private string NextNextTask = "";

    // TODO current quest stuff
    // TODO make private again
    public Quest CurrentQuest = null;

    // Targeting and interacting with enemy and squad
    private GameObject FollowTarget = null;

    private GameObject CombatTarget = null;

    private CharacterData TargetCharacter = null; //save info on target character

    private List<CharacterController>
        SquadCharacterControllers = new List<CharacterController>();

    private CharacterController TargetCharacterController = null; //save info on target character

    private GameObject CameraWithHUD = null;

    private bool hasDoneInit = false;

    // Character customization lists
    private List<GameObject> TorsoOptions = new List<GameObject>();

    private List<GameObject> BeltBackpackOptions = new List<GameObject>();

    private List<GameObject> HeadOptions = new List<GameObject>();

    private List<GameObject> FaceOptions = new List<GameObject>();

    private List<GameObject> HandsOptions = new List<GameObject>();

    private List<GameObject> ShoulderOptions = new List<GameObject>();

    private List<GameObject> ShoeOptions = new List<GameObject>();

    //When character comes online, set vars needed for init
    public void start()
    {
        DoInit("", "");
    }

    public void DoInit(string CharacterSaveFileFolder, string CharacterSaveFile)
    {
        if (!hasDoneInit)
        {
            Debug.Log("doing init");

            //parts of the character
            //rb = gameObject.GetComponent<Rigidbody>();
            rb = GetComponent<Rigidbody>();

            //Debug.Log("rb" + rb);
            CharacterTransform = gameObject.GetComponent<Transform>();
            NavAgent = this.gameObject.GetComponent<NavMeshAgent>();

            /*
        //load chracter save into character
        CDM.Init (CharacterSaveFileFolder, CharacterSaveFile);

        Load();
        */
            Load (CharacterSaveFileFolder, CharacterSaveFile);

            AnimationControlManager =
                AnimationControlManagerChild.GetComponent<DM.ControlManager>();

            Debug.Log("animationcontroller" + AnimationControlManager);

            AnimationControlManager.characterController = this;

            //CharacterAnimator = AnimationTarget.GetComponent<Animator>();
            if (Character.IsPlayer)
            {
                cam = Camera.main;
                this.tag = "player";
            }
            GetFollowTargetFromSquadLeaderId();

            SetNavAgentStateFromIsPlayer();

            // must hit 80 stamina before going again
            StaminaLevelBeforeSprintAgain = Character.MaxStamina * 0.85f;

            DetermineMyFaction();

            // get the load controller and update if is player
            LoadedController = gameObject.GetComponent<IsLoadedController>();
            LoadedController.SetIsPlayer(Character.IsPlayer);

            // pick ui color
            SetColor();
            Circle.color = UIColor;
            MakeSpeechBubble("set cirice color" + Circle.color);

            CurrentTask = Character.DefaultTask;
            hasDoneInit = true;

            // populate the lists of customization parts of the character
            PopulateListsOfCustomizeOptions();
        }
    }

    private void FixedUpdate()
    {
        // save and load for all chars
        //if (Input.GetKeyDown("i"))
        //{
        //    Load();
        //}
        //if (Input.GetKeyDown("o"))
        //{
        //    Save();
        //}
        // for all characters look for hand item
        CheckIfItemInHand();

        if (Character.IsPlayer)
        {
            PlayerMove();

            if (!IsInDialog)
            {
                // dont do ui on player, is now on camera
                //DoUI();
                //controls
                // actions for attach and use "r"
                if (HasItemInHand)
                {
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
                }

                //Debug save and load functions
                if (Input.GetKeyDown("e"))
                {
                    Interact();
                }
                if (Input.GetKeyDown("tab") && !inBuildMode)
                {
                    Target();
                }

                // TODO coordinate this and the above drop system to work for npcs too
                // Item drop controll
                if (Input.GetKey("q"))
                {
                    ItemStatus = "Dropping"; //applies to habd item
                    SetNeedsUIUpdate(true);
                }
                else if (Input.GetKeyDown("f"))
                {
                    SetNeedsUIUpdate(true);
                    ItemStatus = "SwapHandBack";
                }
                else if (Input.GetKeyDown("g"))
                {
                    // check that hand item can go to belt
                    bool CanDoSwap;
                    if (HeldItemController != null)
                    {
                        CanDoSwap = HeldItemController.GetCanGoOnBelt();
                    }
                    else
                    {
                        CanDoSwap = true;
                    }
                    if (CanDoSwap || !HasItemInHand)
                    {
                        SetNeedsUIUpdate(true);
                        ItemStatus = "SwapHandBelt";
                    }
                }
                else
                {
                    ItemStatus = "";
                }

                /*
                if (Input.GetKeyDown("g")) {

                }*/
            }
            else
            {
                DoDialog();
            }
        }
        else
        {
            if (!IsInDialog)
            {
                NPCMove();
            }
            else
            {
                DoDialog();
            }
        }
    }

    // things do to at frame time
    private void Update()
    {
        // TODO only update if not currently moving to be less laggy
        // and fix items dropping
        if (selfDestuctStarted)
        {
            SetNeedsUIUpdate(true);
            SetCharacterCanMove(false);
            SelfDestruct();
        }
        else
        {
            SetCharacterCanMove(true);
        }

        // TODO check not fall from world
        SetNavAgentStateFromIsMoving();

        if (Character.IsPlayer)
        {
            SetNavAgentStateFromIsPlayer();
        }

        // animations
        /*
        DoAnimationState();
        if (LastAnimationState != CurrentAnimationState)
        {
            if (LastAnimationState == Character.midair_animation && IsGrounded)
            {
                CurrentAnimationState = Character.landing_animation;
            }

            //CharacterAnimator.Play(CurrentAnimationState, 0, 0);
            LastAnimationState = CurrentAnimationState;
        }
        */
        // if the health cooldown was up, lower it
        if (HealthDamageCoolDown >= 0.0f)
        {
            HealthDamageCoolDown -= Time.deltaTime;
        }

        if (Character.CurrentHealth <= 0.0f)
        {
            // drop all items and swap to diff squadmate
            Action = -1.0f; //drop all items on death

            //Destroy(this.gameObject);
            StartSelfDestruct();
        }

        //}
        DoTargetCircle();
    }

    private void DoAnimationState()
    {
        //TODO redo this all to be based on current status / moving not keys
        if (AnimationOverrideTimer > 0.0f)
        {
            //Debug.Log("should be getting state from item");
            AnimationOverrideTimer -= Time.deltaTime;

            // set contraints on character transform
            if (AnimationOverrideTimer <= 0.0f)
            {
                //MakeSpeechBubble("finshed animation from item");
                if (HeldItemController.GetItem().PrimaryActionClass == "SPELL")
                {
                    HeldItemController.SetCanDoAction(1.0f);
                }
            }
        } /*
        else if (!IsGrounded && Character.IsPlayer && (JumpCoolDown > 0.0f))
        {
            CurrentAnimationState = Character.midair_animation;
            JumpCoolDown -= Time.deltaTime;
        }
        */ // sprinting for player
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
                    CurrentAnimationState =
                        Character.running_backward_animation;
                }
            }
            else
            {
                //TODO
            }
        } // not sprinting
        else
        {
            if (IsMoving)
            {
                // player movment by key
                if (Character.IsPlayer)
                {
                    if (Input.GetKey("w"))
                    {
                        CurrentAnimationState =
                            Character.walking_forward_animation;
                    }
                    else if (Input.GetKey("s"))
                    {
                        CurrentAnimationState =
                            Character.walking_backward_animation;
                    }
                    else
                    {
                        //TODO do a walking rightleft etc
                        CurrentAnimationState =
                            Character.walking_forward_animation;
                    }
                } // npc based on state
                else
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
        // stamina recharge for npc
        if (!isSprinting)
        {
            if (Character.CurrentStamina < Character.MaxStamina)
            {
                if (Character.CurrentStamina <= 1)
                {
                    Character.CurrentStamina = 2;
                }
                else
                {
                    Character.CurrentStamina =
                        Character.CurrentStamina +
                        Character.CurrentStamina *
                        Character.StaminaRechargeRate;
                }
            }
        }

        //MakeSpeechBubble("im fighitng"+IsFighting.ToString());
        // fighting takes priority
        if (IsFighting)
        {
            AttackTarget();
        }
        else
        {
            SetCharacterCanMove(true);

            // if they are following player or in a squad
            if (Character.IsFollower && Character.squadLeaderId != "")
            {
                // add follow to the queqe of tasks if can
                if (
                    Character.IsFollowing &&
                    CurrentTask != "FOLLOW" &&
                    NextNextTask == ""
                )
                {
                    NextNextTask = "FOLLOW";
                }
            }

            // do quest if they have one
            if (CurrentQuest != null)
            {
                DoQuest();
            }
            else
            {
                // do task specified
                DoTask();
            }
        }
    }

    private void IncrementTask()
    {
        LastTask = CurrentTask;
        CurrentTask = NextTask;
        if (NextNextTask != "")
        {
            NextTask = NextNextTask;
            NextNextTask = "";
        }
        else
        {
            NextTask = LastTask;
        }
    }

    private void DoQuest()
    {
        // path npcs do when they have a quest
    }

    // schedule etc
    private void DoTask()
    {
        /*
        Task list

        FARM find a farm and ask for task
        BANDIT look for factions to fight and also wander around a point
        LAMPLIGHT look for more lamps to light down road etc
        WANDERPOINT wander around a point
        WANDER wander aimlessly
        SLEEP find bed and sleep
        STAND stand still
        BEMERCHANT man a shop building
        SHOP find a merchant and buy something
        HOME go home
        FINDENEMY look for other factions to fight
        MAYOR is the "player" 
        BEBLACKSMITH man a blacksmith shop
        BELUMBERJACK man a lumbermill
        BEALCHEMIST man a potion shop
        GUARD guard their target or their point
        BETRADER do trade quests
        BEMINER work in the mines

        default tasks
        FARM
        LAMPLIGHT
        BANDIT
        MAYOR
        BEBLACKSMITH
        BELUMBERJACK
        BEALCHEMIST
        GUARD
        BETRADER
        BEMINER
        BEMERCHANT



        */
        //Debug.Log("current task is" + CurrentTask + Character.Name);
        if (CurrentTask == "" || CurrentTask == null)
        {
            // if there is a chance to have a task, do that insetad
            if (NextNextTask != "")
            {
                CurrentTask = Character.DefaultTask;
            }
            else
            {
                IncrementTask();
            }
        }

        // debug super helpful
        //MakeSpeechBubble("CURRENT " + CurrentTask + " next " + NextTask + " last " + LastTask + " nextnext " + NextNextTask);
        if (StandingStillTimer > 0.0f)
        {
            bool ShouldOverrideStanding = false;

            // make followers iverride standing if target goes farther away
            if (Character.IsFollowing && FollowTarget != null)
            {
                Transform TargetTransform =
                    FollowTarget.GetComponent<Transform>();
                float distance =
                    Vector3
                        .Distance(CharacterTransform.position,
                        TargetTransform.position);

                ShouldOverrideStanding = distance > 10.0f;
            }

            if (ShouldOverrideStanding)
            {
                // go to next task to try to get to following
                StandingStillTimer = 0.0f;
                SetCharacterCanMove(true);
                //IncrementTask();
            }
            else
            {
                StandingStillTimer -= Time.deltaTime;
                SetCharacterCanMove(false);
            }
        }
        else if (CurrentTask == "FOLLOW")
        {
            bool doneFollowing = FollowSquadLeader();
            if (doneFollowing)
            {
                //MakeSpeechBubble("Done following");
                IncrementTask();
            }
        }
        else if (CurrentTask == "BEFARMER")
        {
            /*
            set to go to nearest farm then wanderpoint, next task is more farm
            if already in farm field, then wanderpoint again until time done then set to sleep
            */
            //find farm sets the new wanderpoint
            if (LastTask == "WANDERPOINT")
            {
                NextTask = "FARM";
            }
            else if (LastTask == "FARM")
            {
                CurrentTask = "WANDERPOINT";
            }
            else
            {
                NextTask = "FARM";
                NextNextTask = "BEFARMER";
                IncrementTask();
            }
        }
        else if (CurrentTask == "FARM")
        {
            // go to farm and get wander range
            //float atFarm = GoFarm();
            float atFarm = GoToBuildingOfType("FARM");

            //MakeSpeechBubble("at farm " + atFarm.ToString());
            if (atFarm != 0.0f)
            {
                SetBuildingHasDoneWork("FARM");

                wanderRange = atFarm;

                //pick a point
                IncrementTask();
            }
        }
        else if (CurrentTask == "BANDIT")
        {
            /*

            Make sure is part of a squad, if not become a leader
            */
            bool inSquadNow = JoinSquadNearbyIfNotYet();

            string wanderTask = "WANDERPOINT";
            if (inSquadNow)
            {
                // if they are in the squad, follow the leader
                if (GetSquadLeaderUUID() != GetUUID())
                {
                    wanderTask = "FOLLOW";
                } // if they are the leader, wander around
            }

            /*
            wander around a point and check for factions to fight
            set ucrrent task to wander, next to bandit
            */
            if (LastTask == wanderTask)
            {
                NextTask = "FINDENEMY";
            }
            else if (LastTask == "FINDENEMY")
            {
                CurrentTask = wanderTask;
            }
            else
            {
                NextTask = "FINDENEMY";
                NextNextTask = "BANDIT";
                IncrementTask();
            }
        }
        else if (CurrentTask == "LAMPLIGHT")
        {
            if (LastTask == "WANDERPOINT")
            {
                NextTask = "FINDENEMY";
            }
            else if (LastTask == "FINDENEMY")
            {
                CurrentTask = "WANDERPOINT";
            }
            else
            {
                NextTask = "FINDENEMY";
                NextNextTask = "LAMPLIGHT";
                IncrementTask();
            }
        }
        else if (CurrentTask == "WANDERPOINT")
        {
            /*
            Wander around a point to a new point, then do next task
            */
            //Debug.Log("wandering point");
            // bandits will wander farther and wait for shorter time
            float timeToStandStill = 10.0f;
            if (LastTask == "BANDIT")
            {
                wanderRange = 10.0f;
                timeToStandStill = 3.0f;
            }

            // TODO based on lasttask get role and set wnader range
            // todo also set this for time to stand still
            bool arrived = WanderAroundPoint(wanderRange);
            if (arrived && !IsMoving)
            {
                StandStillForTime (timeToStandStill);

                IncrementTask();
            }
            /*else{
                if(NextTask != "WANDERPOINT"){
                    NextNextTask = NextTask;
                    NextTask = "WANDERPOINT";
                }
            }
            */
        }
        else if (CurrentTask == "WANDER")
        {
        }
        else if (CurrentTask == "SLEEP")
        {
            // if not home, try to find home, if cant, then do sleep here
            if (!WentHomeToSleep)
            {
                NextNextTask = "SLEEP";
                NextTask = "HOME";
                WentHomeToSleep = true;
                IncrementTask();
            }
            //does nothing
        }
        else if (CurrentTask == "STAND")
        {
            IsMoving = false;
        }
        else if (CurrentTask == "BEMERCHANT")
        {
            // go to work, then check when can go home
            if (LastTask == "CHECKIFDONEWITHWORK")
            {
                NextTask = "MANSHOP";
            }
            else if (LastTask == "MANSHOP")
            {
                CurrentTask = "CHECKIFDONEWITHWORK";
            }
            else
            {
                NextTask = "MANSHOP";
                NextNextTask = "BEMERCHANT";
                IncrementTask();
            }
        }
        else if (CurrentTask == "CHECKIFDONEWITHWORK")
        {
            // if time is > end of day then go home, else incremtn anyway
            if (false)
            {
            }
            else
            {
                StandStillForTime(10.0f);

                IncrementTask();
            }
        }
        else if (CurrentTask == "MANSHOP")
        {
            // man the owned store
            // claim and own a store
            //bool isAtStore = GoShop();
            bool isAtStore = GoToBuildingOfType("SHOP") != 0.0f;
            if (
                isAtStore //} && !IsMoving)
            )
            {
                MakeSpeechBubble("manning shop");
                SetBuildingHasDoneWork("SHOP");
                SetNavAgentDestination(CharacterTransform.position);
                SetCharacterCanMove(false);
                IsMoving = false;
                IncrementTask();
            }
        }
        else if (CurrentTask == "SHOP")
        {
            // go to nearest store
            // do go shop
            // find item and buy somethig
            // ask shop for merhcant
        }
        else if (CurrentTask == "HOME")
        {
            //bool isHome = GoHome();
            bool isHome = GoToBuildingOfType("HOME") != 0.0f;
            if (
                isHome //} && !IsMoving)
            )
            {
                SetNavAgentDestination(CharacterTransform.position);
                SetCharacterCanMove(false);
                IncrementTask();
            }
        }
        else if (CurrentTask == "FINDENEMY")
        {
            CheckForOtherFactionsToFight();

            IncrementTask();
        }
        else if (CurrentTask == "QUEST")
        {
            // TODO read from quest object and do steps, only if step of quest is "DONE" then do incremnt task
            // if is follower, set to ignore this task until not has follow target
        }
        else if (CurrentTask == "")
        {
            IsMoving = false;
        }
        else
        {
            IsMoving = false;
        }

        // reset if stuck in task loop
        if (NextTask == CurrentTask)
        {
            NextTask = "";
        }
    }

    private void SetBuildingHasDoneWork(string buildingType)
    {
        foreach (BuildingController building in Buildings)
        {
            if (building.GetType() == buildingType)
            {
                building.SetHasDoneWork(true);
            }
        }
    }

    private bool FindBuildingOfType(string buildingType)
    {
        BuildingController backupBuilding = null;
        BuildingController foundbuilding = null;
        bool mustUseBackup = true;
        float currentDistanceToBuilding = -1;

        // 100.0f is range to look for house
        Collider[] hitColliders =
            Physics.OverlapSphere(CharacterTransform.position, 100.0f);
        foreach (var hitCollider in hitColliders)
        {
            BuildingController controller =
                hitCollider.gameObject.GetComponent<BuildingController>();
            if (controller != null)
            {
                if (controller.GetType() == buildingType)
                {
                    // compare distance and try to find closest
                    float distanceToThisBuilding =
                        Vector3
                            .Distance(CharacterTransform.position,
                            controller.GetTransform().position);
                    if (currentDistanceToBuilding == -1)
                    {
                        backupBuilding = controller;
                        foundbuilding = controller;
                        currentDistanceToBuilding = distanceToThisBuilding;
                    }
                    else
                    {
                        //now comparing vs backup
                        bool isCloser =
                            currentDistanceToBuilding > distanceToThisBuilding;
                        bool currentIsOwned = foundbuilding.GetOwner() != "";
                        bool newIsOwned = controller.GetOwner() != "";
                        bool backupIsOwned = backupBuilding.GetOwner() != "";

                        // if its closer try it
                        if (isCloser)
                        {
                            // if its unowned, or both it and current are owned, use it
                            if (!newIsOwned || (currentIsOwned && newIsOwned))
                            {
                                backupBuilding = foundbuilding;
                                foundbuilding = controller;

                                currentDistanceToBuilding =
                                    distanceToThisBuilding;
                            }
                            else if (backupIsOwned && newIsOwned)
                            {
                                // if backup is owned, but this is too, use as backup, since closer
                                backupBuilding = foundbuilding;
                            }
                        }
                        else
                        {
                            // if its not closer, but unowned and current is, use it
                            if (!newIsOwned && currentIsOwned)
                            {
                                backupBuilding = foundbuilding;
                                foundbuilding = controller;

                                currentDistanceToBuilding =
                                    distanceToThisBuilding;
                            }
                        }
                    }
                }
            }
        }

        if (foundbuilding != null && backupBuilding != null)
        {
            // if is unclaimed
            if (foundbuilding.GetOwner() == "")
            {
                //MakeSpeechBubble("claiming this farm");
                // assign ownership
                foundbuilding.AssignOwnership(this);

                mustUseBackup = false;

                return true;
            }
            else
            {
                mustUseBackup = true;
            }

            // pick a owned building as a backup if cant get own
            if (mustUseBackup && backupBuilding != null)
            {
                backupBuilding.AssignOwnership(this);
                return true;
            }
        }

        return false;
    }

    private float GoToBuildingOfType(string buildingType)
    {
        // goes to home and if there return
        //bool hasHome = (GetHouseUUID() != "");
        bool hasOne = (GetBuildingUUIDOfType(buildingType) != "");

        if (!hasOne)
        {
            bool foundOne = FindBuildingOfType(buildingType);

            // if failed to find a building, return true to then sleep whereever
            if (!foundOne)
            {
                return 1.0f;
            }
        }
        else
        {
            // go to house transform.positin
            //MakeSpeechBubble("going to house");
            NPCGOTOTargetWithSprint(GetBuildingTransformOfType(buildingType));
        }

        if (hasOne)
        {
            // check if arrived
            float distance =
                Vector3
                    .Distance(CharacterTransform.position,
                    GetBuildingTransformOfType(buildingType).position);
            if (distance <= Character.Reach)
            {
                MakeSpeechBubble("im @" + buildingType);
                return GetBuildingControllerOfType(buildingType)
                    .GetWanderRange();
            }
        }
        return 0.0f;
    }

    private void AttackTarget()
    {
        if (CheckIfTargetIsDead())
        {
            MakeSpeechBubble("i win!");
            IsFighting = false;
            DeTarget();
            return;
        }

        if (CombatTarget == null)
        {
            Target();
            return;
        }
        Transform TargetTransform = CombatTarget.GetComponent<Transform>();

        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float rotationSpeed = 30f; //speed of turning

        float range = 50f; // pursute range
        float range2 = 25f;
        float stop = Character.Reach; // this is range to player

        // if holding weapon, then use its range instead
        if (HeldItemController != null)
        {
            float weaponRange = HeldItemController.GetItem().Range;
            stop = weaponRange;
        }

        //rotate to look at the player
        var distance =
            Vector3
                .Distance(CharacterTransform.position,
                TargetTransform.position);

        /*
        MakeSpeechBubble("stop " +
        stop.ToString() +
        " dist " +
        distance.ToString() +
        " range " +
        range.ToString());
        */
        /*
        if (distance <= range2 && distance >= range)
        {
            MakeSpeechBubble("IM HERE");
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation,
            Quaternion.LookRotation(TargetTransform.position - CharacterTransform.position), rotationSpeed * Time.deltaTime);
        }
        else 
        */
        if (distance <= range && distance > stop)
        {
            //NavAgent.destination = TargetTransform.position;
            //IsMoving = true;
            NPCGOTOTargetWithSprint (TargetTransform);
        }
        else if (distance <= stop)
        // && (NavAgent.enabled))
        {
            //MakeSpeechBubble("im at firest one");
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation =
                Quaternion
                    .Slerp(CharacterTransform.rotation,
                    Quaternion
                        .LookRotation(TargetTransform.position -
                        CharacterTransform.position),
                    rotationSpeed * Time.deltaTime);

            if (ActionCooldown > 0.0f)
            {
                ActionCooldown -= Time.deltaTime;
            }
            else
            {
                Attack();
            }
        }
        /*
        else
        {
            MakeSpeechBubble(" im here>?");
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
        */
    }

    private bool CheckIfTargetIsDead()
    {
        //MakeSpeechBubble("did he die?");
        if (
            CombatTarget == null ||
            CombatTarget.gameObject == null ||
            CombatTarget.gameObject.Equals(null)
        )
        {
            return true;
        }

        if (TargetCharacterController.gameObject.Equals(null))
        {
            return true;
        }

        CharacterController CombatTargetCharacterController =
            CombatTarget.gameObject.GetComponent<CharacterController>();
        if (CombatTargetCharacterController.Equals(null))
        {
            return true;
        }
        else
        {
            if (CombatTargetCharacterController.GetCurrentHealth() <= 0.0f)
            {
                return true;
            }
        }

        if (TargetCharacterController.GetCurrentHealth() <= 0.0f)
        {
            return true;
        }

        return false;
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
            MakeSpeechBubble("unarmed attack");
            Action = 0.0f;
        }
        // set attack cooldown
        //ActionCooldown = 2.5f;
    }

    public void StandStillForTime(float newTime)
    {
        StandingStillTimer = newTime;
    }

    private bool WanderAroundPoint(float wanderRange)
    {
        //float wanderRange = 3.0f;
        //MakeSpeechBubble("Wandering around point: " + WanderPointCenter.ToString() + " to " + WanderPointGoal.ToString());
        if (WanderPointGoal.y <= -1)
        {
            // get a new wanderpoint
            if (WanderPointCenter.y <= -1.0f)
            {
                WanderPointCenter = CharacterTransform.position;
            }

            float newX = Random.Range(-1.0f * wanderRange, wanderRange); // + WanderPointCenter.x;
            float newZ = Random.Range(-1.0f * wanderRange, wanderRange); // + WanderPointCenter.z;

            WanderPointGoal =
                new Vector3(WanderPointCenter.x + newX,
                    WanderPointCenter.y,
                    WanderPointCenter.z + newZ);
            CharacterTransform.rotation =
                Quaternion
                    .Slerp(CharacterTransform.rotation,
                    Quaternion
                        .LookRotation(WanderPointGoal -
                        CharacterTransform.position),
                    90.0f * Time.deltaTime);

            IsMoving = true;
            SetNavAgentDestination (WanderPointGoal);

            return false;
        }
        else
        {
            Vector3 CharXZpos =
                new Vector3(CharacterTransform.position.x,
                    0.0f,
                    CharacterTransform.position.z);
            Vector3 GoalXZpos =
                new Vector3(WanderPointGoal.x, 0.0f, WanderPointGoal.z);

            float distance = Vector3.Distance(CharXZpos, GoalXZpos);
            if (distance < Character.Reach)
            {
                WanderPointGoal = new Vector3(0.0f, -2.0f, 0.0f);
                IsMoving = false;
                SetNavAgentDestination(CharacterTransform.position);

                return true;
            }

            IsMoving = true;

            SetNavAgentDestination (WanderPointGoal);

            // check that have reached destination
            return false;
        }
    }

    private void SetNavAgentDestination(Vector3 goal_position)
    {
        // TODO check is on a navmesh, if not do rescue
        //MakeSpeechBubble("Set destination to " + goal_position.ToString());
        if (NavAgent.enabled)
        {
            NavAgent.destination = goal_position;
        }
    }

    private bool FollowSquadLeader()
    {
        if (FollowTarget == null)
        {
            GetFollowTargetFromSquadLeaderId();
        }

        Transform TargetTransform = FollowTarget.GetComponent<Transform>();

        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float rotationSpeed = 30f; //speed of turning
        float range = 250f; // follow range
        float range2 = 250f;
        float stop = 3.8f; // this is range to player

        //rotate to look at the player
        var distance =
            Vector3
                .Distance(CharacterTransform.position,
                TargetTransform.position);
        if (distance <= range2 && distance >= range)
        {
            Debug.Log("following is here why???");
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation =
                Quaternion
                    .Slerp(CharacterTransform.rotation,
                    Quaternion
                        .LookRotation(TargetTransform.position -
                        CharacterTransform.position),
                    rotationSpeed * Time.deltaTime);
        }
        else if (distance <= range && distance > stop)
        {
            // go to target if within follow range and then stop
            //NavAgent.destination = TargetTransform.position;
            //IsMoving = true;
            NPCGOTOTargetWithSprint (TargetTransform);
        }
        else if ((distance <= stop) && (NavAgent.enabled))
        {
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;
            CharacterTransform.rotation =
                Quaternion
                    .Slerp(CharacterTransform.rotation,
                    Quaternion
                        .LookRotation(TargetTransform.position -
                        CharacterTransform.position),
                    rotationSpeed * Time.deltaTime);

            /*
            // check not too close to squadmate
            CharacterController nearestSquadmateController = CheckIsFarEnoughAwayFromSquadMate(CharacterTransform.position, 3.0f);
            if (nearestSquadmateController != null)
            {
                Vector3 newPos = PickNewPositionAwayFromSquadMate(CharacterTransform.position, nearestSquadmateController);
                SetNavAgentDestination(newPos);
            }
            */
        }
        else
        {
            SetNavAgentDestination(CharacterTransform.position);
            IsMoving = false;

            // if near player, do current task
            WanderPointCenter = CharacterTransform.position; // reset wander point to be set when needed

            //MakeSpeechBubble("doing task" + NextTask);
            return true;
        }
        return false;
    }

    private void NPCGOTOTargetWithSprint(Transform TargetPosition)
    {
        IsMoving = true;

        Vector3 Start = CharacterTransform.position;
        Vector3 End = TargetPosition.position;

        SetNavAgentDestination (End);
        float distance_to_go = (Start - End).magnitude;

        // sprinting cooldown for npcs
        if (isSprintingCooldown)
        {
            if (Character.CurrentStamina > StaminaLevelBeforeSprintAgain)
            {
                isSprintingCooldown = false;
            }
        }

        if (
            (distance_to_go > 20.0f) &&
            (Character.CurrentStamina > 1) &&
            (!isSprintingCooldown)
        )
        {
            Character.CurrentStamina =
                Character.CurrentStamina - Character.StaminaUseRate;
            Character.CurrentSpeed =
                Character.BaseMovementSpeed +
                (
                Character.StaminaBonusSpeed *
                (Character.CurrentStamina / Character.MaxStamina * 0.7f)
                );
            isSprinting = true;
        }
        else
        {
            if (isSprinting)
            {
                isSprintingCooldown = true;
            }
            isSprinting = false;

            Character.CurrentSpeed = Character.BaseMovementSpeed;
        }

        NavAgent.speed = Character.CurrentSpeed;
        //Debug.Log("im " + Character.Name + " and my current speed is" + NavAgent.speed+" im sprinting"+isSprinting+" stamina"+Character.CurrentStamina);
    }

    private void RandomMove()
    {
        float maxForce = 50f;
        Vector3 position =
            new Vector3(Random.Range(-1f * maxForce, maxForce),
                Random.Range(-1f * maxForce, maxForce),
                Random.Range(-1f * maxForce, maxForce));

        //Debug.Log(position);
        rb.AddForce (position);
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
            Character.CurrentStamina =
                Character.CurrentStamina - Character.StaminaUseRate;
            Character.CurrentSpeed =
                Character.BaseMovementSpeed +
                (
                Character.StaminaBonusSpeed *
                (Character.CurrentStamina / Character.MaxStamina * 0.7f)
                );
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
                    Character.CurrentStamina =
                        Character.CurrentStamina +
                        Character.CurrentStamina *
                        Character.StaminaRechargeRate;
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
            transform.rotation =
                Quaternion
                    .Slerp(transform.rotation,
                    Quaternion.LookRotation(forward),
                    0.2f);
        }

        // check if target is dead
        if (IsFighting)
        {
            bool wonFight = CheckIfTargetIsDead();
            if (wonFight)
            {
                IsFighting = false;
                DeTarget();
            }
        }

        // if fighting, must look at enemy
        if (IsFighting && CombatTarget != null)
        {
            var distance =
                Vector3
                    .Distance(CharacterTransform.position,
                    CombatTarget.transform.position);

            if (HeldItemController != null)
            {
                // only lock on in range
                if (distance <= HeldItemController.GetItem().Range)
                {
                    CharacterTransform.rotation =
                        Quaternion
                            .Slerp(CharacterTransform.rotation,
                            Quaternion
                                .LookRotation(CombatTarget.transform.position -
                                CharacterTransform.position),
                            30.0f * Time.deltaTime);
                }
            }
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
        CharacterController HitCharacterController = null;
        bool interacted = false;

        // if in shop, get hit character controller from shop owner
        if (isInShop)
        {
            HitCharacterController =
                CurrentShopController.GetOwnerControllerIfPresent();
            if (HitCharacterController != null)
            {
                interacted = true;
            }
        }
        else
        {
            Vector3 center =
                CharacterTransform.position +
                (CharacterTransform.forward * (0.5f * Character.Reach));
            Collider[] hitColliders =
                Physics.OverlapSphere(center, (0.5f * Character.Reach));
            int j = 0;
            while (j < hitColliders.Length)
            {
                HitCharacterController =
                    hitColliders[j]
                        .gameObject
                        .GetComponent<CharacterController>();
                if (HitCharacterController != null)
                {
                    //MakeSpeechBubble("i almost interacted with a person");
                    // if not targeting self or sqwuad
                    if (HitCharacterController.GetUUID() != GetUUID())
                    {
                        //MakeSpeechBubble("Interacted with " +
                        //HitCharacterController.GetCharacter().Name);
                        // Do interaction with a character controller (talking)
                        // start the dialog action TODO
                        // creates a dialog controller
                        // self joins dialog controller
                        // tells other charatter to join dialog controller
                        // getcandodialog will be the faction check etc TODO
                        interacted = true;
                        break;
                    }
                }

                /*
                else
                {
                    if (hitColliders[j].gameObject.tag != "Ground")
                    {
                        // hitColliders[j].gameObject is what we interacted with
                        MakeSpeechBubble("Interacted with world"+hitColliders[j].gameObject.ToString());
                        // TODO do interaction here

                        interacted = true;
                        break;
                    }

                }
                */
                j += 1;
            }
        }

        if (!interacted)
        {
            // pushed E and didnt get anything, do squad commands
            MakeSpeechBubble("Squad cmd / world interaction");
        }
        else
        {
            // do interaction here
            HitCharacterController.DoInteractAction(this.gameObject);
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
            if (
                !hasTarget // if doesnt have a target, find one
            )
            {
                //MakeSpeechBubble("fiding targt");
                rand = Random.Range(1, 254);

                float radius = Character.TargetRange / 2.0f;
                Vector3 center =
                    CharacterTransform.position +
                    (CharacterTransform.forward * Character.TargetRange / 2.0f);
                Collider[] hitColliders = Physics.OverlapSphere(center, radius);
                int i = 0;

                //Vector3 SummonPositon = center;
                while (i < hitColliders.Length)
                {
                    if (
                        hitColliders[i]
                            .gameObject
                            .GetComponent<CharacterController>() !=
                        null
                    )
                    {
                        int TargetRand =
                            hitColliders[i]
                                .gameObject
                                .GetComponent<CharacterController>()
                                .GetRand();
                        if (TargetRand != rand)
                        {
                            SetTarget(hitColliders[i].gameObject);

                            break;
                        }
                    }
                    i++;
                }
            } // if does have target, de-target
            else
            {
                Destroy (TargetBeaconObject);
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
        //MakeSpeechBubble("I was interacted with by " +
        //WhoInteracted +
        //" my leader is" +
        //Character.squadLeaderId);
        //MakeSpeechBubble(Character.IsFollower.ToString());
        // summon dialog manager
        CharacterController WhoInteractedController =
            WhoInteracted.GetComponent<CharacterController>();

        // if they can join a dialog do so
        if (CanJoinDialog && WhoInteractedController.GetCanJoinDialog())
        {
            // join dialog as 2nd person
            Vector3 SummonPositon =
                CharacterTransform.position + new Vector3(0.0f, 2.0f, 0.0f);
            DialogManagerObject =
                Instantiate(DialogManagerPreFab,
                SummonPositon,
                Quaternion.identity);
            CurrentDialogManager =
                DialogManagerObject.GetComponent<DialogManager>();

            CurrentDialogManager.StartDialog(WhoInteractedController, this);
        }

        // TODO do dialog stuff before this part
        /*

        // If a follower, then make then interact toggles follow
        if (Character.IsFollower)
        {

            CharacterController InteractedCharacterController = WhoInteracted.GetComponent<CharacterController>();
            if (InteractedCharacterController != null)
            {
                if (Character.squadLeaderId == "")
                {
                    MakeSpeechBubble("Joining squad of " + InteractedCharacterController);
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
            
        }

        */
        NeedsUIUpdate = true;
    }

    public void MakeSpeechBubble(string WhatToSay)
    {
        // TODO make stacking speechbubbles above head max 3 with string of what to say
        // make expire after n seconds
        // make new list of speechbubbles
        List<GameObject> tempList = new List<GameObject>();

        // move all bubbles up and remove null bubbles from list
        foreach (GameObject bubble in SpeechBubbles)
        {
            if (bubble != null)
            {
                SpeechBubble speechBubble = bubble.GetComponent<SpeechBubble>();
                speechBubble.moveUp();
                tempList.Add (bubble);
            }
        }
        SpeechBubbles = tempList;

        // make new bubble
        //float RandZ = Random.Range(-0.2f, 0.2f);
        //float RandX = Random.Range(-0.2f, 0.2f);
        //Vector3 SummonPositon = CharacterTransform.position + new Vector3(RandX, 2.0f, RandZ);
        Vector3 SummonPositon =
            CharacterTransform.position + new Vector3(0.0f, 2.0f, 0.0f);
        SpeechBubbleObject =
            Instantiate(SpeechBubblePreFab, SummonPositon, Quaternion.identity);
        SpeechBubbleObject.GetComponent<SpeechBubble>().SetText(WhatToSay);

        //SpeechBubbleObject.gameObject.GetComponent<Transform>().parent = CharacterTransform;
        SpeechBubbleObject
            .gameObject
            .GetComponent<Transform>()
            .SetParent(CharacterTransform, true);

        SpeechBubbles.Add (SpeechBubbleObject);
    }

    private void DoTargetedAction(CharacterController whoTargeted) // character will make a beacon above their head
    {
        MakeSpeechBubble("I was targetd");
        NeedsUIUpdate = true;

        // check if is leader of squad, alert all others in squad
        //TODO reacte to being targeted etc
        if (SquadCharacterControllers != null)
        {
            float allignment =
                GetMyAlignmentWithFaction(whoTargeted.GetFaction());

            // if targeted by enemy
            if (allignment < 0.0f)
            {
                IsFighting = true;

                //SetTarget(whoTargeted.gameObject);
                MakeSpeechBubble("Help! I'm under attack!");
                CallSquadForHelp (whoTargeted);
            }
        }
    }

    private void CallSquadForHelp(CharacterController whoTargeted)
    {
        // all members in squad who have no target target who targed me
        foreach (CharacterController squadCharacter in SquadCharacterControllers
        )
        {
            if (squadCharacter.GetUUID() != GetUUID())
            {
                // set target
                squadCharacter
                    .MakeSpeechBubble("Coming " + Character.Name + "!");

                // if they have no target
                if (!squadCharacter.GetHasTarget())
                {
                    // then have them attack the attacker
                    squadCharacter.SetTarget(whoTargeted.gameObject);
                    squadCharacter.IsFighting = true;
                }
            }
        }
    }

    private bool JoinSquadNearbyIfNotYet()
    {
        if (GetSquadLeaderUUID() != "")
        {
            return true;
        }

        //return true if joined success
        // look for squad of same faction within n meters
        float distToLook = 25.0f;
        Collider[] hitColliders =
            Physics.OverlapSphere(CharacterTransform.position, distToLook);
        foreach (var hitCollider in hitColliders)
        {
            CharacterController controller =
                hitCollider.gameObject.GetComponent<CharacterController>();
            if (controller != null)
            {
                if (GetMyAlignmentWithFaction(controller.GetFaction()) >= 1.0f)
                {
                    //MakeSpeechBubble("found character of same faction nearby!");
                    string otherSquadLeader = controller.GetSquadLeaderUUID();
                    if (otherSquadLeader == "" && GetSquadLeaderUUID() != "")
                    {
                        // if they dont have leader and I do, join me
                        controller.JoinSquadOfCharacter(this);
                        return true;
                    }
                    else if (
                        otherSquadLeader != "" && GetSquadLeaderUUID() == ""
                    )
                    {
                        // if they have leader, and i dont, join them
                        JoinSquadOfCharacter (controller);
                        return true;
                    }
                    else if (
                        otherSquadLeader == "" && GetSquadLeaderUUID() == ""
                    )
                    {
                        // if neither has leader, im leader
                        // become leader
                        JoinSquadOfCharacter(this); // join own squad

                        //SetSquadLeaderUUID(GetUUID());
                        controller.JoinSquadOfCharacter(this);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void CheckIfItemInHand() //updates the hand var
    {
        //TODO enable or disable spell if hand empty
        Transform handTransform = Hand.GetComponent<Transform>();
        int i = 0;
        foreach (Transform child in handTransform)
        {
            //Debug.Log("is child of hand" + child);
            ItemController heldItemController =
                child.gameObject.GetComponent<ItemController>();
            if (heldItemController != null)
            {
                HeldItemController = heldItemController;
            }
            i += 1;
        }
        if (i >= 1)
        {
            HasItemInHand = true;
        }
        else
        {
            HasItemInHand = false;
            HeldItemController = null;
        }

        // if somehow holding many items, drop em all
        if (i >= 2)
        {
            ItemStatus = "Dropping";
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

    public void AddValueToMana(float value)
    {
        Character.CurrentMana += value;

        if (Character.CurrentMana <= 0.0f)
        {
            Character.CurrentMana = 0.0f;
            AddValueToHealth(value * 0.25f);
        }
    }

    public void AddValueToHealth(float value)
    {
        if (value < 0.0f && CurrentDialogManager != null)
        {
            CurrentDialogManager.NotifyOfLeaving();
        }

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

    public float GetCurrentMana()
    {
        return this.Character.CurrentMana;
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
        DeTarget();

        Transform TargetTransform = TargetToSet.GetComponent<Transform>();
        float RandZ = Random.Range(-0.2f, 0.2f);
        float RandX = Random.Range(-0.2f, 0.2f);

        //Vector3 SummonPositon = TargetTransform.position + new Vector3(0.0f, 2.0f, 0.0f);
        Vector3 SummonPositon =
            TargetTransform.position + new Vector3(RandX, 2.0f, RandZ);
        TargetBeaconObject =
            Instantiate(TargetBeacon, SummonPositon, Quaternion.identity);

        // set the color of target beacon
        SpriteRenderer[] SpriteRendersInTargetBeacon =
            TargetBeaconObject
                .gameObject
                .GetComponentsInChildren<SpriteRenderer>();
        SpriteRendersInTargetBeacon[0].color = UIColor;

        //Light[] LightsInTargetBeacon = TargetBeaconObject.gameObject.GetComponentsInChildren<Light>();
        //LightsInTargetBeacon[0].color = UIColor;
        hasTarget = true;
        CombatTarget = TargetToSet;
        TargetCharacter =
            TargetToSet
                .gameObject
                .GetComponent<CharacterController>()
                .GetCharacter();
        TargetCharacterController =
            CombatTarget.GetComponent<CharacterController>();

        //make the target beacon a child of its taret
        TargetBeaconObject.gameObject.GetComponent<Transform>().parent =
            CombatTarget.GetComponent<Transform>();

        // target does action if they were targeted
        TargetCharacterController.DoTargetedAction(this);
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
        MakeSpeechBubble("Saving " + Character.Name);
        Character.x_pos = CharacterTransform.position.x;
        Character.y_pos = CharacterTransform.position.y;
        Character.z_pos = CharacterTransform.position.z;
        CDM.Save (Character);
    }

    public void Load(string CharacterSaveFileFolder, string CharacterSaveFile)
    {
        CDM.Init (CharacterSaveFileFolder, CharacterSaveFile);

        Character = CDM.Load();

        // set world postion
        // TODO move this to recall potion
        //CharacterTransform.position = new Vector3(Character.x_pos, Character.y_pos, Character.z_pos);
        MakeSpeechBubble("Loaded data for " +
        Character.Name +
        " from " +
        CharacterSaveFile);
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
    public bool
    SwapIntoTarget(CharacterController SwapTargetCharacterController)
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
        // if has no leader, set self to leader as needed
        /*
        if (Character.squadLeaderId == "")
        {
            SetSquadLeaderUUID(GetUUID());
        }
        */
        return Character.squadLeaderId;
    }

    public void SetSquadLeaderUUID(string newUUID)
    {
        Character.squadLeaderId = newUUID;
    }

    private void GetFollowTargetFromSquadLeaderId()
    {
        // if character is a foller and in a squad find follow target
        if (Character.squadLeaderId != "")
        {
            var characterControllersList =
                FindObjectsOfType<CharacterController>();
            string id;
            foreach (CharacterController controller in characterControllersList)
            {
                id = controller.GetUUID();

                //Debug.Log("found id" + id);
                if (id == Character.squadLeaderId)
                {
                    FollowTarget = controller.gameObject;

                    //Debug.Log(Character.Name + " found my followtagret " + FollowTarget);
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

    public void LeaveSquad()
    {
        JoinSquadLeadBy("");
        SquadCharacterControllers = null; // blank the list
    }

    // either joins a squad that the inviter is in and climbs that tree, or joins their squad owned by them
    public void JoinSquadOfCharacter(CharacterController InviterController)
    {
        string InviterID = InviterController.GetUUID();
        string InviterLeaderID = InviterController.GetSquadLeaderUUID();

        if (InviterController.GetUUID() == GetUUID())
        {
            // update the list of character controllers to a new empty list if i joined own squad
            SquadCharacterControllers = new List<CharacterController>();
        }

        // add to list of squadmates
        InviterController.SquadCharacterControllers.Add(this);

        // update all members
        InviterController
            .SetSquadListAndUpdateOthers(InviterController
                .SquadCharacterControllers);

        // if inviter is leader or has none, join them
        if (InviterID == InviterLeaderID || InviterLeaderID == "")
        {
            JoinSquadLeadBy (InviterID);
        }
        else
        {
            JoinSquadLeadBy (InviterLeaderID);
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

    private void CheckForOtherFactionsToFight()
    {
        Collider[] hitColliders =
            Physics
                .OverlapSphere(CharacterTransform.position,
                Character.TargetRange);
        foreach (var hitCollider in hitColliders)
        {
            CharacterController controller =
                hitCollider.gameObject.GetComponent<CharacterController>();
            if (controller != null)
            {
                if (GetMyAlignmentWithFaction(controller.GetFaction()) < 0.0f)
                {
                    //Debug.Log(Character.id+"I should attack"+controller.GetUUID());
                    SetTarget(controller.gameObject);
                    IsFighting = true;
                }
            }
        }
    }

    private void DetermineMyFaction()
    {
        // sets the current faction to the one that character is highest with
        myFaction = "none";
        float highestFactionScore = 0.0f;

        if (Character.magic_faction > highestFactionScore)
        {
            myFaction = "magic_faction";
            highestFactionScore = Character.magic_faction;
        }

        if (Character.tech_faction > highestFactionScore)
        {
            myFaction = "tech_faction";
            highestFactionScore = Character.tech_faction;
        }
        if (Character.bandit_faction > highestFactionScore)
        {
            myFaction = "bandit_faction";
            highestFactionScore = Character.bandit_faction;
        }
        if (Character.lamplighter_faction > highestFactionScore)
        {
            myFaction = "lamplighter_faction";
            highestFactionScore = Character.lamplighter_faction;
        }
        if (Character.settlements_faction > highestFactionScore)
        {
            myFaction = "settlements_faction";
            highestFactionScore = Character.settlements_faction;
        }
        if (Character.farmer_faction > highestFactionScore)
        {
            myFaction = "farmer_faction";
            highestFactionScore = Character.farmer_faction;
        }

        if (Character.wild_faction > highestFactionScore)
        {
            myFaction = "wild_faction";
            highestFactionScore = Character.wild_faction;
        }
    }

    public string GetFaction()
    {
        return myFaction;
    }

    // gets characters alignment with each faction dependin on which was asked for
    public float GetMyAlignmentWithFaction(string faction)
    {
        if (faction == "magic_faction")
        {
            return Character.magic_faction;
        }
        else if (faction == "tech_faction")
        {
            return Character.tech_faction;
        }
        else if (faction == "bandit_faction")
        {
            return Character.bandit_faction;
        }
        else if (faction == "lamplighter_faction")
        {
            return Character.lamplighter_faction;
        }
        else if (faction == "settlements_faction")
        {
            return Character.settlements_faction;
        }
        else if (faction == "farmer_faction")
        {
            return Character.farmer_faction;
        }
        else if (faction == "wild_faction")
        {
            return Character.wild_faction;
        }

        return 0.0f;
    }

    public void StartSelfDestruct()
    {
        selfDestuctStarted = true;
        NeedsUIUpdate = true;
    }

    private void SelfDestruct()
    {
        if (selfDestructTimer <= 0.0f)
        {
            DeTarget();
            Destroy(this.gameObject);
        }
        else
        {
            IsMoving = false;
            DeTarget();
            selfDestructTimer -= Time.deltaTime;
            if (FollowTarget != null)
            {
                CharacterController controller =
                    FollowTarget.gameObject.GetComponent<CharacterController>();
                controller.SetNeedsUIUpdate(true);
                Character.squadLeaderId = "";
            }
        }
    }

    private void SetCharacterCanMove(bool newState)
    {
        //CharacterTransform
        if (newState)
        {
            // can move
            rb.constraints = RigidbodyConstraints.None;

            // if has target face them if close enough
            if (hasTarget && CombatTarget != null)
            {
                float distance =
                    Vector3
                        .Distance(CharacterTransform.position,
                        CombatTarget.gameObject.transform.position);
                float lockOnRange = Character.Reach;
                if (HeldItemController != null)
                {
                    float weaponRange = HeldItemController.GetItem().Range;
                    lockOnRange = weaponRange;
                }
                if (distance <= lockOnRange)
                {
                    CharacterTransform.rotation =
                        Quaternion
                            .Slerp(CharacterTransform.rotation,
                            Quaternion
                                .LookRotation(CombatTarget
                                    .gameObject
                                    .transform
                                    .position -
                                CharacterTransform.position),
                            90.0f * Time.deltaTime);
                }
            }
        }
        else
        {
            rb.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;

            // cant move
        }
    }

    private void SetColor()
    {
        // sets players unique color
        UIColor = Color.yellow;
        float red = Random.Range(0.05f, 0.95f);
        float green = Random.Range(0.05f, 0.95f);
        float blue = Random.Range(0.05f, 0.95f);

        UIColor = new Color(red, green, blue, 0.85f);
    }

    private void DoTargetCircle()
    {
        if (hasTarget && IsFighting)
        {
            // if have target reenable circle
            if (!Circle.enabled)
            {
                Circle.enabled = true;
            }
        }
        else
        {
            Circle.enabled = false;
        }
    }

    private void DeTarget()
    {
        Destroy (TargetBeaconObject);
        CombatTarget = null;
        TargetCharacter = null;
        hasTarget = false;
        this.TargetCoolDown = 0.0f;
    }

    public void SetVelocity(Vector3 VelocityVector)
    {
        SetCharacterCanMove(true);
        rb.velocity = VelocityVector;
    }

    public string GetBuildingUUIDOfType(string buildingType)
    {
        foreach (BuildingController building in Buildings)
        {
            if (building.GetType() == buildingType)
            {
                return building.GetUUID();
            }
        }

        // retrun blank if no building of that type
        return "";
    }

    public Transform GetBuildingTransformOfType(string buildingType)
    {
        foreach (BuildingController building in Buildings)
        {
            if (building.GetType() == buildingType)
            {
                return building.GetTransform();
            }
        }

        // retrun blank if no home
        return null;
    }

    public void AddBuildingToList(BuildingController buildingToAdd)
    {
        // cant have multiple of the same type of building in list
        if (GetBuildingControllerOfType(buildingToAdd.GetType()) == null)
        {
            Buildings.Add (buildingToAdd);
        }
    }

    private BuildingController GetBuildingControllerOfType(string buildingType)
    {
        foreach (BuildingController building in Buildings)
        {
            if (building.GetType() == buildingType)
            {
                return building;
            }
        }

        // retrun blank if no home
        return null;
    }

    public void RemoveBuildingFromListByUUID(string UUIDToRemove)
    {
        List<BuildingController> tempList = new List<BuildingController>();

        // move all bubbles up and remove null bubbles from list
        foreach (BuildingController building in Buildings)
        {
            if (building.GetUUID() != UUIDToRemove)
            {
                tempList.Add (building);
            }
        }
        Buildings = tempList;
    }

    public string GetCurrentTask()
    {
        return CurrentTask;
    }

    public string GetDefaultTask()
    {
        return Character.DefaultTask;
    }

    public void SetNextNextTask(string taskToAdd)
    {
        NextNextTask = taskToAdd;
    }

    public void OverrideNextTaskAndPushbackNextTask(string taskToAdd)
    {
        NextNextTask = NextTask;
        NextTask = taskToAdd;
    }

    public bool GetIsFollowingPlayer()
    {
        if (Character.IsFollowing && FollowTarget != null)
        {
            CharacterController followController =
                FollowTarget.gameObject.GetComponent<CharacterController>();
            return followController.GetIsPlayer();
        }
        return false;
    }

    public void WakeUp()
    {
        //Debug.Log("im waking up");
        if (CurrentTask == "SLEEP")
        {
            // heal on sleeping
            Character.CurrentHealth = Character.MaxHealth;
            WentHomeToSleep = false;
            NextTask = Character.DefaultTask; // wake up and do default stuff
            IncrementTask();
            LastTask = Character.DefaultTask; // remove sleep from last task
        }
    }

    public bool GetIsInDialog()
    {
        return IsInDialog;
    }

    // if not fighting then return if can join
    public bool GetCanJoinDialog()
    {
        if (!IsFighting)
        {
            return CanJoinDialog;
        }
        return false;
    }

    public void JoinDialog(
        DialogManager newDialogManager,
        Transform otherCharacterTransform
    )
    {
        CurrentDialogManager = newDialogManager;
        CanJoinDialog = false;
        IsInDialog = true;
        SetCharacterCanMove(false);
        NeedsUIUpdate = true;

        // if not player disable nav agent
        if (!GetIsPlayer())
        {
            IsMoving = false;
            SetNavAgentStateFromIsMoving();
        }

        // face other character
        CharacterTransform.rotation =
            Quaternion
                .Slerp(CharacterTransform.rotation,
                Quaternion
                    .LookRotation(otherCharacterTransform.position -
                    CharacterTransform.position),
                90.0f * Time.deltaTime);
    }

    public void LeaveDialog()
    {
        CurrentDialogManager = null;
        CanJoinDialog = true;
        IsInDialog = false;
        SetCharacterCanMove(true);

        //MakeSpeechBubble("bye!");
        SetDialogText("");
        NeedsUIUpdate = true;

        if (!GetIsPlayer())
        {
            IsMoving = true;
            SetNavAgentStateFromIsMoving();
        }
    }

    public void DoDialog()
    {
        // if is player then they can still move but not attack etc
        // is meant to all be controlled from dialog manager
        // if isplayer, then get update from ui
        NeedsUIUpdate = true;

        // act as dead end for ai tree, so it will be all controlled by dialog manager

        /*
        MakeSpeechBubble("im in a dialog, managed by a dialog manager");

        if (GetIsPlayer())
        {
        }
        */
    }

    public void SetIsInShop(
        bool newStatus,
        BuildingController newCurrentShopController
    )
    {
        isInShop = newStatus;
        CurrentShopController = newCurrentShopController;
    }

    public string GetTownUUID()
    {
        return TownUUID;
    }

    public void SetTownUUID(string newUUID)
    {
        TownUUID = newUUID;
    }

    public void SetInBuildMode(bool newStatus)
    {
        inBuildMode = newStatus;
    }

    public ItemController GetHeldItemController()
    {
        return HeldItemController;
    }

    public void SetDialogText(string newText)
    {
        DialogText = newText;
    }

    public string GetDialogText()
    {
        return DialogText;
    }

    public void SetSquadList(List<CharacterController> newList)
    {
        // sets self squadlist
        SquadCharacterControllers = newList;
    }

    public void SetSquadListAndUpdateOthers(List<CharacterController> newList)
    {
        // sets all in squad to the same squadlist
        foreach (CharacterController squadCharacter in newList)
        {
            squadCharacter.SetSquadList (newList);
        }
    }

    public void SetCameraWithHUD(GameObject newObject)
    {
        CameraWithHUD = newObject;
    }

    public CameraFollow GetCameraFollow()
    {
        // gets camera follow object
        CameraFollow camFollow = CameraWithHUD.GetComponent<CameraFollow>();
        if (camFollow != null)
        {
            return camFollow;
        }

        return null;
    }

    public void SetTown(TownController newTown)
    {
        Town = newTown;
    }

    public TownController GetTown()
    {
        return Town;
    }

    public float GetForwardMovement()
    {
        if (IsMoving)
        {
            if (Character.IsPlayer && Input.GetKey("s"))
            {
                return -1.0f;
            }
            return 1.0f;
        }
        return 0.0f;
    }

    public float GetLeftRightMovement()
    {
        if (IsMoving)
        {
            return 1.0f;
        }
        return 0.0f;
    }

    public bool GetIsSprinting()
    {
        return isSprinting;
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public void PopulateListsOfCustomizeOptions()
    {
        // just populates the lists, doesnt do anything with them
        // like a refresh for loading
        //reset lists
        TorsoOptions = new List<GameObject>();
        BeltBackpackOptions = new List<GameObject>();
        HeadOptions = new List<GameObject>();
        FaceOptions = new List<GameObject>();
        HandsOptions = new List<GameObject>();
        ShoulderOptions = new List<GameObject>();
        ShoeOptions = new List<GameObject>();

        // populate them
        PopulateCustomizatioListsFromChildrenRecusrively(this.gameObject);

        //DebugPrintObjectsInList (TorsoOptions);
        //DebugPrintObjectsInList (BeltBackpackOptions);
        //DebugPrintObjectsInList (HeadOptions);
        //DebugPrintObjectsInList (FaceOptions);
        //DebugPrintObjectsInList (HandsOptions);
        //DebugPrintObjectsInList (ShoulderOptions);
        //DebugPrintObjectsInList (ShoeOptions);
    }

    public void PopulateCustomizatioListsFromChildrenRecusrively(
        GameObject parentObj
    )
    {
        // name
        string name = parentObj.transform.name;

        // populate torso list
        if (name.Contains("Cloth"))
        {
            TorsoOptions.Add (parentObj);
        }

        // if Belt or Backpack
        if (name.Contains("Belt") || name.Contains("Back"))
        {
            if (!name.ToLower().Contains("hold"))
            {
                BeltBackpackOptions.Add (parentObj);
            }
        }

        // if has Hat Crown Hair Helm
        // TODO crown options sometime maybe
        if (
            name.Contains("Hat") ||
            (name.Contains("Hair") && !name.Contains("Half")) ||
            name.Contains("Helm")
        )
        {
            HeadOptions.Add (parentObj);
        }

        // if has Face
        if (name.Contains("Face"))
        {
            FaceOptions.Add (parentObj);
        }

        // if has Glove
        if (name.Contains("Glove"))
        {
            HandsOptions.Add (parentObj);
        }

        // if has Shoulder
        if (name.Contains("Shoulder"))
        {
            ShoulderOptions.Add (parentObj);
        }

        // if has Shoe
        if (name.Contains("Shoe"))
        {
            ShoeOptions.Add (parentObj);
        }

        //Debug.Log(parentObj.transform.name + parentObj.activeInHierarchy);
        foreach (Transform child in parentObj.transform)
        {
            PopulateCustomizatioListsFromChildrenRecusrively(child.gameObject);
        }
    }

    private void DebugPrintObjectsInList(List<GameObject> listToPrint)
    {
        string listString = "";
        foreach (GameObject obj in listToPrint)
        {
            listString = listString + obj.ToString() + ", ";
        }
        Debug.Log (listString);
    }

    public int
    GetIndexFromOneOfOptionInListThatIsActive(List<GameObject> listToEval)
    {
        int i = 1;
        foreach (GameObject obj in listToEval)
        {
            //Debug.Log(obj.transform.name + obj.activeInHierarchy.ToString());
            if (obj.activeInHierarchy)
            {
                return i;
            }

            i = i + 1;
        }
        return 0;
    }

    private void SetIndexFromOneOfOptionInListThatIsActive(
        List<GameObject> listToEval,
        int activeIndex
    )
    {
        int i = 1;
        foreach (GameObject obj in listToEval)
        {
            if (activeIndex == i)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }

            i = i + 1;
        }
    }

    public void SaveCurrentCustomizedValuesToCharacterData()
    {
        // takes current values and puts them into the character data object
        Character.TorsoOption =
            GetIndexFromOneOfOptionInListThatIsActive(TorsoOptions);
        Character.BeltBackpackOption =
            GetIndexFromOneOfOptionInListThatIsActive(BeltBackpackOptions);
        Character.HeadOption =
            GetIndexFromOneOfOptionInListThatIsActive(HeadOptions);
        Character.FaceOption =
            GetIndexFromOneOfOptionInListThatIsActive(FaceOptions);
        Character.HandsOption =
            GetIndexFromOneOfOptionInListThatIsActive(HandsOptions);
        Character.ShoulderOption =
            GetIndexFromOneOfOptionInListThatIsActive(ShoulderOptions);
        Character.ShoeOption =
            GetIndexFromOneOfOptionInListThatIsActive(ShoeOptions);
    }

    public void LoadCurrentCustomizedValuesFromCharacterData()
    {
        // takes current values from character save and applies them to the lists
        SetIndexFromOneOfOptionInListThatIsActive(TorsoOptions,
        Character.TorsoOption);
        SetIndexFromOneOfOptionInListThatIsActive(BeltBackpackOptions,
        Character.BeltBackpackOption);
        SetIndexFromOneOfOptionInListThatIsActive(HeadOptions,
        Character.HeadOption);
        SetIndexFromOneOfOptionInListThatIsActive(FaceOptions,
        Character.FaceOption);
        SetIndexFromOneOfOptionInListThatIsActive(HandsOptions,
        Character.HandsOption);
        SetIndexFromOneOfOptionInListThatIsActive(ShoulderOptions,
        Character.ShoulderOption);
        SetIndexFromOneOfOptionInListThatIsActive(ShoeOptions,
        Character.ShoeOption);
    }

    public List<List<GameObject>> GetCustomizationMatrix()
    {
        // returns list of lists
        List<List<GameObject>> CustomizationOptionsMatrix =
            new List<List<GameObject>>();

        CustomizationOptionsMatrix.Add (TorsoOptions);
        CustomizationOptionsMatrix.Add (BeltBackpackOptions);
        CustomizationOptionsMatrix.Add (HeadOptions);
        CustomizationOptionsMatrix.Add (FaceOptions);
        CustomizationOptionsMatrix.Add (HandsOptions);
        CustomizationOptionsMatrix.Add (ShoulderOptions);
        CustomizationOptionsMatrix.Add (ShoeOptions);

        return CustomizationOptionsMatrix;
    }

    public List<string> GetTasksForSave()
    {
        List<string> TasksList = new List<string>();

        TasksList.Add (CurrentTask);
        TasksList.Add (NextTask);
        TasksList.Add (NextNextTask);
        TasksList.Add (LastTask);

        return TasksList;
    }

    public void SetTasksFromSave(List<string> TasksList)
    {
        CurrentTask = TasksList[0];
        NextTask = TasksList[1];
        NextNextTask = TasksList[2];
        LastTask = TasksList[3];
    }

    public Color GetColorForSave()
    {
        return UIColor;
    }

    public void SetColorFromSave(Color newColor)
    {
        UIColor = newColor;
    }

    public bool GetIsFightingForSave()
    {
        return IsFighting;
    }

    public void SetIsFightingFromSave(bool newValue)
    {
        IsFighting = newValue;
    }

    public bool GetWentToSleepForSave()
    {
        return WentHomeToSleep;
    }

    public void SetWentToSleepFromSave(bool newValue)
    {
        WentHomeToSleep = newValue;
    }

    public float GetStandingStillTimerForSave()
    {
        return StandingStillTimer;
    }

    public void SetStandingStillTimerFromSave(float newValue)
    {
        StandingStillTimer = newValue;
    }

    public string GetFollowTargetUUIDForSave()
    {
        if (FollowTarget != null)
        {
            return FollowTarget.GetComponent<CharacterController>().GetUUID();
        }
        return "";
    }

    public void SetFollowTargetFromSave(GameObject newFollowTarget)
    {
        FollowTarget = newFollowTarget;
    }

    public string GetTargetedCharacterUUIDForSave()
    {
        if (TargetCharacterController != null)
        {
            return TargetCharacterController.GetUUID();
        }
        return "";
    }

    public void SetTargetedCharacterFromSave(GameObject newTarget)
    {
        // TODO make sure this works
        SetTarget (newTarget);
    }

    public List<BuildingController> GetBuildingControllersForSave()
    {
        return Buildings;
    }

    public void SetBuildingControllersFromSave(
        List<BuildingController> newBuildings
    )
    {
        Buildings = newBuildings;
    }

    public List<CharacterController> GetSquadListForSave()
    {
        return SquadCharacterControllers;
    }

    public void SetSquadControllersFromSave(List<CharacterController> newSquad)
    {
        SquadCharacterControllers = newSquad;
    }

    public void SetCurrentQuest(Quest newQuest)
    {
        CurrentQuest = newQuest;
    }

    public Quest GetCurrentQuest()
    {
        return CurrentQuest;
    }
}
