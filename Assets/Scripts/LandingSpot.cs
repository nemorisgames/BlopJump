using UnityEngine;
using System.Collections;

public class LandingSpot : MonoBehaviour {

	private bool _enabled;
	GameController gameController;
	Animator anim;

	// Use this for initialization
	void Awake(){
		_enabled = true;
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
	}

	void OnTriggerEnter(Collider c)
	{
		anim = gameController.ReturnDiver().GetComponent<Animator> ();
		if (_enabled && c.tag != "CoinArea")
		{
			if(anim.GetBool("Spinning"))
			{
				gameController.GoodJump (false);
			}
			else if (c.tag == "Diver") 
			{
				gameController.GoodJump (false);
			}
			else if (c.tag == "Arm" || c.tag == "Foot") 
			{
				gameController.GoodJump (true);
			} 
		}
		_enabled = false;
	}

	public void enableLanding(bool b){
		_enabled = b;
	}

	public bool getLanding(){
		return _enabled;
	}
}
