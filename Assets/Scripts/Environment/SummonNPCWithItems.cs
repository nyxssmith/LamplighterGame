// Instantiates 10 copies of Prefab each 2 units apart from each other
using System.Collections;
using System.Collections.Generic;
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
        GameObject SpawnedNPC =
            Instantiate(NPC, SummonPositon, Quaternion.identity);
        CharacterController SpawnedNPCController =
            SpawnedNPC.GetComponent<CharacterController>();

        //SpawnedNPC.SendMessage("DoInit",(CharacterSaveFileFolder,SaveFileToLoad));
        //SpawnedNPCController.Load(CharacterSaveFileFolder,SaveFileToLoad);
        SpawnedNPCController.DoInit (CharacterSaveFileFolder, SaveFileToLoad);

        // set customization options based on who they are
        DoCharacterCustomize (SpawnedNPCController);

        Transform HandTransform = SpawnedNPCController.GetHandTransform();
        Transform BackTransform = SpawnedNPCController.GetBackTransform();
        Transform BeltTransform = SpawnedNPCController.GetBeltTransform();

        // if each item exsits, summon it and give to npc
        if (HandItem != null)
        {
            GameObject SpawnedHandItem =
                Instantiate(HandItem, SummonPositon, Quaternion.identity);
            SpawnedHandItem
                .GetComponent<ItemController>()
                .SetHeldLocation("Hand", SpawnedNPCController);
        }
        if (BackItem != null)
        {
            GameObject SpawnedBackItem =
                Instantiate(BackItem, SummonPositon, Quaternion.identity);
            SpawnedBackItem
                .GetComponent<ItemController>()
                .SetHeldLocation("Back", SpawnedNPCController);
        }
        if (BeltItem != null)
        {
            GameObject SpawnedBeltItem =
                Instantiate(BeltItem, SummonPositon, Quaternion.identity);
            SpawnedBeltItem
                .GetComponent<ItemController>()
                .SetHeldLocation("Belt", SpawnedNPCController);
        }
    }

    private void DoCharacterCustomize(CharacterController toCustomize)
    {
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
        torso option 1-4
        belt/backpack option 4-6
        head option 1-5
        face option 1-3
        hands/arms option 1-2
        shoulder option 0
        legs option 1-2


        blacksmith options
        torso option 1-2
        belt/backpack option 1
        head option 1-5
        face option 1-3
        hands/arms option 2,4 
        shoulder option 0
        legs option 1-3
        
        
        potion/magic options
        torso option 4
        belt/backpack option 2,3,6
        head option 7-8
        face option 1-3
        hands/arms option 2,4 
        shoulder option 0
        legs option 1-2
        
        brandit outfits
        torso option 1-2  5 7
        belt/backpack option 0,1,4
        head option 1-5 , 9,10
        face option 1-3
        hands/arms option 1-6
        shoulder option 0-6
        legs option 1-4

        guard outfits
        torso option 5-7
        belt/backpack option 0
        head option 9-15
        face option 1-3
        hands/arms option 3-6
        shoulder option 1-6
        legs option 4-6



        other outfits = farmer + merchant options



        */
        //
        ArrayList options;

        List<int> torsoOptions = new List<int>(new int[] { 1 });
        List<int> beltBackOptions = new List<int>(new int[] { 1 });
        List<int> headOptions = new List<int>(new int[] { 1 });
        List<int> faceOptions = new List<int>(new int[] { 1 });
        List<int> handsArmsOptions = new List<int>(new int[] { 1 });
        List<int> shoulderOptions = new List<int>(new int[] { 1 });
        List<int> legsOptions = new List<int>(new int[] { 1 });

        // get profession from default task
        string profession = toCustomize.GetDefaultTask();

        // passing string "subject" in

        // switch statement

        /*
        all options
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
        switch (profession)
        {
            case "BEFARMER":
                torsoOptions = new List<int>(new int[] { 1, 2 });
                beltBackOptions = new List<int>(new int[] { 0, 1 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });

                break;
            case "LAMPLIGHT":
                torsoOptions = new List<int>(new int[] { 5, 6, 7 });
                beltBackOptions = new List<int>(new int[] { 0 });
                headOptions =
                    new List<int>(new int[] { 9, 10, 11, 12, 13, 14, 15 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 3, 4, 5, 6 });
                shoulderOptions = new List<int>(new int[] { 1, 2, 3, 4, 5, 6 });
                legsOptions = new List<int>(new int[] { 4, 5, 6 });
                break;
            case "BANDIT":
                torsoOptions = new List<int>(new int[] { 1, 2, 5, 7 });
                beltBackOptions = new List<int>(new int[] { 0, 1, 4 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5, 9, 10 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions =
                    new List<int>(new int[] { 1, 2, 3, 4, 5, 6 });
                shoulderOptions =
                    new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6 });
                legsOptions = new List<int>(new int[] { 4, 5, 6 });
                break;
            case "MAYOR":
                torsoOptions = new List<int>(new int[] { 1, 2, 3, 4 });
                beltBackOptions = new List<int>(new int[] { 0 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });
                break;
            case "BEMERCHANT":
                torsoOptions = new List<int>(new int[] { 1, 2, 3, 4 });
                beltBackOptions = new List<int>(new int[] { 4, 5, 6 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });
                break;
            case "BEBLACKSMITH":
                torsoOptions = new List<int>(new int[] { 1, 2 });
                beltBackOptions = new List<int>(new int[] { 0, 1 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });
                break;
            case "BELUMBERJACK":
                torsoOptions = new List<int>(new int[] { 1, 2 });
                beltBackOptions = new List<int>(new int[] { 0, 1 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });

                break;
            case "BEALCHEMIST":
                torsoOptions = new List<int>(new int[] { 1, 2, 3, 4 });
                beltBackOptions = new List<int>(new int[] { 4, 5, 6 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });
                break;
            case "BETRADER":
                torsoOptions = new List<int>(new int[] { 1, 2, 3, 4 });
                beltBackOptions = new List<int>(new int[] { 4, 5, 6 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });
                break;
            case "BEMINER":
                torsoOptions = new List<int>(new int[] { 1, 2 });
                beltBackOptions = new List<int>(new int[] { 0, 1 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });

                break;
            case "GUARD":
                torsoOptions = new List<int>(new int[] { 5, 6, 7 });
                beltBackOptions = new List<int>(new int[] { 0 });
                headOptions =
                    new List<int>(new int[] { 9, 10, 11, 12, 13, 14, 15 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 3, 4, 5, 6 });
                shoulderOptions = new List<int>(new int[] { 1, 2, 3, 4, 5, 6 });
                legsOptions = new List<int>(new int[] { 4, 5, 6 });
                break;
            default:
                // if none do same as farmer
                torsoOptions = new List<int>(new int[] { 1, 2 });
                beltBackOptions = new List<int>(new int[] { 0, 1 });
                headOptions = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                faceOptions = new List<int>(new int[] { 1, 2, 3 });
                handsArmsOptions = new List<int>(new int[] { 1, 2 });
                shoulderOptions = new List<int>(new int[] { 0 });
                legsOptions = new List<int>(new int[] { 1, 2 });

                break;
        }

        // set outfits
        int index = 0;
        int customizeTotal = 0;

        index = Random.Range(0, torsoOptions.Count);
        customizeTotal = customizeTotal + index;
        toCustomize.Character.TorsoOption = torsoOptions[index];

        index = Random.Range(0, beltBackOptions.Count);
        customizeTotal = customizeTotal + index;
        toCustomize.Character.BeltBackpackOption = beltBackOptions[index];

        index = Random.Range(0, headOptions.Count);
        customizeTotal = customizeTotal + index;
        toCustomize.Character.HeadOption = headOptions[index];

        index = Random.Range(0, faceOptions.Count);
        customizeTotal = customizeTotal + index;
        toCustomize.Character.FaceOption = faceOptions[index];

        index = Random.Range(0, handsArmsOptions.Count);
        customizeTotal = customizeTotal + index;
        toCustomize.Character.HandsOption = handsArmsOptions[index];

        index = Random.Range(0, shoulderOptions.Count);
        customizeTotal = customizeTotal + index;
        toCustomize.Character.ShoulderOption = shoulderOptions[index];

        index = Random.Range(0, legsOptions.Count);
        customizeTotal = customizeTotal + index;
        toCustomize.Character.ShoeOption = legsOptions[index];

        toCustomize.LoadCurrentCustomizedValuesFromCharacterData();

        // set stats
        // add outfit total squared to the health if bandit so if they look tough then they are
        int health = Random.Range(50 + customizeTotal, 125);
        float bonusHeath = (float) customizeTotal * 2.25f;
        Debug
            .Log("heatlh " +
            health +
            "   custototal   " +
            customizeTotal +
            "bonus " +
            bonusHeath);

        toCustomize.Character.MaxHealth = (float) health + bonusHeath;
        toCustomize.Character.CurrentHealth = toCustomize.Character.MaxHealth;

        if (profession == "MAYOR")
        {
            toCustomize.Character.MaxHealth = 1000.0f;
            toCustomize.Character.CurrentHealth =
                toCustomize.Character.MaxHealth;
        }

        // all pick between 50-125 health
        // all pick between 10-20 mana with 5% chance of 100-200 mana with all mages having between 100-250 mana
        // all use json defined stamina
    }

    //TODO summon random follower with list of options for items to pick from
    // and random names etc
}
