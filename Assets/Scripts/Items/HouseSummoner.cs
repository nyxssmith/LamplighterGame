// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class HouseSummoner : MonoBehaviour
{

    //Parent of the potion
    public GameObject HousePreFab;
    private GameObject GhostImage;
    public Material GhostMaterialAllowedToPlace;
    public Material GhostMaterialNotAllowedToPlace;
    private Material OrigMaterial;
    private GameObject HousePreFabGhost;

    private CharacterController Character;
    private ItemController BaseItem;
    private bool CheckStillHeld = false;

    private bool isShowingGhost = false;
    // which action to trigger on
    private float ActionToFunctionOn = 1.0f;

    private bool canPlace;

    void Start()
    {
        BaseItem = this.gameObject.GetComponent<ItemController>();
        // torches are always lit by default
    }

    public void SetCharacter(CharacterController CurrentCharacter)
    {
        Character = CurrentCharacter;
    }

    public void Update()
    {


        if (BaseItem.CanDoAction == ActionToFunctionOn)
        {
            Debug.Log("doing summon house");
            BaseItem.CanDoAction = 0.0f;
            SetCharacter(BaseItem.ActionTargetCharacterController);
            BaseItem.SetActionTargetCharacterController(null);

            SummonHouse();

        }


        bool isInHand = BaseItem.GetItem().heldLocation == "Hand" && BaseItem.GetIsPickedUp();
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

    private void SummonHouse()
    {
        if (canPlace)
        {
            Debug.Log("summoning house");
            GameObject house = Instantiate(HousePreFab, Character.GetCharacterTransform().position + Character.GetCharacterTransform().forward * 10.0f, Quaternion.identity);

            house.transform.forward = new Vector3(Character.GetCharacterTransform().forward.x, Character.GetCharacterTransform().forward.y, Character.GetCharacterTransform().forward.z);
            Vector3 rot = house.transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, rot.y + 180, rot.z);
            house.transform.rotation = Quaternion.Euler(rot);
        }

    }


    private void ShowGhostImage()
    {
        GhostImage = Instantiate(HousePreFab, Character.GetCharacterTransform().position + Character.GetCharacterTransform().forward * 10.0f, Quaternion.identity);
        //set prefab stuff to ghost mode
        GhostImage.GetComponent<MeshRenderer>().material = GhostMaterialNotAllowedToPlace;
        GhostImage.GetComponent<BuildingController>().enabled = false;


        isShowingGhost = true;
    }

    private void MoveGhostImage()
    {
        GhostImage.transform.forward = new Vector3(Character.GetCharacterTransform().forward.x, Character.GetCharacterTransform().forward.y, Character.GetCharacterTransform().forward.z);
        Vector3 rot = GhostImage.transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        GhostImage.transform.rotation = Quaternion.Euler(rot);
        GhostImage.transform.position = Character.GetCharacterTransform().position + Character.GetCharacterTransform().forward * 10.0f;

        // check if in town etc, so find town controller and ask if allowed to place prefab
        canPlace = false;
        //get town controller, if null then not allowed
        // if in town do a "get can place" from town

        //check within 5m of center to be in a towm
        Collider[] hitColliders = Physics.OverlapSphere(GhostImage.transform.position, 1.0f);
        foreach (var hitCollider in hitColliders)
        {
            TownController controller = hitCollider.gameObject.GetComponent<TownController>();

            if (controller != null)
            {
                canPlace = controller.GetAllowedToPlaceBuilding(GhostImage.GetComponent<BuildingController>());
            }
        }
        if (canPlace)
        {
            GhostImage.GetComponent<MeshRenderer>().material = GhostMaterialAllowedToPlace;

        }
        else
        {
            GhostImage.GetComponent<MeshRenderer>().material = GhostMaterialNotAllowedToPlace;

        }

    }

    private void HideGhostImage()
    {
        Destroy(GhostImage);

        isShowingGhost = false;
    }




}