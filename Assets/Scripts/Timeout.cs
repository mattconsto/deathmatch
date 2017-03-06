using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeout : MonoBehaviour {
	public float lifetime = 5.0f;

	private ParticleSystem _ps = null;

	public void Start() {
		_ps = GetComponent<ParticleSystem>();
	}
	
	public void Update () {
		lifetime -= Time.deltaTime;
		if (lifetime < 1) {
			var emission = _ps.emission;
			var rate = emission.rate;
			rate.constantMax = 0;
			emission.rate = rate;
		}
		if (lifetime < 0) Destroy(gameObject);
	}
}
