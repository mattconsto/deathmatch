using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRotator : MonoBehaviour {
	public Vector3 origin = new Vector3(0, 0, 0);

	void Update () {
		/* Rotate the Game Object, keeping it pointed at origin */
		transform.RotateAround(origin, transform.up, Time.deltaTime);
		transform.LookAt(origin, transform.up);
	}
}
