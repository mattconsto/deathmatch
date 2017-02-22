using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class PlayerCountSelector : MonoBehaviour, IPointerClickHandler {
	public int number = 0;
	public GameController controller;

	public void OnPointerClick(PointerEventData ed) {
		controller.SelectPlayers(number);
	}
}
