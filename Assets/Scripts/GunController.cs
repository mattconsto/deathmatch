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
			for(int i = 0; i < shotCount; i++) {
				float xr = (0.5f - Random.value) * shotSpread;
				float zr = Random.value * 360f;

				Vector3 velocity = Quaternion.AngleAxis(zr, -transform.right) * Quaternion.AngleAxis(xr, transform.forward) * (-transform.right * GetComponent<GunController>().muzzleVelocity);

				var bullet = Instantiate(GetComponent<GunController>().bulletPrefab, _muzzle.transform.position, Quaternion.Euler(velocity));
				bullet.GetComponent<Rigidbody>().velocity = velocity;
			}

			GetComponent<AudioSource>().Play();

			_timing = 0;
			if(ammo > 0) ammo--;
			return true;
		}

		return false;
	}
}
