using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

/*
	Load the appropriate Gamepad bindings
*/
public class InputManagerOSDetector : MonoBehaviour {
	void Start () {
		Debug.Log("Found OS: " + SystemInfo.operatingSystemFamily);

		InputManager manager = GetComponent<InputManager>();
		manager.playerOneDefault = "KeyboardAndMouse";

		switch(SystemInfo.operatingSystemFamily) {
			case OperatingSystemFamily.Windows:
				manager.playerTwoDefault   = "Windows_Gamepad1";
				manager.playerThreeDefault = "Windows_Gamepad2";
				manager.playerFourDefault  = "Windows_Gamepad3";
				break;
			case OperatingSystemFamily.MacOSX:
				manager.playerTwoDefault   = "OSX_Gamepad1";
				manager.playerThreeDefault = "OSX_Gamepad2";
				manager.playerFourDefault  = "OSX_Gamepad3";
				break;
			default:
			case OperatingSystemFamily.Linux:
				manager.playerTwoDefault   = "Linux_Gamepad1";
				manager.playerThreeDefault = "Linux_Gamepad2";
				manager.playerFourDefault  = "Linux_Gamepad3";
				break;
		}
		manager.Initialize();
	}
}
