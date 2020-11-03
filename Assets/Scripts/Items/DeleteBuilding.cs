// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class DeleteBuilding : MonoBehaviour
{

    

    private CharacterController Character;
    private ItemController BaseItem;
    private bool CheckStillHeld = false;

    // which action to trigger on
    private float ActionToFunctionOn = 1.0f;

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
            Debug.Log("doing rm building");
            BaseItem.CanDoAction = 0.0f;
            SetCharacter(BaseItem.ActionTargetCharacterController);
            BaseItem.SetActionTargetCharacterController(null);


            DeleteObject(FindNearestBuilding());

        }
    }
    

    private GameObject FindNearestBuilding(){

        // 50.0f is range to look for house
        Collider[] hitColliders = Physics.OverlapSphere(Character.GetCharacterTransform().position, 10.0f);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider);
            BuildingController controller = hitCollider.gameObject.GetComponent<BuildingController>();
            if (controller != null)
            {
                return controller.gameObject;
            }
        }

        

        return null;
    }

    private void DeleteObject(GameObject toDelete){

        Debug.Log("deleteing");
        Destroy(toDelete);
    }

}