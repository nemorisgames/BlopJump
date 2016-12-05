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

	public GameObject coin;

	int coinGrabHeight;
	int airCoins;

	[Header("Diver")]
	public GameObject diverGameObject; //diver en uso
	public Vector3 diverJumpForce; //vector fuerza para el salto
	public float normalHorizontalSpeed; //velocidad horizontal normal del diver
	public float trickHorizontalSpeed; //velocidad horizontal en truco del diver
	public float verticalSpeed;

	Rigidbody diverRigidbody; //rigidbody del diver
	public Diver diverProps; //propiedades del diver
	GameObject diver;
	public bool controllingDiver;
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
	UILabel endRoundFlipCoins;
	UILabel endRoundHeight;
	UILabel endRoundHeightCoins;
	UILabel endRoundJump;
	UILabel endRoundTotalCoins;
	LandingSpot landingSpot;
	BoxCollider landingSpotBC;
	public JumpBar jumpBar;
	bool _canCountFlip = false;

	[Header("Particles")]
	public GameObject splashBlop;
	public GameObject splashDiver;

	float maxHeight;
	bool splash;

    [Header("SFX")]
    public AudioSource[] sourceSFX;
    public AudioClip[] waterSFX;
    public AudioClip[] blobSFX;
    public AudioClip[] jumperSFX;

	// Use this for initialization
	void Awake () 
	{
		cam = Camera.main;
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
		endRoundScreen = GameObject.Find ("EndRoundScreen");
		endRoundScreen.SetActive (false);
		endRoundScreenVisible = false;
		endRoundFlips = endRoundScreen.transform.FindChild ("EndFlips").GetComponent<UILabel> ();
		endRoundFlipCoins = endRoundScreen.transform.FindChild ("EndFlipCoins").GetComponent<UILabel> ();
		endRoundHeight = endRoundScreen.transform.FindChild ("EndHeight").GetComponent<UILabel> ();
		endRoundHeightCoins = endRoundScreen.transform.FindChild ("EndHeightCoins").GetComponent<UILabel> ();
		endRoundTotalCoins = endRoundScreen.transform.FindChild ("EndTotalCoins").GetComponent<UILabel> ();
		endRoundJump = endRoundScreen.transform.FindChild ("EndRoundText").GetComponent<UILabel> ();
		landingSpot = GameObject.FindGameObjectWithTag ("LandingSpot").GetComponent<LandingSpot> ();
		landingSpotBC = landingSpot.gameObject.GetComponent<BoxCollider> ();
		jumpBar = GameObject.FindGameObjectWithTag ("JumpBar").GetComponent<JumpBar> ();
		Setup ();

		maxHeight = 0;
		coinGrabHeight = 1;
	}

	void FixedUpdate()
	{
		if (waiting) {
			//when controlling Jumper
			jumpBar.UpdateValue();
			diver.GetComponent<Animator> ().SetBool ("Diving", false);
			if (controllingJumper && !controllingDiver) {
				if (Input.GetKeyDown (KeyCode.Space)) { //
					JumperJump();
				}
			}
		}

		if (playing) 
		{
			CalculateDistance ();
			//when controlling Diver
			if (!controllingJumper && controllingDiver) {
				if (diverProps.onGround) {
					flips = 0;
					WindupRotation = 0;
					/*if (Input.GetKeyDown (KeyCode.Space) && testing)
						DiverJump (diverJumpForce);*/
				}

				if (!diverProps.onGround) {
					diver.GetComponent<Animator> ().SetBool ("Diving", true);
					//diverCollider.radius = 0.55f;
					//SetDiverSpinSpeed (diverProps.spinSpeed, diverProps.trickSpinSpeed);
					CalculateFlips();
					if (diver.transform.position.y > maxHeight) {
						maxHeight = diver.transform.position.y;
					}
					if (Input.GetKey (KeyCode.Space) || Input.GetMouseButton(0)) {
						DiverTrickSpin ();
					} else {
						DiverNormalSpin ();
					}
					if (diverRigidbody.position.y < 0.11 && !splash) {
                        PlaySFX(waterSFX[2]);
						Instantiate (splashDiver, diverRigidbody.transform.position, splashDiver.transform.rotation);
						splash = true;
					}
				}

			}

			if(enableWind)
				diverRigidbody.AddForce(new Vector3(-windForce,0f,0f));

			//Cleanup
		}

	}

	void CalculateDistance(){
		float distance = (diverRigidbody.position.x - landingSpot.transform.position.x);
		if (distance < 0)
			distance = 0;
		controller.distanceLabel.text = "Distance: " + Mathf.FloorToInt(distance)+"m";
	}

	IEnumerator CreatePlusCoin(){
		GameObject plus = (GameObject)Instantiate (coin, new Vector3 (diverRigidbody.position.x + 0.5f, diverRigidbody.position.y, diverRigidbody.position.z), coin.transform.rotation);
		yield return new WaitForSeconds (1f);
		Destroy (plus);
	}

	void LateUpdate()
	{
		if (Mathf.FloorToInt(diverRigidbody.position.y) >= coinGrabHeight) {
			StartCoroutine (CreatePlusCoin ());
			Debug.Log(Mathf.FloorToInt(diverRigidbody.position.y)+" "+coinGrabHeight);
			airCoins += 2;
			coinGrabHeight += 1;
		}

		if (diverRigidbody.position.y < -2) {
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
		diverRigidbody.position = diverPos.position;
		diverProps = diver.GetComponent<Diver> ();
		normalHorizontalSpeed = diverJumpForce.x / 50;
		trickHorizontalSpeed = normalHorizontalSpeed / 2;
		diverRigidbody.maxAngularVelocity = 100;
		verticalSpeed = diverProps.trickSpinSpeed / 120;

		controllingDiver = false;

		//Set up platform
		platform = (GameObject)Instantiate(platformGameObject,platformGameObject.transform.position,platformGameObject.transform.rotation);
		platformProps = platform.GetComponent<Platform> ();

		//Set up jumper
		jumperPos = platform.transform.FindChild ("JumperPos").position;
        jumper = (GameObject)Instantiate(jumperGameObject,jumperPos, Quaternion.Euler(new Vector3(0, -90, 0)));
		jumperRigidbody = jumper.GetComponent<Rigidbody> ();
		jumperProps = jumper.GetComponent<Jumper> ();

		controllingJumper = true;
		cam.GetComponent<CameraController> ().target = jumper.transform;
		cam.GetComponent<CameraController> ().UpdateCamera (platformProps);
		jumperJumpForce = new Vector3 (platformProps.jumpForceX, jumperJumpForce.y, jumperJumpForce.z);
		//cam.gameObject.GetComponent<GenericMoveCamera> ().LookAtTarget = diver;

		//GenerateCoins (numCoins);
		ResetRound ();
	}

	void SetDiverSpinSpeed(float normalSpin, float trickSpin)
	{
		CalculateFlips (); //calcula numero de giros
		Vector3 currentVelocity = diverRigidbody.velocity;
		float verticalVelocity = currentVelocity.y - verticalSpeed;
		diverRigidbody.angularVelocity = new Vector3 (0f, 0f, normalSpin);
		diverRigidbody.velocity = new Vector3 (normalHorizontalSpeed, currentVelocity.y, currentVelocity.z);
		if (Input.GetKey (KeyCode.Space)) {
			if (!diver.GetComponent<Animator> ().GetBool ("Spinning")) {
				diver.GetComponent<Animator> ().SetBool ("Spinning", true);
			}
			diverRigidbody.angularVelocity = new Vector3 (0f, 0f, trickSpin);
			diverRigidbody.velocity = new Vector3 (trickHorizontalSpeed, verticalVelocity, currentVelocity.z);
			diverRigidbody.AddForce (new Vector3 (0, -jumpHeightCompensate, 0));
		} else {
			diver.GetComponent<Animator> ().SetBool ("Spinning", false);
		}
	}


	public void DiverNormalSpin(){
		Vector3 currentVelocity = diverRigidbody.velocity;
		diverRigidbody.angularVelocity = new Vector3 (0f, 0f, diverProps.spinSpeed);
		diverRigidbody.velocity = new Vector3 (normalHorizontalSpeed, currentVelocity.y, currentVelocity.z);
		diver.GetComponent<Animator> ().SetBool ("Spinning", false);
	}

	public void DiverTrickSpin(){
		Vector3 currentVelocity = diverRigidbody.velocity;
		float verticalVelocity = currentVelocity.y - verticalSpeed;
		diverRigidbody.angularVelocity = new Vector3 (0f, 0f, diverProps.trickSpinSpeed);
		diverRigidbody.velocity = new Vector3 (trickHorizontalSpeed, verticalVelocity, currentVelocity.z);
		diverRigidbody.AddForce (new Vector3 (0, -jumpHeightCompensate, 0));
		diver.GetComponent<Animator> ().SetBool ("Spinning", true);
	}


	public void ResetPosition(){
		GetComponent<Animator> ().SetBool ("onAction", false);
		jumpBar.gameObject.SetActive (false);
		playing = false;

        jumper.GetComponent<CapsuleCollider>().enabled = true;

        GetComponent<Animator>().SetBool("onAction", false);
        jumper.GetComponent<Animator>().SetBool("onJump", false);
		diver.GetComponent<Animator> ().SetBool ("Spinning", false);
		diver.GetComponent<Animator> ().SetBool ("Diving", false);
		//diverCollider.radius = 0.55f;
		AddFlipCoins (Mathf.FloorToInt (flips));
		Debug.Log ("Flips: "+Mathf.Round (flips));

		jumpEnd = Time.time;
		flips = 0;
		WindupRotation = 0;
		enableWind = false;

		Debug.Log ("x: "+diverRigidbody.position.x + ", y: "+maxHeight);
		Debug.Log ("Time: "+(jumpEnd - jumpStart));

		diverRigidbody.velocity = Vector3.zero;
		diverRigidbody.angularVelocity = Vector3.zero;
		diverRigidbody.rotation = diverPos.rotation;
		diverRigidbody.position = diverPos.position;
		jumperRigidbody.position = jumperPos;
		splash = false;
		cam.GetComponent<CameraController>().follow = false;
	}

	public void ResetRound() //reubica la cámara, muestra pantalla de fin de ronda
	{
		//cam.GetComponent<CameraController> ().target = jumper.transform;
		//cam.GetComponent<CameraController>().follow = true;
		if (endRoundScreenVisible) {
			ToggleEndRoundScreen ();
		}
		controllingJumper = true;
		controllingDiver = false;
		//CoinCleanup ();
		//GenerateCoins (numCoins);
		landingSpot.enableLanding (true);
		SetLandingSpot ();
		CalculateDistance ();
		//ToggleJumpBar ();
		StartCoroutine (cam.GetComponent<CameraController> ().CameraPan(jumper.transform, diver.transform, landingSpot.transform));
		jumpBar.gameObject.SetActive (true);
		jumpBar.Initialize();
		playing = false;
		_canCountFlip = false;
		SplashCleanup ();
		maxHeight = 0;
		coinGrabHeight = 1;
	}

	void DiverJump(Vector3 jumpForce)
	{
		compensateWeightVertical = jumpBar.GetComponent<UISlider> ().value * 2;
		float platformComp = 1 + platformProps.height * 0.1f;
		float jumperWeightX = jumperProps.weight / compensateWeightHorizontal * platformComp;
		float jumperWeightY = jumperProps.weight * compensateWeightVertical / (3 - platformComp);
		Vector3 jump = new Vector3 (jumpForce.x + jumperWeightX - compensatePlatformHeight*platformProps.height, jumpForce.y + jumperWeightY + compensatePlatformHeight * platformProps.height, jumpForce.z);
		Debug.Log (jump.x + " " + jump.y);
		diverRigidbody.AddForce (jump);
		jumpStart = Time.time;
		cam.GetComponent<CameraController> ().target = diver.transform;
		//cam.GetComponent<CameraController>().ChangeTargetV2(diver.transform);
		//Debug.Log (diverRigidbody.velocity.x + " " + diverRigidbody.velocity.y);
	}

	void CalculateFlips()
	{
		float _flipPoint = (Mathf.Round(diver.transform.up.normalized.y * 10) / 10);
		if (_flipPoint != 1.0f)
		{
			_canCountFlip = true;
		}
		if ((_canCountFlip) && (_flipPoint == 1.0f))
		{
			flips++;
			_canCountFlip = false;
			//Debug.Log(flips);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Jumper")
		{
            PlaySFX(waterSFX[4]);

            PlaySFX(blobSFX[0]);

			Instantiate (splashBlop, splashBlop.transform.position, splashBlop.transform.rotation);
			GetComponent<Animator>().SetBool("onAction", true);
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
			StartCoroutine (DiverJumpDelay());
		};
	}

	void AddFlipCoins(int flips){
		endRoundFlips.text = "Flips: " + flips;
		endRoundHeight.text = "Height: " + Mathf.Floor (maxHeight * 10) / 10 + "m";
		int flipCoins = flips * coinsPerFlip;
		int heightCoins = Mathf.FloorToInt(maxHeight) * 2;
		if (goodJump) {
			controller.coins += flipCoins + heightCoins;
			endRoundFlipCoins.text = "+ " + flipCoins + " coins";
			endRoundHeightCoins.text = "+ " + heightCoins + " coins";
			endRoundJump.text = "Good Jump!";
		} else {
			endRoundFlipCoins.text = "+ 0 coins";
			endRoundHeightCoins.text = "+ 0 coins";
			endRoundJump.text = "Bad Jump!";
		}

	}

	public void ToggleEndRoundScreen(){
		if (endRoundScreenVisible) 
		{
			endRoundScreenVisible = false;
			endRoundScreen.SetActive (false);
			controller.EnableAd (false);
		} 
		else 
		{
			endRoundScreenVisible = true;
			endRoundScreen.SetActive (true);
			controller.EnableAd (true);
			ResetPosition ();
		}
	}

	/*void GenerateCoins(int c){
		BoxCollider area = coinArea.GetComponentInChildren<BoxCollider> ();
		float sizeX = area.bounds.extents.x;
		float sizeY = area.bounds.extents.y;
		float coinAreaX = coinArea.transform.FindChild("Area").transform.position.x - sizeX;
		float landingSpotX = landingSpot.transform.position.x - landingSpot.GetComponent<BoxCollider> ().bounds.extents.x;
		Debug.Log (coinAreaX + " " + landingSpotX);
		while (coinAreaX > landingSpotX) {
			coinArea.transform.localScale += new Vector3(1f,0f,0f);
			sizeX = area.bounds.extents.x;
			Debug.Log (coinAreaX + " " + landingSpotX);
			coinAreaX = coinArea.transform.FindChild("Area").transform.position.x - sizeX;

		}
		//Vector3 center = area.center;
		Vector3 pos = area.transform.position;
		float lastX = 0;
		//Debug.Log (center + " " + pos);
		for (int i = 0; i < c; i++) {
			float randX = Random.Range (pos.x - sizeX, pos.x + sizeX);
			float randY = Random.Range (pos.y, pos.y + sizeY);
			if (i != 0) {
				while (Mathf.Abs (randX - lastX) < coinSpacing) {
					randX = Random.Range (pos.x - sizeX, pos.x + sizeX);
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
	}*/

	void SplashCleanup(){
		GameObject[] splashes = GameObject.FindGameObjectsWithTag ("Splash");
		for (int i = 0; i < splashes.Length; i++) {
			Destroy (splashes [i]);
		}
	}

	public void GoodJump(bool b){
		goodJump = b;
	}

	public GameObject GetDiver(){
		return diver;
	}

	public GameObject GetJumper(){
		return jumper;
	}

	public GameObject GetLandingSpot(){
		return landingSpot.gameObject;
	}

	public void JumperJump(){
        
		//jumperRigidbody.AddForce (jumperJumpForce);
        //jumper.GetComponent<CapsuleCollider>().enabled = false;
        jumper.GetComponent<Animator>().SetBool("onJump", true);
		//ToggleJumpBar ();
		cam.GetComponent<CameraController>().TogglePlatformButton();
		controllingJumper = false;
		waiting = false;
		playing = true;
	}

	IEnumerator DiverJumpDelay(){
		yield return new WaitForSeconds (0.1f);
		diverProps.onGround = false;
	}

	public void SetLandingSpot(){
		//Debug.Log (LandingSpotExtent ());
		//Debug.Log (landingSpot.minDistance [jumperProps.index, platformProps.index] + " " + landingSpot.maxDistance [jumperProps.index, platformProps.index]);
		float rand = Random.Range (landingSpot.maxDistance [jumperProps.index, platformProps.index] - LandingSpotExtent(), landingSpot.minDistance [jumperProps.index, platformProps.index]);
		landingSpot.transform.position = new Vector3 (rand, landingSpot.transform.position.y, landingSpot.transform.position.z);
	}

	public float LandingSpotExtent(){
		return landingSpotBC.size.x/2;
	}

    public void JumperEV(string str)
    {
        
        //Debug.Log("Jumper: " + str);
        if (str == "Effort")
        {
            int i = Mathf.RoundToInt(Random.Range(0, jumperSFX.Length));
            PlaySFX(jumperSFX[i]);
        }
        if (str == "Jump")
        {
            jumperRigidbody.AddForce (jumperJumpForce);
            jumper.GetComponent<CapsuleCollider>().enabled = false;
        }

        Debug.Log("Jumper: " + str);

    }
	
    void PlaySFX(AudioClip clip)
    {
        for (int i = 0; i < sourceSFX.Length; i++)
        {
            if (!sourceSFX[i].isPlaying)
            {
                sourceSFX[i].clip = clip;
                sourceSFX[i].Play();
                break;
            }
        }
    }
}
