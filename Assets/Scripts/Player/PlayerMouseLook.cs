using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseLook : MonoBehaviour {
	public float sensitivity = 3;
	public float smoothing = 3;
	public float knockback = 1;

	public GameObject player_camera;

	private Vector2 _smoothMouse;

	public float _recoilForce = 0;
	public float _recoilTime = 0;
	public float _recoilFallOff = 3;

	public void Recoil(float force, float time, float falloff) {
		_recoilForce = force;
		_recoilTime = time;
		_recoilFallOff = falloff;
	}

	public void Update() {
		// Handle recoil
		if(_recoilTime > 0) {
			OnLookVertical(Mathf.Pow(_recoilTime, _recoilFallOff) * _recoilForce);
			_recoilTime -= Time.deltaTime;
			player_camera.transform.localPosition = new Vector3(player_camera.transform.localPosition.x, player_camera.transform.localPosition.y, -1f * knockback * Mathf.Log(_recoilForce * _recoilTime));
		}
	}

	public void OnLookHorizontal(float value) {
		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, value * sensitivity * smoothing, 1f / smoothing);
		transform.Rotate(0, _smoothMouse.x / sensitivity, 0);
	}

	public void OnLookVertical(float value) {
		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, value * sensitivity * smoothing, 1f / smoothing);
		player_camera.transform.Rotate(-_smoothMouse.y / sensitivity, 0, 0);
		player_camera.transform.localEulerAngles = new Vector3((Mathf.Clamp((player_camera.transform.localEulerAngles.x + 90) % 360, 10, 170) + 270) % 360, 0, 0);
		GetComponent<PlayerController>().guns[GetComponent<PlayerController>().selectedGun].transform.localEulerAngles = new Vector3(0, 0, ((Mathf.Clamp((player_camera.transform.localEulerAngles.x + 90) % 360, 10, 170) + 270) % 360) + 10);
	}
}
