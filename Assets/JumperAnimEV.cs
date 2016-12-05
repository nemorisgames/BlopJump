using UnityEngine;
using System.Collections;

public class JumperAnimEV : MonoBehaviour {

    GameController gc;
	// Use this for initialization
	void Start () {
        gc = GameObject.FindGameObjectWithTag("Blop").GetComponent<GameController>();
	}
	
    void JumperAnimationEvents(string s)
    {
        gc.JumperEV(s);
    }
}
