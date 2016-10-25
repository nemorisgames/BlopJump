using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainController : MonoBehaviour {
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
	public int diverKey = 0;
	[HideInInspector]
	public int platformKey = 0;
	[HideInInspector]
	public int jumperKey = 0; 

	private bool selectScreenVisible = false;

	// Use this for initialization
	void Start () 
	{
		unlockables = new Dictionary<int,Unlockable> ();
		LoadUnlockables ();
		rewardMachine.LoadTier ();
		Debug.Log ("Presiona 'L' para ver lista de items y estado, 'R' para desbloquear un item");
		selectScreen.SetActive (false);
	}

	// Update is called once per frame
	void Update () 
	{
		if (testing) 
		{
			if (Input.GetKeyDown (KeyCode.L)) 
			{
				foreach (KeyValuePair<int,Unlockable> u in unlockables) 
				{
					Debug.Log (u.Key+" "+u.Value.name+","+u.Value.unlocked+"="+PlayerPrefs.GetInt(u.Value.name));
				}
			};
			if (Input.GetKeyDown (KeyCode.I)) {
				ToggleSelectScreen ();	
			};
		}
	}

	//carga datos de las listas expuestas en el inspector a los diccionarios
	void LoadUnlockables()
	{
		int count = 0;
		foreach (Diver d in diverList) 
		{
			unlockables.Add (count, d);
			PlayerPrefs.SetInt (d.name, 0);
			count++;
		};
		foreach (Jumper j in jumperList) 
		{
			unlockables.Add (count,j);
			PlayerPrefs.SetInt (j.name, 0);
			count++;
		};
		foreach (Platform p in platformList) 
		{
			unlockables.Add (count,p);
			PlayerPrefs.SetInt (p.name, 0);
			count++;
		};
	}

	void ToggleSelectScreen(){
		if (selectScreenVisible) {
			for (int i = 0; i < 3; i++) {
				foreach (Transform t in scrollViews[i].transform) {
					GameObject.Destroy (t.gameObject);
				};
				containers [i].GetComponent<UIDragScrollView> ().scrollView.ResetPosition ();
			};
			gameController.playing = true;
			selectScreen.SetActive (false);
			selectScreenVisible = false;
		}

		else{
			float diverAux = -210;
			float jumperAux = -210;
			float platformAux = -210;

			foreach(KeyValuePair<int,Unlockable> u in unlockables){
				if (u.Value.unlocked) {
					if (u.Value.GetComponent<Diver> () != null) {
						Debug.Log ("found diver");
						GameObject box = (GameObject)Instantiate (selectScreenBox, containers[0].transform.position, Quaternion.identity, scrollViews [0].transform);
						box.name = u.Key.ToString ();
						box.tag = "Diver";
						box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Diver> ().name;
						box.transform.localPosition = new Vector3 (diverAux, box.transform.position.y, box.transform.position.z);
						box.GetComponent<UIToggle> ().group = 1;
						diverAux += boxDistance;
					}
					if (u.Value.GetComponent<Jumper> () != null) {
						Debug.Log ("found jumper");
						GameObject box = (GameObject)Instantiate (selectScreenBox, containers[1].transform.position, Quaternion.identity, scrollViews [1].transform);

						box.name = u.Key.ToString ();
						box.tag = "Jumper";
						box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Jumper> ().name;
						box.transform.localPosition = new Vector3 (jumperAux, box.transform.position.y, box.transform.position.z);
						box.GetComponent<UIToggle> ().group = 2;
						jumperAux += boxDistance;
					}
					if (u.Value.GetComponent<Platform> () != null) {
						Debug.Log ("found platform");
						GameObject box = (GameObject)Instantiate (selectScreenBox, containers[2].transform.position, Quaternion.identity, scrollViews [2].transform);
						box.name = u.Key.ToString ();
						box.tag = "Platform";
						box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Platform> ().name;
						box.transform.localPosition = new Vector3 (platformAux, box.transform.position.y, box.transform.position.z);
						box.GetComponent<UIToggle> ().group = 3;
						platformAux += boxDistance;
					}
				}
			};

			for (int i = 0; i < 3; i++) {
				containers [i].GetComponent<UIDragScrollView> ().scrollView.ResetPosition ();

			};

			gameController.playing = false;
			selectScreen.SetActive (true);
			selectScreenVisible = true;
		};
	}

	public void SelectItems(){
		Unlockable u = null;
		if (unlockables.TryGetValue (diverKey, out u)) {
			gameController.diverGameObject = u.gameObject;
		}
		if (unlockables.TryGetValue (jumperKey, out u)) {
			gameController.jumperGameObject = u.gameObject;
		}
		if (unlockables.TryGetValue (platformKey, out u)) {
			gameController.platformGameObject = u.gameObject;
		}
		gameController.Setup ();
		ToggleSelectScreen ();
	}

}
