using UnityEngine;
using System.Collections;

public class SaveKey : MonoBehaviour {

	MainController mc;

	void Start(){
		mc = GameObject.FindGameObjectWithTag ("MainController").GetComponent<MainController> ();
	}
	
	// Update is called once per frame
	public void SetKey () {
		if (tag == "Diver") {
			mc.diverKey = int.Parse (name);
			Debug.Log (mc.diverKey);
		} else if (tag == "Jumper") {
			mc.jumperKey = int.Parse (name);
			Debug.Log (mc.jumperKey);
		} else if (tag == "Platform") {
			mc.platformKey = int.Parse (name);
			Debug.Log (mc.platformKey);
		};
	}
}
