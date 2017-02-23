using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public float lifetime = Mathf.Infinity;
	public GameObject decalPrefab = null;
	
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
		bool destroy = false;

		print("Collision");

		foreach (ContactPoint contact in col.contacts) {
			// We don't want to trigger collisions between bullets
			if(contact.thisCollider.name != contact.otherCollider.name) {
				destroy = true;

				if(decalPrefab != null) Instantiate(decalPrefab, contact.point, Quaternion.LookRotation(contact.normal, Vector3.up));

				print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
			}
		}

		if(destroy) Destroy(gameObject);
	}
}
