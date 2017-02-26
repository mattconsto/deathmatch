using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

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
		PlayerID[] ids = new PlayerID[] {PlayerID.One, PlayerID.Two, PlayerID.Three, PlayerID.Four};
		GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");
		if(respawns.Length == 0) {
			respawns = new GameObject[] {new GameObject()};
			respawns[0].transform.position = new Vector3(0f, 2f, 0f);
		} else {
			respawns = respawns.OrderBy(x => Random.value).ToArray();
		}

		/* Calculate grid size, biased towards height */
		int width = Mathf.RoundToInt(Mathf.Sqrt(number)), height = Mathf.CeilToInt(Mathf.Sqrt(number));

		print(string.Format("Starting a {0} player game, grid size {1}x{2}", number, width, height));

		for(int i = 0; i < number; i++) {
			/* Spawn players with the correct cameras */
			int x = i % width, y = i / width;

			players[i] = Instantiate(playerPrefab, respawns[i % respawns.Length].transform.position, respawns[i % respawns.Length].transform.rotation);
			Transform pc = players[i].transform.Find("Camera/CameraObject");
			// pc.GetComponent<AudioListener>().enabled = false;//TODO: ONE AUDIO LISTENER!
			pc.GetComponent<Camera>().rect = new Rect(1f * x / width, (height - 1f) / height - 1f * y / height, 1f / width, 1f / height);

			// Input
			for(int j = 0; j < players[i].GetComponent<InputEventManager>().EventCount; j++) {
				players[i].GetComponent<InputEventManager>().GetEvent(j).playerID = ids[i % ids.Length];
			}

			CanvasScaler scaler = players[i].transform.Find("Player HUD").GetComponent<CanvasScaler>();
			scaler.scaleFactor = Screen.currentResolution.width / scaler.referenceResolution.x / Mathf.Max(width, height) / 2;
		}

		/* Add the spectator camera if needed */
		if(number < width * height) {
			print("Using Spectator Camera");
			spectatorCamera.gameObject.SetActive(true);
			spectatorCamera.GetComponent<Camera>().rect = new Rect(1f * (number % width) / width, 0f, 1f * (width * height - number) / width, 1f / height);
		}
	}
}
