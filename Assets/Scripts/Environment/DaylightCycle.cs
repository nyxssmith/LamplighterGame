using System.Collections;
using UnityEngine;

public class DaylightCycle : MonoBehaviour {
    private Transform CenterOfRotation;
    private float TargetDegreePrev = -1f;
    private float TargetDegreeNext = 1f;

    void Start () {
        CenterOfRotation = gameObject.GetComponent<Transform> ();

    }

    public void Update () {
        if (Input.GetKey ("[")) {
            Debug.Log ("prev time");
            //CenterOfRotation.Rotate (90.0f, 0.0f, 0.0f, Space.World);


            CenterOfRotation.Rotate (TargetDegreePrev, 0.0f, 0.0f, Space.World);

            TargetDegreePrev -= 0.001f;

        }
        if (Input.GetKey ("]")) {
            Debug.Log ("more time");
            CenterOfRotation.Rotate (TargetDegreeNext, 0.0f, 0.0f, Space.World);
            TargetDegreeNext += 0.001f;

        }
    }

}