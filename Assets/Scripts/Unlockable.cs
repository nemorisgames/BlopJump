using UnityEngine;
using System.Collections;

public class Unlockable : MonoBehaviour 
{
	public string name; //nombre
	public string description; //descripcion
	public int tier; //tier de unlockables al que pertenece
	public bool unlocked;
	public GameObject gameObject;
}
