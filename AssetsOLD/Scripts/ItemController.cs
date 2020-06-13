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
        if (ItemTransform.parent != null) {
            if (ItemTransform.parent.gameObject.GetComponent<CharacterController> ().GetIsDroppingItem ()) {
                Debug.Log ("parent id dropping me");
                Physics.IgnoreCollision (ItemTransform.parent.gameObject.GetComponent<Collider> (), GetComponent<Collider> (), false);

                ItemTransform.parent = null;
                isPickedUp = false;
                rb.useGravity = true;
                Toss();// toss the item
            } else {
                ItemTransform.localPosition = new Vector3 (0, 1, 0);

            }
        }

        //TODO check if parent is asking for use function

    }

    private void Toss () {
        float maxForce = 5f;
        Vector3 position = new Vector3 (Random.Range (-2f, maxForce), Random.Range(2f, maxForce), Random.Range(-2f, maxForce));
        rb.AddForce (position);

    }

    void OnCollisionEnter (Collision collision) {
        if (Item.IsPickup && !isPickedUp) {
            foreach (ContactPoint contact in collision.contacts) {
                if (collision.gameObject.GetComponent<CharacterController> ().GetIsPlayer ()) {
                    Debug.DrawRay (contact.point, contact.normal, Color.white);

                    Debug.Log ("I was hit by " + collision.gameObject.GetComponent<CharacterController> ().GetCharacter ().Name);
                    isPickedUp = true;
                    rb.useGravity = false;
                    //TODO move relivant to character
                    Physics.IgnoreCollision (collision.gameObject.GetComponent<Collider> (), GetComponent<Collider> ());

                    ItemTransform.parent = collision.gameObject.GetComponent<CharacterController> ().GetCharacterTransform ();

                    ItemTransform.localPosition = new Vector3 (0, 1, 0);
                    //holder = collision.gameObject;
                    //collision.gameObject.GetComponent<CharacterController> ().Character.name;
                    //if (collision.gameObject.GetComponent<CharacterController> ().GetIsInteracting ()){
                    //    Debug.Log("I was picked up by player");
                    //}
                }
            }
        } else if (isPickedUp) {
            Debug.Log ("Collision after picked up");
        }
    }

    //your code

    //onclick actions etc

}