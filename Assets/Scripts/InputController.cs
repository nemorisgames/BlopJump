using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	UIButton button;
	MainController controller;
	GameController gameController;
	CameraController cameraController;


	// Use this for initialization
	void Start () {
		button = GetComponent<UIButton> ();
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
		cameraController = Camera.main.GetComponent<CameraController> ();
	}

	public void Clicked(){
		if (controller.tutorialScreen.value > 0f) {
			controller.tutorialScreen.PlayReverse ();
			//gameController.Setup ();
			gameController.ResetRound ();
			controller.ToggleButtons (false);
		} 
		if (gameController.waiting && gameController.controllingJumper && !gameController.controllingDiver) {
			if (cameraController.target == gameController.GetJumper ().transform) {
				gameController.JumperJump ();
			} else {
				cameraController.TogglePlatformView ();
			}
		} else {
			controller.CloseWindow ();
		}
	}
}
