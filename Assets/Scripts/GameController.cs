﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public Camera titleCamera;
	public Camera spectatorCamera;
	public GameObject playerPrefab;
	public GameObject titlehud;

	void Start () {
		titleCamera.gameObject.SetActive(true);
	}

	public void SelectPlayers(int number) {
		/* Init */
		titleCamera.gameObject.SetActive(false);
		titlehud.gameObject.SetActive(false);
		Cursor.visible = false;

		GameObject[] players = new GameObject[number];

		/* Calculate grid size, biased towards height */
		int width = Mathf.RoundToInt(Mathf.Sqrt(number)), height = Mathf.CeilToInt(Mathf.Sqrt(number));

		for(int i = 0; i < number; i++) {
			/* Spawn players with the correct cameras */
			int x = i % width, y = i / width;
			players[i] = Instantiate(playerPrefab, new Vector3(10, 10, 10), transform.rotation);
			Transform pc = players[i].transform.Find("Camera");
			pc.GetComponent<AudioListener>().enabled = false;
			pc.GetComponent<Camera>().rect = new Rect(1f * x / width, (height - 1f) / height - 1f * y / height, 1f / width, 1f / height);
		}

		/* Add the spectator camera if needed */
		if(number < width * height) {
			print("Using Spectator Camera");
			spectatorCamera.gameObject.SetActive(true);
			spectatorCamera.GetComponent<Camera>().rect = new Rect(1f * (number % width) / width, 0f, 1f * (width * height - number) / width, 1f / height);
		}
	}
}