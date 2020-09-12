// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class Torch : MonoBehaviour
{

    //Parent of the potion
    private CharacterController Character;
    private ItemController BaseItem;
    public GameObject TorchFlame;
    private FlameController TorchFlameController;
    private bool CheckStillHeld = false;

    // which action to trigger on
    private float ActionToFunctionOn = 1.0f;

    void Start()
    {
        BaseItem = this.gameObject.GetComponent<ItemController>();
        // torches are always lit by default
        TorchFlameController = TorchFlame.GetComponent<FlameController>();
        TorchFlameController.SetLitStatus(true);
        TorchFlameController.SetTimeLeft(5);
    }

    public void SetCharacter(CharacterController CurrentCharacter)
    {
        Character = CurrentCharacter;
    }

    public void Update()
    {
        if (BaseItem.CanDoAction == ActionToFunctionOn)
        {
            BaseItem.CanDoAction = 0.0f;
            SetCharacter(BaseItem.ActionTargetCharacterController);
            BaseItem.SetActionTargetCharacterController(null);

            DoHit(BaseItem.GetDamage());
        }
    }
    public void DoHit(float Damage)
    {
        Character.AddValueToHealth(-1.0f*Damage);
    }


    // todo GENERATE fire color from holding charatcer controlelrs factions
    // normal color = lamploghter (standard time)
    // green = tech (1.25 time)
    // blue = magic (2x time)
    // purple = ??? somehting else? (infinte maybe?)

}