using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeout : MonoBehaviour {

	public float lifetime = 5.0f;

	private float passed = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		passed += Time.deltaTime;
		if (passed > lifetime) {
			Destroy(gameObject);
		}
	}
}
