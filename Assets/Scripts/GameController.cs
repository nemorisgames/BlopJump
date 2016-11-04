using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameController : MonoBehaviour 
{

	Transform diverPos;
	Camera cam;

	public bool testing;
	[HideInInspector]
	public bool playing = false;
	[HideInInspector]
	public bool waiting = true;

	public float windForce;
	public float jumpHeightCompensate;

	[Header("Diver")]
	public GameObject diverGameObject; //diver en uso
	public Vector3 diverJumpForce; //vector fuerza para el salto
	public float normalHorizontalSpeed; //velocidad horizontal normal del diver
	public float trickHorizontalSpeed; //velocidad horizontal en truco del diver
	public float verticalSpeed;

	Rigidbody diverRigidbody; //rigidbody del diver
	Diver diverProps; //propiedades del diver
	GameObject diver;
	bool controllingDiver;

	//temp
	float jumpStart;
	float jumpEnd;
	float flips = 0;
	float deltaRotation = 0;
	float currentRotation = 0;
	float WindupRotation = 0;
	//temp

	[Header("Jumper")]
	public GameObject jumperGameObject;
	public Vector3 jumperJumpForce;
	public float compensateWeightHorizontal;
	public float compensateWeightVertical;


	GameObject jumper;
	bool controllingJumper;
	Rigidbody jumperRigidbody;
	Jumper jumperProps;
	Vector3 jumperPos;

	[Header("Platform")]
	public GameObject platformGameObject;
	public float compensatePlatformHeight;

	GameObject platform;
	Platform platformProps;
	bool enableWind;

	// Use this for initialization
	void Awake () 
	{
		cam = Camera.main;
		Setup ();
	}

	void FixedUpdate(){
		if (waiting) {
			//when controlling Jumper
			if (controllingJumper && !controllingDiver) {
				if (Input.GetKeyDown (KeyCode.Space)) {
					jumperRigidbody.AddForce (jumperJumpForce);
					controllingJumper = false;
					waiting = false;
					playing = true;
				}
			}
		}

		if (playing) 
		{
			//when controlling Diver
			if (!controllingJumper && controllingDiver) {
				if (diverProps.onGround) {
					flips = 0;
					WindupRotation = 0;
					if (Input.GetKeyDown (KeyCode.Space) && testing)
						DiverJump (diverJumpForce);
				}

				if (!diverProps.onGround) {
					SetDiverSpinSpeed (diverProps.spinSpeed, diverProps.trickSpinSpeed);
				}

			}

			if(enableWind)
				diverRigidbody.AddForce(new Vector3(-windForce,0f,0f));

			//Cleanup
		}
	}

	void LateUpdate(){
		if (diverRigidbody.position.y < -1) {
			ResetRound ();
			jumperRigidbody.position = jumperPos;
			controllingJumper = true;
			controllingDiver = false;
			waiting = true;
			playing = false;
			cam.GetComponent<CameraController> ().target = jumper.transform;
		}
	}

	public void Setup()
	{
		if (diver != null) 
		{
			Destroy (diver);
		}

		if(jumper != null)
		{
			Destroy(jumper);
		}

		if (platform != null) 
		{
			Destroy (platform);
		}

		enableWind = false;

		//Set up diver
		diverPos = transform.FindChild ("DiverPos").transform;

		diver = (GameObject)Instantiate(diverGameObject,diverPos.position,diverPos.rotation);
		diverRigidbody = diver.GetComponent<Rigidbody> ();
		diverProps = diver.GetComponent<Diver> ();
		//diverRigidbody = diver.transform.GetComponentInChildren<Rigidbody> ();
		//diverProps = diver.transform.GetComponentInChildren<Diver> ();
		normalHorizontalSpeed = diverJumpForce.x / 50;
		trickHorizontalSpeed = normalHorizontalSpeed / 2;
		diverRigidbody.maxAngularVelocity = 100;
		verticalSpeed = diverProps.trickSpinSpeed / 120;


		controllingDiver = false;

		//Set up platform
		platform = (GameObject)Instantiate(platformGameObject,platformGameObject.transform.position,Quaternion.identity);
		platformProps = platform.GetComponent<Platform> ();

		//Set up jumper
		jumperPos = platform.transform.FindChild ("JumperPos").position;
		jumper = (GameObject)Instantiate(jumperGameObject,jumperPos,Quaternion.identity);
		jumperRigidbody = jumper.GetComponent<Rigidbody> ();
		jumperProps = jumper.GetComponent<Jumper> ();

		controllingJumper = true;
		cam.GetComponent<CameraController> ().target = jumper.transform;
		//cam.gameObject.GetComponent<GenericMoveCamera> ().LookAtTarget = diver;
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
			diverRigidbody.AddForce(new Vector3(0,-jumpHeightCompensate,0));
		};

	}

	void ResetRound()
	{
		Debug.Log ("Flips: "+Mathf.Round (flips));
		//Debug.Log ("Flips: "+(flips%0.5f));
		jumpEnd = Time.time;
		flips = 0;
		WindupRotation = 0;
		enableWind = false;

		//Debug.Log (diverRigidbody.position.x);
		Debug.Log ("Time: "+(jumpEnd - jumpStart));

		diverRigidbody.velocity = Vector3.zero;
		diverRigidbody.angularVelocity = Vector3.zero;
		diverRigidbody.rotation = diverPos.rotation;
		diverRigidbody.position = diverPos.position;
	}

	void DiverJump(Vector3 jumpForce)
	{
		Vector3 jump = new Vector3 (jumpForce.x + jumperProps.weight/compensateWeightHorizontal,jumpForce.y * jumperProps.weight/compensateWeightVertical + compensatePlatformHeight *platformProps.height, jumpForce.z);
		diverRigidbody.AddForce (jump);
		jumpStart = Time.time;
		cam.GetComponent<CameraController> ().target = diver.transform;
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

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Jumper")
		{
			controllingDiver = true;
			DiverJump (diverJumpForce);
			enableWind = true;
		};

		if(other.tag == "Diver")
		{
			diverProps.onGround = true;
		};
	}

	void OnTriggerStay(Collider other)
	{
		if(other.tag == "Diver")
		{
			diverProps.onGround = true;
		};
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Diver")
		{
			diverProps.onGround = false;
		};
	}
}
