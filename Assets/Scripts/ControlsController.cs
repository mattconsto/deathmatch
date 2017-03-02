using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsController : MonoBehaviour, IPointerClickHandler {
	public GameObject target;

	public void OnPointerClick(PointerEventData ed) {
		target.SetActive(!target.activeSelf);
	}
}
