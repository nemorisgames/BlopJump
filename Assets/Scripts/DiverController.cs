using UnityEngine;
using System.Collections;

public class DiverController : MonoBehaviour {
	Vector3 initialPosition;
	Quaternion initialRotation;
	bool onGround = true;
	Rigidbody rb;

	public Vector3 jumpForce;
	public float normalSpinSpeed;
	public float trickSpinSpeed;
	public float normalHorizontalSpeed;
	public float trickHorizontalSpeed;

	float jumpStart;
	float jumpEnd;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		normalHorizontalSpeed = jumpForce.x / 50;
		trickHorizontalSpeed = normalHorizontalSpeed / 2;
		rb.maxAngularVelocity = 100;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (onGround && Input.GetKeyDown (KeyCode.Space)) {
			rb.AddForce (jumpForce);
			jumpStart = Time.time;
			Debug.Log (rb.velocity.x + " " + rb.velocity.y);
			rb.AddTorque (new Vector3(0f,0f,-normalSpinSpeed));
		};

		if (rb.position.y < -1) {
			onGround = true;
			jumpEnd = Time.time;
			Debug.Log (rb.position.x);
			Debug.Log (jumpEnd - jumpStart);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.rotation = initialRotation;
			rb.position = initialPosition;
		}

		if (!onGround) {
			Vector3 currentVelocity = rb.velocity;
			rb.angularVelocity = new Vector3 (0f, 0f, -normalSpinSpeed);
			rb.velocity = new Vector3 (normalHorizontalSpeed, currentVelocity.y, currentVelocity.z);
			if (Input.GetKey (KeyCode.Space)) {
				//rb.angularVelocity.Set (new Vector3(0f,0f,-15f));
				rb.angularVelocity = new Vector3 (0f, 0f, -trickSpinSpeed);
				rb.velocity = new Vector3 (trickHorizontalSpeed, currentVelocity.y, currentVelocity.z);
			};
		};

		if(rb.angularVelocity.z != 0)
			Debug.Log (rb.velocity.x);
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Blop"){
			onGround = true;
			Debug.Log (onGround);
		};
	}

	void OnTriggerExit(Collider other){
		if(other.tag == "Blop"){
			onGround = false;
			Debug.Log (onGround);
		};
	}
}
