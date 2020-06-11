using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemController : MonoBehaviour
{

    //Control settings
    //public float mouseSpeed = 3;
    //public bool isPlayer = false;

    //Objects and vars not loaded from save file
    //private Transform player;
    //private Camera cam;
    //private Rigidbody rb;


    //Character save manager
    //public string CharacterSaveFileFolder = "Assets/CharacterJson";
    //public string CharacterSaveFile = "Player1.json";
    //private CharacterDataManager CDM = new CharacterDataManager();

    //private CharacterData Character = new CharacterData();
    private ItemData Item = new ItemData();




    //When character comes online, set vars needed for init
    private void Awake()
    {
        //cam = Camera.main;
        //rb = gameObject.GetComponent<Rigidbody>();
        //player = gameObject.GetComponent<Transform>();

        Debug.Log("Starting an item");
        //CDM.Init(CharacterSaveFileFolder, CharacterSaveFile);
        //Character = CDM.Load();
        //TODO load on init

    }

    private void FixedUpdate()
    {
        //happens on physics updates
    }
    //TODO make update on frameupdate
    private void update(){

    }


     //your code

    //onclick actions etc

}

