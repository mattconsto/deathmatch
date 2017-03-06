using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextNode : MonoBehaviour {
	public string text = "";
	public float  time = 1;
	public int    uses = -1;

	private AudioSource _source;

	public void Start() {
		_source = GetComponent<AudioSource>();
	}

	public void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Player" && uses != 0) {
			col.gameObject.GetComponent<PlayerController>().SetMesage(text, time);
			if(_source != null) _source.Play();
			if(uses > 0) uses--;
		}
	}
}
