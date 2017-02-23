using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public float lifetime = Mathf.Infinity;
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if(lifetime < 0) Destroy(this.gameObject);
	}

	void OnCollisionEnter(Collision col) {
		Destroy(this.gameObject);
	}
}
