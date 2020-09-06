using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

using System;
using System.Linq;

public class BuildingController : MonoBehaviour
{

    public GameObject Roof;
    public GameObject FloorAndWalls;
    //public GameObject DetectionBox;


    public void Start()
    {


    }

    public void Update()
    {


    }


    private void OnTriggerEnter(Collider EnteringCharacter)
    {
        Debug.Log("enter"+EnteringCharacter);
        CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
        if (EnteringCharacterController != null)
        {
            if (EnteringCharacterController.GetIsPlayer())
            {
                DisableRoof();
            }
        }
    }

    private void OnTriggerExit(Collider EnteringCharacter)
    {
                Debug.Log("exit"+EnteringCharacter);
        CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
        if (EnteringCharacterController != null)
        {
            if (EnteringCharacterController.GetIsPlayer())
            {
                EndableRoof();
            }
        }
    }

    private void DisableRoof()
    {
        Debug.Log("disable roof");
        Roof.SetActive(false);
        //Roof.enabled = false;
    }
    private void EndableRoof()
    {
        Debug.Log("enable roof");
        Roof.SetActive(true);

        //Roof.enabled = true;
    }


}