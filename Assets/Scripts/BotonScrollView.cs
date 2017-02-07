using UnityEngine;
using System.Collections;

public class BotonScrollView : MonoBehaviour {
	public UIPanel panel;
	public enum direction {right, left};
	public direction direccion;
	// Use this for initialization
	void Start () {
	
	}

	public void click(){
		float displace = 140f;
		if (direccion == direction.left)
			displace = -140f;
		panel.transform.localPosition += new Vector3 (displace, 0f, 0f);
		panel.clipOffset += new Vector2 (-displace, 0f);
		panel.transform.GetComponent<UIScrollView> ().RestrictWithinBounds (true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
