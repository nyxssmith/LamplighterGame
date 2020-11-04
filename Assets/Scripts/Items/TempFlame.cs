// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class TempFlame : MonoBehaviour
{

    private CharacterController TargetController;

    private float timeBetweenFireTicks = 1.0f;// time between fire ticks of damage
    private float damage = 1.0f;

    private float coolDown = 0.0f;

    void Update()
    {


        if (coolDown > 0.0f)
        {
            coolDown -= Time.deltaTime;
        }
        else
        {
            coolDown = timeBetweenFireTicks;
            TargetController.AddValueToHealth(-1.0f * damage);
        }
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);



    }


    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void SetTargetController(CharacterController target)
    {
        TargetController = target;
    }


}