using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

using System;
using System.Linq;

public class FlameController : MonoBehaviour
{


    public ParticleSystem Flare;
    public ParticleSystem Fire;

    public Light FireLight;

    private Color FireColor;// color of the fire to make

    // duration 1 = 1 day
    // daylight cycle checks Nx a day all torches to enable or disable
    private int Duration = 1;// length of time to stay lit
    private int DurationLeft = 0;// time left lit // TODO make this set at time when lit as 360:1

    private bool isLit = false;



    //Collider collider;
    public void Start()
    {

        //collider = this.gameObject.GetComponent<collider>();
        if (DurationLeft == 0)
        {
            DurationLeft = Duration;
        }
        if (isLit)
        {
            Fire.Play();
            Flare.Play();
            FireLight.enabled = true;
        }
        else
        {
            Fire.Stop();
            Flare.Stop();
            FireLight.enabled =false;

        }


    }

    public void Update()
    {
        // animate fire


        // if duration left < .1, then start asking daylight for time every update to tell when to go out
    }

    void OnTriggerEnter(Collider collision)
    {
        // if is not lit
        if (!isLit)
        {
            FlameController HitFlameController = collision.gameObject.GetComponent<FlameController>();
            if (HitFlameController != null)
            {
                // if hit a lit flame, then light self
                if (HitFlameController.GetLitStatus())
                {
                    // light and copy the color and duration of what lit it and its color
                    SetLitStatus(true);
                    DurationLeft = HitFlameController.GetBaseDuration();
                    FireColor = HitFlameController.GetColor();
                }
            }
        }
        // TODO if is lit set shit on fire

    }

    public void SetLitStatus(bool newStatus)
    {

        // if status changed, update it
        if (isLit != newStatus)
        {
            FireLight.enabled = newStatus;

            if (newStatus)
            {

                Fire.Play();
                Flare.Play();
            }
            else
            {

                Fire.Stop();
                Flare.Stop();
            }
        }
        //TODO set the lit status
        isLit = newStatus;




    }
    public bool GetLitStatus()
    {
        return isLit;
    }


    public void SetTimeLeft(int newDurationLeft)
    {
        Debug.Log("setting time left to " + newDurationLeft);
        //TODO set the time left from when lit
        if (newDurationLeft == 0)
        {
            SetLitStatus(false);
        }
        DurationLeft = newDurationLeft;
    }

    public int GetTimeLeft()
    {
        return DurationLeft;
    }
    public int GetBaseDuration()
    {
        return Duration;
    }

    public Color GetColor()
    {
        return FireColor;
    }





}