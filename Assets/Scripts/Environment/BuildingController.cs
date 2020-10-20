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
    //private Transform MerchantSpot = null;

    private CharacterController OwnerControllerIfPresent = null;

    public void Start()
    {
        BuildingTransform = gameObject.GetComponent<Transform>();

        if (UUID == "")
        {
            UUID = Guid.NewGuid().ToString();
        }

        //if(Type=="SHOP"){
        //    MerchantSpot = BuildingTransform.transform.Find("MerchantSpot");
        //    Debug.Log("found merchant spot"+MerchantSpot);
        //}


    }

    public void Update()
    {


    }


    private void OnTriggerEnter(Collider EnteringCharacter)
    {

        if (Type == "HOME")
        {

            //Debug.Log("enter" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                AssignHousingOwnership(EnteringCharacterController);
            }
        }
        else if (Type == "SHOP")
        {
            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {

                EnteringCharacterController.SetIsInShop(true,this);

                if(EnteringCharacterController.GetUUID() == GetOwner()){
                    OwnerControllerIfPresent = EnteringCharacterController;
                }

                if(!EnteringCharacterController.GetIsPlayer()){
                string EnteringCharactersTask = EnteringCharacterController.GetCurrentTask();

                    if(EnteringCharactersTask == "SHOP"){
                        EnteringCharacterController.MakeSpeechBubble("im shopping");
                    }// maybe else for items
                }
            }
        }


    }

    private void OnTriggerExit(Collider EnteringCharacter)
    {

        if (Type == "HOME")
        {
            //Debug.Log("exit" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                AssignHousingOwnership(EnteringCharacterController);
            }
        }else if (Type == "SHOP")
        {
            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                if(EnteringCharacterController.GetUUID() == GetOwner()){
                    OwnerControllerIfPresent = null;
                }

                EnteringCharacterController.SetIsInShop(false,null);
            }
        }



    }

    public void AssignHousingOwnership(CharacterController EnteringCharacter)
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

    public void AssignFarmingOwnership(CharacterController EnteringCharacter)
    {
        bool hasOwner = (ownerUUID != "");
        bool hasFarm = (EnteringCharacter.GetFarmUUID() != "");
        // if unowned and chaacter has no house, claim both
        if (!hasOwner && !hasFarm)
        {
            SetOwner(EnteringCharacter.GetUUID());
            EnteringCharacter.AddBuildingToList(this);

        }
        // if has owner but charatcer doesnt, assign to the new owner
        else if (!hasFarm)
        {
            EnteringCharacter.AddBuildingToList(this);
        }
    }


    public void AssignShopOwnership(CharacterController EnteringCharacter)
    {
        bool hasOwner = (ownerUUID != "");
        bool hasFarm = (EnteringCharacter.GetFarmUUID() != "");
        // if unowned and chaacter has no house, claim both
        if (!hasOwner && !hasFarm)
        {
            SetOwner(EnteringCharacter.GetUUID());
            EnteringCharacter.AddBuildingToList(this);

        }
        // if has owner but charatcer doesnt, assign to the new owner
        else if (!hasFarm)
        {
            EnteringCharacter.AddBuildingToList(this);
        }
    }

    public void AssignShopAllocation(CharacterController EnteringCharacter)
    {
        bool hasOwner = (ownerUUID != "");
        bool hasFarm = (EnteringCharacter.GetFarmUUID() != "");
        // if unowned and chaacter has no house, claim both
        if (!hasOwner && !hasFarm)
        {
            SetOwner(EnteringCharacter.GetUUID());
            EnteringCharacter.AddBuildingToList(this);

        }
        // if has owner but charatcer doesnt, assign to the new owner
        else if (!hasFarm)
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

    public float GetFarmWanderRange()
    {

        // get sphere radius and set that to the wander range
        SphereCollider myCollider;
        myCollider = GetComponent<SphereCollider>();

        if (myCollider != null)
        {
            return myCollider.radius;
        }


        return 1.0f;
    }

    public CharacterController GetOwnerControllerIfPresent(){
        return OwnerControllerIfPresent;
    }

    //public Transform GetMerchantSpot(){
    //    return MerchantSpot;
    //}



}