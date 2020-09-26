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

    private bool RoofEnabled = true;

    private string ownerUUID = "";
    private string UUID = "";
    public string Type = "";// HOME SHOP FARM

    private Transform BuildintTransform;


    public void Start()
    {
        BuildintTransform = gameObject.GetComponent<Transform>();

        if (UUID == "")
        {
            UUID = Guid.NewGuid().ToString();
        }
        

    }

    public void Update()
    {


    }


    private void OnTriggerEnter(Collider EnteringCharacter)
    {
        if (RoofEnabled)
        {
            //Debug.Log("enter" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                if (EnteringCharacterController.GetIsPlayer())
                {
                    DisableRoof();
                }
            }

        }
    }

    private void OnTriggerExit(Collider EnteringCharacter)
    {
        if (!RoofEnabled)
        {
            //Debug.Log("exit" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                if (EnteringCharacterController.GetIsPlayer())
                {
                    EndableRoof();
                }
            }

        }
    }

    private void DisableRoof()
    {
        Debug.Log("disable roof");
        Roof.SetActive(false);
        RoofEnabled = false;
        //Roof.enabled = false;
    }
    private void EndableRoof()
    {
        Debug.Log("enable roof");
        Roof.SetActive(true);
        RoofEnabled = true;

        //Roof.enabled = true;
    }

    public string GetType()
    {
        return Type;
    }

    public string GetOwner()
    {
        return ownerUUID;
    }

    public void SetOwner(string newOwnerUUID)
    {
        ownerUUID = newOwnerUUID;
    }


}