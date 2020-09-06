// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class BaseItemTemplate : MonoBehaviour {

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
            
            // ADD per item actions here
        }
        }
}