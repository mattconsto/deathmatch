using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Pull object towards origin
*/
public class GravityObject : MonoBehaviour {

	// Public members
	public Vector3 origin = new Vector3(0,0,0);
	public float force = 9.81f;
	public bool align_to_origin = true;

	// Private members
	private Rigidbody _rb;

	void Start () {
		_rb = gameObject.GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		_rb.AddForce((origin - transform.position).normalized * force, ForceMode.Acceleration);
		if(align_to_origin) {
			transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - GetComponent<GravityObject>().origin) * transform.rotation;
		}
	}
}
