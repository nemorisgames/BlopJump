using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	public int value;
	MainController controller;
    Transform t;

	void Awake(){
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController>();
        t = GetComponent<Transform>();
	}

    void Update()
    {
        t.Rotate(Vector3.right * 50  * Time.deltaTime);
    }
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Diver") {
			controller.coins += value;
			//StartCoroutine (controller.PlusCoin (value));
			Destroy (this.gameObject);
		}
	}
}
