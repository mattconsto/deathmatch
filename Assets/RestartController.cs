using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RestartController : MonoBehaviour, IPointerClickHandler {
	public void OnPointerClick(PointerEventData ed) {
		Application.LoadLevel(Application.loadedLevel);
	}
}
