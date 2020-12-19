using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

using System;
using System.Linq;

public class WorldManager : MonoBehaviour
{


    // distance between lampposts on the road
    public float DistanceBetweenLampPosts = 30.0f;
    public GameObject RoadStart;
    public GameObject RoadEnd;

    public GameObject LampPostPreFab;

    // TODO also summon random buildings along the road
    public GameObject RoadBuildingPreFab;

    private bool generated_road = false;


    // TODO also use this for town
    //public GameObject TownBuildingPreFab;


    public void Start()
    {



    }

    public void Update()
    {
        // push u to generate road
        if (Input.GetKey("p"))
        {
            if (!generated_road)
            {
                GenerateRoad();
                generated_road = true;
            }
        }
    }



    private void GenerateRoad()
    {

        Debug.Log("Generating road");

        int left_weight = 0;
        int right_weight = 0;
        // get distance from start and end

        Vector3 Start = RoadStart.gameObject.GetComponent<Transform>().position;
        Vector3 End = RoadEnd.gameObject.GetComponent<Transform>().position;

        Vector3 heading = End - Start;

        Debug.Log("heading" + heading);

        float distance_to_go = (Start - End).magnitude;
        //int numLampPosts = Convert.ToInt32(distance / DistanceBetweenLampPosts);

        //float distanceBetweenLampPosts = distance / numLampPosts;

        Vector3 newPos = MakeNewPos(heading, DistanceBetweenLampPosts, Start);
        GenerateLampPost(newPos);

        int count = 0;
        while (distance_to_go >= DistanceBetweenLampPosts * 1.5f)
        {
            count += 1;
            if (count > 400)
            {
                break;
            }
            Start = newPos;
            heading = End - Start;
            distance_to_go = (Start - End).magnitude;
            newPos = MakeNewPos(heading, DistanceBetweenLampPosts, Start);
            GenerateLampPost(newPos);

        }




    }

    private void GenerateLampPost(Vector3 SummonPositon)
    {
        // TODO make sure is on ground even if uneave
        Instantiate(LampPostPreFab, SummonPositon, Quaternion.identity);

    }


    private Vector3 MakeNewPos(Vector3 input_vector, float distance_between_posts, Vector3 start_pos)
    {

        float left_right_jitter = UnityEngine.Random.Range(-1.0f * (DistanceBetweenLampPosts / 5.0f), (DistanceBetweenLampPosts / 5.0f));
        float forward_backward_jitter = UnityEngine.Random.Range(-1.0f * (DistanceBetweenLampPosts / 8.0f), (DistanceBetweenLampPosts / 8.0f));

        distance_between_posts = distance_between_posts + forward_backward_jitter;

        if (Math.Abs(input_vector.z) < Math.Abs(input_vector.x))
        {
            if (input_vector.x < 0.0f)
            {
                distance_between_posts = distance_between_posts * -1.0f;
            }
            return new Vector3(start_pos.x + distance_between_posts, start_pos.y, start_pos.z + left_right_jitter);
        }
        else
        {
            if (input_vector.z < 0.0f)
            {
                distance_between_posts = distance_between_posts * -1.0f;
            }
            return new Vector3(start_pos.x + left_right_jitter, start_pos.y, start_pos.z + distance_between_posts);
        }
        return new Vector3(0.0f, 0.0f, 0.0f);

    }


}