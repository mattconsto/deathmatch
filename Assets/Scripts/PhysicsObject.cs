using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {
	public Vector3 COG = new Vector3(0,0,0);
	public float G = 9.8f;

	private Rigidbody _body;

	void Start () {
		_body = gameObject.GetComponent<Rigidbody>();
	}

	void Update () {
		_body.AddForce((COG - transform.position).normalized * G);
	}
}
