using UnityEngine;
using System.Collections;

public class ScreenInput : MonoBehaviour {

	MainController controller;
	GameController gameController;
	RewardMachine rewardController;

	// Use this for initialization
	void Awake () {
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController>();
		rewardController = GameObject.FindGameObjectWithTag ("RewardMachine").GetComponent<RewardMachine> ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
	}

	public void CheckClick(){
		if(Input.GetMouseButtonDown(0)){
			Debug.Log("hola");
		}
	}

	bool CheckScreenVisibility(){
		if (!controller.selectScreenVisible && !rewardController.rewardScreenVisible && !gameController.endRoundScreenVisible) {
			return true;
		} else
			return false;
	}
}
