using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
	public GameObject bulletPrefab;
	public float muzzleVelocity = 10f;
	public float fireRate = 0.1f;
	public int shotCount = 10;
	public float shotSpread = 5;

	public float ammo = 10;

	private Transform _muzzle;
	private float _timing = 0;

	public void Start() {
		_muzzle = transform.Find("Muzzle");
	}

	public void Update() {
		_timing += Time.deltaTime;
	}

	public bool Fire() {
		if(_timing > fireRate && ammo != 0) {
			print("Pew");
			for(int i = 0; i < shotCount; i++) {
				Vector3 rotSpread = _muzzle.transform.rotation.eulerAngles + new Vector3((0.5f - Random.value) * shotSpread, (0.5f - Random.value) * shotSpread, (0.5f - Random.value) * shotSpread);
				var bullet = Instantiate(GetComponent<GunController>().bulletPrefab, _muzzle.transform.position, Quaternion.Euler(rotSpread));
				bullet.GetComponent<Rigidbody>().velocity = -GetComponent<Transform>().right * GetComponent<GunController>().muzzleVelocity;
			}

			_timing = 0;
			if(ammo > 0) ammo--;
			return true;
		}

		return false;
	}
}
