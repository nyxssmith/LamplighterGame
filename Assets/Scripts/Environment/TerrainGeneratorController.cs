using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGeneratorController : MonoBehaviour
{
    private GameObject terrainGamObject; // =  Terrain.activeTerrain.gameObject;

    public List<Collider> colliders = new List<Collider>();

    public GameObject TownCreator;
    // TODO make a town creator prefab that summons a town, for now its just a town sphere
    public GameObject Lamppost;
    // lamppost to mark out roads

    public GameObject RoadPainter;
    // todo a prefab that paints road on the terrain around it

    private bool isGoodShape = false;

    public int num_towns = 5;

    private int max_attempts_to_make_town_or_road = 20;

    private IslandGenerator generator = new IslandGenerator();

    public void Start()
    {
        terrainGamObject = Terrain.activeTerrain.gameObject;

        // populate list of colliders
        foreach (Transform child in transform)
        {
            //child is your child transform
            Collider colliderInChild =
                child.gameObject.GetComponent<Collider>();
            if (colliderInChild != null)
            {
                colliders.Add (colliderInChild);
            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            isGoodShape = false;
            Debug.Log("generating new terrain!");
            GenerateIsland();
        }
    }

    private void GenerateIsland()
    {
        // start generation live
        generator.StartGenerationLive(terrainGamObject.GetComponent<Terrain>());
        Debug.Log(generator.height+""+generator.width);
        // create the island now that it is known it is a good shape
        
        //generator.DoIsland(terrainGamObject.GetComponent<Terrain>());

        // todo do towns etc
        // base land is y=60

        // water = 54

        // default towns = 4   
        for (int i = 0; i < num_towns+1; i++){
            Debug.Log("making town at");
            // run the generate town method
            bool succeeded = GenerateTown(0);
       }

       // todo do roads between towns
       // first make a terrain painter thing
        




    }

    private bool GenerateTown(int attempt){

        // make sure its ok to attempt to make this town
        if(attempt > max_attempts_to_make_town_or_road){
            return false;
        }

        // if attempt > max then fail out and be done
        Vector3 position = GenerateRandPosition();
        bool isSafe = CheckPosIsSafe(50,position);
        if(!isSafe){
            return GenerateTown(attempt+1);
        }

        //check position is far enough away from other towns
        // range = 150
        var towns = FindObjectsOfType<TownController>();
        foreach (TownController controller in towns)
        {
            Vector3 otherPostion = controller.transform.position;
            isSafe = CheckPositionsAreFarEnoughAway(position,otherPostion,150);
            if(!isSafe){
                return GenerateTown(attempt+1);
            }
        }

        
        Debug.Log("making town its safe "+isSafe);
        Instantiate(TownCreator,position,Quaternion.identity);

        return true;
    }


    private Vector3 GenerateRandPosition(){
        float min_x = 100.0f;
        float min_y = 100.0f;
        float max_x = 900.0f;
        float max_y = 900.0f;

        float height = 60.0f;


        float x = UnityEngine.Random.Range(min_x,max_x);
        float y = UnityEngine.Random.Range(min_y,max_y);

        return new Vector3(x,height,y);


    }

    private bool CheckPosIsSafe(float rangeToEndOfMesh, Vector3 position){
        // do a find nearest navmesh edge, if its bigger than range, then say its safe

        // TODO also make sure its flat

        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(position, out hit, NavMesh.AllAreas))
        {

            return CheckPositionsAreFarEnoughAway(hit.position,position,rangeToEndOfMesh);
        }
        return false;
    }

    private bool CheckPositionsAreFarEnoughAway(Vector3 positionA,Vector3 positionB, float dist){
        float distance = Vector3.Distance(positionA,positionB);
        return distance >= dist;
    }

    

}
