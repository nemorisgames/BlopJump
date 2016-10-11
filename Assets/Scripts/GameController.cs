using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameController : MonoBehaviour 
{

	Transform diverPos;

	[Header("Diver")]
	public GameObject diverGameObject; //diver en uso
	public Vector3 diverJumpForce; //vector fuerza para el salto
	public float normalHorizontalSpeed; //velocidad horizontal normal del diver
	public float trickHorizontalSpeed; //velocidad horizontal en truco del diver
	public float verticalSpeed;

	Rigidbody diverRigidbody; //rigidbody del diver
	Diver diverProps; //diver (script) del diver (parámetros)
	GameObject diver;
	float jumpStart;
	float jumpEnd;
	bool controllingDiver;

	float flips = 0;
	float deltaRotation = 0;
	float currentRotation = 0;
	float WindupRotation = 0;

	[Header("Jumper")]
	public Vector3 jumperJumpForce;
	public GameObject jumperGameObject;

	GameObject jumper;
	bool controllingJumper;
	Rigidbody jumperRigidbody;
	Vector3 jumperPos;

	[Header("Platform")]
	public GameObject platformGameObject;

	GameObject platform;


	// Use this for initialization
	void Awake () 
	{
		SetUp ();
	}

	void FixedUpdate(){

		//when controlling Jumper
		if (controllingJumper && !controllingDiver) 
		{
			if (Input.GetKeyDown (KeyCode.Space)) 
			{
				jumperRigidbody.AddForce (jumperJumpForce);
				controllingJumper = false;
			}
		}

		//when controlling Diver
		if(!controllingJumper && controllingDiver){
			if (diverProps.onGround && Input.GetKeyDown (KeyCode.Space)) 
			{
				flips = 0;
				WindupRotation = 0;
				DiverJump (diverJumpForce);
			};
			if (!diverProps.onGround) 
			{
				SetDiverSpinSpeed (diverProps.spinSpeed, diverProps.trickSpinSpeed);
			};
		}

		//Cleanup
		if (diverRigidbody.position.y < -1) 
		{
			ResetDiverPositionRotation ();
			jumperRigidbody.position = jumperPos;
			controllingJumper = true;
			controllingDiver = false;
		}
	}

	void SetUp()
	{
		//Set up diver
		diverPos = transform.FindChild ("DiverPos").transform;

		diver = (GameObject)Instantiate(diverGameObject,diverPos.position,diverPos.rotation);
		diverRigidbody = diver.GetComponent<Rigidbody> ();
		diverProps = diver.GetComponent<Diver> ();
		normalHorizontalSpeed = diverJumpForce.x / 50;
		trickHorizontalSpeed = normalHorizontalSpeed / 2;
		diverRigidbody.maxAngularVelocity = 100;
		verticalSpeed = diverProps.trickSpinSpeed / 100;


		controllingDiver = false;

		//Set up platform
		platform = (GameObject)Instantiate(platformGameObject,platformGameObject.transform.position,Quaternion.identity);

		//Set up jumper
		jumperPos = platform.transform.FindChild ("JumperPos").position;
		jumper = (GameObject)Instantiate(jumperGameObject,jumperPos,Quaternion.identity);
		jumperRigidbody = jumper.GetComponent<Rigidbody> ();

		controllingJumper = true;
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
		diverProps.onGround = true;
		jumpEnd = Time.time;

		//Debug.Log (diverRigidbody.position.x);
		Debug.Log (jumpEnd - jumpStart);

		diverRigidbody.velocity = Vector3.zero;
		diverRigidbody.angularVelocity = Vector3.zero;
		diverRigidbody.rotation = diverPos.rotation;
		diverRigidbody.position = diverPos.position;
	}

	void DiverJump(Vector3 jumpForce)
	{
		diverRigidbody.AddForce (jumpForce);
		jumpStart = Time.time;
		//Debug.Log (diverRigidbody.velocity.x + " " + diverRigidbody.velocity.y);
	}

	void CalculateFlips()
	{
		deltaRotation = (currentRotation - diverProps.transform.eulerAngles.z);
		currentRotation = diverProps.transform.eulerAngles.z;
		if (deltaRotation >= 300) 
			deltaRotation -= 360;
		if (deltaRotation <= -300) 
			deltaRotation += 360;
		WindupRotation += (deltaRotation);

		flips = WindupRotation / 360;
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Jumper"){
			controllingDiver = true;
			DiverJump (diverJumpForce);
		};
	}
}
