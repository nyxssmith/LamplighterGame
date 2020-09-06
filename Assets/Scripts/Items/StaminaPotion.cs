// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class StaminaPotion : MonoBehaviour {

    //Parent of the potion
    private CharacterController Character;
    private ItemController BaseItem;
    private bool CheckStillHeld = false;

    // which action to trigger on
    private float ActionToFunctionOn = 1.0f;

    void Start () { 
        BaseItem = this.gameObject.GetComponent<ItemController>();
    }

    public void SetCharacter(CharacterController CurrentCharacter){
        Character = CurrentCharacter;
    }

    public void Update () {
        if(BaseItem.CanDoAction == ActionToFunctionOn){

            BaseItem.CanDoAction = 0.0f;
            SetCharacter(BaseItem.ActionTargetCharacterController);
            BaseItem.SetActionTargetCharacterController(null);
            Drink(BaseItem.GetDamage());
        }
        // Check still held if shoudl deo effect
        /*
        if (CheckStillHeld){
            Character = this.transform.parent.gameObject.GetComponent<CharacterController> ();
        
            CheckStillHeld = false;
            Character = null;
        }
        */
        }

    //TOOD do throw as a possibilty and effect all things nearby for cooldown duration

    // 
    public void Drink (float EffectAmount) {
        Character.AddValueToStamina(EffectAmount);
    }

}