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
        Item = IDM.Load();
        //Character = CDM.Load();
        //TODO load on init

    }

    private void FixedUpdate () {
        //happens on physics updates
    }
    //TODO make update on frameupdate
    private void Update () {
        // ask parent if dropped
        if (ItemTransform.parent != null){
        if (ItemTransform.parent.gameObject.GetComponent<CharacterController>().GetIsDroppingItem()){
            Debug.Log("parent id dropping me");
            ItemTransform.parent = null;
            isPickedUp = false;
        }
        }

        //TODO check if parent is asking for use function

    }

    void OnCollisionEnter (Collision collision) {
        if (Item.IsPickup && !isPickedUp) {
            foreach (ContactPoint contact in collision.contacts) {
                if (collision.gameObject.GetComponent<CharacterController> ().GetIsPlayer ()) {
                    Debug.DrawRay (contact.point, contact.normal, Color.white);

                    Debug.Log ("I was hit by " + collision.gameObject.GetComponent<CharacterController> ().GetCharacter ().Name);
                    isPickedUp = true;
                    ItemTransform.parent = collision.gameObject.GetComponent<CharacterController>().GetCharacterTransform();
                    //holder = collision.gameObject;
                    //collision.gameObject.GetComponent<CharacterController> ().Character.name;
                    //if (collision.gameObject.GetComponent<CharacterController> ().GetIsInteracting ()){
                    //    Debug.Log("I was picked up by player");
                    //}
                }
            }
        }
    }

    //your code

    //onclick actions etc

}