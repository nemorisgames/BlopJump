using UnityEngine;
using System.Collections;

public class GetReward : MonoBehaviour {

	RewardMachine rm;

	// Use this for initialization
	void Start () {
		rm = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ().rewardMachine;
	}
	
	// Update is called once per frame
	public void UnlockReward () {
		rm.GetReward ();
	}
}
