using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Bullet Controller
*/
public class BulletController : MonoBehaviour {
	public GameObject decalPrefab = null;
	public float lifetime = 5;
	public float damageFalloff = 0;
	public float damageMinimum = 0;
	public float damageSpread = 0;
	public float bulletDamage = 1;
	public float explosionDamage = 0;
	public float explosionRadius = 0;
	public float incindiaryTime = 0;
	public float criticalChance = 0;
	public float criticalMultiplier = 2;

	public AudioClip hitSound;

	private float _lifetime;

	void Start() {
		_lifetime = lifetime;
	}

	void Update() {
		_lifetime -= Time.deltaTime;

		if(_lifetime < 0) Destroy(gameObject);
	}

	void OnCollisionEnter(Collision col) {
		bool destroy = false;

		if(col.gameObject.tag != "Projectiles") {
			destroy = true;

			if(col.gameObject.tag == "Player") {
				print("Hit Player");
				float damage = Mathf.Max(damageMinimum, Mathf.Pow(_lifetime / lifetime, damageFalloff) + (Random.value - 0.5f) * damageSpread) * bulletDamage * (Random.value < criticalChance ? criticalMultiplier : 1);
				col.gameObject.GetComponent<PlayerController>().OnHurt(damage, incindiaryTime);

				// TODO: test this when collisions are better.
				if(explosionRadius > 0) {
					// Find players within radius
					GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
					foreach(GameObject player in players) {
						float distance = (col.contacts[0].point - player.transform.position).magnitude;
						if(distance <= explosionRadius) {
							player.GetComponent<PlayerController>().OnHurt((explosionRadius - distance) * explosionDamage, 0);
						}
					}
				}
			}

			if(decalPrefab != null) Instantiate(decalPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));

			// print(gameObject.name + " hit " + col.gameObject.name);
		}

		if(destroy) Destroy(gameObject);
	}
}
