using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

using System;
using System.Linq;

public class IsLoadedController : MonoBehaviour
{


    private bool isLoaded = true;
    private bool isPlayer = false;

    private float LoadRange = 10.0f;

    private Transform transform;

    private int NumFrameToWaitBetweenLoading = 60;
    private int framesLeftUntilLoad = 0;

    public void Start()
    {
        transform = gameObject.GetComponent<Transform>();


    }

    public void Update()
    {
        if (isPlayer)
        {

            if (framesLeftUntilLoad < NumFrameToWaitBetweenLoading)
            {
                framesLeftUntilLoad += 1;
            }
            else
            {

                framesLeftUntilLoad = 0;
                //CoordinateLoadControllers();

            }
        }
        
        if (Input.GetKey("h"))
        {

            if (isPlayer)
            {
                CoordinateLoadControllers();
            }

        }

    }


    // tell all load controllers to load or unload
    private void CoordinateLoadControllers()
    {

        var IsLoadedControllerList = FindObjectsOfType<IsLoadedController>();
        foreach (IsLoadedController LoadControler in IsLoadedControllerList)
        {
            float dist = Vector3.Distance(transform.position, LoadControler.GetTransform().position);
            if (dist <= LoadRange)
            {
                LoadControler.Load();
            }
            else
            {
                LoadControler.Unload();
            }
        }
    }


    public void Unload()
    {
        if (isLoaded)
        {
            Debug.Log("unloading");

            isLoaded = false;
        }
    }

    public void Load()
    {
        if (!isLoaded)
        {

            Debug.Log("loading back in");

            isLoaded = true;

        }
    }


    public void SetIsPlayer(bool newState)
    {
        isPlayer = newState;
    }

    public Transform GetTransform()
    {
        return transform;
    }


}