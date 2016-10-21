﻿using UnityEngine;
using System.Collections;

public class Diver : Unlockable 
{
	public string difficulty; //dificultad de juego del diver
	public string trickName; //nombre del truco que realiza este diver
	public string trickDescription; //descripcion del truco que realiza este diver
	public float spinSpeed; //velocidad de giro del diver
	public float trickSpinSpeed; //velocidad de giro del diver al realizar el truco

	[HideInInspector]
	public bool onGround = true;

	/*void OnTriggerEnter(Collider other){
		if(other.tag == "Blop"){
			onGround = true;
		};
	}

	void OnTriggerExit(Collider other){
		if(other.tag == "Blop"){
			onGround = false;
		};
	}*/
}
