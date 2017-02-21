using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public Vector3 gravityOrigin;
	public float gravityForce;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 50.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 50.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

		rb.AddForce((gravityOrigin - transform.position).normalized * gravityForce);

		transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - gravityOrigin) * transform.rotation;
	}
}
