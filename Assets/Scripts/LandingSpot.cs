using UnityEngine;
using System.Collections;

public class LandingSpot : MonoBehaviour {

	private bool _enabled;
	GameController gameController;
	Animator anim;

	public float[,] minDistance = new float[,]{
		{25.51f,25.1f,24.67f,25.51f,24.11f},
		{25.22f,24.88f,24.32f,24.004f,23.2f},
		{23.46f,23.13f,23.16f,22.94f,22.91f},
		{23.15f,23.23f,23.11f,22.49f,20.76f},
		{22.66f,22.02f,21.15f,20.95f,20.95f}};
	public float[,] maxDistance = new float[,] { 
		{16.19f,15.76f,15.1f,14.81f,12.45f},
		{14.09f,13.83f,12.92f,12.57f,10.4f},
		{11.56f,10.98f,10.26f,9.86f,7.74f},
		{9.08f,9.71f,7.92f,7.35f,5.23f},
		{7.49f,6.99f,6.15f,5.6f,2.05f}};

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
				if (c.transform.position.x > transform.position.x - gameController.LandingSpotExtent ())
					gameController.GoodJump (true);
				else
					gameController.GoodJump (false);
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
