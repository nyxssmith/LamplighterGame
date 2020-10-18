using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class DaylightCycle : MonoBehaviour
{
    private Transform CenterOfRotation;

    public float RotationDegree = 0.0f;

    private bool UpdatedFlamesToday = true;
    private bool WasBedtime = false;

    private float SecondsItTakesForTwentyFourHoursToGoBy = 600.0f;
    private float DegreesPerTimeIncrement = 0.1f;
    private float SecondsBetweenTimeIncrements = 0.166666f;


    private float SecondsSinceLastTimeIncrement = 0.0f;

    void Start()
    {
        CenterOfRotation = gameObject.GetComponent<Transform>();
        RotationDegree += 90.0f;// start at noon


    }

    public void Update()
    {
        if (RotationDegree >= 360.0f)
        {
            RotationDegree = 0.0f;
            UpdatedFlamesToday = false;
            WasBedtime = false;

        }
        else if (RotationDegree <= 0.0f)
        {
            RotationDegree = 360.0f;
        }

        CenterOfRotation.localEulerAngles = new Vector3(RotationDegree, 0.0f, 0.0f);

        /*
        // Temp disable rewind time
        if (Input.GetKey ("[")) {
            RotationDegree -= 0.5f;
        }
        */

        if (Input.GetKey("]"))
        {
            RotationDegree += 10.5f;
            //RotationDegree += 0.5f;
        }

        if (!UpdatedFlamesToday && RotationDegree > 10.0f)
        {

            UpdateAllFlames();
            //Debug.Log(UpdatedFlamesToday);// tell to wake up
            TellAllCharactersToWakeUp();

        }
        else if (!WasBedtime && RotationDegree > 170.0f)
        {
            TellAllCharactersToSleep();
        }

        // do the standard passage of time
        DoPassageOfTime();


    }

    public float GetTime()
    {
        return RotationDegree;
    }

    private void DoPassageOfTime()
    {

        if (SecondsSinceLastTimeIncrement > SecondsBetweenTimeIncrements)
        {
            RotationDegree += DegreesPerTimeIncrement;

            if (RotationDegree >= 360.0f)
            {
                RotationDegree = 0.0f;
                UpdatedFlamesToday = false;
                WasBedtime = false;

            }
            else if (RotationDegree <= 0.0f)
            {
                RotationDegree = 360.0f;
            }

            SecondsSinceLastTimeIncrement = 0.0f;
        }
        else
        {
            SecondsSinceLastTimeIncrement += Time.deltaTime;
        }


    }


    private void UpdateAllFlames()
    {
        if (!UpdatedFlamesToday)
        {
            // find all flames in world and tick them down
            //Debug.Log("upating all flames");

            var flameControllersList = FindObjectsOfType<FlameController>();
            int DurationLeft;
            foreach (FlameController controller in flameControllersList)
            {
                //Debug.Log(controller.GetTimeLeft());


                DurationLeft = controller.GetTimeLeft();
                if (DurationLeft >= 1)
                {
                    controller.SetTimeLeft(DurationLeft - 1);
                }
            }
            UpdatedFlamesToday = true;
        }
    }

    private void TellAllCharactersToSleep()
    {
        //Debug.Log("go to sleep");

        // set all current task to sleep

        var CharacterControllerList = FindObjectsOfType<CharacterController>();
        foreach (CharacterController controller in CharacterControllerList)
        {
            string CurrentTask = controller.GetCurrentTask();
            string DefaultTask = controller.GetDefaultTask();
            // lamplighters and those followng the player are busy at night
            bool isBusy = (DefaultTask == "LAMPLIGHT" || controller.GetIsFollowingPlayer() || controller.GetIsPlayer());

            if (!isBusy)
            {// tell to go to sleep
                controller.MakeSpeechBubble("im going to bed");
                controller.OverrideNextTaskAndPushbackNextTask("SLEEP");
            }
        }

        WasBedtime = true;
    }

    private void TellAllCharactersToWakeUp()
    {
        // get all characters again, then if their task is sleep, increment task
        //Debug.Log("wake up!");
        var CharacterControllerList = FindObjectsOfType<CharacterController>();
        foreach (CharacterController controller in CharacterControllerList)
        {
            controller.WakeUp();
        }
    }

}