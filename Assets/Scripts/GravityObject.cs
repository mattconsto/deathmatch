using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour {
	public Vector3 origin = new Vector3(0,0,0);
	public float   force  = 9.81f;

	private Rigidbody _body;

	void Start () {
		_body = gameObject.GetComponent<Rigidbody>();
	}

	void Update () {
		_body.AddForce((origin - transform.position).normalized * force);
	}
}
