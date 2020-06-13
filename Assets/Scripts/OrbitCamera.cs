using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {
	[SerializeField] private Transform target;		// Serialized reference to the object to oribt around

	private float rotSpeed = 1.5f;

	private float _rotY;
	private Vector3 _offset;

	void Start() {
		_rotY = transform.eulerAngles.y;
		_offset = target.position - transform.position;		// Store the starting position offset between the camera and the target
	}

	void LateUpdate() {
		float horInput = Input.GetAxis ("Horizontal");
		if (horInput != 0) {
			_rotY += horInput * rotSpeed;							// Either rotate the camera slowly using arrow keys ...
		} else {
			_rotY += Input.GetAxis ("Mouse X") * rotSpeed * 3;	// ... or rotate quickly with the mouse.
		}

		Quaternion rotation = Quaternion.Euler(0, _rotY, 0);
		transform.position = target.position - (rotation * _offset);	// Maintain the starting offset, shifted according to the camera's rotation
		transform.LookAt(target);										// No matter where the camera is relative to the target, always face the target
	}
}
