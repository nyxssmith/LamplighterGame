using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    bool gamehasEnded = false;
    float restartDelay = 1f;

   public void endGame()
    {
        if(!gamehasEnded)
        {
            gamehasEnded = true;
            Debug.Log("Game Over");

            Invoke("restartGame", restartDelay);
        }
       
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
