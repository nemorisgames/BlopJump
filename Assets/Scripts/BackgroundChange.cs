using UnityEngine;
using System.Collections;

public class BackgroundChange : MonoBehaviour {
	public GameObject[] fondos;
	public Material[] skyboxes;
	int indice = 0;
	// Use this for initialization
	void Start () {
		//change ();
	}

	public void change(){
		indice++;
		for (int i = 0; i < fondos.Length; i++) {
			fondos [i].SetActive (i == indice);
		}
		RenderSettings.skybox = skyboxes[indice];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
