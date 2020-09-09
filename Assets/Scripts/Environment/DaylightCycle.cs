using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class DaylightCycle : MonoBehaviour
{
    private Transform CenterOfRotation;

    public float RotationDegree = 0.0f;

    private bool UpdatedFlamesToday = true;

    void Start()
    {
        CenterOfRotation = gameObject.GetComponent<Transform>();
        RotationDegree += 220.0f;

    }

    public void Update()
    {
        if (RotationDegree >= 360.0f)
        {
            RotationDegree = 0.0f;
            UpdatedFlamesToday = false;

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
            Debug.Log(UpdatedFlamesToday);
        }



    }

    public float GetTime()
    {
        return RotationDegree;
    }



    private void UpdateAllFlames()
    {
        if (!UpdatedFlamesToday)
        {
            // find all flames in world and tick them down
            Debug.Log("upating all flames");

            var flameControllersList = FindObjectsOfType<FlameController>();
            int DurationLeft;
            foreach (FlameController controller in flameControllersList)
            {
                Debug.Log(controller.GetTimeLeft());


                DurationLeft = controller.GetTimeLeft();
                if (DurationLeft >= 1)
                {
                    controller.SetTimeLeft(DurationLeft - 1);
                }
            }
            UpdatedFlamesToday = true;
        }
    }



}