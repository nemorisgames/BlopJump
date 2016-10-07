using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameController : MonoBehaviour 
{

	[Header("Diver")]
	public GameObject diverGameObject; //diver en uso
	public Vector3 jumpForce; //vector fuerza para el salto
	public float normalHorizontalSpeed; //velocidad horizontal normal del diver
	public float trickHorizontalSpeed; //velocidad horizontal en truco del diver
	public float verticalSpeed;

	Vector3 diverInitialPosition; //posicion inicial del diver
	Quaternion diverInitialRotation; //rotacion inicial del diver
	Rigidbody diverRigidbody; //rigidbody del diver
	Diver diver; //diver (script) del diver (parámetros)
	float jumpStart;
	float jumpEnd;

	float flips = 0;
	float deltaRotation = 0;
	float currentRotation = 0;
	float WindupRotation = 0;

	// Use this for initialization
	void Start () 
	{
		diverRigidbody = diverGameObject.GetComponent<Rigidbody> ();
		diver = diverGameObject.GetComponent<Diver> ();
		diverInitialPosition = diverRigidbody.position;
		diverInitialRotation = diverRigidbody.rotation;
		normalHorizontalSpeed = jumpForce.x / 50;
		trickHorizontalSpeed = normalHorizontalSpeed / 2;
		diverRigidbody.maxAngularVelocity = 100;
		verticalSpeed = diver.trickSpinSpeed / 100;
	}

	void FixedUpdate(){
		if (diver.onGround && Input.GetKeyDown (KeyCode.Space)) 
		{
			flips = 0;
			WindupRotation = 0;
			DiverJump (jumpForce);
		};

		if (diverRigidbody.position.y < -1) 
		{
			ResetDiverPositionRotation ();
		}

		if (!diver.onGround) 
		{
			SetDiverSpinSpeed (diver.spinSpeed, diver.trickSpinSpeed);
		};

	}

	void Update()
	{
		
	}

	void SetDiverSpinSpeed(float normalSpin, float trickSpin)
	{
		CalculateFlips ();
		Vector3 currentVelocity = diverRigidbody.velocity;
		float verticalVelocity = currentVelocity.y - verticalSpeed;
		diverRigidbody.angularVelocity = new Vector3 (0f, 0f, -normalSpin);
		diverRigidbody.velocity = new Vector3 (normalHorizontalSpeed, currentVelocity.y, currentVelocity.z);
		if (Input.GetKey (KeyCode.Space)) 
		{
			//rb.angularVelocity.Set (new Vector3(0f,0f,-15f));
			diverRigidbody.angularVelocity = new Vector3 (0f, 0f, -trickSpin);
			diverRigidbody.velocity = new Vector3 (trickHorizontalSpeed, verticalVelocity, currentVelocity.z);
		};
	}

	void ResetDiverPositionRotation()
	{
		Debug.Log (Mathf.Round (flips));
		diver.onGround = true;
		jumpEnd = Time.time;

		//Debug.Log (diverRigidbody.position.x);
		Debug.Log (jumpEnd - jumpStart);

		diverRigidbody.velocity = Vector3.zero;
		diverRigidbody.angularVelocity = Vector3.zero;
		diverRigidbody.rotation = diverInitialRotation;
		diverRigidbody.position = diverInitialPosition;
	}

	void DiverJump(Vector3 jumpForce)
	{
		diverRigidbody.AddForce (jumpForce);
		jumpStart = Time.time;
		//Debug.Log (diverRigidbody.velocity.x + " " + diverRigidbody.velocity.y);
	}

	void CalculateFlips()
	{
		deltaRotation = (currentRotation - diver.transform.eulerAngles.z);
		currentRotation = diver.transform.eulerAngles.z;
		if (deltaRotation >= 300) 
			deltaRotation -= 360;
		if (deltaRotation <= -300) 
			deltaRotation += 360;
		WindupRotation += (deltaRotation);

		flips = WindupRotation / 360;
	}
}
