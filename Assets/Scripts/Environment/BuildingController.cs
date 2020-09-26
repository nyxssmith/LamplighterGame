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


    private string ownerUUID = "";
    private string UUID = "";
    public string Type = "";// HOME SHOP FARM

    private Transform BuildingTransform;


    public void Start()
    {
        BuildingTransform = gameObject.GetComponent<Transform>();

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

        //Debug.Log("enter" + EnteringCharacter);

        CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
        if (EnteringCharacterController != null)
        {
            AssignHousingAndOwnership(EnteringCharacterController);
        }


    }

    private void OnTriggerExit(Collider EnteringCharacter)
    {

        //Debug.Log("exit" + EnteringCharacter);

        CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
        if (EnteringCharacterController != null)
        {
            AssignHousingAndOwnership(EnteringCharacterController);
        }


    }

    public void AssignHousingAndOwnership(CharacterController EnteringCharacter)
    {
        bool hasOwner = (ownerUUID != "");
        bool hasHouse = (EnteringCharacter.GetHouseUUID() != "");
        // if unowned and chaacter has no house, claim both
        if (!hasOwner && !hasHouse)
        {
            SetOwner(EnteringCharacter.GetUUID());
            EnteringCharacter.AddBuildingToList(this);

        }
        // if has owner but charatcer doesnt, assign to the new owner
        else if (!hasHouse)
        {
            EnteringCharacter.AddBuildingToList(this);
        }
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

    public string GetUUID()
    {
        return UUID;
    }

    public Transform GetTransform()
    {
        return BuildingTransform;
    }

}