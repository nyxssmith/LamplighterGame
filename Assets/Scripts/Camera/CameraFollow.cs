
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class CameraFollow : MonoBehaviour
{

    public float CameraMoveSpeed = 120.0f;
    public GameObject CameraFollowObj;
    Vector3 FollowPOS;
    public float clampAngle = 80.0f;
    public float inputSensitivity = 150.0f;
    public GameObject CameraObj;
    public GameObject PlayerObj;
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
    [SerializeField] GameObject ManaUI;
    [SerializeField] GameObject HealthUI;
    [SerializeField] GameObject StaminaUI;
    [SerializeField] GameObject TargetUI;//healthbar for target
    [SerializeField] GameObject TargetName;//name of target

    private bool hasTarget = false;

    // Use this for initialization
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // TODO call on switch too
        GetPlayer();
        GetPlayerCharacter();
    }

    // Update is called once per frame
    void Update()
    {

        // We setup the rotation of the sticks here
        float inputX = Input.GetAxis("RightStickHorizontal");
        float inputZ = Input.GetAxis("RightStickVertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        finalInputX = inputX + mouseX;
        finalInputZ = inputZ + mouseY;

        rotY += finalInputX * inputSensitivity * Time.deltaTime;
        rotX += finalInputZ * inputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

        // get stuff from current player and do ui

        if (Player == null)
        {
            GetPlayer();
        }

        GetPlayersTargetCharacter();
        DoUI();



        // TODO move this to camera
        if (Input.GetKeyDown("y"))
        {
            Debug.Log("swapping into target");
            SwitchToTarget();
        }


    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        // set the target object to follow
        Transform target = CameraFollowObj.transform;

        //move towards the game object that is the target
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }


    private void SwitchToTarget()
    {

        CameraFollowObj = Player.GetTargetsCameraTarget();
        Player.SwapIntoTarget();

        GetPlayer();
        GetPlayerCharacter();

    }


    private void GetPlayer()
    {
        Player = CameraFollowObj.gameObject.GetComponentInParent<CharacterController>();
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
    }

    private void DoHealthUI()
    {
        HealthUI.GetComponent<FillUI>().SetTo(Character.CurrentHealth / Character.MaxHealth);

    }
    private void DoStaminaUI()
    {
        StaminaUI.GetComponent<FillUI>().SetTo(Character.CurrentStamina / Character.MaxStamina);
    }

    private void DoManaUI()
    {
        ManaUI.GetComponent<FillUI>().SetTo(Character.CurrentMana / Character.MaxMana);

    }

    private void DoTargetHealtBarUI()
    {
        if (hasTarget)
        //check if freindly, if so show only name, else show health bar
        {
            TargetName.GetComponent<Text>().text = TargetCharacter.Name;
            if (!TargetCharacter.IsFollower)
            {
                //TargetUI.GetComponent<FillUI>().SetTo(TargetCharacter.CurrentHealth);
                //float targetsHealth = CombatTarget.GetComponent<CharacterController>().GetCurrentHealth();
                TargetUI.GetComponent<FillUI>().SetTo(TargetCharacter.CurrentHealth / TargetCharacter.MaxHealth);
            }

        }
        else// if no target, hide UI
        {
            TargetUI.GetComponent<FillUI>().SetTo(0.0f);
            TargetName.GetComponent<Text>().text = "";
        }
    }


    private void DoSquadUI(){

    }


    private void SetFollowObject(GameObject newFollowObj)
    {
        CameraFollowObj = newFollowObj;
    }




}