using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool Paused = false;

    private Canvas MenuCanvas;

    void Start()
    {
        MenuCanvas = this.gameObject.GetComponent<Canvas>();

        MenuCanvas.enabled = false;
    }

    void Update()
    {
        // toggle pause on esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                Paused = false;
            }
            else
            {
                Paused = true;
            }

            Debug.Log("paused" + Paused.ToString());

            MenuCanvas.enabled = Paused;

            // if its now paused
            if (Paused)
            {
                Debug.Log("showing menu");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;

                // turn off build mode if was active
                CameraFollow cameraControl = FindObjectOfType<CameraFollow>();
                Debug.Log("camera" + cameraControl);
                if (cameraControl.GetInBuildMode())
                {
                    cameraControl.ExitBuildMode();
                }

                // pause mode for camera too
                cameraControl.SetIsPaused(true);
            }
            else
            {
                Debug.Log("unshowing menu");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;

                CameraFollow cameraControl = FindObjectOfType<CameraFollow>();
                cameraControl.SetIsPaused(false);

            }
        }
    }
}
