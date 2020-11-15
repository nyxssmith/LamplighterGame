// Instantiates 10 copies of Prefab each 2 units apart from each other
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTool : MonoBehaviour
{
    //list of prefabs to build
    private GameObject CurrentPrefab;

    public GameObject TargetBeacon;

    private GameObject GhostImage;

    public Material GhostMaterialAllowedToPlace;

    public Material GhostMaterialNotAllowedToPlace;

    public Material GhostMaterialDeleteTarget;

    private Material OrigMaterial;

    private GameObject HousePreFabGhost;

    private CharacterController Character;

    private ItemController BaseItem;

    private bool CheckStillHeld = false;

    private bool isShowingGhost = false;

    //[System.Serializable]
    public GameObject Prefab1;

    public GameObject Prefab2;

    public GameObject Prefab3;

    public GameObject Prefab4;

    public GameObject Prefab5;

    private List<GameObject> BuildObjects = new List<GameObject>();

    public int Index = 0;

    // which action to trigger on
    private float ActionToFunctionOn = 1.0f;

    private bool canPlace;

    private GameObject TargetedBuilding;

    private GameObject TargetBeaconObject;

    void Start()
    {
        BaseItem = this.gameObject.GetComponent<ItemController>();

        // torches are always lit by default
        FillListOfBuildObjects();
    }

    private void FillListOfBuildObjects()
    {
        // buildojects 0 is null
        // add each prefab to list
        BuildObjects.Add(null);
        BuildObjects.Add (Prefab1);
        BuildObjects.Add (Prefab2);
        BuildObjects.Add (Prefab3);
        BuildObjects.Add (Prefab4);
        BuildObjects.Add (Prefab5);
    }

    public void SetCharacter(CharacterController CurrentCharacter)
    {
        Character = CurrentCharacter;
    }

    public void Update()
    {
        if (Input.GetKeyDown("tab"))
        {
            Index += 1;
            if (Index >= BuildObjects.Count)
            {
                Index = 0;
            }
            HideGhostImage();
            DeTarget();
        }

        CurrentPrefab = BuildObjects[Index];

        if (BaseItem.CanDoAction == ActionToFunctionOn)
        {
            UnityEngine.Debug.Log("doing summon house");
            BaseItem.CanDoAction = 0.0f;

            //SetCharacter(BaseItem.ActionTargetCharacterController);
            //BaseItem.SetActionTargetCharacterController(null);
            if (Index > 0)
            {
                SummonHouse();
            }
            else
            {
                Debug.Log("deleteing");
                BuildingController controller =
                    TargetedBuilding.GetComponent<BuildingController>();
                if (controller != null)
                {
                    controller.SelfDestruct();
                }
                else
                {
                    // if not destroying buildng but a fence or something
                    Character
                        .MakeSpeechBubble("destroyingbuilding and not telling owner");
                    Destroy (TargetedBuilding);
                }
            }
        }

        bool isInHand =
            BaseItem.GetItem().heldLocation == "Hand" &&
            BaseItem.GetIsPickedUp();

        isInHand = true;

        // if nidex isnt 0
        if (Index != 0)
        {
            if (isInHand && !isShowingGhost && Character != null)
            {
                ShowGhostImage();
            }
            else if (!isInHand)
            {
                HideGhostImage();
            }

            if (isShowingGhost)
            {
                MoveGhostImage();
                // add an update ghost image to check if allowed to be placed to avoid overlap etc
            }
        }
        else
        {
            // if delete mode
            //Character.MakeSpeechBubble("deleting");
            GameObject nearestBuilding = FindNearestBuilding();
            if (nearestBuilding != null)
            {
                // put target beacon on building to be deleted
                SetTarget (nearestBuilding);
            }
        }
        // else if index is 0 do delete tool here
        // else if index is 0 do delete tool hereindex
    }

    public void SetTarget(GameObject TargetToSet)
    {
        DeTarget();

        OrigMaterial = TargetToSet.GetComponent<MeshRenderer>().material;
        TargetToSet.GetComponent<MeshRenderer>().material =
            GhostMaterialDeleteTarget;

        Transform TargetTransform = TargetToSet.GetComponent<Transform>();

        //float RandZ = Random.Range(-0.2f, 0.2f);
        //float RandX = Random.Range(-0.2f, 0.2f);
        //Vector3 SummonPositon = TargetTransform.position + new Vector3(0.0f, 2.0f, 0.0f);
        Vector3 SummonPositon =
            TargetTransform.position + new Vector3(0.0f, 5.0f, 0.0f);
        TargetBeaconObject =
            Instantiate(TargetBeacon, SummonPositon, Quaternion.identity);

        // set the color of target beacon
        SpriteRenderer[] SpriteRendersInTargetBeacon =
            TargetBeaconObject
                .gameObject
                .GetComponentsInChildren<SpriteRenderer>();
        SpriteRendersInTargetBeacon[0].color = Color.red;

        //Light[] LightsInTargetBeacon = TargetBeaconObject.gameObject.GetComponentsInChildren<Light>();
        //LightsInTargetBeacon[0].color = UIColor;
        TargetedBuilding = TargetToSet;

        //make the target beacon a child of its taret
        TargetBeaconObject.gameObject.GetComponent<Transform>().parent =
            TargetToSet.GetComponent<Transform>();
    }

    public void DeTarget()
    {
        if (TargetedBuilding != null)
        {
            TargetedBuilding.GetComponent<MeshRenderer>().material =
                OrigMaterial;
        }

        TargetedBuilding = null;
        if (TargetBeaconObject != null)
        {
            Destroy (TargetBeaconObject);
        }
    }

    private GameObject FindNearestBuilding()
    {
        BuildingController foundbuilding = null;
        float currentDistanceToBuilding = -1;
        Transform CharacterTransform = Character.GetCharacterTransform();

        // 100.0f is range to look for house
        Collider[] hitColliders =
            Physics.OverlapSphere(CharacterTransform.position, 100.0f);
        foreach (var hitCollider in hitColliders)
        {
            BuildingController controller =
                hitCollider.gameObject.GetComponent<BuildingController>();
            if (controller != null)
            {
                //if (controller.GetType() == buildingType)
                //{
                // compare distance and try to find closest
                float distanceToThisBuilding =
                    Vector3
                        .Distance(CharacterTransform.position,
                        controller.GetTransform().position);
                if (currentDistanceToBuilding == -1)
                {
                    foundbuilding = controller;
                    currentDistanceToBuilding = distanceToThisBuilding;
                }
                else
                {
                    //now comparing vs backup
                    bool isCloser =
                        currentDistanceToBuilding > distanceToThisBuilding;

                    // if its closer try it
                    if (isCloser)
                    {
                        foundbuilding = controller;

                        currentDistanceToBuilding = distanceToThisBuilding;
                    }
                }
                //}
            }
        }

        return foundbuilding.gameObject;
    }

    private void SummonHouse()
    {
        if (canPlace)
        {
            Debug.Log("summoning house");
            GameObject house =
                Instantiate(CurrentPrefab,
                Character.GetCharacterTransform().position +
                Character.GetCharacterTransform().forward * 10.0f,
                Quaternion.identity);

            house.transform.forward =
                new Vector3(Character.GetCharacterTransform().forward.x,
                    Character.GetCharacterTransform().forward.y,
                    Character.GetCharacterTransform().forward.z);
            Vector3 rot = house.transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, rot.y + 180, rot.z);
            house.transform.rotation = Quaternion.Euler(rot);
        }
    }

    private void ShowGhostImage()
    {
        GhostImage =
            Instantiate(CurrentPrefab,
            Character.GetCharacterTransform().position +
            Character.GetCharacterTransform().forward * 10.0f,
            Quaternion.identity);

        //set prefab stuff to ghost mode
        GhostImage.GetComponent<MeshRenderer>().material =
            GhostMaterialNotAllowedToPlace;
        GhostImage.GetComponent<BuildingController>().enabled = false;

        isShowingGhost = true;
    }

    private void MoveGhostImage()
    {
        GhostImage.transform.forward =
            new Vector3(Character.GetCharacterTransform().forward.x,
                Character.GetCharacterTransform().forward.y,
                Character.GetCharacterTransform().forward.z);
        Vector3 rot = GhostImage.transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        GhostImage.transform.rotation = Quaternion.Euler(rot);
        GhostImage.transform.position =
            Character.GetCharacterTransform().position +
            Character.GetCharacterTransform().forward * 10.0f;

        // check if in town etc, so find town controller and ask if allowed to place prefab
        canPlace = false;

        //get town controller, if null then not allowed
        // if in town do a "get can place" from town
        //check within 5m of center to be in a towm
        Collider[] hitColliders =
            Physics.OverlapSphere(GhostImage.transform.position, 1.0f);
        foreach (var hitCollider in hitColliders)
        {
            TownController controller =
                hitCollider.gameObject.GetComponent<TownController>();

            if (controller != null)
            {
                canPlace =
                    controller
                        .GetAllowedToPlaceBuilding(GhostImage
                            .GetComponent<BuildingController>());
            }
        }
        if (canPlace)
        {
            GhostImage.GetComponent<MeshRenderer>().material =
                GhostMaterialAllowedToPlace;
        }
        else
        {
            GhostImage.GetComponent<MeshRenderer>().material =
                GhostMaterialNotAllowedToPlace;
        }
    }

    public void HideGhostImage()
    {
        if (GhostImage != null)
        {
            Destroy (GhostImage);
        }

        isShowingGhost = false;
    }
}
