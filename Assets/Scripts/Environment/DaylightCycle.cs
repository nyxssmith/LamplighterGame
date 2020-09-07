using System.Collections;
using UnityEngine;

public class DaylightCycle : MonoBehaviour {
    private Transform CenterOfRotation;

    public float RotationDegree = 0.0f;

    void Start () {
        CenterOfRotation = gameObject.GetComponent<Transform> ();
        RotationDegree += 220.0f;

    }

    public void Update () {
        if(RotationDegree >= 360.0f){
            RotationDegree = 0.0f;
        }else if(RotationDegree <= 0.0f){
            RotationDegree = 360.0f;
        }

        CenterOfRotation.localEulerAngles = new Vector3(RotationDegree,0.0f,0.0f);

        if (Input.GetKey ("[")) {
            RotationDegree -= 0.5f;
        }
        if (Input.GetKey ("]")) {
            RotationDegree += 0.5f;
        }

    }

    public float GetTime(){
        return RotationDegree;
    }


}