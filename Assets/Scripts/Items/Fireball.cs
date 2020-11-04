// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{

    private CharacterController CasterController;
    public GameObject FlamePrefab;
    private GameObject FlameObject;


    private float damage;
    private void OnTriggerEnter(Collider EnteringCharacter)
    {
        Debug.Log("fireball hit" + EnteringCharacter);
        CharacterController HitController = EnteringCharacter.GetComponent<CharacterController>();
        if (HitController != null)
        {

            if (HitController.GetUUID() != CasterController.GetUUID() && HitController.GetSquadLeaderUUID() != CasterController.GetSquadLeaderUUID())
            {
                // do hit on character
                HitController.SetTarget(CasterController.gameObject);
                CasterController.SetTarget(HitController.gameObject);
                HitController.SetFighting(true);
                CasterController.SetFighting(true);

                HitController.AddValueToHealth(-1.0f * damage);


                // set them on fire
                FlameObject = Instantiate(FlamePrefab, HitController.GetCharacterTransform().position, Quaternion.identity);
                FlameObject.transform.parent = HitController.GetCharacterTransform();
                TempFlame flameController = FlameObject.GetComponent<TempFlame>();
                flameController.SetTargetController(HitController);



                // destroy self on collision
                Destroy(this.gameObject);
            }
        }
        else
        {

            // destroy self on collision
            //Destroy(this.gameObject);
        }




    }


    public void SetCasterController(CharacterController caster)
    {
        CasterController = caster;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

}