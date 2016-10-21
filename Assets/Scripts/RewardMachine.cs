using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RewardMachine : MonoBehaviour {

	Dictionary<int,Unlockable> availableRewards;
	public MainController controller;

	public int [] tierCost = {250,300,500};
	public int currentTier = 0;

	public int remainingRewards;

	// Use this for initialization
	void Start () 
	{
		availableRewards = new Dictionary<int,Unlockable> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		remainingRewards = availableRewards.Count;

		if (Input.GetKeyDown (KeyCode.R) && controller.testing) 
		{
			GetReward ();
		};

		if (Input.GetKeyDown (KeyCode.T) && controller.testing) 
		{
			LoadTier(currentTier);
		};
	}

	//intenta desbloquear un item
	public void GetReward()
	{
		if (availableRewards.Count > 0 && controller.coins >= tierCost [currentTier]) {
			int key = GetRandom ();
			Unlockable u;
			if (controller.unlockables.TryGetValue (key, out u)) 
			{
				UnlockItem (u, key);
			};
			controller.coins -= tierCost [currentTier];
		}
		else if (controller.coins < tierCost [currentTier]) 
		{
			Debug.Log ("Not enough coins!");
		}
		else
		{
			Debug.Log ("All items are unlocked!");
		}
	}

	//carga los unlockables del tier indicado
	public void LoadTier(int tier)
	{
		foreach (KeyValuePair<int,Unlockable> kp in controller.unlockables) 
		{
			if (kp.Value.tier == tier && kp.Value.unlocked == false) 
			{
				availableRewards.Add (kp.Key,kp.Value);	
			};
		};
	}

	//TESTING: carga todos los unlockables sin diferenciar por tier
	public void LoadTier()
	{
		foreach (KeyValuePair<int,Unlockable> kp in controller.unlockables) 
		{
			availableRewards.Add (kp.Key,kp.Value);	
		};
	}

	//devuelve una key al azar del diccionario availableRewards
	int GetRandom()
	{
		int rand = (int)Random.Range (0, availableRewards.Count);
		List<int> key = Enumerable.ToList (availableRewards.Keys);
		return key[rand];
	}

	//desbloquea un item y lo remueve de la lista de items disponibles
	void UnlockItem(Unlockable u, int k)
	{
		u.unlocked = true;
		PlayerPrefs.SetInt (u.name, 1);
		availableRewards.Remove (k);
		Debug.Log ("Unlocked " + u.name);
	}
}
