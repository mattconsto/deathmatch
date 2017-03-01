using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
	public GameObject bulletPrefab;

	public string displayName = "Gun";
	public float muzzleVelocity = 10f;
	public float fireRate = 0.1f;
	public int   shotCount = 10;
	public float shotSpread = 5;
	public float clipSize = 10;
	public float ammoCount = 10;
	public float reloadRate = 0.5f;

	private Transform _muzzle;
	private float _fireTiming = 0;
	private float _reloadTiming = 0;

	public void Start() {
		_muzzle = transform.Find("Muzzle");
	}

	public void Update() {
		_fireTiming += Time.deltaTime;

		if(_reloadTiming >= 0f && ammoCount <= 0) {
			_reloadTiming -= Time.deltaTime;
		} else if(_reloadTiming < 0f && ammoCount <= 0) {
			_reloadTiming = 0;
			ammoCount = clipSize;
		}
	}

	public bool Fire() {
		if(_reloadTiming <= 0f && _fireTiming > fireRate && ammoCount > 0) {
			for(int i = 0; i < shotCount; i++) {
				// Calculate Spread
				float xr = (0.5f - Random.value) * shotSpread;
				float zr = Random.value * 360f;

				Vector3 velocity = Quaternion.AngleAxis(zr, -transform.right) * Quaternion.AngleAxis(xr, transform.forward) * (-transform.right * muzzleVelocity);

				var bullet = Instantiate(bulletPrefab, _muzzle.transform);
				bullet.GetComponent<Rigidbody>().velocity = velocity;
			}

			GetComponent<AudioSource>().Play();

			_fireTiming = 0;
			ammoCount--;
			return true;
		}

		if(_reloadTiming <= 0 && ammoCount <= 0) _reloadTiming = reloadRate;

		return false;
	}
}
