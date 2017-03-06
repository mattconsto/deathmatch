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
	public List<GameObject> guns = new List<GameObject>();

	public float burnTime = 0f;
	public float invincibleTime = 1f;
	public float baseHealth = 100;
	public float health = 100;
	public float damageBatchingWindow = 0.2f;
	public float healthRegen = 0.5f;
	public float healthDecay = 0.5f;
	public string message = "";
	public Color color = Color.white;
	public int selectedGun = 0;

	public AudioClip switchAudio;

	public PlayerMovement movement_script;
	public PlayerMouseLook mouselook_script;

	/* Private Properties */
	private Rigidbody _body;

	private float _hurtOverlayTimer = 0f;
	private float _messageTimer = 0f;
	private float _respawnTimer = 0f;
	private float _fireTimer = 0f;
	private float _damageBatchingTimer = 0f;
	private float _damageBatchingDamage = 0;
	private float _bloodTimer = 0f;

	private Transform _hudDeadOverlay;
	private Transform _hudHurtOverlay;
	private Transform _hudHintText;
	private Transform _hudHealthText;
	private Transform _hudClipText;
	private Transform _hudAmmoText;

	private GameObject _fireParticles;
	private GameObject _bloodParticles;

	/* Unity Methods */

	public void Start () {
		_body = GetComponent<Rigidbody>();

		Transform hand = transform.Find("Hand");
		for(int i = 0; i < guns.Count; i++) {
			guns[i] = Instantiate(guns[i], hand.transform.position, hand.transform.rotation);
			guns[i].SetActive(false);
			guns[i].transform.parent = hand;
		}
		guns[selectedGun].SetActive(true);

		_hudDeadOverlay = transform.Find("Player HUD/Dead Overlay");
		_hudHurtOverlay = transform.Find("Player HUD/Hurt Overlay");
		_hudHintText = transform.Find("Player HUD/Hint Text");
		_hudHealthText = transform.Find("Player HUD/Health Text");
		_hudClipText = transform.Find("Player HUD/Clip Text");
		_hudAmmoText = transform.Find("Player HUD/Ammo Text");

		_fireParticles = transform.Find("Fire Particles").gameObject;
		_bloodParticles = transform.Find("Blood Particles").gameObject;

		// Random colour
		transform.Find("Model/Character").GetComponent<SkinnedMeshRenderer>().material.color = color;
		_hudClipText.gameObject.GetComponent<Text>().color = color;
	}

	public void Update() {
		// Timers
		_messageTimer -= Time.deltaTime;
		_hurtOverlayTimer -= Time.deltaTime;
		invincibleTime -= Time.deltaTime;
		if(_respawnTimer > 0) _respawnTimer -= Time.deltaTime;
		if(_damageBatchingTimer > 0) _damageBatchingTimer -= Time.deltaTime;

		// Health regen and decay
		if(health > baseHealth) {
			health = Mathf.Max(baseHealth, health - healthDecay * Time.deltaTime);
		} else {
			health = Mathf.Min(baseHealth, health + healthRegen * Time.deltaTime);
		}

		if(_fireTimer > 0) {
			_fireTimer -= Time.deltaTime;
			OnHurt(Time.deltaTime * 5, 0);

			var emission = _fireParticles.GetComponent<ParticleSystem>().emission;
			var rate = emission.rate;
			rate.constantMax = 10;
			emission.rate = rate;
		} else {
			var emission = _fireParticles.GetComponent<ParticleSystem>().emission;
			var rate = emission.rate;
			rate.constantMax = 0;
			emission.rate = rate;
		}

		if(_bloodTimer > 0) {
			_bloodTimer -= Time.deltaTime;
		} else {
			var emission = _bloodParticles.GetComponent<ParticleSystem>().emission;
			var rate = emission.rate;
			rate.constantMax = 0;
			emission.rate = rate;
		}

		if(_damageBatchingTimer < 0) {
			_damageBatchingTimer = 0;
			_damageBatchingDamage = 0;
		}

		if(_respawnTimer < 0) {
			_respawnTimer = 0f;
			controller.respawnPlayer(transform.gameObject);
			SetActive(true);
			health = baseHealth;
		}

		UpdateHUD();
	}

	/* Misc. Methods */

	public void SetActive(bool active) {
		// Enable/Disable player
		transform.GetComponent<Rigidbody>().isKinematic = !active;
		transform.Find("Model").gameObject.SetActive(active);
		transform.Find("Hand").gameObject.SetActive(active);
		GetComponent<CapsuleCollider>().enabled = active;
		movement_script.enabled = active;
		mouselook_script.enabled = active;
	}

	public void UpdateHUD() {
		Color old = _hudDeadOverlay.GetComponent<RawImage>().color;
		_hudDeadOverlay.GetComponent<RawImage>().color = new Color(old.r, old.g, old.b, Mathf.Clamp(_respawnTimer * 4, 0, 1));

		old = _hudHurtOverlay.GetComponent<RawImage>().color;
		_hudHurtOverlay.GetComponent<RawImage>().color = new Color(old.r, old.g, old.b, Mathf.Clamp(_hurtOverlayTimer, 0, 1));

		_hudHintText.GetComponent<Text>().text = _messageTimer > 0 ? message : "";
		_hudHealthText.GetComponent<Text>().text = Mathf.CeilToInt(health).ToString();
		_hudHealthText.GetComponent<Text>().color = health > baseHealth ? new Color(0.565f, 0.855f, 0.882f) : Color.Lerp(new Color(0.808f, 0.196f, 0.0549f), new Color(0.804f, 0.741f, 0.678f), Mathf.Min(1.5f*health/baseHealth, 1));

		GunController gc = guns[selectedGun].GetComponent<GunController>();
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

			var emission = _bloodParticles.GetComponent<ParticleSystem>().emission;
			var rate = emission.rate;
			rate.constantMax = 10;
			emission.rate = rate;
			_bloodTimer = 0.25f;
		}
	}

	/* Event Listeners */

	public void OnFire() {
		if(_respawnTimer > 0) return;
		GunController controller = guns[selectedGun].GetComponent<GunController>();
		if(controller != null) controller.Fire();
	}

	public void OnSwitch(int value) {
		if(_respawnTimer > 0) return;
		guns[selectedGun].SetActive(false);
		selectedGun = (selectedGun + value + guns.Count) % guns.Count;
		guns[selectedGun].SetActive(true);
		SetMesage(guns[selectedGun].GetComponent<GunController>().displayName, 1f);
		_messageTimer = 1f;
		if(switchAudio != null) GetComponent<AudioSource>().PlayOneShot(switchAudio, 1f);
	}

	public void OnReload() {
		guns[selectedGun].GetComponent<GunController>().Reload();
	}

	public void AddGun(GameObject prefab) {
	    Transform hand = transform.Find("Hand");

	    for(int i = 0; i < guns.Count; i++) guns[i].SetActive(false);

	    guns.Add(Instantiate(prefab, hand.transform.position, hand.transform.rotation));
	    guns[guns.Count - 1].transform.parent = hand;
	    selectedGun = guns.Count - 1;

	    SetMesage(guns[selectedGun].GetComponent<GunController>().displayName, 1f);
	    if(switchAudio != null) GetComponent<AudioSource>().PlayOneShot(switchAudio, 1f);
  	}

	public void SetMesage(string text, float time) {
	    message = text;
	    _messageTimer = time;
	    Debug.Log(message);
	}

	public void OnCollisionEnter (Collision col) {
		if(col.gameObject.tag == "Unjumpable")
			return;
	}
}
