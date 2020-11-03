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

    private bool HasDoneWork = false;


    private Transform BuildingTransform;
    //private Transform MerchantSpot = null;

    private CharacterController OwnerControllerIfPresent = null;

    private List<CharacterController> CharactersWhoInteract = new List<CharacterController>();


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
                AssignOwnership(EnteringCharacterController);
                //AssignHousingOwnership(EnteringCharacterController);
            }
        }
        else if (Type == "SHOP")
        {
            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {

                EnteringCharacterController.SetIsInShop(true, this);

                if (EnteringCharacterController.GetUUID() == GetOwner())
                {
                    OwnerControllerIfPresent = EnteringCharacterController;
                }

                if (!EnteringCharacterController.GetIsPlayer())
                {
                    string EnteringCharactersTask = EnteringCharacterController.GetCurrentTask();

                    if (EnteringCharactersTask == "SHOP")
                    {
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
                AssignOwnership(EnteringCharacterController);
                //AssignHousingOwnership(EnteringCharacterController);
            }
        }
        else if (Type == "SHOP")
        {
            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                if (EnteringCharacterController.GetUUID() == GetOwner())
                {
                    OwnerControllerIfPresent = null;
                }

                EnteringCharacterController.SetIsInShop(false, null);
            }
        }



    }


    public void AssignOwnership(CharacterController EnteringCharacter)
    {
        // assign ownership, if already has owner, and is allowed shared types, assign allocation

        bool hasOwner = (ownerUUID != "");
        //bool hasHouse = (EnteringCharacter.GetHouseUUID() != "");
        bool hasOne = (EnteringCharacter.GetBuildingUUIDOfType(this.Type) != "");
        // if unowned and chaacter has no house, claim both
        if (!hasOwner && !hasOne)
        {
            SetOwner(EnteringCharacter.GetUUID());
            AssociateCharacterAndBuilding(EnteringCharacter);

        }
        // if has owner but charatcer doesnt, assign to the new owner
        else if (!hasOne)
        {
            AssociateCharacterAndBuilding(EnteringCharacter);

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

    public float GetWanderRange()
    {

        if (Type == "FARM")
        {

            // get sphere radius and set that to the wander range
            SphereCollider myCollider;
            myCollider = GetComponent<SphereCollider>();

            if (myCollider != null)
            {
                return myCollider.radius;
            }
        }


        return 1.0f;
    }


    public CharacterController GetOwnerControllerIfPresent()
    {
        return OwnerControllerIfPresent;
    }

    //public Transform GetMerchantSpot(){
    //    return MerchantSpot;
    //}

    // TODO implenment resources
    public void GetResources()
    {
        // todo return what resources to produce based on type
    }

    public void SetHasDoneWork(bool newStatus)
    {
        HasDoneWork = newStatus;
    }

    public bool GetHasDoneWork()
    {
        return HasDoneWork;
    }

    public void SelfDestruct()
    {
        // for all in people to notify, rm building by uuid
        foreach (CharacterController character in CharactersWhoInteract)
        {
            character.RemoveBuildingFromListByUUID(UUID);
        }
        
        Destroy(this.gameObject);


    }


    private void AssociateCharacterAndBuilding(CharacterController CharatcerToAssociate)
    {
        CharactersWhoInteract.Add(CharatcerToAssociate);
        CharatcerToAssociate.AddBuildingToList(this);
    }

    // TODO a destroy function to tell all ascociated that its gone

}