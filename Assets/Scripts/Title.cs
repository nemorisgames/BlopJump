using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
	public AudioClip clip;
	public SpilGamesAPI api;
	public UILabel muteLabel;
	public UISprite muteButton;
	public UISprite moreGames;
	// Use this for initialization
	void Start () {
		StartCoroutine (playSplash());
		muteButton.alpha = 0;
		moreGames.alpha = 0;
	}

	IEnumerator playSplash(){
		yield return new WaitForSeconds (1.5f);
		GetComponent<AudioSource> ().PlayOneShot (clip);
	}

	public void PlayButton(){
		SceneManager.LoadScene ("Setting");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MuteButton(){
		if (PlayerPrefs.GetInt ("Muted") == 0) {
			PlayerPrefs.SetInt ("Muted", 1);
			Camera.main.GetComponent<AudioSource> ().mute = true;
			muteLabel.text = "Unmute\nAudio";
		} else {
			PlayerPrefs.SetInt ("Muted", 0);
			Camera.main.GetComponent<AudioSource> ().mute = false;
			muteLabel.text = "Mute\nAudio";
		}
	}
}
