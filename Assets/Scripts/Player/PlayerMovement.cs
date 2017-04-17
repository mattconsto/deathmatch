using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	/* Public Properties */
	public float speed = 5f;
	public int canJump = 0;
	public AudioClip footsteps;
	public AudioClip jump;

	/* Private Properties */
	private Rigidbody _rb;
	private float _stepTimer = 15f;
	private Vector2 _rotation = new Vector2(0, 0);

	// Use this for initialization
	public void Start () {
		_rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	public void FixedUpdate () {
		_rb.AddForce(transform.right * _rotation.x * speed);
		_rb.AddForce(transform.forward * _rotation.y * speed);

		if(_stepTimer < 0) {
			_stepTimer = 15f;
			if(footsteps != null) GetComponent<AudioSource>().PlayOneShot(footsteps, 0.5f);
		}
	}

	public void OnMoveHorizontal(float value) {
		_rotation.x = value;
		if(canJump > 0) _stepTimer -= Mathf.Abs(value);
	}

	public void OnMoveVertical(float value) {
		_rotation.y = value;
		if(canJump > 0) _stepTimer -= Mathf.Abs(value);
	}

	public void OnJump(float value) {
		if (value > 0 && canJump > 0) {
			if(jump != null) GetComponent<AudioSource>().PlayOneShot(jump, 1f);
			_rb.AddForce(transform.up * 500);
		}
	}

	public void OnCollisionEnter (Collision col) {
		if(col.gameObject.tag != "Unjumpable" && col.gameObject.tag != "Projectiles") canJump++;
	}

	public void OnCollisionExit (Collision col) {
		if(col.gameObject.tag != "Unjumpable" && col.gameObject.tag != "Projectiles") canJump--;
	}
}
