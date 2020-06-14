// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class Potion : MonoBehaviour {

    //Parent of the potion
    private CharacterController Character;
    private bool CheckStillHeld = false;

    void Start () { }

    public void SetCharacter(CharacterController CurrentCharacter){
        Character = CurrentCharacter;
    }

    public void Update () {
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

    public void Summon () {
        //Instantiate (PreFab, SummonPositon, Quaternion.identity);

    }
}