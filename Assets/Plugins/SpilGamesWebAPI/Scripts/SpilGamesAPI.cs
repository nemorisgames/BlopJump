using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SpilGamesAPI : MonoBehaviour 
{


	//debug text for example project, feel free to take this out
	public Text debugText;

	//replace with your Spil `Games web game ID
	public string GAME_ID = "576742227280293562";
	
	// the custom height used by the custom height method
	public int customHeight;
	
	//the logo of the portal
	private Texture2D _logo;

	//the button that shows the branding logo
	public Image brandingButtonImage;

	//have we recived the logo texture from the portal
	private bool _hasTexture = false;

	string version = "0.0.8";
	
	void Start () 
	{
		this.gameObject.name = "SpilGamesAPI";
		//call to the API with the game ID, request assets



#if UNITY_WEBPLAYER
		Application.ExternalEval (
			"(function() {function onLoadError () {if(typeof u !== 'undefined' && u.getUnity && typeof u.getUnity === 'function'){u.getUnity().SendMessage('SpilGamesAPI', 'APILoadingError', '');}};function initUnity () {if(typeof u !== 'undefined' && u.getUnity && typeof u.getUnity === 'function'){var setGameAPILogo = function (logoUrl) {u.getUnity().SendMessage('SpilGamesAPI', 'setGameAPILogo', logoUrl);};var apiInstance;GameAPI.loadAPI(function(api){apiInstance = api;apiInstance.Branding.displaySplashScreen(function(){});var logoData = apiInstance.Branding.getLogo();if(logoData.image){setGameAPILogo(logoData.image);}if (api.Game.isSiteLock()) {u.getUnity().SendMessage('SpilGamesAPI', 'OnSiteLock', '');}window.onkeydown = function(){};}, {id:'" + GAME_ID + "'});}};function loadScript(src, callback) {var s,r,t;r = false;s = document.createElement('script');s.type = 'text/javascript';s.src = src;s.onerror = onLoadError;s.onload = s.onreadystatechange = function() {if ( !r && (!this.readyState || this.readyState == 'complete') ){r = true;callback();}};t = document.getElementsByTagName('script')[0];t.parentNode.insertBefore(s, t);};if (typeof GameAPI !== 'undefined') {initUnity();} else {loadScript('http://cdn.gameplayer.io/api/js/game.js', initUnity);}})();"
			);
#endif
#if UNITY_WEBGL
		Application.ExternalEval (
			"(function() {function onLoadError () {SendMessage('SpilGamesAPI', 'APILoadingError', '');};function initUnity () {var setGameAPILogo = function (logoUrl) {SendMessage('SpilGamesAPI', 'setGameAPILogo', logoUrl);};var apiInstance;GameAPI.loadAPI(function(api){apiInstance = api;apiInstance.Branding.displaySplashScreen(function(){});var logoData = apiInstance.Branding.getLogo();if(logoData.image){setGameAPILogo(logoData.image);}if (api.Game.isSiteLock()) {SendMessage('SpilGamesAPI', 'OnSiteLock', '');}window.onkeydown = function(){};}, {id:'" + GAME_ID + "'});};function loadScript(src, callback) {var s;var r;var t;r = false;s = document.createElement('script');s.type = 'text/javascript';s.src = src;s.onerror = onLoadError;s.onload = s.onreadystatechange = function() {if ( !r && (!this.readyState || this.readyState == 'complete') ){r = true;callback();}};t = document.getElementsByTagName('script')[0];t.parentNode.insertBefore(s, t);};if (typeof GameAPI !== 'undefined') {initUnity();} else {loadScript('http://cdn.gameplayer.io/api/js/game.js', initUnity);}})();"
			);
#endif


		debugText.text = "APILoaded";
		Application.ExternalEval ("if (console && console.log) console.log('plugin version : " + version + "');");
	}

	//this will fire if the API is unable to load for any reason
	void APILoadingError(){
		debugText.text = "No API Loaded";
	}

	void Awake() 
	{
		//keep the api object throughout the game
		DontDestroyOnLoad(gameObject);
	}


	public bool isInitialized 
	{
		get { return _hasTexture; }
	}
	
	void WebAlert(string txt)
	{
		// Application.ExternalCall( "SayHello", txt);
	}
	void setGameAPILogo (string url) 
	{
		if (url != "false") {
			debugText.text = "setGameAPILogo" + url;
			WebAlert ("Got URL " + url + " - will load!");
			StartCoroutine (loadLogo (url));
		}
	}
	
	public void AdjustHeight(){
		Application.ExternalEval (
			"GameAPI.Game.adjustHeight("+ customHeight.ToString() + ");"
		);
	}

	IEnumerator loadLogo(string url) 
	{
		debugText.text ="loadlogostart" + url;
		WebAlert( "Loading texture from "+url );
		
		WWW www = new WWW(url);
		yield return www;
		
		WebAlert( "Loaded texture: "+www.error );
		
		_logo = www.texture;
		_hasTexture = true;
		debugText.text = url;
		if(_logo != null){
			brandingButtonImage.sprite = Sprite.Create(_logo , new Rect(0,0,_logo.width,_logo.height),Vector2.zero);
			brandingButtonImage.color = new Color(1,1,1,1);
		}
	}

	public void OnSiteLock()
	{
		// lock the game
	}

	//social features
	public void ForceAuthentication(){
		Application.ExternalEval (
			"GameAPI.User.forceAuthentication();"
		);
	}

	public void SetUpUser(string userData){
		debugText.text = "SetUpUser Called";
		debugText.text += userData;
	}
	
	public void GetUser(){

		#if UNITY_WEBPLAYER
		Application.ExternalEval (
			"GameAPI.User.getUser(function (userData){if(typeof u !== 'undefined' && u.getUnity && typeof u.getUnity === 'function'){u.getUnity().SendMessage('SpilGamesAPI', 'SetUpUser', JSON.stringify(userData));}});"
			);
		#endif
		#if UNITY_WEBGL
		Application.ExternalEval (
			"GameAPI.User.getUser(function(userData) {SendMessage('SpilGamesAPI', 'SetUpUser', JSON.stringify(userData));});"
			);
			#endif
	}
	
	public void ShowInvite(){
		Application.ExternalEval (
			"GameAPI.Friends.showInvite();"
		);
	}
	
	public void SetUpFriends(string friendsData){
		debugText.text = "SetUpFriends Called";
		debugText.text += friendsData;
	}
	
	public void GetFriends(){
		#if UNITY_WEBPLAYER
		Application.ExternalEval (
			"GameAPI.Friends.getFriends(function (friendsData){if(typeof u !== 'undefined' && u.getUnity && typeof u.getUnity === 'function'){u.getUnity().SendMessage('SpilGamesAPI', 'SetUpFriends', JSON.stringify(friendsData));}});"
		);
		#endif
		#if UNITY_WEBGL
		Application.ExternalEval (
			"GameAPI.Friends.getFriends(function(friendsData) {SendMessage('SpilGamesAPI', 'SetUpFriends', JSON.stringify(friendsData));});"
			);
		#endif
	}

	//submit a player score to the portal
	public void ScoreSubmit(int score){
		Application.ExternalEval ("GameAPI.Score.submit('" + score.ToString() + "');"
		);
	}

	//submit an award to the portal (dont forget that this needs to be setup in the portal first)
	public void AwardSubmit(string awardName){
		Application.ExternalEval ( "GameAPI.Award.submit({award:'" + awardName + "'});"
			);
	}

	//Enter the code here to pause your game before an AD, make sure the sound is muted too
	public void pauseGame () 
	{
		debugText.text = "Game Paused";
		Time.timeScale = 0;
	}

	//Enter the code here to resume your game after an AD
	public void resumeGame () 
	{
		debugText.text = "Game Resumed";
		Time.timeScale = 1;
	}
	
	//call for a break in the game, this will most likely be an Ad but could be other branding
	public void GameBreak()
	{
		#if UNITY_WEBPLAYER
		Application.ExternalEval (
			"GameAPI.GameBreak.request(function(){if(typeof u !== 'undefined' && u.getUnity && typeof u.getUnity === 'function'){u.getUnity().SendMessage('SpilGamesAPI', 'pauseGame', '');}},function(){if(typeof u !== 'undefined' && u.getUnity && typeof u.getUnity === 'function'){u.getUnity().SendMessage('SpilGamesAPI', 'resumeGame', '');}});"
			);
		#endif
		#if UNITY_WEBGL
		Application.ExternalEval (
			"GameAPI.GameBreak.request(function(){SendMessage('SpilGamesAPI', 'pauseGame', '');},function(){SendMessage('SpilGamesAPI', 'resumeGame', '');});"
			);
		#endif
	}
	
	//show the more games stuff
	public void ShowSpilMoreGames()
	{
		Application.ExternalEval (
			"GameAPI.Branding.getLink('more_games').action();"
		);
	}
	
	//Open the iOS app store in a new tab. Ask your publishing contact to set this link up with the label 'app_store'
	public void OpenAppStoreLink()
	{
		Application.ExternalEval (
			"if (GameAPI && GameAPI.isReady) {GameAPI.Branding.getLink('app_store').action();}"
		);
	}
	
	//Open the Google Play store in a new tab. Ask your publishing contact to set this link up with the label 'goole_play'
	public void OpenGooglePlayLink()
	{
		Application.ExternalEval (
			"if (GameAPI && GameAPI.isReady) {GameAPI.Branding.getLink('google_play').action();}"
		);
	}

	//Open the Amazon store in a new tab. Ask your publishing contact to set this link up with the label 'amazon'
	public void OpenAmazonLink()
	{
		Application.ExternalEval (
			"if (GameAPI && GameAPI.isReady) {GameAPI.Branding.getLink('amazon').action();}"
		);
	}

	//when the branding logo is clicked
	public void BrandingLogoClicked(){
		if (_logo != null) {
			Application.ExternalEval ("GameAPI.Branding.getLogo().action();");
		}
	}

}
