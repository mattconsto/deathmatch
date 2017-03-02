using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
	public float force = 1;

	void OnCollisionEnter(Collision col) {
		col.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * force);
		GetComponent<AudioSource>().Play();
	}
}
