using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FaceCamera : MonoBehaviour {


    private void LateUpdate()
     {
         transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
     }
}
