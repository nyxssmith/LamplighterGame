using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    private Transform CenterOfRotation;

    public float RotationDegree = 0.0f;

    private bool UpdatedFlamesToday = true;

    private bool WasBedtime = false;

    private float SecondsItTakesForTwentyFourHoursToGoBy = 600.0f;

    private float DegreesPerTimeIncrement = 0.1f;

    private float SecondsBetweenTimeIncrementsDay = 0.016666f;

    private float SecondsBetweenTimeIncrementsNight = 0.09666f;

    private float SecondsSinceLastTimeIncrement = 0.0f;

    private bool IsDay;

    void Start()
    {
        CenterOfRotation = gameObject.GetComponent<Transform>();
        RotationDegree += 90.0f; // start at noon
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

        if (RotationDegree > 1.0f && RotationDegree < 180.0f)
        {
            IsDay = true;
        }
        else
        {
            IsDay = false;
        }

        CenterOfRotation.localEulerAngles =
            new Vector3(RotationDegree, 0.0f, 0.0f);



        if (!UpdatedFlamesToday && RotationDegree > 10.0f)
        {
            UpdateAllFlames();

            //Debug.Log(UpdatedFlamesToday);// tell to wake up
            TellAllCharactersToWakeUp();
        }
        else if (!WasBedtime && RotationDegree > 170.0f)
        {
            TellAllCharactersToSleep();

            // count all resouces at end of day
            DoTownResourceUpdates();
        }

        // do the standard passage of time
        DoPassageOfTime();
    }

    private void DoTownResourceUpdates()
    {
        var TownControllerList = FindObjectsOfType<TownController>();
        foreach (TownController controller in TownControllerList)
        {
            controller.DoResourcesUpdate();
        }
    }

    public float GetTime()
    {
        return RotationDegree;
    }

    private void DoPassageOfTime()
    {
        // change time speed at day or night
        float SecondsBetweenTimeIncrements;
        if (IsDay)
        {
            SecondsBetweenTimeIncrements = SecondsBetweenTimeIncrementsDay;
        }
        else
        {
            SecondsBetweenTimeIncrements = SecondsBetweenTimeIncrementsNight;
        }

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
            bool isBusy =
                (
                DefaultTask == "LAMPLIGHT" ||
                controller.GetIsFollowingPlayer() ||
                controller.GetIsPlayer()
                );

            if (!isBusy)
            {
                // tell to go to sleep
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
