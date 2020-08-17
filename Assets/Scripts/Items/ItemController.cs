using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{


    private Rigidbody rb;
    private Transform ItemTransform;
    private Collider ItemCollider;
    private ItemData Item = new ItemData();
    private ItemDataManager IDM = new ItemDataManager();
    public string ItemSaveFileFolder = "Assets/ItemJson";
    public string ItemSaveFile = "Torch.json";

    private bool isPickedUp = false;
    private float CooldownTimer = 0f;


    private GameObject HoldingCharacter;

    //When character comes online, set vars needed for init
    private void Awake()
    {

        // TODO have this copy the configs per unqiue torch item etc

        //cam = Camera.main;
        rb = gameObject.GetComponent<Rigidbody>();
        ItemTransform = gameObject.GetComponent<Transform>();
        ItemCollider = gameObject.GetComponent<Collider>();

        Debug.Log("Starting an item");
        IDM.Init(ItemSaveFileFolder, ItemSaveFile);
        Load();
        //Character = CDM.Load();
        //TODO load on init

    }

    private void FixedUpdate()
    {
        //happens on physics updates

        //saving etc
        if (Input.GetKeyDown("i"))
        {
            Load();
        }
        if (Input.GetKeyDown("o"))
        {
            Save();
        }


    }

    private void Load()
    {
        Item = IDM.Load();

        Debug.Log("loading item" + Item.Name);
        /*
        // if picked up, go to holder, if not, go to last postion
        if (Item.holderUUID != "")
        {
            var characterControllersList = FindObjectsOfType<CharacterController>();
            string id;
            foreach (CharacterController controller in characterControllersList)
            {
                id = controller.GetUUID();
                if (id == Item.holderUUID)
                {

                    isPickedUp = true;
                    rb.useGravity = false;
                    rb.isKinematic = true;

                    rb.constraints = RigidbodyConstraints.None;

                    //TODO move relivant to character
                    Physics.IgnoreCollision(controller.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

                    //ItemTransform.parent = collision.gameObject.GetComponent<CharacterController> ().GetCharacterTransform ();
                    HoldingCharacter = controller.gameObject;


                    //based on held location, change location
                    if (Item.heldLocation == "Hand")
                    {
                        ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetHandTransform();
                    }
                    else if (Item.heldLocation == "Back")
                    {
                        ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetBackTransform();
                    }
                    else if (Item.heldLocation == "Belt")
                    {
                        ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetBeltTransform();
                    }

                    ItemTransform.localPosition = new Vector3(0, 0, 0);

                    break;
                }
            }
        }
        else
        {
            ItemTransform.position = new Vector3(Item.x_pos, Item.y_pos, Item.z_pos);
        }
        */

    }
    private void Save()
    {
        if (!isPickedUp)
        {
            Item.x_pos = ItemTransform.position.x;
            Item.y_pos = ItemTransform.position.y;
            Item.z_pos = ItemTransform.position.z;
        }
        IDM.Save(Item);
    }


    //TODO make update on frameupdate
    private void Update()
    {
        // ask parent if dropped
        if (HoldingCharacter != null)
        {
            string Status = HoldingCharacter.GetComponent<CharacterController>().GetItemStatus();

            if (Status == "Dropping" && Item.heldLocation == "Hand")
            {
                Debug.Log("parent id dropping me");
                EnableCollsion();


                Transform HoldingCharacterTransform = HoldingCharacter.GetComponent<Transform>();

                Vector3 dropPoint = HoldingCharacterTransform.position + (HoldingCharacterTransform.forward * 1.5f) + (HoldingCharacterTransform.up * 1.5f);

                //ItemTransform.localPosition = new Vector3 (Random.Range (-0.25f, 0.25f), Random.Range (0f, 0.5f), Random.Range (-0.25f, 0.25f));

                //ItemTransform.localPosition = ItemTransform.forward* new Vector3 (Random.Range (-0.25f, 0.25f), Random.Range (0f, 0.5f), Random.Range (-0.25f, 0.25f));

                //ItemTransform.position = ItemTransform.position + (ItemTransform.forward * 1.5f);

                ItemTransform.position = dropPoint;

                ItemTransform.parent = null;

                Physics.IgnoreCollision(HoldingCharacter.GetComponent<Collider>(), GetComponent<Collider>(), false);

                Item.heldLocation = null;
                Item.holderUUID = null;

                HoldingCharacter = null;
                isPickedUp = false;
                rb.useGravity = true;
                rb.isKinematic = false;
                Status = "";
            }
            else if (Status == "SwapHandBack")
            {
                if (Item.heldLocation == "Hand")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetBackTransform();
                    Item.heldLocation = "Back";
                }
                else if (Item.heldLocation == "Back")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetHandTransform();
                    Item.heldLocation = "Hand";
                }

                Status = "";

            }
            else if (Status == "SwapHandBelt")
            {

                if (Item.heldLocation == "Hand")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetBeltTransform();
                    Item.heldLocation = "Belt";
                }
                else if (Item.heldLocation == "Belt")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetHandTransform();
                    Item.heldLocation = "Hand";
                }

                Status = "";

            }
            else
            {
                //Keep the item above the player / at location
                // TODO have it check which slot its held in

                if (Item.heldLocation == "Hand")
                {
                    ItemTransform.localPosition = new Vector3(Item.HoldingOffsetX, Item.HoldingOffsetY, Item.HoldingOffsetZ);
                }
                else
                {
                    ItemTransform.localPosition = new Vector3(0, 0, 0);
                }

                //ItemTransform.localPosition = new Vector3(0, 1, 0);
                ItemTransform.localRotation = Quaternion.identity;

                //cooldown timer if needed
                if (CooldownTimer > 0)
                {
                    CooldownTimer -= Time.deltaTime;
                }

                if (Item.heldLocation == "Hand")
                {


                    float action = HoldingCharacter.GetComponent<CharacterController>().GetItemActionFloat();
                    if (action > 0.0f)
                    {
                        // enable collision
                        EnableCollsion();
                    }
                    else
                    {
                        //disable collision
                        DisableCollsion();
                    }
                    //mouse click inputs
                    if (action == 1.0f)
                    {
                        DoPrimaryAction();
                        HoldingCharacter.GetComponent<CharacterController>().ResetItemActionFloat();

                    }
                    if (action == 2.0f)
                    {
                        DoSecondaryAction();
                        HoldingCharacter.GetComponent<CharacterController>().ResetItemActionFloat();

                    }
                    if (action == 3.0f)
                    {
                        //TODO use action
                        DoSecondaryAction();
                        HoldingCharacter.GetComponent<CharacterController>().ResetItemActionFloat();

                    }

                }
                //TODO else if heldlocation is belt, then do primary action on keypress etc


                //TODO if has status effect if held/on belt, put here


            }
        }
    }


    //have these do the item efect and trigger the holders animation 

    private void DoPrimaryAction()
    {
        Debug.Log("Pressed primary button.");

        string ItemClass = Item.PrimaryActionClass;
        if (ItemClass == "SUMMON")
        {
            Debug.Log("SUMMONIGNGGG");
            SummonPrefab Summoner = this.gameObject.GetComponent<SummonPrefab>();
            Debug.Log(Summoner);
            Summoner.Summon();
        }//TODO else if basic etc
        else if (ItemClass == "POTION")
        {
            Debug.Log("potion cooldown:" + CooldownTimer);
            if (CooldownTimer <= 0)
            {
                Potion DrinkPotion = this.gameObject.GetComponent<Potion>();
                DrinkPotion.SetCharacter(HoldingCharacter.GetComponent<CharacterController>());
                DrinkPotion.Drink(Item.Damage);
                CooldownTimer += Item.Cooldown;
            }
        }
        else
        {
            // TODO move to attac secion maybe?
            Debug.Log("doing basic attack 01");
            float animationDuration = 1.0f;
            AnimateHoldingCharacter("m_slash1", animationDuration);
        }

    }

    private void DoSecondaryAction()
    {
        // TODO base this on item json
        Debug.Log("Pressed Seconary button. TODO THIS");
        Debug.Log("doing basic attack 02");
        float animationDuration = 1.0f;
        AnimateHoldingCharacter("m_slash2", animationDuration);

    }

    private void AnimateHoldingCharacter(string animation, float overrideDuration)
    {
        CharacterController controller = HoldingCharacter.GetComponent<CharacterController>();
        controller.SetAnimation(animation, overrideDuration);
    }

    private void SetTargetOnImpact(GameObject WhosTargetToSet, GameObject TargetToSet)
    {
        CharacterController controller = WhosTargetToSet.GetComponent<CharacterController>();
        controller.SetTarget(TargetToSet);
        controller.SetFighting(true);

    }

    private void EnableCollsion()
    {
        ItemCollider.enabled = true;
    }

    private void DisableCollsion()
    {
        ItemCollider.enabled = false;
    }

    //TODO check if parent is asking for use function

    // ITEM CLASS
    //Basic (act like sword and do attack)

    //Summon (look for summonprefab and set allow to true once)


    void OnCollisionEnter(Collision collision)
    {
        if (Item.IsPickup && !isPickedUp)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                CharacterController CollidingCharacter = collision.gameObject.GetComponent<CharacterController>();
                if (CollidingCharacter != null)
                {
                    //TODO remove this player only check
                    //if (CollidingCharacter.GetIsPlayer())
                    //{
                    if (!CollidingCharacter.GetHasItemInHand())
                    {

                        Debug.DrawRay(contact.point, contact.normal, Color.white);

                        Debug.Log("I was hit by " + collision.gameObject.GetComponent<CharacterController>().GetCharacter().Name);
                        isPickedUp = true;
                        rb.useGravity = false;
                        rb.isKinematic = true;

                        rb.constraints = RigidbodyConstraints.None;

                        //TODO move relivant to character
                        Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

                        //ItemTransform.parent = collision.gameObject.GetComponent<CharacterController> ().GetCharacterTransform ();
                        HoldingCharacter = collision.gameObject;
                        ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetHandTransform();

                        ItemTransform.localPosition = new Vector3(0, 0, 0);

                        Item.heldLocation = "Hand";
                        Item.holderUUID = HoldingCharacter.GetComponent<CharacterController>().GetUUID();
                        break;
                    }
                    //}
                }
            }
            if (!isPickedUp)
            {
                Debug.Log("Collision without picked up");

                ItemTransform.position += new Vector3(0.0f, 0.2f, 0.0f);
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.AddTorque(transform.up * 0.2f);

            }


        }
        else if (isPickedUp)
        {
            if (Item.heldLocation == "Hand")// if item in hand, do dmg on impact
            {
                CharacterController CollidingCharacter = collision.gameObject.GetComponent<CharacterController>();
                if (CollidingCharacter != null)
                {
                    //TODO remove this add set this to action part
                    CollidingCharacter.AddValueToHealth(-1 * Item.Damage);
                    // Set to target eachother
                    SetTargetOnImpact(HoldingCharacter, collision.gameObject);
                    // if can fight
                    if (CollidingCharacter.GetCanFight())
                    {
                        SetTargetOnImpact(collision.gameObject, HoldingCharacter);

                    }
                }
            }
        }
    }

    //your code

    //onclick actions etc

}