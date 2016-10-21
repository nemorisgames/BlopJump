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
	public GameObject selectScreen;
	public GameObject selectScreenBox;
	public GameObject[] containers;
	public float boxDistance;

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
				foreach (Transform t in containers[i].transform) {
					GameObject.Destroy (t.gameObject);
				};
			};

				
			selectScreen.SetActive (false);
			selectScreenVisible = false;
		}
		else{
			float diverAux = -180;
			float jumperAux = -180;
			float platformAux = -180;
			foreach(KeyValuePair<int,Unlockable> u in unlockables){
				if (u.Value.unlocked) {
					if (u.Value.GetComponent<Diver> () != null) {
						Debug.Log ("found diver");
						GameObject box = (GameObject)Instantiate (selectScreenBox, containers[0].transform.position, Quaternion.identity, containers [0].transform);
						box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Diver> ().name;
						box.transform.localPosition = new Vector3 (diverAux, box.transform.position.y, box.transform.position.z);
						diverAux += boxDistance;
					}
					if (u.Value.GetComponent<Jumper> () != null) {
						Debug.Log ("found jumper");
						GameObject box = (GameObject)Instantiate (selectScreenBox, containers[1].transform.position, Quaternion.identity, containers [1].transform);
						box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Jumper> ().name;
						box.transform.localPosition = new Vector3 (jumperAux, box.transform.position.y, box.transform.position.z);
						jumperAux += boxDistance;
					}
					if (u.Value.GetComponent<Platform> () != null) {
						Debug.Log ("found platform");
						GameObject box = (GameObject)Instantiate (selectScreenBox, containers[2].transform.position, Quaternion.identity, containers [2].transform);
						box.GetComponentInChildren<UILabel> ().text = u.Value.GetComponent<Platform> ().name;
						box.transform.localPosition = new Vector3 (platformAux, box.transform.position.y, box.transform.position.z);
						platformAux += boxDistance;
					}
				}
			};

			//select = (GameObject)Instantiate (selectScreen, transform.position, Quaternion.identity);
			selectScreen.SetActive (true);
			selectScreenVisible = true;
		};
	}
}
