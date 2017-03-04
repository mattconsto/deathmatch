using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {
	public GameObject weaponPrefab;

	public void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Player") {
			print("Give Player" + weaponPrefab.GetComponent<GunController>().displayName);
			col.gameObject.GetComponent<PlayerController>().AddGun(weaponPrefab);
			gameObject.SetActive(false);
		}
	}
}
