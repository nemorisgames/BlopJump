using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RewardMachine : MonoBehaviour {

	[HideInInspector]
	public Dictionary<int,Unlockable> availableRewards;
	public MainController controller;
	public GameObject rewardScreen;

	public int [] tierCost = {250,300,500};
	public int currentTier = 0;

	public int remainingRewards;

	public bool rewardScreenVisible = false;

	UILabel infoLabel;
	UILabel rewardBoxLabel;
	UILabel rewardButtonLabel;
	UISprite rewardBox;

	GameController gameController;

	// Use this for initialization
	void Start ()
	{
		controller = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
		gameController = GameObject.FindGameObjectWithTag ("Blop").GetComponent<GameController> ();
		//availableRewards = new Dictionary<int,Unlockable> ();
		//rewardScreen.SetActive (false);
		infoLabel = rewardScreen.transform.FindChild ("Info").GetComponent<UILabel> ();
		rewardBoxLabel = rewardScreen.transform.FindChild ("RewardItem").transform.FindChild ("Label").GetComponent<UILabel> ();
		rewardBox = rewardScreen.transform.FindChild ("RewardItem").transform.FindChild ("Checked").GetComponent<UISprite> ();
		rewardButtonLabel = rewardScreen.transform.FindChild ("RewardButton").transform.FindChild ("Label").GetComponent<UILabel> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		remainingRewards = availableRewards.Count;

		if (Input.GetKeyDown (KeyCode.T) && controller.testing) 
		{
			LoadTier(currentTier);
		};
	}

	//intenta desbloquear un item
	public void GetReward()
	{
		if (availableRewards.Count > 0 && controller.coins >= tierCost [currentTier]) {
			StartCoroutine (RewardMachineEffect (20));
		}
		else if (controller.coins < tierCost [currentTier]) 
		{
			infoLabel.text = "Not enough coins!";
			Debug.Log ("Not enough coins!");
		}
		else
		{
			infoLabel.text = "All items are unlocked!";
			Debug.Log ("All items are unlocked!");
		}
	}

	IEnumerator RewardMachineEffect(int length){
		Unlockable u;
		int key;
		for (int i = 0; i < length; i++) {
			key = GetRandom ();
			if (controller.unlockables.TryGetValue (key, out u)) {
				rewardBoxLabel.text = u.name;
				rewardBox.spriteName = u.gameObject.name;
			}
			if(i == length-1)
				UnlockItem (u, key);
			yield return new WaitForSeconds (i / 20);
		}
		controller.coins -= tierCost [currentTier];
		//rewardBoxLabel.text = u.name;
		infoLabel.text = "Item unlocked!";
		rewardButtonLabel.text = "Get Reward (" + tierCost [currentTier]+" coins)";
		UpdateCoins ();
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
			if (kp.Value.unlocked == false) {
				availableRewards.Add (kp.Key, kp.Value);
			}
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

	public void ToggleRewardScreen()
	{
		if (!gameController.playing || gameController.controllingJumper) {
			if (rewardScreenVisible && gameController.controllingDiver) {
				gameController.ResetRound ();
			}
			if (controller.selectScreenVisible) {
				controller.ToggleSelectScreen ();
				controller.LastLoadout ();
			}
			if (gameController.endRoundScreenVisible) {
				gameController.ToggleEndRoundScreen ();
			}
			if (rewardScreenVisible) {
				controller.gameController.waiting = true;
				rewardScreen.GetComponent<TweenAlpha>().PlayReverse();
				//rewardScreen.SetActive (false);
				rewardScreenVisible = false;
				controller.EnableAd (false);
			} else {
				gameController.jumpBar.Initialize ();
				UpdateCoins ();
				infoLabel.text = "";
				rewardBoxLabel.text = "???";
				rewardBox.spriteName = "";
				rewardButtonLabel.text = "Get Reward (" + tierCost [currentTier] + " coins)";
				controller.gameController.waiting = false;
				//rewardScreen.SetActive (true);
				rewardScreen.GetComponent<TweenAlpha>().PlayForward();
				rewardScreenVisible = true;
				controller.EnableAd (true);
			}
		}
	}

	void UpdateCoins()
	{
		rewardScreen.transform.FindChild ("Coins").GetComponent<UILabel> ().text = "Your coins: " + controller.coins;
	}
}
