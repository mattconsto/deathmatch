using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResumeController : MonoBehaviour, IPointerClickHandler {
	public GameObject controller;

	public void OnPointerClick(PointerEventData ed) {
		controller.GetComponent<GameController>().paused = false;
		controller.GetComponent<GameController>().pausehud.SetActive(false);
		Time.timeScale = 1;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}
