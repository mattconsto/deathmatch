using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextNode : MonoBehaviour {
	public string text = "";
	public float  time = 1;

	public void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Player") col.gameObject.GetComponent<PlayerController>().SetMesage(text, time);
	}
}
