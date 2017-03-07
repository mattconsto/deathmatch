using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeout : MonoBehaviour {
	public float lifetime = 5.0f;

	private ParticleSystem _ps = null;
	private bool _emitting = true;

	public void Start() {
		_ps = GetComponent<ParticleSystem>();
	}
	
	public void Update () {
		lifetime -= Time.deltaTime;

		// Stop emissions 1s before deleting the object, so it looks better. 
		if (_emitting && lifetime < 1 && _ps != null) {
			var emission = _ps.emission;
			emission.rateOverTime = 0;
			_emitting = false;
		}

		if (lifetime < 0) Destroy(gameObject);
	}
}
