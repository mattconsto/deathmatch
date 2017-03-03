using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Gun Controller
*/
public class GunController : MonoBehaviour {
	public GameObject bulletPrefab;

	public string displayName = "Gun";
	public float muzzleVelocity = 10f; // Speed
	public float fireRate = 0.1f; // Seconds
	public int   shotCount = 1; // Pellets per shot
	public float shotSpread = 5; // How wide the pellets fly
	public float clipSize = 10; // The ammo the gun can hold
	public int   ammoCount = 10; // Current ammo
	public float reloadRate = 0.5f; // Seconds
	public bool  partialReload = false; // Let us only reload one bullet, and then fire.
	public float recoilForce = 0; // Upwards force on the gun

	public AudioClip reloadAudio;
	public AudioClip shotAudio;

	private Transform _muzzle;
	private float _fireTiming = 0;
	private float _reloadTiming = 0;
	private bool  _reloading = false;

	public void Start() {
		_muzzle = transform.Find("Muzzle");
	}

	public void Update() {
		_fireTiming += Time.deltaTime;

		// Handle reloading
		if(_reloading) {
			if(_reloadTiming >= 0f) {
				_reloadTiming -= Time.deltaTime;
				if(partialReload) ammoCount = Mathf.FloorToInt(clipSize * (reloadRate - _reloadTiming) / reloadRate);
			} else {
				_reloadTiming = 0;
				_reloading = false;
				if(!partialReload) ammoCount = Mathf.FloorToInt(clipSize);
			}
		}
	}

	public void Reload() {
		// Only reload if we need to reload
		if(!_reloading && ammoCount < clipSize) {
			_reloading = true;
			_reloadTiming = partialReload ? (clipSize - ammoCount) / clipSize * reloadRate : reloadRate;
		}
	}

	public bool Fire() {
		// Let us fire while we partially reload.
		if(_reloading && ammoCount > 0) {
			_reloading = false;
			_reloadTiming = 0f;
		}

		if(!_reloading && _fireTiming > fireRate && ammoCount > 0) {
			for(int i = 0; i < shotCount; i++) {
				// Calculate Spread
				float xr = (0.5f - Random.value) * shotSpread;
				float zr = Random.value * 360f;
				Vector3 velocity = Quaternion.AngleAxis(zr, -transform.right) * Quaternion.AngleAxis(xr, transform.forward) * (-transform.right * muzzleVelocity);

				// Fire bullet
				var bullet = Instantiate(bulletPrefab, _muzzle.transform.position, Quaternion.Euler(velocity));
				bullet.GetComponent<Rigidbody>().velocity = velocity;
			}

			// One sound per shot
			if(shotAudio != null) GetComponent<AudioSource>().PlayOneShot(shotAudio, 1f);

			// Limits
			_fireTiming = 0;
			ammoCount--;

			// Start reloading if needed
			if(ammoCount <= 0) {
				_reloadTiming = reloadRate;
				_reloading = true;
				if(reloadAudio != null) GetComponent<AudioSource>().PlayOneShot(reloadAudio, 1f);
			}

			// Recoil
			transform.parent.parent.Find("Camera").Rotate(-recoilForce, 0, 0);
			transform.parent.parent.Find("Camera").localEulerAngles = new Vector3((Mathf.Clamp((transform.parent.parent.Find("Camera").localEulerAngles.x + 90) % 360, 10, 170) + 270) % 360, 0, 0);

			return true;
		}

		return false;
	}
}
