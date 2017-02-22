using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour {
	public Vector3 origin = new Vector3(0, 0, 0);

	// Use this for initialization
	void Start () {
		Update();
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(origin, transform.up, Time.deltaTime);
		transform.LookAt(origin, transform.up);
	}
}
