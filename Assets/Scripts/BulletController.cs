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
	public float bulletDamage = 1;
	public float explosionDamage = 0;
	public float explosionRadius = 0;
	public float incindiaryTime = 0;

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
				float damage = Mathf.Max(damageMinimum, Mathf.Pow(_lifetime / lifetime, damageFalloff)) * bulletDamage;
				col.gameObject.GetComponent<PlayerController>().OnHurt(damage);
			}

			if(decalPrefab != null) Instantiate(decalPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));

			// print(gameObject.name + " hit " + col.gameObject.name);
		}

		if(destroy) Destroy(gameObject);
	}
}
