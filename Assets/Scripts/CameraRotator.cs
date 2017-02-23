using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour {
	public Vector3 origin = new Vector3(0, 0, 0);

	void Update () {
		/* Rotate the Camera, keeping it pointed at origin */
		transform.RotateAround(origin, transform.up, Time.deltaTime);
		transform.LookAt(origin, transform.up);
	}
}
