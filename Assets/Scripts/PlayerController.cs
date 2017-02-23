using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		/* Jumping */
		if (_canJump && Input.GetButton("Jump")) rb.AddForce(transform.up * 500);

		/* Movement */
		float dx = Input.GetAxis("Horizontal");
		float dy = Input.GetAxis("Vertical");

		if(dx + dy != 0) {
			// Properly handle diagonals
			float adx = Mathf.Abs(dx), ady = Mathf.Abs(dy);

			var x = adx / (adx + ady) * dx * movementspeed * Time.deltaTime;
			var z = ady / (adx + ady) * dy * movementspeed * Time.deltaTime;

			// rb.AddForce(transform.forward * z);
			// rb.AddForce(transform.right * x);
			transform.Translate(x, 0, 0);
			transform.Translate(0, 0, z);
		}

		/* Gravity */
		rb.AddForce((gravityOrigin - transform.position).normalized * gravityForce);
		transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - gravityOrigin) * transform.rotation;
	}

	// Update is called once per frame
	void Update () {
		/* Mouselook */

		// Get raw mouse input for a cleaner reading on more sensitive mice.
		var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		// Scale input against the sensitivity setting and multiply that against the smoothing value.
		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

		transform.Rotate(0, _smoothMouse.x / sensitivity.x, 0);
		thecam.transform.Rotate(-_smoothMouse.y / sensitivity.y, 0, 0);
		thecam.transform.localEulerAngles = new Vector3((Mathf.Clamp((thecam.transform.localEulerAngles.x + 90) % 360, 0, 120) + 270) % 360, 0, 0);
		_gunInstance.transform.localEulerAngles = new Vector3(0, 0, (Mathf.Clamp((thecam.transform.localEulerAngles.x + 90) % 360, 0, 120) + 270) % 360 - 30);

		/* Bullets */
		if(Input.GetButton("Fire1")) {
			GunController gun = _gunInstance.GetComponent<GunController>();
			if(gun != null) gun.Fire();
		}
	}

	void OnCollisionEnter (Collision col) {
		if(col.collider.name == "Planet") _canJump = true;
	}

	void OnCollisionExit (Collision col) {
		if(col.collider.name == "Planet") _canJump = false;
	}
}
