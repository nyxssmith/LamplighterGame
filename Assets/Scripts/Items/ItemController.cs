using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    //Control settings
    //public float mouseSpeed = 3;
    //public bool isPlayer = false;

    //Objects and vars not loaded from save file
    //private Transform player;
    //private Camera cam;
    private Rigidbody rb;
    private Transform ItemTransform;

    //Character save manager
    //public string CharacterSaveFileFolder = "Assets/CharacterJson";
    //public string CharacterSaveFile = "Player1.json";
    //private CharacterDataManager CDM = new CharacterDataManager();

    //private CharacterData Character = new CharacterData();
    private ItemData Item = new ItemData ();
    private ItemDataManager IDM = new ItemDataManager ();
    public string ItemSaveFileFolder = "Assets/ItemJson";
    public string ItemSaveFile = "Torch.json";

    private bool isPickedUp = false;
    private float CooldownTimer = 0f;

    private GameObject HoldingCharacter;

    //When character comes online, set vars needed for init
    private void Awake () {
        //cam = Camera.main;
        rb = gameObject.GetComponent<Rigidbody> ();
        ItemTransform = gameObject.GetComponent<Transform> ();

        Debug.Log ("Starting an item");
        IDM.Init (ItemSaveFileFolder, ItemSaveFile);
        Item = IDM.Load ();
        //Character = CDM.Load();
        //TODO load on init

    }

    private void FixedUpdate () {
        //happens on physics updates
    }
    //TODO make update on frameupdate
    private void Update () {
        // ask parent if dropped
        if (HoldingCharacter != null) {
            if (HoldingCharacter.GetComponent<CharacterController> ().GetIsDroppingItem ()) {
                Debug.Log ("parent id dropping me");
                
                ItemTransform.localPosition = new Vector3 (Random.Range (-0.25f, 0.25f), Random.Range (0f, 0.5f), Random.Range (-0.25f, 0.25f));

                Physics.IgnoreCollision (HoldingCharacter.GetComponent<Collider> (), GetComponent<Collider> (), false);

                HoldingCharacter = null;
                isPickedUp = false;
                rb.useGravity = true;
                Toss (); // toss the item
            } else {
                //Keey the item above the player / at location
                // TODO have it check which slot its held in
                ItemTransform.localPosition = new Vector3 (0, 0, 0);

                //cooldown timer if needed
                if (CooldownTimer > 0){
                    CooldownTimer -=Time.deltaTime;
                }


                if (Input.GetMouseButtonDown (0)) { 
                    DoPrimaryAction();
                }
                if (Input.GetMouseButtonDown (1)) { 
                    DoSecondaryAction();
                }

            }
        }
    }

    private void DoPrimaryAction () {
        Debug.Log ("Pressed primary button.");

        string ItemClass = Item.PrimaryActionClass;
        if (ItemClass == "SUMMON"){
            Debug.Log("SUMMONIGNGGG");
            SummonPrefab Summoner = this.gameObject.GetComponent<SummonPrefab>();
            Debug.Log(Summoner);
            Summoner.Summon();
        }//TODO else if basic etc
        else if (ItemClass == "POTION"){
            if (CooldownTimer <= 0){
                Potion DrinkPotion = this.gameObject.GetComponent<Potion>();
                DrinkPotion.SetCharacter(HoldingCharacter.GetComponent<CharacterController> ());
                DrinkPotion.Drink(Item.Damage);
                CooldownTimer+=Item.Cooldown;
            }
        }

    }

    private void DoSecondaryAction () {
        Debug.Log ("Pressed Seconary button. TODO THIS");

    }

    //TODO check if parent is asking for use function

    // ITEM CLASS
    //Basic (act like sword and do attack)

    //Summon (look for summonprefab and set allow to true once)

    private void Toss () {
        float maxForce = 5f;
        Vector3 position = new Vector3 (Random.Range (-2f, maxForce), Random.Range (2f, maxForce), Random.Range (-2f, maxForce));
        rb.AddForce (position);

    }

    void OnCollisionEnter (Collision collision) {
        if (Item.IsPickup && !isPickedUp) {
            foreach (ContactPoint contact in collision.contacts) {
                CharacterController CollidingCharacter = collision.gameObject.GetComponent<CharacterController> ();
                if (CollidingCharacter != null) {
                    if (CollidingCharacter.GetIsPlayer ()) {
                        Debug.DrawRay (contact.point, contact.normal, Color.white);

                        Debug.Log ("I was hit by " + collision.gameObject.GetComponent<CharacterController> ().GetCharacter ().Name);
                        isPickedUp = true;
                        rb.useGravity = false;
                        //TODO move relivant to character
                        Physics.IgnoreCollision (collision.gameObject.GetComponent<Collider> (), GetComponent<Collider> ());

                        //ItemTransform.parent = collision.gameObject.GetComponent<CharacterController> ().GetCharacterTransform ();
                        HoldingCharacter = collision.gameObject;
                        ItemTransform.parent = HoldingCharacter.GetComponent<CharacterController> ().GetHandTransform ();

                        ItemTransform.localPosition = new Vector3 (0, 0, 0);
                        //holder = collision.gameObject;
                        //collision.gameObject.GetComponent<CharacterController> ().Character.name;
                        //if (collision.gameObject.GetComponent<CharacterController> ().GetIsInteracting ()){
                        //    Debug.Log("I was picked up by player");
                        //}
                    }
                }
            }
        } else if (isPickedUp) {
            Debug.Log ("Collision after picked up");
        }
    }

    //your code

    //onclick actions etc

}