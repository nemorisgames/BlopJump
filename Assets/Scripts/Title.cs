using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void PlayButton(){
		SceneManager.LoadScene ("Setting");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
