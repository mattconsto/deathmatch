﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;
using UnityEngine.EventSystems;

/*
	Game Controller
*/
public class GameController : MonoBehaviour {
	public Camera[] titleCamera;
	public Camera spectatorCamera;
	public GameObject playerPrefab;
	public GameObject titlehud;
	public GameObject titleSelected;
	public GameObject pausehud;
	public GameObject pauseSelected;
	public GameObject tutorialStuff;
	public GameObject tutorialSpawn;
	public bool paused = false;
	private bool started = false;

	private GameObject[] _respawns;
	private PlayerID[] _ids = new PlayerID[] {PlayerID.One, PlayerID.Two, PlayerID.Three, PlayerID.Four};
	private bool _moveSelector = true;

	public void Start() {
		Time.timeScale = 1;

		/* Pick a camera for the title screen */
		for(int i = 0; i < titleCamera.Length; i++) titleCamera[i].gameObject.SetActive(false);
		titleCamera[Random.Range(0, titleCamera.Length)].gameObject.SetActive(true);

		/* Get a list of spawns and shuffle, to prevent people spawning on top of each other */
		_respawns = GameObject.FindGameObjectsWithTag("Respawn");
		if(_respawns.Length == 0) {
			/* Fallback if no spawns */
			_respawns = new GameObject[] {new GameObject()};
			_respawns[0].transform.position = new Vector3(0f, 2f, 0f);
		} else {
			_respawns = _respawns.OrderBy(x => Random.value).ToArray();
		}

	}

	public void Update() {
		if(!started) titleSelected.GetComponent<IPointerEnterHandler>().OnPointerEnter(null);
		if(paused)   pauseSelected.GetComponent<IPointerEnterHandler>().OnPointerEnter(null);
	}

	public IEnumerator WaitForRealSeconds(float time) {
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time) yield return null;
	}

	IEnumerator LateCall() {
		_moveSelector = false;
		yield return StartCoroutine(WaitForRealSeconds(0.2f));
		_moveSelector = true;
	}

	public void OnMoveVertical(float value) {
		if(!_moveSelector) return;

		if(!started) {
			if(value > 0) {
				if(titleSelected.GetComponent<Selectable>().navigation.selectOnUp != null) {
					titleSelected.GetComponent<IPointerExitHandler>().OnPointerExit(null);
					titleSelected = titleSelected.GetComponent<Selectable>().navigation.selectOnUp.gameObject;
				}
				StartCoroutine(LateCall());
			} else if(value < 0) {
				if(titleSelected.GetComponent<Selectable>().navigation.selectOnDown != null) {
					titleSelected.GetComponent<IPointerExitHandler>().OnPointerExit(null);
					titleSelected = titleSelected.GetComponent<Selectable>().navigation.selectOnDown.gameObject;
				}
				StartCoroutine(LateCall());
			}
		} else if(paused) {
			if(value > 0) {
				if(pauseSelected.GetComponent<Selectable>().navigation.selectOnUp != null) {
					pauseSelected.GetComponent<IPointerExitHandler>().OnPointerExit(null);
					pauseSelected = pauseSelected.GetComponent<Selectable>().navigation.selectOnUp.gameObject;
				}
				StartCoroutine(LateCall());
			} else if(value < 0) {
				if(pauseSelected.GetComponent<Selectable>().navigation.selectOnDown != null) {
					pauseSelected.GetComponent<IPointerExitHandler>().OnPointerExit(null);
					pauseSelected = pauseSelected.GetComponent<Selectable>().navigation.selectOnDown.gameObject;
				}
				StartCoroutine(LateCall());
			}
		}
	}

	public void OnMoveHorizontal(float value) {
		if(!_moveSelector && (!started && !paused)) return;

		if(value > 0) {
			if(titleSelected.GetComponent<Selectable>().navigation.selectOnRight != null) {
				titleSelected.GetComponent<IPointerExitHandler>().OnPointerExit(null);
				titleSelected = titleSelected.GetComponent<Selectable>().navigation.selectOnRight.gameObject;
			}
			StartCoroutine(LateCall());
		} else if(value < 0) {
			if(titleSelected.GetComponent<Selectable>().navigation.selectOnLeft != null) {
				titleSelected.GetComponent<IPointerExitHandler>().OnPointerExit(null);
				titleSelected = titleSelected.GetComponent<Selectable>().navigation.selectOnLeft.gameObject;
			}
			StartCoroutine(LateCall());
		}
	}

	public void OnSelect() {
		if(!started) {
			titleSelected.GetComponent<GlobalSoundSource>().OnPointerClick(null);
			titleSelected.GetComponent<IPointerClickHandler>().OnPointerClick(null);
		} else if(paused) {
			pauseSelected.GetComponent<GlobalSoundSource>().OnPointerClick(null);
			pauseSelected.GetComponent<IPointerClickHandler>().OnPointerClick(null);
		}
	}

	public void OnPause() {
		// Pause menu
		if(started) {
			paused = !paused;
			Time.timeScale = paused ? 0 : 1;
			pausehud.SetActive(paused);
			Cursor.visible = paused;
			Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
		}
	}

	public void respawnPlayer(GameObject player) {
		int spawn = Random.Range(0, _respawns.Length);
		player.transform.position = _respawns[spawn].transform.position;
		player.transform.rotation = _respawns[spawn].transform.rotation;
	}

	public void SelectPlayers(int number) {
		/* Init */
		for(int i = 0; i < titleCamera.Length; i++) titleCamera[i].gameObject.SetActive(false);
		titlehud.gameObject.SetActive(false);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Tutorial Respawn
		if(number == 1) {
			_respawns = new GameObject[] {tutorialSpawn};
			tutorialStuff.SetActive(true);
		} else {
			tutorialStuff.SetActive(false);
		}

		GameObject[] players = new GameObject[number];

		/* Calculate grid size, biased towards height */
		int width = Mathf.RoundToInt(Mathf.Sqrt(number)), height = Mathf.CeilToInt(Mathf.Sqrt(number));

		print(string.Format("Starting a {0} player game, grid size {1}x{2}", number, width, height));

		for(int i = 0; i < number; i++) {
			/* Spawn players with the correct cameras */
			int x = i % width, y = i / width;

			players[i] = Instantiate(playerPrefab, _respawns[i % _respawns.Length].transform.position, _respawns[i % _respawns.Length].transform.rotation);
			players[i].GetComponent<PlayerController>().controller = this;
			players[i].GetComponent<PlayerController>().color = Color.HSVToRGB((1.0f * i / (number + 1) + Random.value / (number + 1)) % 1, 0.75f, 0.75f);
			Transform pc = players[i].transform.Find("Camera/CameraObject");
			pc.GetComponent<Camera>().rect = new Rect(1f * x / width, (height - 1f) / height - 1f * y / height, 1f / width, 1f / height);

			// Input
			for(int j = 0; j < players[i].GetComponent<InputEventManager>().EventCount; j++)
				players[i].GetComponent<InputEventManager>().GetEvent(j).playerID = _ids[i % _ids.Length];

			CanvasScaler scaler = players[i].transform.Find("Player HUD").GetComponent<CanvasScaler>();
			scaler.scaleFactor = Screen.currentResolution.width / scaler.referenceResolution.x / Mathf.Max(width, height) / 2;

			if(number == 1) {
				players[i].GetComponent<PlayerController>().guns.RemoveAt(0);
				players[i].GetComponent<PlayerController>().guns.RemoveAt(0);
				players[i].GetComponent<PlayerController>().guns.RemoveAt(0);
			}
		}

		players[0].transform.Find("Camera/CameraObject").GetComponent<AudioListener>().enabled = true;

		/* Add the spectator camera if needed */
		if(number < width * height) {
			print("Using Spectator Camera");
			spectatorCamera.gameObject.SetActive(true);
			spectatorCamera.GetComponent<Camera>().rect = new Rect(1f * (number % width) / width, 0f, 1f * (width * height - number) / width, 1f / height);
		}

		started = true;
		
		// #if !UNITY_EDITOR
		// 	transform.Find("Music").GetComponent<AudioSource>().Play();
		// #endif
	}
}
