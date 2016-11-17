using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	UIButton button;
	bool pressed;
	MainController controller;
	GameController gameController;

	// Use this for initialization
	void Start () {
		button = GetComponent<UIButton> ();
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
	}
	
	// Update is called once per frame
	void Update () {
		/*if (gameController.playing && !gameController.diverProps.onGround) {
			if(button.state == UIButtonColor.State.Pressed){
				gameController.DiverTrickSpin ();
			}
			else {
				gameController.DiverNormalSpin ();
			}
		} */
	}

	public void Clicked(){
		if (gameController.waiting && gameController.controllingJumper && !gameController.controllingDiver) {
			gameController.JumperJump ();
		} else {
			controller.CloseWindow ();
		}
	}
}
