using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform target;
	public Vector3 min;
	public Vector3 max;
	public Vector3 jumperOffset;
	public Vector3 diverOffset;

	// Update is called once per frame
	void Update () {
		Vector3 newPos = target.position;
		if (newPos.y <= min.y) {
			newPos = new Vector3 (newPos.x, min.y, newPos.z);
		}
		if (target != null) {
			if(target.tag == "Diver")
				transform.position = new Vector3 (newPos.x + diverOffset.x, newPos.y + diverOffset.y, transform.position.z);
			if(target.tag == "Jumper")
				transform.position = new Vector3 (newPos.x + jumperOffset.x, newPos.y + jumperOffset.y, transform.position.z);
		}
	}

	public void ChangeTarget(Transform newTarget){
		target = newTarget;
	}
}
