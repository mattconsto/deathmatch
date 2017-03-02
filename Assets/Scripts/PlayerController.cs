using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	Player Controller
*/
public class PlayerController : MonoBehaviour {

	/* Public Properties */

	public GameController controller;

	public GameObject[] guns;

	public Vector2 sensitivity = new Vector2(3, 3);
	public Vector2 smoothing = new Vector2(3, 3);

	public float burnTime = 0f;
	public float invincibleTime = 1f;
	public float movementspeed = 5f;
	public float health = 100;
	public float damageBatchingWindow = 0.2f;
	public float healthRegen = 0.5f;
	public string message = "";
	public Color color = Color.white;

	private Rigidbody _body;
	private GameObject _thecam;

	/* Private Properties */

	private Vector2 _smoothMouse;
	private int _selectedGun = 0;
	private bool _canJump = false;
	private float _hurtOverlayTimer = 0f;
	private float _messageTimer = 0f;
	private float _respawnTimer = 0f;
	private float _fireTimer = 0f;
	private float _damageBatchingTimer = 0f;
	private float _damageBatchingDamage = 0;

	private Transform _hudDeadOverlay;
	private Transform _hudHurtOverlay;
	private Transform _hudHintText;
	private Transform _hudHealthText;
	private Transform _hudClipText;
	private Transform _hudAmmoText;

	/* Unity Methods */

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

		// Random colour
		transform.Find("Model/Character").GetComponent<SkinnedMeshRenderer>().material.color = color;
		_hudClipText.gameObject.GetComponent<Text>().color = color;
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
		if(_damageBatchingTimer > 0) _damageBatchingTimer -= Time.deltaTime;

		// Health regen
		health = Mathf.Min(100, health + healthRegen * Time.deltaTime);

		if(_fireTimer > 0) {
			_fireTimer -= Time.deltaTime;
			OnHurt(Time.deltaTime * 5, 0);
		}

		if(_damageBatchingTimer < 0) {
			_damageBatchingTimer = 0;
			_damageBatchingDamage = 0;
		}

		if(_respawnTimer < 0) {
			_respawnTimer = 0f;
			controller.respawnPlayer(transform.gameObject);
			SetActive(true);
			health = 100;
		}
		UpdateHUD();
	}

	/* Misc. Methods */

	public void SetActive(bool active) {
		transform.GetComponent<Rigidbody>().isKinematic = !active;
		transform.Find("Model").gameObject.SetActive(active);
		transform.Find("Hand").gameObject.SetActive(active);
		GetComponent<CapsuleCollider>().enabled = active;
	}

	public void UpdateHUD() {
		Color old = _hudDeadOverlay.GetComponent<RawImage>().color;
		_hudDeadOverlay.GetComponent<RawImage>().color = new Color(old.r, old.g, old.b, Mathf.Clamp(_respawnTimer * 4, 0, 1));

		old = _hudHurtOverlay.GetComponent<RawImage>().color;
		_hudHurtOverlay.GetComponent<RawImage>().color = new Color(old.r, old.g, old.b, _messageTimer);

		_hudHintText.GetComponent<Text>().text = _messageTimer > 0 ? message : "";
		_hudHealthText.GetComponent<Text>().text = Mathf.CeilToInt(health).ToString();

		GunController gc = guns[_selectedGun].GetComponent<GunController>();
		if(gc.clipSize >= 0 && gc.clipSize != Mathf.Infinity) {
			_hudClipText.GetComponent<Text>().text = gc.clipSize.ToString();
			_hudAmmoText.GetComponent<Text>().text = gc.ammoCount.ToString();
		} else {
			_hudClipText.GetComponent<Text>().text = "";
			_hudAmmoText.GetComponent<Text>().text = "";
		}
	}

	public void OnHurt(float damage, float fire) {
		if(invincibleTime > 0) return;

		_fireTimer = Mathf.Max(fire, _fireTimer);
		health -= damage;
		_hurtOverlayTimer = 1f;
		_damageBatchingTimer = damageBatchingWindow;
		_damageBatchingDamage += damage;

		GetComponent<AudioSource>().Play();
		if(health <= 0) {
			_respawnTimer = 5f;
			SetActive(false);

			message = "You have died.";
			_messageTimer = 1f;
		} else {
			message = "Hit for " + Mathf.RoundToInt(_damageBatchingDamage);
			_messageTimer = 1f;
		}
	}

	/* Event Listeners */

	public void OnFire() {
		if(_respawnTimer > 0) return;
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
		if(_respawnTimer > 0) return;
		_body.AddForce(transform.right * value * movementspeed);
	}

	public void OnMoveVertical(float value) {
		if(_respawnTimer > 0) return ;
		_body.AddForce(transform.forward * value * movementspeed);
	}

	public void OnJump() {
		if(_respawnTimer > 0) return;
		if (_canJump) _body.AddForce(transform.up * 500);
	}

	public void OnCollisionEnter (Collision col) {
		_canJump = true;
	}

	public void OnCollisionExit (Collision col) {
		_canJump = false;
	}

	public void OnSwitch(int value) {
		if(_respawnTimer > 0) return;
		guns[_selectedGun].SetActive(false);
		_selectedGun = (_selectedGun + value + guns.Length) % guns.Length;
		guns[_selectedGun].SetActive(true);
		message = guns[_selectedGun].GetComponent<GunController>().displayName;
		_messageTimer = 1f;
	}
}
