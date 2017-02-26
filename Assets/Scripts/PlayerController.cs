using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public Vector3 gravityOrigin;
	public float gravityForce = 9.8f;

	private Vector2 _smoothMouse;
	private bool _canJump = false;

	public Vector2 sensitivity = new Vector2(3, 3);
	public Vector2 smoothing = new Vector2(3, 3);

	public float burnTime = 0;

	public float movementspeed = 5f;

	public GameObject gun;
	private GameObject _gunInstance;

	private Rigidbody rb;
	private GameObject thecam;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		thecam = transform.Find("Camera").gameObject;

		_gunInstance = Instantiate(gun, transform.Find("Hand").transform.position, transform.Find("Hand").transform.rotation);
		_gunInstance.transform.parent = transform.Find("Hand");
	}

	void FixedUpdate() {
		/* Gravity */
		rb.AddForce((gravityOrigin - transform.position).normalized * gravityForce);
		transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - gravityOrigin) * transform.rotation;
		// transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up, transform.position - gravityOrigin) * transform.rotation, 30 * Time.deltaTime);
	}

	public void OnFire() {
		print("Fire");
		GunController gun = _gunInstance.GetComponent<GunController>();
		if(gun != null) gun.Fire();
	}

	public void OnLookHorizontal(float value) {
		float delta = value * sensitivity.x * smoothing.x;

		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, delta, 1f / smoothing.x);

		transform.Rotate(0, _smoothMouse.x / sensitivity.x, 0);
	}

	public void OnLookVertical(float value) {
		float delta = value * sensitivity.x * smoothing.x;

		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, delta, 1f / smoothing.y);

		thecam.transform.Rotate(-_smoothMouse.y / sensitivity.y, 0, 0);
		thecam.transform.localEulerAngles = new Vector3((Mathf.Clamp((thecam.transform.localEulerAngles.x + 90) % 360, 80, 120) + 270) % 360, 0, 0);
		_gunInstance.transform.localEulerAngles = new Vector3(0, 0, (Mathf.Clamp((thecam.transform.localEulerAngles.x + 90) % 360, 80, 120) + 270) % 360);
	}

	public void OnMoveHorizontal(float value) {
		rb.AddForce(transform.right * value * movementspeed);
	}

	public void OnMoveVertical(float value) {
		rb.AddForce(transform.forward * value * movementspeed);
	}

	public void OnJump() {
		print("Jump");
		if (_canJump) rb.AddForce(transform.up * 500);
	}

	public void OnCollisionEnter (Collision col) {
		if(col.collider.name == "Planet") _canJump = true;
	}

	public void OnCollisionExit (Collision col) {
		if(col.collider.name == "Planet") _canJump = false;
	}
}
