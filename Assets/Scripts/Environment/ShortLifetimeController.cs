using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

using System;
using System.Linq;

public class ShortLifetimeController : MonoBehaviour
{


    public float Duration = 10.0f;// length of time to exist
    private float DurationLeft;// time left before gone



    //Collider collider;
    public void Start()
    {
        DurationLeft = Duration;
        

    }

    public void Update()
    {
        // animate fire
        
        if(DurationLeft <= 0.0f){
            Destroy(this.gameObject);
        }else{
            DurationLeft -= Time.deltaTime;
        }

        // if duration left < .1, then start asking daylight for time every update to tell when to go out
    }

    




}