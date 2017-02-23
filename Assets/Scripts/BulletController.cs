using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public float lifetime = Mathf.Infinity;
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;

		if(lifetime < -2) {
			Destroy(gameObject);
		} else {
			// Sink into the ground
			GetComponent<Rigidbody>().velocity /= 2;
			GetComponent<PhysicsObject>().G = 2;
			Destroy(GetComponent<Collider>());
		}
	}

	void OnCollisionEnter(Collision col) {
		Destroy(gameObject);
	}
}
