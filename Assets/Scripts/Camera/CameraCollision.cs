using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

	public float minDistance = 1.0f;
	public float maxDistance = 4.0f;
	public float maxDistanceLimit = 10.0f;
	public float smooth = 10.0f;
	Vector3 dollyDir;
	public Vector3 dollyDirAdjusted;
	public float distance;

	// Use this for initialization
	void Awake () {
		dollyDir = transform.localPosition.normalized;
		distance = transform.localPosition.magnitude;
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetAxis ("Mouse ScrollWheel") > 0f || Input.GetKey(",")) // forward
		{
			maxDistance -= 0.1f;
		} else if (Input.GetAxis ("Mouse ScrollWheel") < 0f  || Input.GetKey(".")) // backwards
		{
			if (maxDistance < maxDistanceLimit) {
				maxDistance += 0.1f;
			}
		}

		Vector3 desiredCameraPos = transform.parent.TransformPoint (dollyDir * maxDistance);
		RaycastHit hit;

		if (Physics.Linecast (transform.parent.position, desiredCameraPos, out hit)) {
			distance = Mathf.Clamp ((hit.distance * 0.87f), minDistance, maxDistance);

		} else {
			distance = maxDistance;
		}

		transform.localPosition = Vector3.Lerp (transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
	}
}