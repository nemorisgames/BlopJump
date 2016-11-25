using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	public int value;
	MainController controller;

	void Awake(){
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController>();
	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Diver") {
			controller.coins += value;
			//StartCoroutine (controller.PlusCoin (value));
			Destroy (this.gameObject);
		}
	}
}
