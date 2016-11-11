using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameController : MonoBehaviour 
{

	Transform diverPos;
	Camera cam;
	MainController controller;
	GameObject endRoundScreen;
	[HideInInspector]
	public bool endRoundScreenVisible;

	public bool testing;
	[HideInInspector]
	public bool playing = false;
	[HideInInspector]
	public bool waiting = true;

	public float windForce;
	public float jumpHeightCompensate;
	public int coinsPerFlip;
	public int numCoins;
	public float coinSpacing;
	GameObject coinArea;
	GameObject coin;

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
	bool goodJump;

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
	public bool controllingJumper;
	Rigidbody jumperRigidbody;
	Jumper jumperProps;
	Vector3 jumperPos;

	[Header("Platform")]
	public GameObject platformGameObject;
	public float compensatePlatformHeight;

	GameObject platform;
	Platform platformProps;
	bool enableWind;
	UILabel endRoundFlips;
	UILabel endRoundCoins;
	UILabel endRoundJump;
	LandingSpot landingSpot;
	//CapsuleCollider diverCollider;

	// Use this for initialization
	void Awake () 
	{
		cam = Camera.main;
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
		endRoundScreen = GameObject.Find ("EndRoundScreen");
		endRoundScreen.SetActive (false);
		endRoundScreenVisible = false;
		endRoundFlips = endRoundScreen.transform.FindChild ("EndFlips").GetComponent<UILabel> ();
		endRoundCoins = endRoundScreen.transform.FindChild ("EndCoins").GetComponent<UILabel> ();
		endRoundJump = endRoundScreen.transform.FindChild ("EndRoundText").GetComponent<UILabel> ();
		coinArea = GameObject.Find ("CoinArea");
		coin = GameObject.Find ("Coin");
		landingSpot = GameObject.FindGameObjectWithTag ("LandingSpot").GetComponent<LandingSpot> ();
		Setup ();
	}

	void FixedUpdate()
	{
		if (waiting) {
			//when controlling Jumper
			diver.GetComponent<Animator> ().SetBool ("Diving", false);
			if (controllingJumper && !controllingDiver) {
				if (Input.GetKeyDown (KeyCode.Space)) { //
					//diver.GetComponent<Animator> ().SetTrigger ("Jump");
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
					diver.GetComponent<Animator> ().SetBool ("Diving", true);
					//diverCollider.radius = 0.55f;
					SetDiverSpinSpeed (diverProps.spinSpeed, diverProps.trickSpinSpeed);
				}

			}

			if(enableWind)
				diverRigidbody.AddForce(new Vector3(-windForce,0f,0f));

			//Cleanup
		}
	}

	void LateUpdate()
	{
		if (diverRigidbody.position.y < -1) {
			if (landingSpot.getLanding ())
				goodJump = false;
			ToggleEndRoundScreen ();
			//ResetRound ();
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
		cam.GetComponent<CameraController> ().UpdateCamera (platformProps);
		jumperJumpForce = new Vector3 (platformProps.jumpForceX, jumperJumpForce.y, jumperJumpForce.z);
		//cam.gameObject.GetComponent<GenericMoveCamera> ().LookAtTarget = diver;

		GenerateCoins (numCoins);
		ResetRound ();
	}

	void SetDiverSpinSpeed(float normalSpin, float trickSpin)
	{
		CalculateFlips ();
		Vector3 currentVelocity = diverRigidbody.velocity;
		float verticalVelocity = currentVelocity.y - verticalSpeed;
		diverRigidbody.angularVelocity = new Vector3 (0f, 0f, -normalSpin);
		diverRigidbody.velocity = new Vector3 (normalHorizontalSpeed, currentVelocity.y, currentVelocity.z);
		if (Input.GetKey (KeyCode.Space)) {
			//rb.angularVelocity.Set (new Vector3(0f,0f,-15f));
			diver.GetComponent<Animator> ().SetBool ("Spinning", true);
			diverRigidbody.angularVelocity = new Vector3 (0f, 0f, -trickSpin);
			diverRigidbody.velocity = new Vector3 (trickHorizontalSpeed, verticalVelocity, currentVelocity.z);
			diverRigidbody.AddForce (new Vector3 (0, -jumpHeightCompensate, 0));
			//diverCollider.radius = 0.85f;
		} else {
			diver.GetComponent<Animator> ().SetBool ("Spinning", false);
		}

	}

	public void ResetPosition(){
		diver.GetComponent<Animator> ().SetBool ("Spinning", false);
		diver.GetComponent<Animator> ().SetBool ("Diving", false);
		//diverCollider.radius = 0.55f;
		AddFlipCoins (Mathf.FloorToInt (flips));
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
		jumperRigidbody.position = jumperPos;
		controllingJumper = true;
		controllingDiver = false;
		cam.GetComponent<CameraController>().follow = false;
	}

	public void ResetRound()
	{
		waiting = true;
		playing = false;
		cam.GetComponent<CameraController> ().target = jumper.transform;
		cam.GetComponent<CameraController>().follow = true;
		if (endRoundScreenVisible) {
			ToggleEndRoundScreen ();
		}
		CoinCleanup ();
		GenerateCoins (numCoins);
		landingSpot.enableLanding (true);
	}

	void DiverJump(Vector3 jumpForce)
	{
		Vector3 jump = new Vector3 (jumpForce.x + jumperProps.weight/compensateWeightHorizontal + compensatePlatformHeight*platformProps.height, jumpForce.y * jumperProps.weight/compensateWeightVertical + compensatePlatformHeight *platformProps.height, jumpForce.z);
		diverRigidbody.AddForce (jump);
		jumpStart = Time.time;
		cam.GetComponent<CameraController> ().target = diver.transform;
		//cam.GetComponent<CameraController>().ChangeTargetV2(diver.transform);
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

	void AddFlipCoins(int flips){
		endRoundFlips.text = "Flips: " + flips;
		int coins = flips * coinsPerFlip;
		if (goodJump) {
			controller.coins += coins;
			endRoundCoins.text = "Coins: " + coins;
			Debug.Log ("+" + coins + " coins!");
			endRoundJump.text = "Good Jump!";
		} else {
			endRoundCoins.text = "Coins: 0";
			endRoundJump.text = "Bad Jump!";
		}

	}

	public void ToggleEndRoundScreen(){
		if (endRoundScreenVisible) 
		{
			endRoundScreenVisible = false;
			endRoundScreen.SetActive (false);
		} 
		else 
		{
			endRoundScreenVisible = true;
			endRoundScreen.SetActive (true);
			ResetPosition ();
		}
	}

	void GenerateCoins(int c){
		BoxCollider area = coinArea.GetComponentInChildren<BoxCollider> ();
		float size = area.bounds.extents.x;
		//Vector3 center = area.center;
		Vector3 pos = area.transform.position;
		float lastX = 0;
		//Debug.Log (center + " " + pos);
		for (int i = 0; i < c; i++) {
			float randX = Random.Range (pos.x - size, pos.x + size);
			float randY = Random.Range (pos.y, pos.y + size);
			if (i != 0) {
				while (Mathf.Abs (randX - lastX) < coinSpacing) {
					randX = Random.Range (pos.x - size, pos.x + size);
				}
			}
			lastX = randX;
			GameObject newCoin = (GameObject)Instantiate (coin, new Vector3 (randX, randY, pos.z), coin.transform.rotation);
			newCoin.tag = "Coin";
		}
	}

	void CoinCleanup(){
		GameObject[] coinsInScene = GameObject.FindGameObjectsWithTag ("Coin");
		for (int i = 0; i < coinsInScene.Length; i++) {
			Destroy (coinsInScene [i]);
		}
	}
		
	public void GoodJump(bool b){
		goodJump = b;
	}

	public GameObject ReturnDiver(){
		return diver;
	}
}
