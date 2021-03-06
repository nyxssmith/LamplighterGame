using System;
using System;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class TownController : MonoBehaviour
{
    private Transform TownTransform;

    private SphereCollider TownCollider;

    public string UUID;

    // population supported (1 house = 1 or 2 person)
    // wood (given by lumberman per day)
    // stone (given by mining per day)
    // metal (given by mining per day)
    // gold (town funds) / taxes on each shop add money per day
    // range = dist from center to farthest building * 1.5
    // current population
    // list of all buildings
    public float wood = 0;

    public float stone = 0;

    public float metal = 0;

    public float ore = 0;

    public float money = 0;

    public float food = 0;

    public float range = 12;

    // TODO name all towns
    public string Name = "town";

    public float current_population = 0;

    public float supported_population = 0;

    private List<BuildingController> Buildings = new List<BuildingController>();

    private List<CharacterController>
        Residents = new List<CharacterController>();

    private float minRange = 10.0f;

    private bool playerInTown = false;

    private bool doneInit = false;

    public void Start()
    {
        if (!doneInit)
        {
            DoInit();
        }
    }

    public void DoInit()
    {
        TownTransform = gameObject.GetComponent<Transform>();

        TownCollider = GetComponent<SphereCollider>();

        if (UUID == "")
        {
            UUID = Guid.NewGuid().ToString();
        }

        // init set range
        SetTownRange (range);

        doneInit = true;
    }

    public void Update()
    {
        //Debug.Log("im town " + UUID + " cp " + current_population.ToString() + " sp " + supported_population.ToString() + " range " + range.ToString());
        //Debug.Log(Buildings.Count);
        // TODO move triggering these to the daylight cycle, trigger a few times a day
        //DoBuildingRecount();
        //DoResourcesUpdate();
    }

    private void OnTriggerEnter(Collider Entering)
    {
        DoTownUpdate();
        UpdateCameraTownUI(Entering.gameObject, GenerateTownUIString());
    }

    private void OnTriggerExit(Collider Entering)
    {
        DoTownUpdate();
        UpdateCameraTownUI(Entering.gameObject, "");
    }

    public void UpdateCameraTownUI(GameObject EnteringObject, string newText)
    {
        CharacterController EnteringCharacterController =
            EnteringObject.GetComponent<CharacterController>();
        if (EnteringCharacterController != null)
        {
            if (EnteringCharacterController.GetIsPlayer())
            {
                // set playerintown
                playerInTown = !(newText == "");

                // do the town UI
                EnteringCharacterController.GetCameraFollow().DoTownUI(newText);

                // update the players ref to the town they are in
                if (newText == "")
                {
                    EnteringCharacterController.SetTown(null);
                }
                else
                {
                    EnteringCharacterController.SetTown(this);
                }
            }
        }
    }

    public string GenerateTownUIString()
    {
        string townUIString = "[";

        townUIString = townUIString + Name + "]\n";
        townUIString =
            townUIString +
            "Population : " +
            current_population.ToString() +
            "/" +
            supported_population.ToString() +
            "\n";
        townUIString = townUIString + "Food : " + food.ToString() + "\n";
        townUIString = townUIString + "Wood : " + wood.ToString() + "\n";
        townUIString = townUIString + "Stone : " + stone.ToString() + "\n";
        townUIString = townUIString + "Metal : " + metal.ToString() + "\n";
        townUIString = townUIString + "Ore : " + ore.ToString() + "\n";
        townUIString = townUIString + "Gold : " + money.ToString() + "\n";

        return townUIString;
    }

    public void DoResourcesUpdate()
    {
        // called each day, get resources given by each building controller inside it
        // for each building in town
        // if each type of work generating building
        //   add its specific resource type
        List<BuildingController> NotFarms = new List<BuildingController>();

        //process all farms first and add rest to second round
        // farms take and make food but do not require any starting food to work
        foreach (BuildingController building in Buildings)
        {
            if (building.GetType() == "FARM")
            {
                if (building.GetHasDoneWork())
                {
                    int AmountOfWorkers =
                        building.GetCharactersWhoInteract().Count;
                    if (AmountOfWorkers > 3)
                    {
                        AmountOfWorkers = 3;
                    }
                    food = food + (1.5f * AmountOfWorkers);

                    // reset work counter and cost food
                    food = food - 1.0f;

                    building.SetHasDoneWork(false);
                }
            }
            else
            {
                NotFarms.Add (building);
            }
        }

        // process all other buildings
        foreach (BuildingController building in NotFarms)
        {
            if (building.GetHasDoneWork() && food > 0.0f)
            {
                food = food - 1.0f;
                Debug.Log("buildng " + building.GetType() + " has done work");
                building.SetHasDoneWork(false);
            }
        }

        UpdatePlayerUIIfPlayerIsPresentInTown();
    }

    public void DoBuildingRecount()
    {
        // update on all buildings in range
        // expand range if needed
        List<BuildingController> NewBuildings = new List<BuildingController>();

        float longest_distance = 0.0f;

        // look within current range
        Collider[] hitColliders =
            Physics.OverlapSphere(TownTransform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            BuildingController controller =
                hitCollider.gameObject.GetComponent<BuildingController>();
            if (controller != null && controller.enabled)
            {
                NewBuildings.Add (controller);

                float distance =
                    Vector3
                        .Distance(TownTransform.position,
                        controller.GetTransform().position);

                if (distance > longest_distance)
                {
                    longest_distance = distance;
                }
            }
        }

        Buildings = NewBuildings;

        SetTownRange(10.0f + longest_distance);
    }

    public void DoTownUpdate()
    {
        Debug.Log("doing town update");
        DoBuildingRecount();

        // only needs to be done daily
        //DoResourcesUpdate();
        DoResidentRecount();
    }

    private void SetTownRange(float newRange)
    {
        if (newRange < minRange)
        {
            range = minRange;
        }
        else
        {
            range = newRange;
        }

        TownCollider.radius = range;
    }

    public bool GetAllowedToPlaceBuilding(BuildingController controller)
    {
        // TODO check if has resources to build buildint and return if can build or not
        return true;
    }

    private void AddResident(CharacterController newResident)
    {
        Residents.Add (newResident);
    }

    private void DoResidentRecount()
    {
        // for all houses
        // get all charatcers associated with the houses in the interacted with list
        supported_population = 0;
        current_population = 0;

        foreach (BuildingController building in Buildings)
        {
            if (building.GetType() == "HOME")
            {
                supported_population += 1;

                // then get len of associated people with building
                if (building.GetOwner() != "")
                {
                    current_population += 1;
                } //get len of associeated
            }
        }
    }

    public void SubtractResourceCostsBuilding(BuildingController building)
    {
        // TODO gets resources of a building and subtracts them
    }

    public void UpdatePlayerUIIfPlayerIsPresentInTown()
    {
        // if player is in town, find them and update their UI
        // must look for player again since they can swap bodies
        if (playerInTown)
        {
            CharacterController player =
                GetPlayerInTown(GetAllCharacterControllersInTown());
            if (player != null)
            {
                UpdateCameraTownUI(player.gameObject, GenerateTownUIString());
            }
        }
    }

    private List<CharacterController> GetAllCharacterControllersInTown()
    {
        // gets a list of all character controllers in town
        List<CharacterController> returnList = new List<CharacterController>();

        Collider[] hitColliders =
            Physics.OverlapSphere(TownTransform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            CharacterController controller =
                hitCollider.gameObject.GetComponent<CharacterController>();
            if (controller != null)
            {
                returnList.Add (controller);
            }
        }
        if (returnList.Count > 0)
        {
            return returnList;
        }

        return null;
    }

    private CharacterController
    GetPlayerInTown(List<CharacterController> controllers)
    {
        foreach (CharacterController controller in controllers)
        {
            if (controller.GetIsPlayer())
            {
                return controller;
            }
        }

        return null;
    }
}
