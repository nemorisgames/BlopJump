using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
	public AudioClip clip;
	public SpilGamesAPI api;
	// Use this for initialization
	void Start () {
		StartCoroutine (playSplash());
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
}
