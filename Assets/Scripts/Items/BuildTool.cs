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

    public GameObject Prefab6;

    public GameObject Prefab7;
    public GameObject Prefab8;

    private List<GameObject> BuildObjects = new List<GameObject>();

    public int Index = 0;

    // which action to trigger on
    private float ActionToFunctionOn = 1.0f;

    private bool canPlace;

    private string reasonCantPlace;

    private GameObject TargetedBuilding;

    private GameObject TargetBeaconObject;

    // building placement modifiers
    private float distanceFromCharater = 5.0f;

    private float maxDistanceFromCharater = 10.0f;

    private float minDistanceFromCharater = 2.5f;

    private float buildingRotation = 0.0f;

    private float minBuildingRotation = -90.0f;

    private float maxBuildingRotation = 90.0f;

    private float buildingTilt = 0.0f;

    private float maxBuildingTilt = 15.0f;

    private float minBuildingTilt = -15.0f;

    private float buildingHeight = 0.0f;

    private float maxBuildingHeight = 2.0f;

    private float minBuildingHeight = -2.0f;

    // overlap distance check
    private float overlapDistance;

    // TODO resource costs per material
    void Start()
    {
        BaseItem = this.gameObject.GetComponent<ItemController>();
        BaseItem.SetCanBeDropped(false);

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
        BuildObjects.Add (Prefab6);
        BuildObjects.Add (Prefab7);
        BuildObjects.Add (Prefab8);
    }

    public void SetCharacter(CharacterController CurrentCharacter)
    {
        Character = CurrentCharacter;
    }

    public void Update()
    {
        // cycle prefab to summon
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

        //move building closer and farther away
        if (Input.GetKeyDown("i"))
        {
            if (distanceFromCharater <= maxDistanceFromCharater)
            {
                distanceFromCharater += 0.5f;
            }
        }
        if (Input.GetKeyDown("k"))
        {
            if (distanceFromCharater >= minDistanceFromCharater)
            {
                distanceFromCharater -= 0.5f;
            }
        }

        //rotate building direction
        if (Input.GetKeyDown("j"))
        {
            if (buildingRotation <= maxBuildingRotation)
            {
                buildingRotation += 5.0f;
            }
        }
        if (Input.GetKeyDown("l"))
        {
            if (buildingRotation >= minBuildingRotation)
            {
                buildingRotation -= 5.0f;
            }
        }

        //tild building
        if (Input.GetKeyDown("u"))
        {
            if (buildingTilt <= maxBuildingTilt)
            {
                buildingTilt += 1.0f;
            }
        }
        if (Input.GetKeyDown("o"))
        {
            if (buildingTilt >= minBuildingTilt)
            {
                buildingTilt -= 1.0f;
            }
        }

        // change building hieght
        if (Input.GetKeyDown("n"))
        {
            if (buildingHeight >= minBuildingHeight)
            {
                buildingHeight -= 0.25f;
            }
        }
        if (Input.GetKeyDown("m"))
        {
            if (buildingHeight <= maxBuildingHeight)
            {
                buildingHeight += 0.25f;
            }
        }

        // pick which prefab
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
            Physics.OverlapSphere(CharacterTransform.position, 25.0f);
        foreach (var hitCollider in hitColliders)
        {
            BuildingController controller =
                hitCollider.gameObject.GetComponent<BuildingController>();
            if (controller != null)
            {
                // TODO readd the check that the town of the building is same as owned town
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
        if (foundbuilding != null)
        {
            return foundbuilding.gameObject;
        }
        else
        {
            return null;
        }
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

            // use same position as ghost image
            house.transform.forward =
                new Vector3(Character.GetCharacterTransform().forward.x,
                    Character.GetCharacterTransform().forward.y,
                    Character.GetCharacterTransform().forward.z);

            Vector3 rot = house.transform.rotation.eulerAngles;

            // rotate to face character
            rot =
                new Vector3(rot.x + buildingTilt,
                    rot.y + 180 + buildingRotation,
                    rot.z);

            Debug.Log("rot for building" + rot.ToString());
            house.transform.rotation = Quaternion.Euler(rot);

            // set position
            house.transform.position =
                Character.GetCharacterTransform().position +
                Character.GetCharacterTransform().forward *
                distanceFromCharater +
                Character.GetCharacterTransform().up * buildingHeight;
        }
    }

    private void ShowGhostImage()
    {
        GhostImage =
            Instantiate(CurrentPrefab,
            Character.GetCharacterTransform().position +
            Character.GetCharacterTransform().forward * distanceFromCharater,
            Quaternion.identity);

        //set prefab stuff to ghost mode
        //GhostImage.GetComponent<MeshRenderer>().material =
        //    GhostMaterialNotAllowedToPlace;
        ChangeMaterialOfGhostImage (GhostMaterialNotAllowedToPlace);

        string Type = GhostImage.GetComponent<BuildingController>().GetType();

        //Debug.Log (Type);
        if (Type == "HOME")
        {
            overlapDistance = 5.0f;
        }
        else if (Type == "SHOP")
        {
            overlapDistance = 5.0f;
        }
        else if (Type == "FARM")
        {
            overlapDistance = 10.0f;
        }
        else if (Type == "OBJECT")
        {
            // objects are allowed to overlap
            overlapDistance = 0.0f;
        }
        else
        {
            // TODO cover all sizes or make better rule for sizing
            overlapDistance = 1.0f;
        }

        GhostImage.GetComponent<BuildingController>().enabled = false;

        // disable all collision
        foreach (Collider c in GhostImage.GetComponents<Collider>())
        {
            c.enabled = false;
        }

        // disable all walls and navmesh holes, so ghost is just visual
        foreach (Transform child in GhostImage.transform)
        {
            child.gameObject.SetActive(false); // or false
        }

        isShowingGhost = true;
    }

    private void MoveGhostImage()
    {
        GhostImage.transform.forward =
            new Vector3(Character.GetCharacterTransform().forward.x,
                Character.GetCharacterTransform().forward.y,
                Character.GetCharacterTransform().forward.z);

        Vector3 rot = GhostImage.transform.rotation.eulerAngles;

        // rotate to face character
        rot =
            new Vector3(rot.x + buildingTilt,
                rot.y + 180 + buildingRotation,
                rot.z);

        //Debug.Log("rot for building" + rot.ToString());
        GhostImage.transform.rotation = Quaternion.Euler(rot);

        // set position
        GhostImage.transform.position =
            Character.GetCharacterTransform().position +
            Character.GetCharacterTransform().forward * distanceFromCharater +
            Character.GetCharacterTransform().up * buildingHeight;

        // check if in town etc, so find town controller and ask if allowed to place prefab
        canPlace = false;
        reasonCantPlace = "\n";

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
            else
            {
                reasonCantPlace = "Buildings can only be placed in a town\n";
            }
        }

        // DEBUG to allow place out of town
        // TODO RM THIS LATER
        canPlace = true;

        // TODO resource checks
        // Check that building is not too close to others
        //Debug.Log("overlapdist" + overlapDistance.ToString());
        if (overlapDistance > 0.0f)
        {
            // checks to make sure there are no building controllers in that range, if so, then can place
            hitColliders =
                Physics
                    .OverlapSphere(GhostImage.transform.position,
                    overlapDistance);
            foreach (var hitCollider in hitColliders)
            {
                BuildingController controller =
                    hitCollider.gameObject.GetComponent<BuildingController>();

                if (controller != null)
                {
                    // object type buildings do not count as overlap
                    string otherBuildingType = controller.GetType();

                    //Debug.Log("other type "+otherBuildingType);
                    if (otherBuildingType != "OBJECT")
                    {
                        canPlace = false;
                    }
                }
                else
                {
                    reasonCantPlace =
                        "Building of this type cannot be overlapping or too close to another building\n";
                }
            }
        }

        if (canPlace)
        {
            ChangeMaterialOfGhostImage (GhostMaterialAllowedToPlace);
            //GhostImage.GetComponent<MeshRenderer>().material =
            //    GhostMaterialAllowedToPlace;
        }
        else
        {
            ChangeMaterialOfGhostImage (GhostMaterialNotAllowedToPlace);
            //GhostImage.GetComponent<MeshRenderer>().material =
            //    GhostMaterialNotAllowedToPlace;
        }
    }

    private void ChangeMaterialOfGhostImage(Material newMaterial)
    {

        ChangeMaterialOfObject(GhostImage.gameObject,newMaterial);
        /*

        MeshRenderer ObjectMeshRenderer = GhostImage.GetComponent<MeshRenderer>();
        if(ObjectMeshRenderer != null){
            ObjectMeshRenderer.material = newMaterial;
        }else
        {
            // TODO fix this and make it so stuff shows up
            foreach (Transform child in GhostImage.transform)
            {

                



                MeshRenderer childMeshRenderer =
                    child.GetComponent<MeshRenderer>(); // = newMaterial;
                

                    Material[] materials = childMeshRenderer.materials;
                    Debug.Log(child.ToString() +" "+ materials.Length.ToString());

                // change child material
                childMeshRenderer.material = newMaterial;
                
            }
        }
        */
    }

    private void ChangeMaterialOfObject(GameObject ObjectToModify, Material newMaterial)
    {
        MeshRenderer ObjectMeshRenderer = ObjectToModify.GetComponent<MeshRenderer>();
        if(ObjectMeshRenderer != null){
            Debug.Log("found i can change material of "+ObjectToModify.ToString());
            ObjectMeshRenderer.material = newMaterial;
        }else
        {
            // TODO fix this and make it so stuff shows up
            foreach (Transform child in ObjectToModify.transform)
            {
                Debug.Log("recusivly changng material "+ObjectToModify.ToString()+" p:c " +child.gameObject.ToString());
                ChangeMaterialOfObject(child.gameObject,newMaterial);
                /*
                MeshRenderer childMeshRenderer =
                    child.GetComponent<MeshRenderer>(); // = newMaterial;
                

                    Material[] materials = childMeshRenderer.materials;
                    Debug.Log(child.ToString() +" "+ materials.Length.ToString());

                // change child material
                childMeshRenderer.material = newMaterial;
                */
                
            }
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

    public string GetResourcesCostString()
    {
        // TODO when swithcing prefabs, before disable the controller, set the costs vars from building controller
        string costsString = "stone: 0\nwood: 1\netc: 42";

        return costsString;
    }

    public bool GetCanPlace()
    {
        return canPlace;
    }

    public string GetReasonCantPlace()
    {
        return reasonCantPlace;
    }

    // get len of build list for info string
    public int GetLength()
    {
        return BuildObjects.Count;
    }

    //public void SetIndex(int newIndex){
    //    Index = newIndex;
    //}
}
