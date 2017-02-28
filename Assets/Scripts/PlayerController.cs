using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public GameController controller;

	private Vector2 _smoothMouse;
	private bool _canJump = false;

	public Vector2 sensitivity = new Vector2(3, 3);
	public Vector2 smoothing = new Vector2(3, 3);

	public float burnTime = 0f;
	public float invincibleTime = 1f;

	public float movementspeed = 5f;

	public GameObject[] guns;
	private int _selectedGun = 0;

	private Rigidbody rb;
	private GameObject thecam;

	public float health = 100;

	public string message = "";
	private float _messageTimer = 0f;

	// Use this for initialization
	public void Start () {
		rb = GetComponent<Rigidbody>();
		thecam = transform.Find("Camera").gameObject;

		Transform hand = transform.Find("Hand");
		for(int i = 0; i < guns.Length; i++) {
			guns[i] = Instantiate(guns[i], hand.transform.position, hand.transform.rotation);
			guns[i].active = false;
			guns[i].transform.parent = hand;
		}
		guns[_selectedGun].active = true;
	}

	public void FixedUpdate() {
		/* Gravity */
		transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - GetComponent<GravityObject>().origin) * transform.rotation;
	}

	public void Update() {
		if(_messageTimer > 0) _messageTimer -= Time.deltaTime;
		if(invincibleTime > 0) invincibleTime -= Time.deltaTime;
		UpdateHUD();
	}

	public void UpdateHUD() {
		if(_messageTimer > 0) {
			transform.Find("Player HUD/Hint Text").GetComponent<Text>().text = message;
		} else {
			transform.Find("Player HUD/Hint Text").GetComponent<Text>().text = "";
		}

		transform.Find("Player HUD/Health Text").GetComponent<Text>().text = Mathf.CeilToInt(health).ToString();

		if(guns[_selectedGun].GetComponent<GunController>().ammo >= 0 && guns[_selectedGun].GetComponent<GunController>().ammo != Mathf.Infinity) {
			transform.Find("Player HUD/Ammo Text").GetComponent<Text>().text = guns[_selectedGun].GetComponent<GunController>().ammo.ToString();
		} else {
			transform.Find("Player HUD/Ammo Text").GetComponent<Text>().text = "";
		}
	}

	public void OnHurt(float value) {
		if(invincibleTime > 0) return;

		health -= value;
		GetComponent<AudioSource>().Play();
		if(health <= 0) {
			controller.respawnPlayer(transform.gameObject);
			health = 100;

			message = "You have died.";
			_messageTimer = 1f;
		} else {
			message = "Hit for " + Mathf.RoundToInt(value);
			_messageTimer = 1f;
		}
	}

	public void OnFire() {
		GunController controller = guns[_selectedGun].GetComponent<GunController>();
		if(controller != null) controller.Fire();
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
		thecam.transform.localEulerAngles = new Vector3((Mathf.Clamp((thecam.transform.localEulerAngles.x + 90) % 360, 80, 160) + 270) % 360, 0, 0);
		guns[_selectedGun].transform.localEulerAngles = new Vector3(0, 0, (Mathf.Clamp((thecam.transform.localEulerAngles.x + 90) % 360, 80, 160) + 270) % 360);
	}

	public void OnMoveHorizontal(float value) {
		rb.AddForce(transform.right * value * movementspeed);
	}

	public void OnMoveVertical(float value) {
		rb.AddForce(transform.forward * value * movementspeed);
	}

	public void OnJump() {
		if (_canJump) rb.AddForce(transform.up * 500);
	}

	public void OnCollisionEnter (Collision col) {
		if(col.collider.name == "Planet") _canJump = true;
	}

	public void OnCollisionExit (Collision col) {
		if(col.collider.name == "Planet") _canJump = false;
	}

	public void OnSwitchLeft() {
		guns[_selectedGun].active = false;
		_selectedGun = (_selectedGun - 1 + guns.Length) % guns.Length;
		guns[_selectedGun].active = true;
		message = guns[_selectedGun].GetComponent<GunController>().name;
		_messageTimer = 1f;
	}

	public void OnSwitchRight() {
		guns[_selectedGun].active = false;
		_selectedGun = (_selectedGun + 1 + guns.Length) % guns.Length;
		guns[_selectedGun].active = true;
		message = guns[_selectedGun].GetComponent<GunController>().name;
		_messageTimer = 1f;
	}
}
