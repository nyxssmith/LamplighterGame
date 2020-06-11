
using UnityEngine;

public class playerCollision : MonoBehaviour {


    public playerMove movement;

    private void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.collider.name == "obstacle")
        {
            Debug.Log("We Hit Obstacle.....");

            FindObjectOfType<GameManager>().endGame();
           
             movement.enabled = false;
             
         }

    }
}
