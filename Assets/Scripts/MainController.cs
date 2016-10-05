using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainController : MonoBehaviour {
	public bool testing; //remove l8r

	int _currentSceneIndex;

	[HideInInspector]
	public int highScore;
	[HideInInspector]
	public int coins;

	public Dictionary<int,Unlockable> unlockables;

	public List<Diver> diverList; //para agregar prefabs desde el inspector
	public List<Jumper> jumperList; //para agregar prefabs desde el inspector
	public List<Platform> platformList; //para agregar prefabs desde el inspector
	public RewardMachine rewardMachine;

	// Use this for initialization
	void Start () 
	{
		unlockables = new Dictionary<int,Unlockable> ();
		LoadDictionaries ();
		rewardMachine.LoadTier ();
		Debug.Log ("Presiona 'L' para ver lista de items y estado, 'R' para desbloquear un item");
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
		}
	}

	//carga datos de las listas expuestas en el inspector a los diccionarios
	void LoadDictionaries()
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
}
