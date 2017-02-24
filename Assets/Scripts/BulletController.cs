using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public float lifetime = 5;
	public float bulletDamage = 1;
	public float explosionDamage = 0;
	public float explosionRadius = 0;
	public float incindiaryTime = 0;
	public GameObject decalPrefab = null;

	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;

		if(lifetime < 0) {
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision col) {
		bool destroy = false;

		if(gameObject.name != col.gameObject.name) {
			destroy = true;

			if(col.gameObject.name == "Player") {

			}

			if(decalPrefab != null) Instantiate(decalPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));

			print(gameObject.name + " hit " + col.gameObject.name);
		}

		if(destroy) Destroy(gameObject);
	}
}
