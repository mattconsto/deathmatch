using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	Player Controller
*/
public class PlayerController : MonoBehaviour {
	public GameController controller;

	public GameObject[] guns;

	public Vector2 sensitivity = new Vector2(3, 3);
	public Vector2 smoothing = new Vector2(3, 3);

	public float burnTime = 0f;
	public float invincibleTime = 1f;
	public float movementspeed = 5f;
	public float health = 100;
	public string message = "";

	private Rigidbody _body;
	private GameObject _thecam;

	private Vector2 _smoothMouse;
	private int _selectedGun = 0;
	private bool _canJump = false;
	private float _hurtOverlayTimer = 0f;
	private float _messageTimer = 0f;
	private float _respawnTimer = 0f;

	private Transform _hudDeadOverlay;
	private Transform _hudHurtOverlay;
	private Transform _hudHintText;
	private Transform _hudHealthText;
	private Transform _hudClipText;
	private Transform _hudAmmoText;

	// Use this for initialization
	public void Start () {
		_body = GetComponent<Rigidbody>();
		_thecam = transform.Find("Camera").gameObject;

		Transform hand = transform.Find("Hand");
		for(int i = 0; i < guns.Length; i++) {
			guns[i] = Instantiate(guns[i], hand.transform.position, hand.transform.rotation);
			guns[i].SetActive(false);
			guns[i].transform.parent = hand;
		}
		guns[_selectedGun].SetActive(true);

		_hudDeadOverlay = transform.Find("Player HUD/Dead Overlay");
		_hudHurtOverlay = transform.Find("Player HUD/Hurt Overlay");
		_hudHintText = transform.Find("Player HUD/Hint Text");
		_hudHealthText = transform.Find("Player HUD/Health Text");
		_hudClipText = transform.Find("Player HUD/Clip Text");
		_hudAmmoText = transform.Find("Player HUD/Ammo Text");
	}

	public void FixedUpdate() {
		/* Force upright */
		transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - GetComponent<GravityObject>().origin) * transform.rotation;
	}

	public void Update() {
		if(_messageTimer > 0) _messageTimer -= Time.deltaTime;
		if(_hurtOverlayTimer > 0) _hurtOverlayTimer -= Time.deltaTime;
		if(invincibleTime > 0) invincibleTime -= Time.deltaTime;
		if(_respawnTimer > 0) _respawnTimer -= Time.deltaTime;

		if(_respawnTimer < 0) {
			_respawnTimer = 0f;
			controller.respawnPlayer(transform.gameObject);
			transform.Find("Model").gameObject.SetActive(true);
			transform.Find("Hand").gameObject.SetActive(true);
			health = 100;
		}
		UpdateHUD();
	}

	public void UpdateHUD() {
		Color old = _hudDeadOverlay.GetComponent<RawImage>().color;
		_hudDeadOverlay.GetComponent<RawImage>().color = new Color(old.r, old.g, old.b, Mathf.Clamp(_respawnTimer, 0, 1));

		old = _hudHurtOverlay.GetComponent<RawImage>().color;
		_hudHurtOverlay.GetComponent<RawImage>().color = new Color(old.r, old.g, old.b, _messageTimer);

		if(_messageTimer > 0) {
			_hudHintText.GetComponent<Text>().text = message;
		} else {
			_hudHintText.GetComponent<Text>().text = "";
		}

		_hudHealthText.GetComponent<Text>().text = Mathf.CeilToInt(health).ToString();

		if(guns[_selectedGun].GetComponent<GunController>().clipSize >= 0 && guns[_selectedGun].GetComponent<GunController>().clipSize != Mathf.Infinity) {
			_hudClipText.GetComponent<Text>().text = guns[_selectedGun].GetComponent<GunController>().clipSize.ToString();
			_hudAmmoText.GetComponent<Text>().text = guns[_selectedGun].GetComponent<GunController>().ammoCount.ToString();
		} else {
			_hudClipText.GetComponent<Text>().text = "";
			_hudAmmoText.GetComponent<Text>().text = "";
		}
	}

	public void OnHurt(float value) {
		if(invincibleTime > 0) return;

		health -= value;
		_hurtOverlayTimer = 1f;
		GetComponent<AudioSource>().Play();
		if(health <= 0) {
			_respawnTimer = 5f;
			transform.Find("Model").gameObject.SetActive(false);
			transform.Find("Hand").gameObject.SetActive(false);

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
		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, value * sensitivity.x * smoothing.x, 1f / smoothing.x);
		transform.Rotate(0, _smoothMouse.x / sensitivity.x, 0);
	}

	public void OnLookVertical(float value) {
		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, value * sensitivity.y * smoothing.y, 1f / smoothing.y);
		_thecam.transform.Rotate(-_smoothMouse.y / sensitivity.y, 0, 0);
		_thecam.transform.localEulerAngles = new Vector3((Mathf.Clamp((_thecam.transform.localEulerAngles.x + 90) % 360, 80, 160) + 270) % 360, 0, 0);
		guns[_selectedGun].transform.localEulerAngles = new Vector3(0, 0, (Mathf.Clamp((_thecam.transform.localEulerAngles.x + 90) % 360, 80, 160) + 270) % 360);
	}

	public void OnMoveHorizontal(float value) {
		_body.AddForce(transform.right * value * movementspeed);
	}

	public void OnMoveVertical(float value) {
		_body.AddForce(transform.forward * value * movementspeed);
	}

	public void OnJump() {
		if (_canJump) _body.AddForce(transform.up * 500);
	}

	public void OnCollisionEnter (Collision col) {
		if(col.collider.name == "Planet") _canJump = true;
	}

	public void OnCollisionExit (Collision col) {
		if(col.collider.name == "Planet") _canJump = false;
	}

	public void OnSwitch(int value) {
		guns[_selectedGun].SetActive(false);
		_selectedGun = (_selectedGun + value + guns.Length) % guns.Length;
		guns[_selectedGun].SetActive(true);
		message = guns[_selectedGun].GetComponent<GunController>().displayName;
		_messageTimer = 1f;
	}
}
