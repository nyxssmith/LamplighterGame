using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

using System;
using System.Linq;

public class TownController : MonoBehaviour
{

    private Transform TownTransform;

    private SphereCollider TownCollider;


    private string UUID;


    // population supported (1 house = 1 or 2 person)
    // wood (given by lumberman per day) 
    // stone (given by mining per day)
    // metal (given by mining per day)
    // gold (town funds) / taxes on each shop add money per day

    // range = dist from center to farthest building * 1.5

    // current population
    // list of all buildings

    private float supported_population = 0;
    private float wood = 0;
    private float stone = 0;
    private float metal = 0;
    private float gold = 0;
    private float range = 12;
    private float current_population = 0;

    private List<BuildingController> Buildings = new List<BuildingController>();

    private float minRange = 10.0f;


    public void Start()
    {
        TownTransform = gameObject.GetComponent<Transform>();
 
        TownCollider = GetComponent<SphereCollider>();
 
        if (UUID == "")
        {
            UUID = Guid.NewGuid().ToString();
        }



    }

    public void Update()
    {

        //Debug.Log("im town " + UUID + " cp " + current_population.ToString() + " sp " + supported_population.ToString() + " range " + range.ToString());
        //Debug.Log(Buildings.Count);


        // TODO move triggering these to the daylight cycle, trigger a few times a day

        //DoBuildingRecount();

        //DoResourcesUpdate();



    }


    private void OnTriggerEnter(Collider EnteringCharacter)
    {




    }

    private void OnTriggerExit(Collider EnteringCharacter)
    {



    }

    public void DoResourcesUpdate()
    {
        // called each day, get resources given by each building controller inside it

        // for each building in town
        // if house
        //  add supported + 1
        //  if owned: add current population + 1
        // if each type of work generating building
        //   add its specific resource type

        supported_population = 0;
        current_population = 0;

        foreach (BuildingController building in Buildings)
        {
            if (building.GetType() == "HOME")
            {
                supported_population += 1;
                if (building.GetOwner() != "")
                {
                    current_population += 1;
                }
            }

            if (building.GetHasDoneWork())
            {
                // get resouces type etc and add
            }


        }



    }

    public void DoBuildingRecount()
    {
        // update on all buildings in range
        // expand range if needed
        List<BuildingController> NewBuildings = new List<BuildingController>();

        float longest_distance = 0.0f;

        // look within current range
        Collider[] hitColliders = Physics.OverlapSphere(TownTransform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            BuildingController controller = hitCollider.gameObject.GetComponent<BuildingController>();
            if (controller != null && controller.enabled)
            {
                NewBuildings.Add(controller);
                float distance = Vector3.Distance(TownTransform.position, controller.GetTransform().position);
                if (distance > longest_distance)
                {
                    longest_distance = distance;
                }
            }
        }

        Buildings = NewBuildings;

        SetTownRange(10.0f+ longest_distance);


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



    public bool GetAllowedToPlaceBuilding(BuildingController controller){



        return true;
    }


}