using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleRotator : MonoBehaviour {
	public float sensitivity = 1f;

	void Update () {
		transform.RotateAround(transform.position, transform.up, Time.deltaTime * sensitivity);
	}
}
