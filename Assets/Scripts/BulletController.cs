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

		if(col.gameObject.tag != "Projectiles") {
			destroy = true;

			if(col.gameObject.tag == "Player") {
				print("Hit Player");
				col.gameObject.GetComponent<PlayerController>().OnHurt(bulletDamage);
			}

			if(decalPrefab != null) Instantiate(decalPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));

			print(gameObject.name + " hit " + col.gameObject.name);
		}

		if(destroy) Destroy(gameObject);
	}
}
