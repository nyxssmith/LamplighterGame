// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class SummonNPCWithItems : MonoBehaviour
{

    public GameObject NPC;
    public GameObject HandItem;
    public GameObject BackItem;
    public GameObject BeltItem;
    public string SaveFileToLoad;

    public string CharacterSaveFileFolder = "Assets/CharacterJson";



    //TODO make have a bool for "on awake" vs "when player in range"
    private Vector3 SummonPositon;

    void Start()
    {
        SummonPositon = this.transform.position;

        Summon();
        Destroy(this.gameObject);

    }

    public void Update()
    {
    }

    public void Summon()
    {
        // make npc
        GameObject SpawnedNPC = Instantiate(NPC, SummonPositon, Quaternion.identity);
        CharacterController SpawnedNPCController = SpawnedNPC.GetComponent<CharacterController>();

        //SpawnedNPC.SendMessage("DoInit",(CharacterSaveFileFolder,SaveFileToLoad));
        //SpawnedNPCController.Load(CharacterSaveFileFolder,SaveFileToLoad);
        SpawnedNPCController.DoInit(CharacterSaveFileFolder,SaveFileToLoad);


        // set customization options based on who they are

        DoCharacterCustomize(SpawnedNPCController);

        Transform HandTransform = SpawnedNPCController.GetHandTransform();
        Transform BackTransform = SpawnedNPCController.GetBackTransform();
        Transform BeltTransform = SpawnedNPCController.GetBeltTransform();

        // if each item exsits, summon it and give to npc
        if (HandItem != null)
        {
            GameObject SpawnedHandItem = Instantiate(HandItem, SummonPositon, Quaternion.identity);
            SpawnedHandItem.GetComponent<ItemController>().SetHeldLocation("Hand", SpawnedNPCController);
        }
        if (BackItem != null)
        {
            GameObject SpawnedBackItem = Instantiate(BackItem, SummonPositon, Quaternion.identity);
            SpawnedBackItem.GetComponent<ItemController>().SetHeldLocation("Back", SpawnedNPCController);
        }
        if (BeltItem != null)
        {
            GameObject SpawnedBeltItem = Instantiate(BeltItem, SummonPositon, Quaternion.identity);
            SpawnedBeltItem.GetComponent<ItemController>().SetHeldLocation("Belt", SpawnedNPCController);
        }

    }

    private void DoCharacterCustomize(CharacterController toCustomize){

        // set name from random names

        // get the default action to determine outfits

        /*
        %name% outfits
        torso option 1-7
        belt/backpack option 0-6
        head option 1-15
        face option 1-3
        hands/arms option 1-6
        shoulder option 0-6
        legs option 1-6


        farmer/lumberjack outfits
        torso option 1-2
        belt/backpack option 0-1
        head option 1-5
        face option 1-3
        hands/arms option 1-2
        shoulder option 0
        legs option 1-2


        merchant/shopkeeper/mayor outfits
        torso option 1-7
        belt/backpack option 0-1
        head option 1-5
        face option 1-3
        hands/arms option 1-2
        shoulder option 0
        legs option 1-2


        blacksmith options

        potion/magic options

        brandit outfits

        guard outfits

        other outfits = farmer + merchant options



        */

        // set outfits


        // set stats

        
        // all pick between 75-125 health
        // all pick between 10-20 mana with 5% chance of 100-200 mana with all mages having between 100-250 mana
        // all use json defined stamina



    }

    //TODO summon random follower with list of options for items to pick from
    // and random names etc
}