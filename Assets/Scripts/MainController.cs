using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainController : MonoBehaviour 
{
	public bool testing; //remove l8r

	int _currentSceneIndex;

	[HideInInspector]
	public int highScore;
	public int coins;

	public Dictionary<int,Unlockable> unlockables;

	public List<Diver> diverList; //para agregar prefabs desde el inspector
	public List<Jumper> jumperList; //para agregar prefabs desde el inspector
	public List<Platform> platformList; //para agregar prefabs desde el inspector
	public RewardMachine rewardMachine;
	public GameController gameController;
	public TweenAlpha tutorialScreen;
	public GameObject selectScreen;
	public GameObject selectScreenBox;
	public GameObject selectScreenBoxLocked;
	public GameObject[] containers;
	public GameObject[] scrollViews;
	public float boxDistance;
	public GameObject rewardButton;
	public GameObject inventoryButton;

	[HideInInspector]
	public int diverKey;
	[HideInInspector]
	public int platformKey;
	[HideInInspector]
	public int jumperKey;
	int diverKeyAux;
	int jumperKeyAux;
	int platformKeyAux;

	public BackgroundChange bg;
	public int roundsToChange;

	//[HideInInspector]
	public bool selectScreenVisible = false;
	UILabel coinsLabel;
	public UILabel distanceLabel;
	GameObject distance;
	//TweenAlpha distanceTween;

	[HideInInspector]
	public bool restartAllowed = false;

	[Header("Ads")]
	public GameObject ad;
	public SpilGamesAPI spilAPI;
	public GameObject adWndw;

	public AudioSource bgForest;

	// Use this for initialization
	void Awake () 
	{
		coins = PlayerPrefs.GetInt ("Coins");
		unlockables = new Dictionary<int,Unlockable> ();
		LoadUnlockables ();
		LoadDefaults ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
		gameController.playing = false;
		rewardMachine = GameObject.FindGameObjectWithTag ("RewardMachine").GetComponent<RewardMachine> ();
		rewardMachine.availableRewards = new Dictionary<int, Unlockable> ();
		Debug.Log ("Presiona 'L' para ver lista de items y estado, 'R' para desbloquear un item");
		coinsLabel = GameObject.Find ("Coins").GetComponent<UILabel> ();
		distance = GameObject.Find ("Distance");
		//distanceLabel = distance.GetComponent<UILabel> ();
		EnableAd (true);
		ToggleButtons (false);
		if (PlayerPrefs.GetInt ("notFirstTime") == 1) {
			ToggleButtons (true);
			rewardButton.SetActive (false);
		}
		rewardMachine.LoadTier (0);
		highScore = PlayerPrefs.GetInt ("highScore");
	}

	/*void Start(){
		plusCoinTween.ResetToBeginning ();
		plusCoinTween.value = 0;
	}*/

	void Start(){
		gameController.Setup ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (testing && !gameController.playing) 
		{
			if (Input.GetKeyDown (KeyCode.L)) 
			{
				foreach (KeyValuePair<int,Unlockable> u in unlockables) 
				{
					Debug.Log (u.Key+" "+u.Value.name+","+u.Value.unlocked+"="+PlayerPrefs.GetInt(u.Value.name));
				}
			};
			if (Input.GetKeyDown (KeyCode.I) && !rewardMachine.rewardScreenVisible) 
			{
				ToggleSelectScreen ();	
			};
			if (Input.GetKeyDown (KeyCode.R) && !selectScreenVisible) 
			{
				rewardMachine.ToggleRewardScreen ();
			};

			if (Input.GetKeyDown (KeyCode.U) && !selectScreenVisible && !rewardMachine.rewardScreenVisible) 
			{
				UnlockAll (true);
				Debug.Log ("cheater!");
			};
		}
	}

	void LateUpdate(){
		coinsLabel.text = ""+coins;
	}

	//carga datos de las listas expuestas en el inspector a los diccionarios
	void LoadUnlockables()
	{
		int count = 0;
		foreach (Diver d in diverList) 
		{
			if (PlayerPrefs.GetInt (d.name) != 1) {
				d.unlocked = false;
				PlayerPrefs.SetInt (d.name, 0);
			} else {
				d.unlocked = true;
			}
			unlockables.Add (count, d);
			/*
			d.unlocked = false;
			unlockables.Add (count, d);
			PlayerPrefs.SetInt (d.name, 0);
			 */
			count++;
		};
		foreach (Jumper j in jumperList) 
		{
			if (PlayerPrefs.GetInt (j.name) != 1) {
				j.unlocked = false;
				PlayerPrefs.SetInt (j.name, 0);
			} else {
				j.unlocked = true;
			}
			unlockables.Add (count,j);
			/*j.unlocked = false;
			unlockables.Add (count,j);
			PlayerPrefs.SetInt (j.name, 0);*/
			count++;
		};
		foreach (Platform p in platformList) 
		{
			if (PlayerPrefs.GetInt (p.name) != 1) {
				p.unlocked = false;
				PlayerPrefs.SetInt (p.name, 0);
			} else {
				p.unlocked = true;
			}
			unlockables.Add (count, p);
			/*p.unlocked = false;
			unlockables.Add (count,p);
			PlayerPrefs.SetInt (p.name, 0);*/
			count++;
		};
	}

	public void EnableAd(bool b){
		if (b) {
			bgForest.Pause ();
			Camera.main.GetComponent<AudioSource> ().Pause ();
			spilAPI.pauseGame ();
			spilAPI.GameBreak ();
			adWndw.SetActive (true);
		} else {
			Camera.main.GetComponent<AudioSource> ().UnPause ();
			bgForest.UnPause ();
		}
		ad.SetActive (b);
	}

	public void ToggleSelectScreen(){
		if (!gameController.playing || gameController.controllingJumper) {
			if (selectScreenVisible) {
				// si esta visible -> cerrar pantalla, chequear si las otras estan cerradas
				gameController.waiting = true;
				//selectScreen.SetActive (false);
				selectScreen.GetComponent<TweenAlpha>().PlayReverse();
				selectScreenVisible = false;
				StartCoroutine (SelectScreenItemCleanup ());

			} else {
				//si no esta visible -> cerrar otras pantallas, abrir esta

				if(tutorialScreen.value > 0f)
					tutorialScreen.PlayReverse ();

				if (rewardMachine.rewardScreenVisible) {
					rewardMachine.ToggleRewardScreen ();
				}

				if (gameController.endRoundScreenVisible) {
					gameController.ToggleEndRoundScreen ();
				}
					

				gameController.waiting = false;
				//gameController.jumpBar.Initialize ();
				float diverAux = -210;
				float jumperAux = -210;
				float platformAux = -210;
				diverKeyAux = diverKey;
				jumperKeyAux = jumperKey;
				platformKeyAux = platformKey;
				foreach (KeyValuePair<int,Unlockable> u in unlockables) {
					//if (u.Value.unlocked) {
						if (u.Value.GetComponent<Diver> () != null) {
						GameObject box = (GameObject)Instantiate (u.Value.unlocked?selectScreenBox:selectScreenBoxLocked, containers [0].transform.position, Quaternion.identity, scrollViews [0].transform);
							box.name = u.Key.ToString ();
							box.tag = "Diver";
							box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Diver> ().name;
							box.transform.FindChild("Checked").GetComponent<UISprite>().spriteName = u.Value.gameObject.name;
							box.transform.localPosition = new Vector3 (diverAux, box.transform.position.y, box.transform.position.z);
							box.GetComponent<UIToggle> ().group = 1;
							diverAux += boxDistance;
							if (u.Key == diverKey) {
								box.GetComponent<UIToggle> ().Set (true);
							}

						}
						if (u.Value.GetComponent<Jumper> () != null) {
						GameObject box = (GameObject)Instantiate (u.Value.unlocked?selectScreenBox:selectScreenBoxLocked, containers [1].transform.position, Quaternion.identity, scrollViews [1].transform);
							box.name = u.Key.ToString ();
							box.tag = "Jumper";
							box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Jumper> ().name;
							box.transform.FindChild("Checked").GetComponent<UISprite>().spriteName = u.Value.gameObject.name;
							box.GetComponent<UIDragScrollView> ().scrollView = GetComponentInParent<UIScrollView> ();
							box.transform.localPosition = new Vector3 (jumperAux, box.transform.position.y, box.transform.position.z);
							box.GetComponent<UIToggle> ().group = 2;
							jumperAux += boxDistance;
							if (u.Key == jumperKey) {
								box.GetComponent<UIToggle> ().Set (true);
							}
						}
						if (u.Value.GetComponent<Platform> () != null) {
						GameObject box = (GameObject)Instantiate (u.Value.unlocked?selectScreenBox:selectScreenBoxLocked, containers [2].transform.position, Quaternion.identity, scrollViews [2].transform);
							box.name = u.Key.ToString ();
							box.tag = "Platform";
							box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Platform> ().name;
							box.transform.FindChild("Checked").GetComponent<UISprite>().spriteName = u.Value.gameObject.name;
							box.GetComponent<UIDragScrollView> ().scrollView = GetComponentInParent<UIScrollView> ();
							box.transform.localPosition = new Vector3 (platformAux, box.transform.position.y, box.transform.position.z);
							box.GetComponent<UIToggle> ().group = 3;
							platformAux += boxDistance;
							if (u.Key == platformKey) {
								box.GetComponent<UIToggle> ().Set (true);
							}
						}
					//}
				}

				for (int i = 0; i < 3; i++) {
					containers [i].GetComponent<UIDragScrollView> ().scrollView.ResetPosition ();
				}
				//selectScreen.SetActive (true);
				selectScreen.GetComponent<TweenAlpha>().PlayForward();
				selectScreenVisible = true;
			}

		}
	}

	public void SelectItems()
	{
		Unlockable u = null;
		if (unlockables.TryGetValue (diverKey, out u)) 
		{
			gameController.diverGameObject = u.gameObject;
		}
		if (unlockables.TryGetValue (jumperKey, out u)) 
		{
			gameController.jumperGameObject = u.gameObject;
		}
		if (unlockables.TryGetValue (platformKey, out u)) 
		{
			gameController.platformGameObject = u.gameObject;
		}
		UpdateGearPrefs ();
		gameController.EndRound ();
		gameController.Setup ();
		gameController.ResetRound ();
		ToggleSelectScreen ();
	}

	public void LoadDefaults(){
		if (PlayerPrefs.GetInt ("notFirstTime") != 1) {
			Debug.Log ("here");
			diverKey = 0; //wetsuit diver
			jumperKey = 6; //normal jumper
			platformKey = 11; //metal platform
			UpdateGearPrefs();
			PlayerPrefs.SetInt("notFirstTime",1);
		} else {
			diverKey = PlayerPrefs.GetInt ("lastDiver");
			jumperKey = PlayerPrefs.GetInt ("lastJumper");
			platformKey = PlayerPrefs.GetInt ("lastPlatform");
		}
		Unlockable u;
		if (unlockables.TryGetValue (diverKey, out u)) 
		{
			if(!u.unlocked){
				u.unlocked = true;
				PlayerPrefs.SetInt (u.name, 1);
			}
			gameController.diverGameObject = u.gameObject;
		}
		if (unlockables.TryGetValue (jumperKey, out u)) 
		{
			if(!u.unlocked){
				u.unlocked = true;
				PlayerPrefs.SetInt (u.name, 1);
			}
			gameController.jumperGameObject = u.gameObject;
		}
		if (unlockables.TryGetValue (platformKey, out u)) 
		{
			if(!u.unlocked){
				u.unlocked = true;
				PlayerPrefs.SetInt (u.name, 1);
			}
			gameController.platformGameObject = u.gameObject;
		}

	}

	public void CheckOpenScreens(){
		if (!selectScreenVisible && !rewardMachine.rewardScreenVisible && (gameController.controllingDiver || !gameController.controllingDiver && gameController.rounds == 0)) {
			gameController.ResetRound ();
			gameController.jumpBar.Initialize ();
		}
	}

	public void CloseWindow()
	{
		if (selectScreenVisible) {
			ToggleSelectScreen ();
			LastLoadout ();
			if (gameController.controllingDiver || gameController.rounds == 0) {
				gameController.ResetRound ();
			}
		} else if (rewardMachine.rewardScreenVisible) {
			rewardMachine.ToggleRewardScreen ();
			if (gameController.controllingDiver) {
				gameController.ResetRound ();
			}
		} else if (gameController.endRoundScreenVisible && restartAllowed) {
			gameController.ToggleEndRoundScreen ();
			gameController.ResetRound ();
			restartAllowed = false;
		}
	}

	public IEnumerator enableRestart(float f){
		yield return new WaitForSeconds (f);
		restartAllowed = true;
	}

	public void disableRestart(){
		restartAllowed = false;
	}

	public void UnlockAll(bool b){ //for testing purposes only
		foreach(KeyValuePair<int,Unlockable> u in unlockables){
			u.Value.unlocked = b;
		};
		rewardMachine.availableRewards.Clear ();
	}

	public void LastLoadout(){
		diverKey = diverKeyAux;
		jumperKey = jumperKeyAux;
		platformKey = platformKeyAux;
		UpdateGearPrefs ();
	}

	public void ToggleButtons(bool b){
		rewardButton.SetActive (b);
		inventoryButton.SetActive (b);
	}

	void OnApplicationQuit(){
		PlayerPrefs.SetInt ("Coins", coins);
	}

	void UpdateGearPrefs(){
		PlayerPrefs.SetInt ("lastDiver", diverKey);
		PlayerPrefs.SetInt ("lastJumper", jumperKey);
		PlayerPrefs.SetInt ("lastPlatform", platformKey);
	}

	IEnumerator SelectScreenItemCleanup(){
		yield return new WaitForSeconds (1);
		for (int i = 0; i < 3; i++) {
			foreach (Transform t in scrollViews[i].transform) {
				GameObject.Destroy (t.gameObject);
			}
			containers [i].GetComponent<UIDragScrollView> ().scrollView.ResetPosition ();
		}
	}

}
