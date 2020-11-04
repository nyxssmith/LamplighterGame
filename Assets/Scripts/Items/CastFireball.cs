// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class CastFireball : MonoBehaviour
{

    //Parent of the potion
    public GameObject FireballPreFab;

    private GameObject Fireball;
    private CharacterController Character;
    private ItemController BaseItem;
    private bool CheckStillHeld = false;


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
            Debug.Log("doing fireball cast");
            BaseItem.CanDoAction = 0.0f;
            SetCharacter(BaseItem.ActionTargetCharacterController);
            BaseItem.SetActionTargetCharacterController(null);

            Cast();

        }



    }


    public void Cast()
    {

        Vector3 summonPosition = BaseItem.GetItemTransform().position + Character.GetCharacterTransform().forward * 0.2f;
        summonPosition.y += 0.7f;
        Fireball = Instantiate(FireballPreFab, summonPosition, Quaternion.identity);

        Rigidbody rb = Fireball.GetComponent<Rigidbody>();
        rb.AddForce(Character.GetCharacterTransform().forward * 30.0f);

        Fireball fireballController = Fireball.GetComponent<Fireball>();
        fireballController.SetCasterController(BaseItem.GetHoldingCharacterController());
        fireballController.SetDamage(BaseItem.GetDamage());
    }



}