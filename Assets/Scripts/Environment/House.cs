using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

using System;
using System.Linq;

public class House : MonoBehaviour
{


    private string ownerUUID = "";
    private string UUID = "";

    private Transform BuildintTransform;
    //private Transform BuildintTransform;


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
        
            //Debug.Log("enter" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                if (EnteringCharacterController.GetIsPlayer())
                {
                }
            }

        
    }

    private void OnTriggerExit(Collider EnteringCharacter)
    {
        
            //Debug.Log("exit" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                if (EnteringCharacterController.GetIsPlayer())
                {
                }
            }

        
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