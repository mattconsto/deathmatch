using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRotator : MonoBehaviour {
	public Vector3 origin    = new Vector3(0, 0, 0);
	public float   magnitude = 1f;
	public Vector3 direction = Vector3.up;

	void Update () {
		/* Rotate the Game Object, keeping it pointed at origin */
		transform.RotateAround(origin, direction, Time.deltaTime * magnitude);
		transform.LookAt(origin, direction);
	}
}
