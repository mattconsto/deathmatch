using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
	public float force = 1;

	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Player" || col.gameObject.tag == "Target") {
			col.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * force);
			GetComponent<AudioSource>().Play();
		}
	}
}
