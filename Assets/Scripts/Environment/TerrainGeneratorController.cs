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

    public GameObject TownPrefab;

    // TODO make a town creator prefab that summons a town, for now its just a town sphere
    public GameObject LamppostPrefab;

    // lamppost to mark out roads
    public GameObject RoadPainter;

    // todo a prefab that paints road on the terrain around it
    private bool isGoodShape = false;

    public int num_towns = 5;

    //TODO split town and road apart so roads can be bigger
    private int max_attempts_to_make_town_or_road = 1000;

    private float DistanceBetweenLampPosts = 25.0f;

    private IslandGenerator generator = new IslandGenerator();

    private PositionChecks PosChecks = new PositionChecks();

    // allowed wobble and randomness to try to make road
    // TODO rm or use variance in roads v2
    private float variance = 1.0f;

    private float defaultVariance = 1.0f;

    private List<BuildingController>
        roadNetworkJustGenerated = new List<BuildingController>();

    private List<BuildingController>
        isolatedRoadNetworkJustGenerated = new List<BuildingController>();

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

        //Debug.Log(generator.height + "" + generator.width);
        // create the island now that it is known it is a good shape
        //generator.DoIsland(terrainGamObject.GetComponent<Terrain>());
        // base land is y=60
        // water = 54
        // default towns = 4
        for (int i = 0; i < num_towns; i++)
        {
            // run the generate town method
            bool succeeded = GenerateTown(0);
            Debug.Log("made town " + succeeded.ToString());
        }

        // generates road network of just lampposts
        GenerateRoadsBetweenTowns();

        // next generate all buildings for each town
        // TODO update build tool with all buildings and use that to spawn buildings in the towns
        /*
        Town generation

        nodes = a point that generates buildings around it all face that point

        things that can generate off a node
        any building
        other nodes if enabled
            other nodes = road to other node generates
        
        each item in node is pick a random point, use that point to get heading, find point on heading direction that is n distance from center (n = input width)
        if that point is OK to summon building, then do it, if not, try 5 times total before double n for another row around

        TODO change road between points into generate_object between points
        that way spacing can be done and any object can be used, this can be used to generate roads that npcs walk on and paint terrain without simulation


        2 town types
        node
            single large node with many layers

        burrows
            node can have n houses but must have 2 other nodes


        town quantities decided at start
        each town market is always start node

        generates nodes until either trys runs out or town is generated

        will always generate resources like mines, farms and lumber

        if houses missing, then sucks for them



        */



        // once all buildings are made, use build tool to summon people by building the people summoners
        // TODO update buildtool with summoners
        // each building in a town gets a person
        // each town gets a mayor (set one as the player)


        // random populate island with trees and folliage / must be on navmesh

        // for each buildingcontroller and town, rm all folliage in range


        // TODO base this on a cube that paints the terrain near it with sand/grass/road

        // paint the terrain
        // paint all sand, then paint all grass on island as long as not to close to the edge of navmesh
        // all area with grass also do foliage
        // this can be done in a grid pattern, maybe multithread it
        
        // let world run for 1 second to init all characters

        // add painters to each character in the world
        // add foliage rm to all characters so they remove all stuff on the roads and in the way
        // each painter paints road
        
        // simulate the world for 2 days to make the road network and towns stuff and clear area


    }

    private bool GenerateTown(int attempt)
    {
        // make sure its ok to attempt to make this town
        if (attempt > max_attempts_to_make_town_or_road)
        {
            return false;
        }

        // if attempt > max then fail out and be done
        Vector3 position = GenerateRandPosition();
        bool isSafe = PosChecks.CheckPosIsSafe(50, position);
        if (!isSafe)
        {
            return GenerateTown(attempt + 1);
        }

        //check position is far enough away from other towns
        // range = 150
        var towns = FindObjectsOfType<TownController>();
        foreach (TownController controller in towns)
        {
            Vector3 otherPostion = controller.transform.position;
            isSafe =
                PosChecks
                    .CheckPositionsAreFarEnoughAway(position,
                    otherPostion,
                    150);
            if (!isSafe)
            {
                return GenerateTown(attempt + 1);
            }
        }

        Debug.Log("making town its safe " + isSafe);

        // create and init the town
        GameObject TownObject =
            Instantiate(TownPrefab, position, Quaternion.identity);
        TownController newTown = TownObject.GetComponent<TownController>();
        newTown.DoInit();

        // TODO populate the town
        return true;
    }

    private Vector3 GenerateRandPosition()
    {
        float min_x = 100.0f;
        float min_y = 100.0f;
        float max_x = 900.0f;
        float max_y = 900.0f;

        float height = 60.0f;

        float x = UnityEngine.Random.Range(min_x, max_x);
        float y = UnityEngine.Random.Range(min_y, max_y);
        return new Vector3(x, height, y);
    }

    private void GenerateRoadsBetweenTowns()
    {
        Debug.Log("Making roads");

        // for each town, get the shortest distances and do those first
        List<TownController> AllTowns = new List<TownController>();

        // uuids of towns already having a road
        List<String> TownsInNetwork = new List<String>();

        var towns = FindObjectsOfType<TownController>();
        foreach (TownController controller in towns)
        {
            // add town to list of all towns
            AllTowns.Add (controller);

            // if town not in list in network, do GetListOfTownsOrderedByCloseness
        }

        //for all towns, make a list of towns they have a road too
        List<List<String>> TownPairs = new List<List<String>>();

        // each town has a list / road network touching it, it road generates, add this to its list
        List<List<BuildingController>> TownRoads =
            new List<List<BuildingController>>();

        int j = 0;
        foreach (TownController controller in AllTowns)
        {
            // make a list of town uuids that each town has a road to
            TownPairs.Add(new List<string>());

            TownRoads.Add(new List<BuildingController>());

            // add self to list so doesnt make road to self
            TownPairs[j].Add(controller.UUID);
            j = j + 1;
        }

        int k = 0;
        foreach (TownController controller in AllTowns)
        {
            // get list of towns near it
            List<TownController> TownsOrderedClosestFirst =
                GetListOfOtherTownsOrderedByCloseness(controller, AllTowns);

            bool madeRoad = false;

            // find closest thats not in the network
            foreach (TownController otherTown in TownsOrderedClosestFirst)
            {
                // if town isnt in network, then consider making a road there
                if (!TownsInNetwork.Contains(otherTown.UUID))
                {
                    // try to make a road and use the existing road networks
                    madeRoad =
                        GenerateRoadBetweenTwoPoints(controller
                            .transform
                            .position,
                        otherTown.transform.position,
                        0,
                        TownRoads[k]);
                    if (madeRoad)
                    {
                        // if was able to make a road there, then be done and add both to network
                        TownsInNetwork.Add(controller.UUID);
                        TownsInNetwork.Add(otherTown.UUID);

                        // update that this town has a link to other town already
                        TownPairs[k].Add(otherTown.UUID);

                        // get index for the othertown in townpairs, and update its townpairs to controller.uuid
                        int t = 0;
                        foreach (TownController townToPair in AllTowns)
                        {
                            if (townToPair.UUID == otherTown.UUID)
                            {
                                TownPairs[t].Add(controller.UUID);

                                // update the other towns road network with the one that just got made
                                foreach (BuildingController
                                    lamp
                                    in
                                    roadNetworkJustGenerated
                                )
                                {
                                    TownRoads[t].Add(lamp);
                                }
                            }
                            t = t + 1;
                        }

                        break;
                    }
                }
            }

            // if road making fails then make a road to the nearest town that it doesnt have a road to
            if (!madeRoad)
            {
                foreach (TownController otherTown in TownsOrderedClosestFirst)
                {
                    // if town isnt in list of towns that this one has a road to, then make a new road
                    if (!TownPairs[k].Contains(otherTown.UUID))
                    {
                        madeRoad =
                            GenerateRoadBetweenTwoPoints(controller
                                .transform
                                .position,
                            otherTown.transform.position,
                            0,
                            TownRoads[k]);
                        if (madeRoad)
                        {
                            // if was able to make a road there, then be done and add both to network
                            TownsInNetwork.Add(controller.UUID);
                            TownsInNetwork.Add(otherTown.UUID);

                            // update that this town has a link to other town already
                            TownPairs[k].Add(otherTown.UUID);

                            // get index for the othertown in townpairs, and update its townpairs to controller.uuid
                            int t = 0;
                            foreach (TownController townToPair in AllTowns)
                            {
                                if (townToPair.UUID == otherTown.UUID)
                                {
                                    TownPairs[t].Add(controller.UUID);

                                    // update the other towns road network with the one that just got made
                                    foreach (BuildingController
                                        lamp
                                        in
                                        roadNetworkJustGenerated
                                    )
                                    {
                                        TownRoads[t].Add(lamp);
                                    }
                                    //break;
                                }
                                t = t + 1;
                            }

                            break;
                        }
                    }
                }
            }
            k = k + 1;
        }

        Debug.Log("townpairs");
        foreach (List<string> pairlist in TownPairs)
        {
            string a = "";
            foreach (string s in pairlist)
            {
                a = a + s + ", ";
            }
            Debug.Log (a);
        }

        //float distance = Vector3.Distance(positionA,positionB);
    }

    private bool
    GenerateRoadBetweenTwoPoints(
        Vector3 positionA,
        Vector3 positionB,
        int attempt,
        List<BuildingController> lampsInRoadSegment
    )
    {
        Debug.Log("making road from " + positionA + " and " + positionB);

        // TODO rename attempt to link/index in road, and that will have a max len of road, acts similar
        // make sure not too many attempts
        if (attempt >= max_attempts_to_make_town_or_road)
        {
            Debug.Log("ran out of attempts to make this road :(");

            // if we ran out of attempts
            // remove the failed road
            foreach (BuildingController
                lampPost
                in
                isolatedRoadNetworkJustGenerated
            )
            {
                Destroy(lampPost.gameObject);
            }
            isolatedRoadNetworkJustGenerated = new List<BuildingController>();

            // wipe list of roads just generated
            roadNetworkJustGenerated = new List<BuildingController>();

            // and notify of failure
            return false;
        }

        // look within 50m for any building controller that is a lamppost, if its closer than positionB then set positionB to it
        BuildingController otherRoad =
            GetLamppostFromOtherRoadIfCloser(positionA,
            50.0f,
            lampsInRoadSegment);
        if (otherRoad != null)
        {
            Debug.Log("targeting a road instead of town");
            positionB = otherRoad.transform.position;
        }

        // check within lamppost range of B if so return true
        bool NeedToBuildMoreRoad =
            PosChecks
                .CheckPositionsAreFarEnoughAway(positionA,
                positionB,
                DistanceBetweenLampPosts);
        if (!NeedToBuildMoreRoad)
        {
            // set the road network just generated
            roadNetworkJustGenerated = lampsInRoadSegment;

            // make last generated lamppost face destination
            if (isolatedRoadNetworkJustGenerated.Count > 1)
            {
                Transform lastPost =
                    isolatedRoadNetworkJustGenerated[isolatedRoadNetworkJustGenerated
                        .Count -
                    1].transform;
                lastPost.rotation =
                    Quaternion
                        .Slerp(lastPost.rotation,
                        Quaternion.LookRotation(positionB - lastPost.position),
                        1.0f);
            }

            // wipe the isolated run
            isolatedRoadNetworkJustGenerated = new List<BuildingController>();

            return true;
        }

        // make new pos for lamppost to go with a varience for random
        Vector3 newPos = MakeNewLampPostPositionTwo(positionA, positionB);

        // TODO debug this and make sure it triggres sooner
        if (newPos == positionA)
        {
            Debug.Log("giving up on making road, cant make any new positions");
            // if the position didnt change, say we are out of attempts
            return GenerateRoadBetweenTwoPoints(positionA,
            positionB,
            max_attempts_to_make_town_or_road + attempt,
            lampsInRoadSegment);
        }

        /*
        // check if pos is good with range of 10
        bool canContinueRoadThere = PosChecks.CheckPosIsSafe(10.0f, newPos);
        if (!canContinueRoadThere)
        {
            // TODO find way to vary attempts more
            variance = variance * 1.2f;
            Debug.Log("variance" + variance);
            return GenerateRoadBetweenTwoPoints(positionA,
            positionB,
            attempt + 1,
            lampsInRoadSegment);
            // TODO if position not good, then vary it n attempts
            // if not, return false
        }
        */
        // TODO
        // check that we are not too close to another road, if so, skip generating this lamppost but consider road merged
        // get any lamppost/road within distance that is not from this generation
        otherRoad =
            GetLamppostFromOtherRoadIfCloser(positionA,
            (DistanceBetweenLampPosts/4.0f )*3.0f,
            isolatedRoadNetworkJustGenerated);

        // if there isnt any lamppost from another road within range, generate one
        if (otherRoad == null)
        {
            // summon lamppost at new position
            GameObject LampPostObject =
                Instantiate(LamppostPrefab, newPos, Quaternion.identity);
            BuildingController lampPostController =
                LampPostObject.GetComponent<BuildingController>();

            // make prev lamppost face the new one
            if (isolatedRoadNetworkJustGenerated.Count > 1)
            {
                Transform lastPost =
                    isolatedRoadNetworkJustGenerated[isolatedRoadNetworkJustGenerated
                        .Count -
                    1].transform;
                lastPost.rotation =
                    Quaternion
                        .Slerp(lastPost.rotation,
                        Quaternion
                            .LookRotation(lampPostController
                                .transform
                                .position -
                            lastPost.position),
                        1.0f);
            }

            lampPostController.DoInit(); // assign uuid
            lampsInRoadSegment.Add (lampPostController);

            // add to the isolated road network
            isolatedRoadNetworkJustGenerated.Add (lampPostController);

            // TODO rm variance
            // reset variance to default in case it was not
            variance = defaultVariance;
        }

        return GenerateRoadBetweenTwoPoints(newPos,
        positionB,
        attempt + 1,
        lampsInRoadSegment);
    }

    // TODO rm this

    private Vector3
    MakeNewLampPostPosition(Vector3 startPosition, Vector3 TargetPosition)
    {
        // get heading
        Vector3 heading = TargetPosition - startPosition;

        // add varianece
        float left_right_jitter =
            UnityEngine
                .Random
                .Range((-1.0f * variance * (DistanceBetweenLampPosts / 5.0f)),
                ((DistanceBetweenLampPosts / 5.0f) * variance));
        float forward_backward_jitter =
            UnityEngine
                .Random
                .Range((-1.0f * variance * (DistanceBetweenLampPosts / 8.0f)),
                ((DistanceBetweenLampPosts / 8.0f) * variance));

        float LocalDistanceBetweenLampPosts =
            DistanceBetweenLampPosts + forward_backward_jitter;

        if (Math.Abs(heading.z) < Math.Abs(heading.x))
        {
            if (heading.x < 0.0f)
            {
                LocalDistanceBetweenLampPosts =
                    LocalDistanceBetweenLampPosts * -1.0f;
            }
            return new Vector3(startPosition.x + LocalDistanceBetweenLampPosts,
                startPosition.y,
                startPosition.z + left_right_jitter);
        }
        else
        {
            if (heading.z < 0.0f)
            {
                LocalDistanceBetweenLampPosts =
                    LocalDistanceBetweenLampPosts * -1.0f;
            }
            return new Vector3(startPosition.x + left_right_jitter,
                startPosition.y,
                startPosition.z + LocalDistanceBetweenLampPosts);
        }

        return new Vector3(0.0f, 0.0f, 0.0f);
    }

    private Vector3
    MakeNewLampPostPositionTwo(Vector3 startPosition, Vector3 TargetPosition)
    {
        // assume up/down = y left/right is x on a 2d plane
        Vector3 heading = TargetPosition - startPosition;

        float LocalDistanceBetweenLampPosts = DistanceBetweenLampPosts;

        // generate list of 8 positions, up, down, left, right, up-right,up-left,down-right,down-left
        // then from heading order them by preferance, if pref not good, then do next and so on
        List<Vector3> newPosOptions = new List<Vector3>();

        newPosOptions
            .Add(new Vector3(startPosition.x + LocalDistanceBetweenLampPosts,
                startPosition.y,
                startPosition.z)); //up 0
        newPosOptions
            .Add(new Vector3(startPosition.x - LocalDistanceBetweenLampPosts,
                startPosition.y,
                startPosition.z)); //down 1
        newPosOptions
            .Add(new Vector3(startPosition.x,
                startPosition.y,
                startPosition.z + LocalDistanceBetweenLampPosts)); //right 2
        newPosOptions
            .Add(new Vector3(startPosition.x,
                startPosition.y,
                startPosition.z - LocalDistanceBetweenLampPosts)); //left 3
        newPosOptions
            .Add(new Vector3(startPosition.x + LocalDistanceBetweenLampPosts,
                startPosition.y,
                startPosition.z + LocalDistanceBetweenLampPosts)); //up-right  4
        newPosOptions
            .Add(new Vector3(startPosition.x - LocalDistanceBetweenLampPosts,
                startPosition.y,
                startPosition.z + LocalDistanceBetweenLampPosts)); //down-rght 5
        newPosOptions
            .Add(new Vector3(startPosition.x + LocalDistanceBetweenLampPosts,
                startPosition.y,
                startPosition.z - LocalDistanceBetweenLampPosts)); //up-left 6
        newPosOptions
            .Add(new Vector3(startPosition.x - LocalDistanceBetweenLampPosts,
                startPosition.y,
                startPosition.z - LocalDistanceBetweenLampPosts)); //down-left 7

        // make the list of indexes that order is prefered
        List<int> preferedIndexOrder = new List<int>();

        float diff = Math.Abs(Math.Abs(heading.z) - Math.Abs(heading.x));

        /*
        Debug
            .Log("heading z, x" +
            Math.Abs(heading.z).ToString() +
            "  " +
            Math.Abs(heading.x).ToString() +
            " diff " +
            diff.ToString());
        */
        // if diff < 20 then prefer diagonal
        if (
            diff <= 20.0f // if diff greater than 20 then pick up or down
        )
        {
            // add the prefered diagonal
            // if going up/down via y axis on 2d plane
            if (heading.x < 0.0f)
            {
                // diagonal is down
                if (heading.z < 0.0f)
                {
                    // diagonal is left
                    preferedIndexOrder.Add(7);
                }
                else
                {
                    // diagonal right
                    preferedIndexOrder.Add(5);
                }
            }
            else
            {
                // diagonal is up
                if (heading.z < 0.0f)
                {
                    // diagonal is left
                    preferedIndexOrder.Add(6);
                }
                else
                {
                    // diagonal right
                    preferedIndexOrder.Add(4);
                }
            }

            // add the up and downs in prefered order
            if (Math.Abs(heading.z) < Math.Abs(heading.x))
            {
                // if up/down is prefered
                // if going up/down via y axis on 2d plane
                if (heading.x < 0.0f)
                {
                    // prefer is down
                    preferedIndexOrder.Add(1);
                    if (heading.z < 0.0f)
                    {
                        // prefer is left
                        preferedIndexOrder.Add(3);
                    }
                    else
                    {
                        // prefer right
                        preferedIndexOrder.Add(2);
                    }
                }
                else
                {
                    //prefer is up
                    preferedIndexOrder.Add(0);

                    if (heading.z < 0.0f)
                    {
                        // prefer is left
                        preferedIndexOrder.Add(3);
                    }
                    else
                    {
                        // prefer right
                        preferedIndexOrder.Add(2);
                    }
                }
            }
            else
            {
                // if left/right is prefered
                if (heading.z < 0.0f)
                {
                    // prefer is left
                    preferedIndexOrder.Add(3);
                    if (heading.x < 0.0f)
                    {
                        // prefer is down
                        preferedIndexOrder.Add(1);
                    }
                    else
                    {
                        // prefer up
                        preferedIndexOrder.Add(0);
                    }
                }
                else
                {
                    //prefer is right
                    preferedIndexOrder.Add(2);
                    if (heading.x < 0.0f)
                    {
                        // prefer is down
                        preferedIndexOrder.Add(1);
                    }
                    else
                    {
                        // prefer up
                        preferedIndexOrder.Add(0);
                    }
                }
            }
        }
        else if (
            Math.Abs(heading.z) < Math.Abs(heading.x) // if diff greater than 20 then pick up or down
        )
        {
            // if going up/down via y axis on 2d plane
            if (heading.x < 0.0f)
            {
                // if want to go down
                preferedIndexOrder.Add(1);

                // now pick r/l
                if (heading.z < 0.0f)
                {
                    // want to go down left
                    preferedIndexOrder.Add(7);

                    // want to go left
                    preferedIndexOrder.Add(3);
                }
                else
                {
                    // want to go down right
                    preferedIndexOrder.Add(5);

                    // want to go right
                    preferedIndexOrder.Add(2);
                }
            }
            else
            {
                // if heading is positive, then want to go up
                preferedIndexOrder.Add(0);

                // now pick r/l
                if (heading.z < 0.0f)
                {
                    // want to go up left
                    preferedIndexOrder.Add(6);

                    // want to go left
                    preferedIndexOrder.Add(3);
                }
                else
                {
                    // want to go up right
                    preferedIndexOrder.Add(4);

                    // want to go right
                    preferedIndexOrder.Add(2);
                }
            }
        }
        else
        {
            // if going left/right on x axis
            if (heading.z < 0.0f)
            {
                // want to go left
                preferedIndexOrder.Add(3);

                // now pick u/d
                if (heading.x < 0.0f)
                {
                    // want to go down left
                    preferedIndexOrder.Add(7);

                    // want to go up left
                    preferedIndexOrder.Add(6);
                }
                else
                {
                    // want to go up left
                    preferedIndexOrder.Add(6);

                    // want to go down left
                    preferedIndexOrder.Add(7);
                }
            }
            else
            {
                // want to go right
                preferedIndexOrder.Add(2);

                // now pick u/d
                if (heading.x < 0.0f)
                {
                    // want to go down right
                    preferedIndexOrder.Add(5);

                    // want to go up right
                    preferedIndexOrder.Add(4);
                }
                else
                {
                    // want to go up right
                    preferedIndexOrder.Add(4);

                    // want to go down right
                    preferedIndexOrder.Add(5);
                }
            }
        }

        // now prefered indexes has 3 options
        // if direction to go is up, it has up, then up/right/left x2 for prefered r/l 1st
        // in a while loop where the range to edge of map starts at 10 then goes down to 1 as options dwindle
        // for each index in the pref indexes, check point is safe, if so return it, if not, then try next
        float preferedDistanceFromEdge = 25.0f;
        int selectedIndex = -1;
        while (preferedDistanceFromEdge >= 1.0f)
        {
            bool foundPos = false;

            foreach (int index in preferedIndexOrder)
            {
                // check all the indexes in prefered order that they are safe
                foundPos =
                    PosChecks
                        .CheckPosIsSafe(preferedDistanceFromEdge,
                        newPosOptions[index]);
                if (foundPos)
                {
                    selectedIndex = index;
                    break;
                }
            }
            if (foundPos)
            {
                break;
            }

            // subtract 1 from the distance to edge preference
            preferedDistanceFromEdge = preferedDistanceFromEdge - 1.0f;
        }

        // if found a position, use that
        if (selectedIndex != -1)
        {
            return newPosOptions[selectedIndex];
        }

        // if cant decide/ all are bad, stay same place and run out of attempts
        return startPosition;
    }

    private List<TownController>
    GetListOfOtherTownsOrderedByCloseness(
        TownController startTown,
        List<TownController> Towns
    )
    {
        List<TownController> TownsCacled = new List<TownController>();
        List<float> TownsDistances = new List<float>();

        foreach (TownController controller in Towns)
        {
            // dont add self
            if (controller.UUID != startTown.UUID)
            {
                bool append = true;
                int index = 0;

                float distance =
                    Vector3
                        .Distance(startTown.transform.position,
                        controller.transform.position);

                Debug.Log (distance);

                // if append, add to end of list
                // else insert at index, pushing index towards end
                int i = 0;
                foreach (float otherDist in TownsDistances)
                {
                    if (otherDist > distance)
                    {
                        append = false;
                        index = i;
                        break;
                    }

                    i = i + 1;
                }

                if (append)
                {
                    // add distance and town to the lists
                    TownsCacled.Add (controller);
                    TownsDistances.Add (distance);
                }
                else
                {
                    // insert it at that index
                    TownsCacled.Insert (index, controller);
                    TownsDistances.Insert (index, distance);
                }
            }
        }

        return TownsCacled;
    }

    private BuildingController
    GetLamppostFromOtherRoadIfCloser(
        Vector3 currentPosition,
        float rangeToLook,
        List<BuildingController> myNetwork
    )
    {
        BuildingController controllerToReturn = null; // = new BuildingController();
        float bestDistance = 999.0f; // set best to be really high

        Collider[] hitColliders =
            Physics.OverlapSphere(currentPosition, rangeToLook);
        foreach (var hitCollider in hitColliders)
        {
            BuildingController controller =
                hitCollider.gameObject.GetComponent<BuildingController>();
            if (controller != null)
            {
                if (controller.Type == "LAMP")
                {
                    if (!myNetwork.Contains(controller))
                    {
                        // found a road we can attach to, next see set best distance and

                        float distanceToPost =
                            Vector3
                                .Distance(currentPosition,
                                controller.transform.position);
                        if (distanceToPost < bestDistance)
                        {
                            bestDistance = distanceToPost;
                            controllerToReturn = controller;
                        }
                    }
                }
            }
        }

        // if found a road, use that, else return null
        if (bestDistance >= 1000.0f)
        {
            return null;
        }
        else
        {
            return controllerToReturn;
        }
    }
}

public class PositionChecks
{
    // class avail to check if a point is good to build on
    public bool CheckPosIsSafe(float rangeToEndOfMesh, Vector3 position)
    {
        // do a find nearest navmesh edge, if its bigger than range, then say its safe
        // TODO also make sure its flat and on navmesh
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(position, out hit, NavMesh.AllAreas))
        {
            return CheckPositionsAreFarEnoughAway(hit.position,
            position,
            rangeToEndOfMesh);
        }
        return false;
    }

    public bool
    CheckPositionsAreFarEnoughAway(
        Vector3 positionA,
        Vector3 positionB,
        float dist
    )
    {
        float distance = Vector3.Distance(positionA, positionB);
        return distance >= dist;
    }
}
