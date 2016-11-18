using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform target;
	public Vector3 min;
	public Vector3 max;
	public Vector3 jumperOffset;
	public Vector3 diverOffset;
	public Vector3 platformOffset;
	public float depthOffset;
	MainController controller;
	GameController gameController;
	public float initialDepth;
	[HideInInspector]
	public bool follow;
	float dampTime = 0.15f;
	float moveSpeed = 0.5f;
	float slow = 0.1f;
	Vector3 velocity = Vector3.zero;
	Camera cam;

	void Awake(){
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
		jumperOffset = new Vector3 (-1.6f, 2.5f, 0f);
		follow = true;
		cam = Camera.main;
	}

	// Update is called once per frame
	void Update () {
		if (follow) {
			Vector3 newPos = target.position;
			if (newPos.y <= min.y) {
				newPos = new Vector3 (newPos.x, min.y, newPos.z);
			}
			float move = moveSpeed;
			if (target != null) {
				if (target.tag == "Diver") {
					//transform.position = new Vector3 (newPos.x + diverOffset.x, newPos.y + diverOffset.y, newPos.z + initialDepth - newPos.y / depthOffset);
					newPos = newPos + new Vector3 (diverOffset.x, diverOffset.y, initialDepth - newPos.y / depthOffset);
				}
				if (target.tag == "Jumper") {
					//transform.position = new Vector3 (newPos.x + jumperOffset.x + platformOffset.x, newPos.y + jumperOffset.y + platformOffset.y, newPos.z + initialDepth);
					newPos = newPos + new Vector3 (jumperOffset.x + platformOffset.x, jumperOffset.y + platformOffset.y, initialDepth);
				}
				if (target.tag == "LandingSpot") {
					newPos = newPos + new Vector3 (0f, diverOffset.y, initialDepth);
					move = slow;
				}
				Vector3 delta = newPos - cam.ViewportToWorldPoint (new Vector3 (0.5f, move, 0f));
				Vector3 destination = transform.position + delta;

				transform.position = Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime);
			}
		}
	}

	public void ChangeTarget(Transform newTarget){
		target = newTarget;
	}

	public void ChangeTargetV2(Transform newTarget){
		target = newTarget.GetComponentInChildren<Rigidbody> ().transform;
	}

	public void UpdateCamera(Platform p){
		platformOffset = new Vector3 (platformOffset.x, p.cameraOffset, platformOffset.z);
		min = new Vector3 (min.x, p.cameraMinHeight, min.z);
		diverOffset = new Vector3 (diverOffset.x, jumperOffset.y + platformOffset.y, diverOffset.z);
	}

	public IEnumerator CameraPan(Transform player, Transform landingSpot){
		gameController.waiting = false;
		follow = true;
		target = landingSpot;
		yield return new WaitForSeconds (1f);
		target = player;
		gameController.waiting = true;
	}
}
