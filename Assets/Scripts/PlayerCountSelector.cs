using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCountSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
	public int number = 0;
	public GameController controller;

	public void OnPointerEnter(PointerEventData ed) {
		GetComponent<Text>().color = Color.white;
	}

	public void OnPointerExit(PointerEventData ed) {
		GetComponent<Text>().color = new Color(0.804f, 0.741f, 0.678f, 1f);
	}

	public void OnPointerClick(PointerEventData ed) {
		controller.SelectPlayers(number);
	}
}
