using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public Vector3 gravityOrigin;
	public float gravityForce = 9.8f;

	private Vector2 _smoothMouse;
	private bool _canJump = true;

	public Vector2 sensitivity = new Vector2(3, 3);
	public Vector2 smoothing = new Vector2(3, 3);

	public float movementspeed = 5f;

	private Rigidbody rb;
	private GameObject thecam;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
		thecam = gameObject.transform.Find("Camera").gameObject;
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
		print(thecam.transform.localEulerAngles.x);
		thecam.transform.localEulerAngles = new Vector3((Mathf.Clamp((thecam.transform.localEulerAngles.x + 90) % 360, 0, 120) + 270) % 360, 0, 0);

		/* Jumping */
		if (_canJump && Input.GetKeyDown("space")) rb.AddForce(transform.up * 500);

		/* Movement */

		float dx = Input.GetAxis("Horizontal"), dy = Input.GetAxis("Vertical");

		if(dx + dy != 0) {
			// Properly handle diagonals
			float adx = Mathf.Abs(dx), ady = Mathf.Abs(dy);

			var x = adx / (adx + ady) * dx * Time.deltaTime * movementspeed;
			var z = ady / (adx + ady) * dy * Time.deltaTime * movementspeed;

			transform.Translate(x, 0, 0);
			transform.Translate(0, 0, z);
		}

		/* Gravity */

		rb.AddForce((gravityOrigin - transform.position).normalized * gravityForce);

		transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - gravityOrigin) * transform.rotation;
	}

	void OnCollisionEnter (Collision other) {
		_canJump = true;
	}

	void OnCollisionExit (Collision other) {
		_canJump = false;
	}
}
