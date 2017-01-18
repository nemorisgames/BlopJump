using UnityEngine;
using System.Collections;
using BlopJump;

public class Coin : MonoBehaviour {

	public int value;
	MainController controller;
    Transform t;

	void Awake(){
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController>();
        t = GetComponent<Transform>();
		t.Rotate(0f, Random.Range(0f, 180f), 90f);
	}

    void Update()
    {
        t.Rotate(Vector3.right * 100  * Time.deltaTime);
    }
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Diver" || other.tag == "Foot" || other.tag == "Arm") {
			//controller.coins += value;
			//StartCoroutine (controller.PlusCoin (value));
			Destroy (this.gameObject);
		}
	}
}
