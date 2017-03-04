using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {
	public void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Projectiles") gameObject.SetActive(false);
	}
}
