using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public float CameraMoveSpeed = 120.0f;

    public GameObject CameraFollowObj;

    Vector3 FollowPOS;

    public float clampAngle = 80.0f;

    public float inputSensitivity = 150.0f;

    public GameObject CameraObj;

    public GameObject PlayerObj;

    public GameObject BuildToolPrefab;

    public GameObject CustomizeMenuPrefab;

    public float camDistanceXToPlayer;

    public float camDistanceYToPlayer;

    public float camDistanceZToPlayer;

    public float mouseX;

    public float mouseY;

    public float finalInputX;

    public float finalInputZ;

    public float smoothX;

    public float smoothY;

    private float rotY = 0.0f;

    private float rotX = 0.0f;

    private CharacterController Player = null;

    private CharacterData Character = null;

    private CharacterData TargetCharacter = null;

    // ui stuff
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

    [SerializeField]
    GameObject SquadList; //name of target

    [SerializeField]
    GameObject DialogBox;

    [SerializeField]
    GameObject InfoBox;

    [SerializeField]
    GameObject TownUI;

    private bool hasTarget = false;

    private bool inDialog = false;

    private bool inBuildMode = false;

    private bool inCustomizeMode = false;

    private string BaseSquadListText = "[Squad List]";

    private string SquadListText = "[Squad List]";

    private List<CharacterController> SquadCharacterControllers = null;

    private GameObject PlayersHeldItem;

    private GameObject BuildToolObject;

    private GameObject CustomizeMenuObject;

    private BuildTool BuildToolController;

    private string InfoText = ""; // text for info box

    private string DialogText = "";

    private bool needsUIUpdate = true;

    private int buildToolIndex = 0;

    private int buildToolLength;

    private float ConstUpdateTimer = 0f;

    private float PrevTimescale = 1.0f;

    private bool isPaused = false;

    private bool hasDoneInit = false;

    // Use this for initialization
    void Start()
    {
        CameraFollowObj = null;

        DoInit();
    }

    private void DoInit()
    {
        CameraFollowObj = FindPlayerToFollow();

        if (CameraFollowObj == null)
        {
            return;
        }

        UnityEngine.Debug.Log (CameraFollowObj);

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // TODO call on switch too
        GetPlayer();
        GetPlayerCharacter();
        SquadListText = GenerateSquadList();
        hasDoneInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasDoneInit)
        {
            DoInit();
        }
        else
        {
            CheckIfCurrentCharacterDied();

            if (ConstUpdateTimer > 0)
            {
                ConstUpdateTimer -= Time.deltaTime;
                needsUIUpdate = true;
            }

            // We setup the rotation of the sticks here
            //float inputX = Input.GetAxis("RightStickHorizontal");
            //float inputZ = Input.GetAxis("RightStickVertical");
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            finalInputX = mouseX;
            finalInputZ = mouseY;

            float timeFloat = Time.unscaledDeltaTime;
            if (isPaused)
            {
                // if is paused also pause camera
                timeFloat = Time.deltaTime;
            }
            rotY += finalInputX * inputSensitivity * timeFloat;
            rotX += finalInputZ * inputSensitivity * timeFloat;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            transform.rotation = localRotation;

            // get stuff from current player and do ui
            if (Player == null)
            {
                GetPlayer();
            }

            if (GetIfPlayerRequestedUIUpdate() || needsUIUpdate)
            {
                //UnityEngine.Debug.Log("player requested ui update");
                GetPlayer();
                GetPlayerCharacter();
                SquadListText = GenerateSquadList();
                Player.SetNeedsUIUpdate(false);
                inDialog = Player.GetIsInDialog();

                // when needs update update info text
                UpdateInfoUIForItem();
                needsUIUpdate = false;

                // update town info if realivant
            }

            GetPlayersTargetCharacter();
            DoUI();

            if (Input.GetKeyDown("h"))
            {
                UnityEngine.Debug.Log("setting player to ask for ui update");
                Player.SetNeedsUIUpdate(true);
            }

            // To time warp
            // toggle half, full, etc time speed
            if (Input.GetKeyDown("]"))
            {
                if (Time.timeScale >= 10.0f)
                {
                    Time.timeScale = 0.5f;
                }
                else
                {
                    Time.timeScale += 0.5f;
                }
                Player
                    .MakeSpeechBubble("Timescale set to " +
                    Time.timeScale.ToString());
                //RotationDegree += 0.5f;
            }

            // if not in dialog mode
            if (inDialog)
            {
                // dont get keys for squad, since dialogmanager is handling it
                //if (Input.GetKeyDown("1"))
                //{
                //    Player.MakeSpeechBubble("pushed a key");
                //}
            }
            else if (inCustomizeMode)
            {
            }
            else if (inBuildMode)
            {
                // dont swap in build mode
                // update buildmode text here
                //InfoText = "build mode";
                UpdateInfoUIForBuildMode();
                buildToolIndex = BuildToolController.Index;
            }
            else
            {
                // get numbers 1-9 and swap to squad member
                if (Input.GetKeyDown("1"))
                {
                    SafeSwithctoTarget(0);
                }
                if (Input.GetKeyDown("2"))
                {
                    SafeSwithctoTarget(1);
                }
                if (Input.GetKeyDown("3"))
                {
                    SafeSwithctoTarget(2);
                }
                if (Input.GetKeyDown("4"))
                {
                    SafeSwithctoTarget(3);
                }
                if (Input.GetKeyDown("5"))
                {
                    SafeSwithctoTarget(4);
                }
                if (Input.GetKeyDown("6"))
                {
                    SafeSwithctoTarget(5);
                }
                if (Input.GetKeyDown("7"))
                {
                    SafeSwithctoTarget(6);
                }
                if (Input.GetKeyDown("8"))
                {
                    SafeSwithctoTarget(7);
                }
                if (Input.GetKeyDown("9"))
                {
                    SafeSwithctoTarget(8);
                }
            }

            if (Input.GetKeyDown("b"))
            {
                UnityEngine.Debug.Log("switching to build mode");
                Player.SetNeedsUIUpdate(true);

                if (!inBuildMode)
                {
                    EnterBuildMode();
                }
                else
                {
                    ExitBuildMode();
                }
            }

            // do customize menu
            if (Input.GetKeyDown("c"))
            {
                UnityEngine.Debug.Log("switching to customizer mode");
                Player.SetNeedsUIUpdate(true);

                if (!inCustomizeMode)
                {
                    EnterCustomizerMode();
                }
                else
                {
                    ExitCustomizerMode();
                }

                UnityEngine.Debug.Log("character customize button set");
                Player.PopulateListsOfCustomizeOptions();
            }
        }
    }

    private void EnterCustomizerMode()
    {
        inCustomizeMode = true;

        // pause time for customizer
        PrevTimescale = Time.timeScale;
        Time.timeScale = 0.0f;

        // update current character data from customization options
        Player.SaveCurrentCustomizedValuesToCharacterData();

        // instanciate a customize menu and save reference to it
        CustomizeMenuObject = Instantiate(CustomizeMenuPrefab);
        CustomizeMenuObject.GetComponent<CustomizeMenu>().SetCharacter(Player);
        CustomizeMenuObject
            .GetComponent<CustomizeMenu>()
            .PopulateCustomizationOptionsMatrix();

        CustomizeMenuObject
            .GetComponent<CustomizeMenu>()
            .GenerateCurrentStatusText();
    }

    private void ExitCustomizerMode()
    {
        inCustomizeMode = false;
        Time.timeScale = PrevTimescale;

        // destroy customize menu object
        Destroy (CustomizeMenuObject);

        // just to be sure
        Player.SaveCurrentCustomizedValuesToCharacterData();
    }

    public GameObject FindPlayerToFollow()
    {
        // get all characters who have same squad leader
        var characterControllersList = FindObjectsOfType<CharacterController>();
        UnityEngine.Debug.Log("finding player");
        foreach (CharacterController controller in characterControllersList)
        {
            UnityEngine.Debug.Log (controller);
            UnityEngine.Debug.Log(controller.GetIsPlayer());
            if (controller.GetIsPlayer())
            {
                UnityEngine.Debug.Log("found player");
                return controller.GetCameraTarget();
            }
        }
        return null; //FindPlayerToFollow();
    }

    void EnterBuildMode()
    {
        // to swap to build mode
        //get current item in hand and disable it
        ItemController heldItemController = Player.GetHeldItemController();
        if (heldItemController != null)
        {
            PlayersHeldItem = heldItemController.gameObject;

            PlayersHeldItem.SetActive(false);
        }

        Transform hand = Player.GetHandTransform();
        BuildToolObject =
            Instantiate(BuildToolPrefab, hand.position, Quaternion.identity);

        //make the target beacon a child of its taret
        BuildToolObject.gameObject.GetComponent<Transform>().parent = hand;

        BuildToolObject
            .gameObject
            .GetComponent<BuildTool>()
            .SetCharacter(Player);

        BuildToolObject
            .gameObject
            .GetComponent<ItemController>()
            .GetPickedUpBy(Player);

        BuildToolController =
            BuildToolObject.gameObject.GetComponent<BuildTool>();

        BuildToolController.Index = buildToolIndex;

        buildToolLength = BuildToolController.GetLength();

        //BuildToolController.SetIndex( buildToolIndex);
        // give the build tool as hand item
        inBuildMode = true;

        //Player.Setinbu
        Player.SetInBuildMode(true);
    }

    public void ExitBuildMode()
    {
        // to swap out
        // enable hand item
        // delete build tool object
        if (PlayersHeldItem != null)
        {
            PlayersHeldItem.SetActive(true);
        }
        BuildToolObject.gameObject.GetComponent<BuildTool>().DeTarget();
        BuildToolObject.gameObject.GetComponent<BuildTool>().HideGhostImage();

        Destroy (BuildToolObject);

        inBuildMode = false;
        Player.SetInBuildMode(false);
        needsUIUpdate = true; // update ui after exit build mode
    }

    void LateUpdate()
    {
        if (hasDoneInit)
        {
            CameraUpdater();
        }
    }

    void CameraUpdater()
    {
        // set the target object to follow
        Transform target = CameraFollowObj.transform;

        //move towards the game object that is the target
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position =
            Vector3.MoveTowards(transform.position, target.position, step);
    }

    private void SafeSwithctoTarget(int SquadMemberIndexFromZero)
    {
        if (SquadCharacterControllers.Count >= SquadMemberIndexFromZero + 1)
        {
            SwitchToTarget(SquadCharacterControllers[SquadMemberIndexFromZero]);
        }
        else
        {
            UnityEngine.Debug.Log("index is out of range");
        }
        needsUIUpdate = true;
    }

    private void SwitchToTarget(
        CharacterController SwitchTargetCharacterController
    )
    {
        if (Player.SwapIntoTarget(SwitchTargetCharacterController))
        {
            // set that new character is player to its loadcontroller
            //Player.SetLoadedControllerIsPlayer(false);
            //SwitchTargetCharacterController.SetLoadedControllerIsPlayer(true);
            // set the new player for the isloadedcontroller
            //clear ref to this camera
            UpdateCharactersReferenceToCamera(null);

            CameraFollowObj = SwitchTargetCharacterController.GetCameraTarget();

            GetPlayer();
            GetPlayerCharacter();
            SquadListText = GenerateSquadList();
        }
    }

    private void GetPlayer()
    {
        Player =
            CameraFollowObj
                .gameObject
                .GetComponentInParent<CharacterController>();

        // tell new player about its camera
        UpdateCharactersReferenceToCamera(this.gameObject);
    }

    private void GetPlayerCharacter()
    {
        Character = Player.GetCharacter();
    }

    private void GetPlayersTargetCharacter()
    {
        hasTarget = Player.GetHasTarget();
        TargetCharacter = Player.GetTargetCharacter();
    }

    private void DoUI()
    {
        DoHealthUI();
        DoStaminaUI();
        DoManaUI();
        DoTargetHealtBarUI();
        DoSquadUI();

        // if in dialog mode, then do dialog
        //if (inDialog)
        //{
        DoDialogUI();

        //}
        DoInfoUI();
    }

    public void DoTownUI(string NewText)
    {
        // TODO fix townui not showing up if swap to player back inside town
        // might be fixed by changing hit collider that exits to cameraFollow if I can get tht to to work
        TownUI.GetComponent<Text>().text = NewText;
    }

    private void UpdateTownUIFromPlayer()
    {
        TownController Town = Player.GetTown();
        if (Town != null)
        {
            DoTownUI(Town.GenerateTownUIString());
        }
    }

    private void DoHealthUI()
    {
        HealthUI
            .GetComponent<FillUI>()
            .SetTo(Character.CurrentHealth / Character.MaxHealth);
    }

    private void DoStaminaUI()
    {
        StaminaUI
            .GetComponent<FillUI>()
            .SetTo(Character.CurrentStamina / Character.MaxStamina);
    }

    private void DoManaUI()
    {
        ManaUI
            .GetComponent<FillUI>()
            .SetTo(Character.CurrentMana / Character.MaxMana);
    }

    private void DoTargetHealtBarUI()
    {
        if (hasTarget)
        //check if freindly, if so show only name, else show health bar
        {
            string HealthBarText = TargetCharacter.Name;
            if (!TargetCharacter.IsFollower)
            {
                //TargetUI.GetComponent<FillUI>().SetTo(TargetCharacter.CurrentHealth);
                //float targetsHealth = CombatTarget.GetComponent<CharacterController>().GetCurrentHealth();
                TargetUI
                    .GetComponent<FillUI>()
                    .SetTo(TargetCharacter.CurrentHealth /
                    TargetCharacter.MaxHealth);

                HealthBarText =
                    HealthBarText +
                    "  (" +
                    TargetCharacter.CurrentHealth.ToString() +
                    "/" +
                    TargetCharacter.MaxHealth.ToString() +
                    ")";
            }

            TargetName.GetComponent<Text>().text = HealthBarText;
        } // if no target, hide UI
        else
        {
            TargetUI.GetComponent<FillUI>().SetTo(0.0f);
            TargetName.GetComponent<Text>().text = "";
        }
    }

    private void DoSquadUI()
    {
        SquadList.GetComponent<Text>().text = SquadListText;
    }

    private void DoInfoUI()
    {
        InfoBox.GetComponent<Text>().text = InfoText;
    }

    private void UpdateInfoUIForItem()
    {
        ItemController heldItemController = Player.GetHeldItemController();
        if (heldItemController != null)
        {
            //Player.MakeSpeechBubble("i am holding"+heldItemController.ToString());
            InfoText = heldItemController.GetSummaryString();
        }
        else
        {
            // if not holding anything then show nothing
            InfoText = "";
        }
    }

    private void UpdateInfoUIForBuildMode()
    {
        string buildString = "[Build Mode]\n";

        buildString =
            buildString +
            "Build Option: " +
            (buildToolIndex + 1).ToString() +
            "/" +
            BuildToolController.GetLength() +
            "\n";

        buildString =
            buildString +
            "Push [TAB] to cycle through buildable objects and delete tool\nUse [I] and [K] to change build distance\nUse [U] and [O] to change tilt\nUse [J] and [L] to change rotation\nUse [N] and [M] to change the build height postion\n";

        if (BuildToolController.Index == 0)
        {
            buildString = buildString + "[Mode: Deconstruction]\n";
            buildString =
                buildString +
                "The closest building that can be deleted will be selected automatically\n";
        }
        else
        {
            buildString = buildString + "[Mode: Construction]\n";

            if (!BuildToolController.GetCanPlace())
            {
                // if cant place, get debug reason
                buildString =
                    buildString + BuildToolController.GetReasonCantPlace();
            }

            // add resoruce costs
            buildString =
                buildString + BuildToolController.GetResourcesCostString();
        }

        InfoText = buildString;
    }

    private void DoDialogUI()
    {
        // const set text to dialogText
        DialogText = Player.GetDialogText(); // get dialog text from player that is set from dialog manager

        DialogBox.GetComponent<Text>().text = DialogText;
    }

    public void SetDialogText(string newText)
    {
        DialogText = newText;
    }

    private string GenerateSquadList()
    {
        // clear squadlist
        SquadCharacterControllers = new List<CharacterController>();

        string ListString = "\n";
        int counter = 1;
        string myId = Player.GetUUID();

        // get all characters who have same squad leader
        var characterControllersList = FindObjectsOfType<CharacterController>();
        string id;
        foreach (CharacterController controller in characterControllersList)
        {
            id = controller.GetUUID();
            if (IsCharacterInMySquad(controller))
            {
                ListString =
                    ListString +
                    AddCharatcerNameToList(controller, counter, (id == myId));
                SquadCharacterControllers.Add (controller);
                counter = counter + 1;
            }
        }

        // set the squad list
        Player.SetSquadListAndUpdateOthers (SquadCharacterControllers);

        //Debug.Log("list of new squad" + SquadCharacterControllers.Count);
        return BaseSquadListText + ListString;
    }

    private bool
    IsCharacterInMySquad(CharacterController targetCharacterController)
    {
        string mySquadLeader = Player.GetSquadLeaderUUID();
        string theirSquadLeader =
            targetCharacterController.GetSquadLeaderUUID();
        if (mySquadLeader == theirSquadLeader && theirSquadLeader != "")
        {
            return true;
        }
        return false;
    }

    private string
    AddCharatcerNameToList(
        CharacterController targetCharacterController,
        int ListCounter,
        bool isMe
    )
    {
        string lineString;

        // only show nums outsied of dialog
        if (inDialog)
        {
            lineString = targetCharacterController.GetCharacter().Name;
        }
        else
        {
            lineString =
                ListCounter.ToString() +
                " : " +
                targetCharacterController.GetCharacter().Name;
        }

        //lineString = ListCounter.ToString() + " : " + targetCharacterController.GetCharacter().Name;
        if (isMe)
        {
            lineString = "* <color=green>" + lineString + "</color>";
        }
        if (
            targetCharacterController.GetSquadLeaderUUID() ==
            targetCharacterController.GetUUID()
        )
        {
            lineString = "#" + lineString;
        }
        return lineString + "\n";
    }

    private bool GetIfPlayerRequestedUIUpdate()
    {
        bool status = Player.GetNeedsUIUpdate();
        if (status)
        {
            ConstUpdateTimer += 0.5f;
        }
        return status;
    }

    private void SetFollowObject(GameObject newFollowObj)
    {
        CameraFollowObj = newFollowObj;
    }

    private void CheckIfCurrentCharacterDied()
    {
        if (Player.GetCurrentHealth() <= 0.0f)
        {

            // TODO if squadlist is none, ues townlist then if that is empty do end of game

            string id;
            int count = 0;
            int squadMemberToRemove = -1;
            int squadMemberToSwapTo = -1;
            foreach (CharacterController controller in SquadCharacterControllers
            )
            {
                id = controller.GetUUID();
                if (id != Player.GetUUID())
                {
                    squadMemberToSwapTo = count;
                }
                else
                {
                    squadMemberToRemove = count;
                }

                if (squadMemberToSwapTo > 0 && squadMemberToRemove > 0)
                {
                    break;
                }
                count = count + 1;
            }

            // destroy old player
            Player.StartSelfDestruct();
            Player.SetSquadLeaderUUID("");

            // swap to new one
            SafeSwithctoTarget (squadMemberToSwapTo);
            SquadCharacterControllers.RemoveAt (squadMemberToRemove);

            // reset ui etc
            GetPlayer();
            GetPlayerCharacter();
            SquadListText = GenerateSquadList();
            Player.SetNeedsUIUpdate(true);
        }
    }

    private void UpdateCharactersReferenceToCamera(GameObject newObject)
    {
        Player.SetCameraWithHUD (newObject);
    }

    public bool GetInBuildMode()
    {
        return inBuildMode;
    }

    public void SetIsPaused(bool newState)
    {
        isPaused = newState;
    }

    public bool GetInDialog()
    {
        return inDialog;
    }

    public CharacterController GetCharacterController()
    {
        return Player;
    }
}
