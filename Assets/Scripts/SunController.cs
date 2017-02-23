using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {
	void Update () {
		/* Rotate the sun */
		transform.Rotate(Time.deltaTime, 0, 0);
	}
}
