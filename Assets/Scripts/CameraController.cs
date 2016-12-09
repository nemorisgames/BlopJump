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
	float dampTime = 0.22f;
	float moveSpeed = 0.2f;
	float slow = 0.001f;
	Vector3 velocity = Vector3.zero;
	Camera cam;
	public bool platformViewToggled = false;
	GameObject platformButton;
	UILabel platformButtonLabel;

	void Awake(){
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
		jumperOffset = new Vector3 (-1.6f, 2.5f, 0f);
		follow = true;
		cam = Camera.main;
		platformButton = GameObject.Find ("PlatformButton");
		platformButtonLabel = platformButton.GetComponentInChildren<UILabel> ();
		platformButton.SetActive (false);
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
					if(gameController.controllingDiver)
						newPos = newPos + new Vector3 (diverOffset.x, diverOffset.y, initialDepth - newPos.y / depthOffset);
					else
						newPos = newPos + new Vector3 (diverOffset.x, diverOffset.y, initialDepth - depthOffset);
				}
				if (target.tag == "Jumper") {
					//transform.position = new Vector3 (newPos.x + jumperOffset.x + platformOffset.x, newPos.y + jumperOffset.y + platformOffset.y, newPos.z + initialDepth);
					newPos = newPos + new Vector3 (jumperOffset.x + platformOffset.x, jumperOffset.y + platformOffset.y, initialDepth);
				}
				if (target.tag == "LandingSpot") {
					newPos = newPos + new Vector3 (0f, diverOffset.y, initialDepth - depthOffset);
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

	public IEnumerator CameraPan(Transform end, Transform mid, Transform start){
		platformButton.SetActive (false);
		gameController.waiting = false;
		depthOffset = -8f;
		follow = true;
		target = start;
		yield return new WaitForSeconds (1.5f);
		target = mid;
		yield return new WaitForSeconds (1f);
		target = end;
		depthOffset = -0.7f;
		gameController.waiting = true;
		platformButton.SetActive (true);
		gameController.jumpBar.Initialize ();
		controller.ToggleButtons (false);
	}

	public IEnumerator CameraPan(Transform end, float endDepth){
		platformButton.SetActive (false);
		gameController.waiting = false;
		follow = true;
		yield return new WaitForSeconds (0.05f);
		target = end;
		depthOffset = endDepth;
		gameController.waiting = true;
		platformButton.SetActive (true);
	}

	public void TogglePlatformView(){
		Transform jumper = gameController.GetJumper().transform;
		Transform landingSpot = gameController.GetLandingSpot().transform;
		if (platformViewToggled) {
			platformButtonLabel.text = "Show Platform";
			//target = jumper;
			StartCoroutine(CameraPan(jumper,-0.7f));
			platformViewToggled = false;
		} else {
			platformButtonLabel.text = "Back to Jumper";
			//target = landingSpot;
			StartCoroutine(CameraPan(landingSpot,-8f));
			platformViewToggled = true;
		}
	}

	public void TogglePlatformButton(){
		if (platformButton.activeSelf)
			platformButton.SetActive (false);
		else
			platformButton.SetActive (true);
	}
}
