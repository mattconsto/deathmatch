using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

	public Vector3 COG = new Vector3(0,0,0);
	public float G = 9.8f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {
		rb.AddForce((COG - transform.position).normalized * G);
	}
}
