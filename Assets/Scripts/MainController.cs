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
	public GameObject selectScreen;
	public GameObject selectScreenBox;
	public GameObject[] containers;
	public GameObject[] scrollViews;
	public float boxDistance;

	[HideInInspector]
	public int diverKey;
	[HideInInspector]
	public int platformKey;
	[HideInInspector]
	public int jumperKey;
	int diverKeyAux;
	int jumperKeyAux;
	int platformKeyAux;

	[HideInInspector]
	public bool selectScreenVisible = false;
	UILabel coinsLabel;
	public UILabel distanceLabel;
	GameObject distance;
	//TweenAlpha distanceTween;

	[Header("Ads")]
	public GameObject ad;
	public SpilGamesAPI spilAPI;

	// Use this for initialization
	void Awake () 
	{
		unlockables = new Dictionary<int,Unlockable> ();
		LoadUnlockables ();
		LoadDefaults ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
		gameController.playing = false;
		rewardMachine = GameObject.FindGameObjectWithTag ("RewardMachine").GetComponent<RewardMachine> ();
		rewardMachine.availableRewards = new Dictionary<int, Unlockable> ();
		rewardMachine.LoadTier ();
		Debug.Log ("Presiona 'L' para ver lista de items y estado, 'R' para desbloquear un item");
		coinsLabel = GameObject.Find ("Coins").GetComponent<UILabel> ();
		distance = GameObject.Find ("Distance");
		distanceLabel = distance.GetComponent<UILabel> ();
		//distanceTween = distance.GetComponent<TweenAlpha> ();
		selectScreen.SetActive (false);
		EnableAd (false);
	}

	/*void Start(){
		plusCoinTween.ResetToBeginning ();
		plusCoinTween.value = 0;
	}*/

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
				UnlockAll ();
				Debug.Log ("cheater!");
			};
		}
	}

	void LateUpdate(){
		coinsLabel.text = "Coins: " + coins;
	}

	//carga datos de las listas expuestas en el inspector a los diccionarios
	void LoadUnlockables()
	{
		int count = 0;
		foreach (Diver d in diverList) 
		{
			d.unlocked = false;
			unlockables.Add (count, d);
			PlayerPrefs.SetInt (d.name, 0);
			count++;
		};
		foreach (Jumper j in jumperList) 
		{
			j.unlocked = false;
			unlockables.Add (count,j);
			PlayerPrefs.SetInt (j.name, 0);
			count++;
		};
		foreach (Platform p in platformList) 
		{
			p.unlocked = false;
			unlockables.Add (count,p);
			PlayerPrefs.SetInt (p.name, 0);
			count++;
		};
	}

	public void EnableAd(bool b){
		if (b) {
			spilAPI.GameBreak ();
		}
		ad.SetActive (b);
	}

	public void ToggleSelectScreen(){
		if (!gameController.playing || gameController.controllingJumper) {
			if (rewardMachine.rewardScreenVisible) {
				rewardMachine.ToggleRewardScreen ();
			}
			if (gameController.endRoundScreenVisible) {
				gameController.ToggleEndRoundScreen ();
			}
			if (selectScreenVisible) {
				for (int i = 0; i < 3; i++) {
					foreach (Transform t in scrollViews[i].transform) {
						GameObject.Destroy (t.gameObject);
					}
					containers [i].GetComponent<UIDragScrollView> ().scrollView.ResetPosition ();
				}
				gameController.waiting = true;
				selectScreen.SetActive (false);
				selectScreenVisible = false;
			} else {
				gameController.waiting = false;
				gameController.jumpBar.Initialize ();
				float diverAux = -210;
				float jumperAux = -210;
				float platformAux = -210;
				diverKeyAux = diverKey;
				jumperKeyAux = jumperKey;
				platformKeyAux = platformKey;
				foreach (KeyValuePair<int,Unlockable> u in unlockables) {
					if (u.Value.unlocked) {
						if (u.Value.GetComponent<Diver> () != null) {
							GameObject box = (GameObject)Instantiate (selectScreenBox, containers [0].transform.position, Quaternion.identity, scrollViews [0].transform);
							box.name = u.Key.ToString ();
							box.tag = "Diver";
							box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Diver> ().name;

							box.transform.localPosition = new Vector3 (diverAux, box.transform.position.y, box.transform.position.z);
							box.GetComponent<UIToggle> ().group = 1;
							diverAux += boxDistance;
							if (u.Key == diverKey) {
								box.GetComponent<UIToggle> ().Set (true);
							}

						}
						if (u.Value.GetComponent<Jumper> () != null) {
							GameObject box = (GameObject)Instantiate (selectScreenBox, containers [1].transform.position, Quaternion.identity, scrollViews [1].transform);
							box.name = u.Key.ToString ();
							box.tag = "Jumper";
							box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Jumper> ().name;
							box.GetComponent<UIDragScrollView> ().scrollView = GetComponentInParent<UIScrollView> ();
							box.transform.localPosition = new Vector3 (jumperAux, box.transform.position.y, box.transform.position.z);
							box.GetComponent<UIToggle> ().group = 2;
							jumperAux += boxDistance;
							if (u.Key == jumperKey) {
								box.GetComponent<UIToggle> ().Set (true);
							}
						}
						if (u.Value.GetComponent<Platform> () != null) {
							GameObject box = (GameObject)Instantiate (selectScreenBox, containers [2].transform.position, Quaternion.identity, scrollViews [2].transform);
							box.name = u.Key.ToString ();
							box.tag = "Platform";
							box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Platform> ().name;
							box.GetComponent<UIDragScrollView> ().scrollView = GetComponentInParent<UIScrollView> ();
							box.transform.localPosition = new Vector3 (platformAux, box.transform.position.y, box.transform.position.z);
							box.GetComponent<UIToggle> ().group = 3;
							platformAux += boxDistance;
							if (u.Key == platformKey) {
								box.GetComponent<UIToggle> ().Set (true);
							}
						}
					}
				}

				for (int i = 0; i < 3; i++) {
					containers [i].GetComponent<UIDragScrollView> ().scrollView.ResetPosition ();
				}
				selectScreen.SetActive (true);
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
		gameController.ResetPosition ();
		gameController.Setup ();
		ToggleSelectScreen ();
	}

	void LoadDefaults(){
		diverKey = 0; //wetsuit diver
		jumperKey = 7; //normal jumper
		platformKey = 13; //metal platform
		Unlockable u;
		if (unlockables.TryGetValue (diverKey, out u)) 
		{
			u.unlocked = true;
		}
		if (unlockables.TryGetValue (jumperKey, out u)) 
		{
			u.unlocked = true;
		}
		if (unlockables.TryGetValue (platformKey, out u)) 
		{
			u.unlocked = true;
		}
	}

	public void CloseWindow()
	{
		if (selectScreenVisible) {
			ToggleSelectScreen ();
			LastLoadout ();
			if (gameController.controllingDiver) {
				gameController.ResetRound ();
			}
		} else if (rewardMachine.rewardScreenVisible) {
			rewardMachine.ToggleRewardScreen ();
			if (gameController.controllingDiver) {
				gameController.ResetRound ();
			}
		} else if (gameController.endRoundScreenVisible) {
			gameController.ToggleEndRoundScreen ();
			gameController.ResetRound ();
		}
	}

	/*public IEnumerator PlusCoin(int value){
		plusCoinLabel.text = "+" + value;
		plusCoinTween.Toggle ();
		yield return new WaitForSeconds (1.2f);
		plusCoinTween.Toggle ();
	}*/

	void UnlockAll(){ //for testing purposes only
		foreach(KeyValuePair<int,Unlockable> u in unlockables){
			u.Value.unlocked = true;
		};
		rewardMachine.availableRewards.Clear ();
	}

	public void LastLoadout(){
		diverKey = diverKeyAux;
		jumperKey = jumperKeyAux;
		platformKey = platformKeyAux;
	}
}
