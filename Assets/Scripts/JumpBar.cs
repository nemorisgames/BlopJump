using UnityEngine;
using System.Collections;

public class JumpBar : MonoBehaviour {
	UISlider jumpBar;
	UISprite jumpBarFill;
	bool jumpBarForward;
	public float jumpBarStep;
	public float sweetSpotLowLimit;
	public float sweetSpotHighLimit;
	Color baseColor;
	Color targetColor;

	// Use this for initialization
	void Awake () {
		jumpBar = GetComponent<UISlider> ();
		jumpBarFill = transform.FindChild ("JumpBarFill").GetComponent<UISprite>();
		baseColor = Color.red;
		targetColor = jumpBarFill.color;
	}
	
	// Update is called once per frame
	void Update () {
		jumpBarFill.color = Color.Lerp (baseColor, targetColor, jumpBar.value);
	}

	public void Initialize()
	{
		jumpBar.gameObject.SetActive (true);
		//jumpBarForward = true;
		jumpBar.value = 0;
	}

	public void UpdateValue()
	{
		if (jumpBarForward) 
		{
			if (jumpBar.value >= 1) 
			{
				jumpBarForward = false;
			} 
			else
				jumpBar.value += jumpBarStep;
		} 
		else 
		{
			if(jumpBar == null) 
				jumpBar = GetComponent<UISlider> ();
			if (jumpBar.value <= 0) 
			{
				jumpBarForward = true;
			} 
			else
				jumpBar.value -= jumpBarStep;
		}
	}
}
