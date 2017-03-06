using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	/* Public Properties */
	public float speed = 5f;
	public AudioClip footsteps;
	public AudioClip jump;

	/* Private Properties */
	private Rigidbody _rb;
	private bool _canJump = false;
	private float _step_timer = 15f;

	// Use this for initialization
	void Start () {
		_rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {
		if(_step_timer < 0) {
			_step_timer = 15f;
			if(footsteps != null) {
				GetComponent<AudioSource>().PlayOneShot(footsteps, 0.5f);
			}
		}
	}

	public void OnMoveHorizontal(float value) {
		_rb.AddForce(transform.right * value * speed);
		if(_canJump) {
			_step_timer -= Mathf.Abs(value);
		}
	}

	public void OnMoveVertical(float value) {
		_rb.AddForce(transform.forward * value * speed);
		if(_canJump) {
			_step_timer -= Mathf.Abs(value);
		}
	}

	public void OnJump() {
		if (_canJump) {
			if(jump != null) GetComponent<AudioSource>().PlayOneShot(jump, 1f);
			_rb.AddForce(transform.up * 500);
		}
	}

	public void OnCollisionEnter (Collision col) {
		if(col.gameObject.tag != "Unjumpable") _canJump = true;
	}

	public void OnCollisionExit (Collision col) {
		_canJump = false;
	}
}
