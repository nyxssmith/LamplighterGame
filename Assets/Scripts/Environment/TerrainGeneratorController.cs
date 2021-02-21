using System;
using System;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    private GameObject terrainGamObject; // =  Terrain.activeTerrain.gameObject;

    public List<Collider> colliders = new List<Collider>();

    private bool isGoodShape = false;

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
    }
}
