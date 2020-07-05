using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{


    private Rigidbody rb;
    private Transform ItemTransform;
    private ItemData Item = new ItemData();
    private ItemDataManager IDM = new ItemDataManager();
    public string ItemSaveFileFolder = "Assets/ItemJson";
    public string ItemSaveFile = "Torch.json";

    private bool isPickedUp = false;
    private float CooldownTimer = 0f;

    private string heldLocation = "";

    private GameObject HoldingCharacter;

    //When character comes online, set vars needed for init
    private void Awake()
    {
        //cam = Camera.main;
        rb = gameObject.GetComponent<Rigidbody>();
        ItemTransform = gameObject.GetComponent<Transform>();

        Debug.Log("Starting an item");
        IDM.Init(ItemSaveFileFolder, ItemSaveFile);
        Item = IDM.Load();
        //Character = CDM.Load();
        //TODO load on init

    }

    private void FixedUpdate()
    {
        //happens on physics updates
    }
    //TODO make update on frameupdate
    private void Update()
    {
        // ask parent if dropped
        if (HoldingCharacter != null)
        {
            string Status = HoldingCharacter.GetComponent<CharacterController>().GetItemStatus();

            if (Status == "Dropping" && heldLocation == "Hand")
            {
                Debug.Log("parent id dropping me");


                Transform HoldingCharacterTransform = HoldingCharacter.GetComponent<Transform>();

                Vector3 dropPoint = HoldingCharacterTransform.position + (HoldingCharacterTransform.forward * 1.5f) + (HoldingCharacterTransform.up * 1.5f);

                //ItemTransform.localPosition = new Vector3 (Random.Range (-0.25f, 0.25f), Random.Range (0f, 0.5f), Random.Range (-0.25f, 0.25f));

                //ItemTransform.localPosition = ItemTransform.forward* new Vector3 (Random.Range (-0.25f, 0.25f), Random.Range (0f, 0.5f), Random.Range (-0.25f, 0.25f));

                //ItemTransform.position = ItemTransform.position + (ItemTransform.forward * 1.5f);

                ItemTransform.position = dropPoint;

                ItemTransform.parent = null;

                Physics.IgnoreCollision(HoldingCharacter.GetComponent<Collider>(), GetComponent<Collider>(), false);

                HoldingCharacter = null;
                isPickedUp = false;
                rb.useGravity = true;
                rb.isKinematic = false;
                Status = "";
            }
            else if (Status == "SwapHandBack")
            {
                if (heldLocation == "Hand")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetBackTransform();
                    heldLocation = "Back";
                }
                else if (heldLocation == "Back")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetHandTransform();
                    heldLocation = "Hand";
                }

                Status = "";

            }
            else if (Status == "SwapHandBelt")
            {
                if (heldLocation == "Hand")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetBeltTransform();
                    heldLocation = "Belt";
                }
                else if (heldLocation == "Belt")
                {
                    ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetHandTransform();
                    heldLocation = "Hand";
                }

                Status = "";

            }
            else
            {
                //Keep the item above the player / at location
                // TODO have it check which slot its held in
                ItemTransform.localPosition = new Vector3(0, 0, 0);
                ItemTransform.localRotation = Quaternion.identity;

                //cooldown timer if needed
                if (CooldownTimer > 0)
                {
                    CooldownTimer -= Time.deltaTime;
                }

                if (heldLocation == "Hand")
                {

                    //mouse click inputs
                    if (Input.GetMouseButtonDown(0))
                    {
                        DoPrimaryAction();
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        DoSecondaryAction();
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
            if (CooldownTimer <= 0)
            {
                Potion DrinkPotion = this.gameObject.GetComponent<Potion>();
                DrinkPotion.SetCharacter(HoldingCharacter.GetComponent<CharacterController>());
                DrinkPotion.Drink(Item.Damage);
                CooldownTimer += Item.Cooldown;
            }
        }

    }

    private void DoSecondaryAction()
    {
        Debug.Log("Pressed Seconary button. TODO THIS");

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
                    if (CollidingCharacter.GetIsPlayer())
                    {
                        if (!CollidingCharacter.GetHasItemInHand())
                        {

                            Debug.DrawRay(contact.point, contact.normal, Color.white);

                            Debug.Log("I was hit by " + collision.gameObject.GetComponent<CharacterController>().GetCharacter().Name);
                            isPickedUp = true;
                            rb.useGravity = false;
                            rb.isKinematic = true;
                            //TODO move relivant to character
                            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

                            //ItemTransform.parent = collision.gameObject.GetComponent<CharacterController> ().GetCharacterTransform ();
                            HoldingCharacter = collision.gameObject;
                            ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController>().GetHandTransform();

                            ItemTransform.localPosition = new Vector3(0, 0, 0);

                            heldLocation = "Hand";
                            //holder = collision.gameObject;
                            //collision.gameObject.GetComponent<CharacterController> ().Character.name;
                            //if (collision.gameObject.GetComponent<CharacterController> ().GetIsInteracting ()){
                            //    Debug.Log("I was picked up by player");
                            //}
                        }
                    }
                }
            }
        }
        else if (isPickedUp)
        {
            if (heldLocation == "Hand")// if item in hand, do dmg on impact
            {
                Debug.Log("Collision after picked up");
                CharacterController CollidingCharacter = collision.gameObject.GetComponent<CharacterController>();
                if (CollidingCharacter != null)
                {
                    //TODO remove this add set this to action part
                    CollidingCharacter.AddValueToHealth(-1 * Item.Damage);
                }
            }
        }
    }

    //your code

    //onclick actions etc

}