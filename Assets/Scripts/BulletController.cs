using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Bullet Controller
*/
public class BulletController : MonoBehaviour {
	public GameObject decalPrefab = null;
	public float lifetime = 5; // Seconds

	public float bulletDamage = 1; // Damage per bullet
	public float damageFalloff = 0; // damage = distance^falloff, so 0=constant, 1=linear, 2=quadratic
	public float damageMinimum = 0; // Minimum damage after falloff
	public float damageSpread = 0; // How far the bullet spreads

	public float explosionDamage = 0; // Damage inside 
	public float explosionFalloff = 0; // damage = distance^falloff, so 0=constant, 1=linear, 2=quadratic
	public float explosionRadius = 0; // How wide to look
	public bool  explosionFused = false; // pills/rockets

	public float criticalChance = 0; // Chance to deal criticals
	public float criticalMultiplier = 2; // Critical Multiplier

	public float incindiaryTime = 0; // Time player ignited for

	private float _lifetime;

	void Start() {
		_lifetime = lifetime;
	}

	void Update() {
		_lifetime -= Time.deltaTime;

		if(_lifetime < 0) {
			// Explode when they despawn.
			if(explosionFused) {
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach(GameObject player in players) {
					float distance = (transform.position - player.transform.position).magnitude;
					if(distance <= explosionRadius) {
						float edamage = Mathf.Pow((explosionRadius - distance) / explosionRadius, explosionFalloff) * explosionDamage;
						player.GetComponent<PlayerController>().OnHurt(edamage, 0);
					}
				}
			}

			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision col) {
		// Don't hit other projectiles
		if(col.gameObject.tag != "Projectiles") {
			// print(gameObject.name + " hit " + col.gameObject.name);

			bool destroy = false;

			if(col.gameObject.tag == "Player") {
				print("Hit Player");
				destroy = true;
				float bdamage = Mathf.Max(damageMinimum, Mathf.Pow(_lifetime / lifetime, damageFalloff) + (Random.value - 0.5f) * damageSpread) * bulletDamage * (Random.value < criticalChance ? criticalMultiplier : 1);
				col.gameObject.GetComponent<PlayerController>().OnHurt(bdamage, incindiaryTime);
			}

			if(explosionRadius > 0) {
				// Find players within radius
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach(GameObject player in players) {
					float distance = (col.contacts[0].point - player.transform.position).magnitude;
					if(distance <= explosionRadius) {
						float edamage = Mathf.Pow((explosionRadius - distance) / explosionRadius, explosionFalloff) * explosionDamage;
						player.GetComponent<PlayerController>().OnHurt(edamage, 0);
					}
				}
			}

			if(decalPrefab != null && col.gameObject.tag != "Unjumpable" && !explosionFused) {
				Destroy(gameObject);
				GameObject decal = Instantiate(decalPrefab, col.contacts[0].point, Quaternion.FromToRotation(Vector3.forward, col.contacts[0].normal));
				// GameObject decal = Instantiate(decalPrefab, col.contacts[0].point, Quaternion.FromToRotation(Vector3.up, col.contacts[0].normal));
				DecalDynamic.BuildDecal(decal.GetComponent<Decal>());
			} else if(destroy || !explosionFused) Destroy(gameObject);
		}
	}
}