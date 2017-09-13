using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	public Texture2D Background;
	string mainMenuSceneName;
    public Font pauseMenuFont;
	public GUISkin skin;
	public GameObject CameraGodRays;
	public GameObject CameraNoGodRays;
	GameObject playerscamera;




public	Component godrays;
//settings be here
	bool mainmenu;
	bool showcontrols;
	bool showgraphics;
	bool showaudio;
	bool showquitoption;
	bool shadowqualitydropdown;
	bool qualitysettingsdropdown;
	bool usergraphicsettings;
	//
	bool VSync;
	bool GodRays;
	//int renderdistance;
	bool shadows = true;
	float renderdistance = 10000;
	float shadowdistance = 1000;
	//float waterreflectionquality = 512;
	float mousespeed  = 10.0f;
	bool fullscreen;
	bool resolutiondropdown;
	bool texturequalitydropdown;
	bool AADropDown;
	bool waterqualitydropdown;
	bool waterreflectmodedropdown;
	bool waterreflectqualitydropdown;
	float mastervolume = 1f;
	float musicvolume = 1f;
	float ambientvolume = 1f;
	//float ambientvolume = 1f;

	//Resolution[] resolutions = Screen.resolutions;

	 Vector2 overallqualityscrollPosition = Vector2.zero;
	 Vector2 advancedgraphicsPosition = Vector2.zero;
	 Vector2 Resolutionscroll = Vector2.zero;
	 Vector2 TextureQualityScroll = Vector2.zero;
	 Vector2 AAScroll = Vector2.zero;
	//Vector2 WaterQuality = Vector2.zero;
	//Vector2 ReflectionMode = Vector2.zero;


	void Awake(){
		fullscreen = Screen.fullScreen;
		playerscamera = GameObject.FindGameObjectWithTag ("MainCamera").gameObject;
		mainmenu = false;
		showaudio = false;
		showgraphics = false;
		showcontrols = false;
		var distances = new float[32];
		distances[0] = (int)renderdistance;
		distances[10] =100000;
		playerscamera.GetComponent<Camera>().layerCullDistances = distances;
		//these next lines are just to get rid of caution labels

	}
	void OnEnable(){
		mainmenu = false;
		showaudio = false;
		showgraphics = false;
		showcontrols = false;

	}
	
	void Update(){
		
		//check if pause button (escape key) is pressed

		if(Input.GetKeyDown("escape")){
			
					
			if(mainmenu == true || showaudio == true || showgraphics == true){
				//unpause the game
				mainmenu = false;
				showaudio = false;
				showgraphics = false;
				showcontrols = false;
				Cursor.lockState.Equals(CursorLockMode.Locked);
				//Screen.lockCursor = true; Deprecated
				Cursor.visible = false;	
			}
			

			else if(mainmenu == false){
	
				mainmenu = true;
				Cursor.lockState.Equals(CursorLockMode.None);
				//Screen.lockCursor = false;  Deprecated
				Cursor.visible = true;
				
			}

		}
	
	}

	void SetNewDrawDistance(){
		var distances = new float[32];
		distances[0] = (int)renderdistance;
		distances[10] =100000;
		playerscamera.GetComponent<Camera>().layerCullDistances = distances;

		}



	
	void OnGUI(){
		
		GUI.skin.box.font = pauseMenuFont;
		GUI.skin.button.font = pauseMenuFont;
		
		if(mainmenu == true || showquitoption == true){	
			DrawPauseMenu();
		}
		if (showgraphics == true) {
			DrawGraphicsSettings();
				}
		if (showaudio == true) {
			DrawAudioSettings();
				}
		if (showcontrols == true) {
			DrawControlSettings();
		}


	}


	void DrawPauseMenu(){
		if(mainmenu == true){
				//Make a background box
				GUI.DrawTexture (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 100, 300, 300), Background);

				//Make Main Menu button
				//	if(GUI.Button(new Rect(Screen.width /2 - 100,Screen.height /2 - 50,250,50), "Unstuck")){
				//	PlayerPrefs.SetInt("Level",3);
				//		Application.LoadLevel(1);
				//	}
				if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 50, 250, 50), "Controls")) {
					showcontrols = !showcontrols;
					mainmenu = false;
				}
	
				if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2, 250, 50), "Audio")) {
						showaudio = !showaudio;
						mainmenu = false;
				}
				if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2 + 50, 250, 50), "Graphics")) {
						showgraphics = !showgraphics;
						mainmenu = false;
				}
				if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2 + 100, 250, 50), "Quit Game")) {
				showquitoption = true;
				mainmenu = false;
					//	Application.Quit ();
				}
		
	

		

		
		
		
		}

		if(showquitoption == true){
			GUI.Label (new Rect (Screen.width / 2 - 100, Screen.height / 2 , 50, 50), "Are you sure?");
			if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2 + 50, 50, 50), "Yes")) {
				Application.Quit ();
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 50, Screen.height / 2 + 50, 50, 50), "No")) {
				mainmenu = true;
				showquitoption = false;
			}
			
			
		}
	




	}

	void DrawGraphicsSettings(){

		GUI.skin = skin;
		//scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 2 -20, Screen.height / 2, 200, 200), scrollPosition, new Rect(0, 0, 0, 300));
		GUI.DrawTexture (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 100, 300, 300), Background);
	//	Rect Antialiasing = new Rect (Screen.width / 2 - 100, Screen.height - 50, 250, 50);

		Rect OverallQuality = new Rect (Screen.width / 2, Screen.height/2 -50, 55,25);



		//preset settings
		GUI.Label (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 50 , 200, 30), "Overall Quality");
		string quality = "";
		int qualitylevel = QualitySettings.GetQualityLevel ();
		if (qualitylevel == 0) {
			quality = "Very Low";
		}
		if (qualitylevel == 1) {
			quality = "Low";
		}
		if (qualitylevel == 2) {
			quality = "Medium";
		}
		if (qualitylevel == 3) {
			quality = "High";
		}
		if (qualitylevel == 4) {
			quality = "Very High";
		}
		if (qualitylevel == 5) {
			quality = "Ultra";
		}
		if (usergraphicsettings == true) {
			quality = "Custom";
				}
		GUI.Box((OverallQuality),quality,skin.GetStyle ("dropdown"));

		if (OverallQuality.Contains (Event.current.mousePosition)) {
			
			if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
				
				qualitysettingsdropdown = !qualitysettingsdropdown;
			}
		}
		if(qualitysettingsdropdown == true){
			overallqualityscrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 2+70, Screen.height/2 -50, 80, 50), overallqualityscrollPosition, new Rect(0, 0, 0, 150));
			for(int i = 0; i < 7; i++){
				Rect QualitySettingsDropdown = new Rect (0,i*20, 80,25);
				if( i == 0){
					GUI.Box((QualitySettingsDropdown)," Very Low",skin.GetStyle ("dropdown"));
					if (QualitySettingsDropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							QualitySettings.SetQualityLevel(0);
							renderdistance = 500;
							SetNewDrawDistance();
							qualitysettingsdropdown = false;
							usergraphicsettings = false;
						}
					}
				}
				if( i == 1){
					GUI.Box((QualitySettingsDropdown),"Low",skin.GetStyle ("dropdown"));
					if (QualitySettingsDropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							QualitySettings.SetQualityLevel(1);
							renderdistance = 1000;
							SetNewDrawDistance();
							qualitysettingsdropdown = false;
							usergraphicsettings = false;
						}
					}
				}
				if( i == 2){
					GUI.Box((QualitySettingsDropdown),"Medium",skin.GetStyle ("dropdown"));
					if (QualitySettingsDropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							QualitySettings.SetQualityLevel(2);
							renderdistance = 2500;
							SetNewDrawDistance();
							qualitysettingsdropdown = false;
							usergraphicsettings = false;
						}
					}
				}
				if( i == 3){
					GUI.Box((QualitySettingsDropdown),"High",skin.GetStyle ("dropdown"));
					if (QualitySettingsDropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							QualitySettings.SetQualityLevel(3);
							renderdistance = 5000;
							SetNewDrawDistance();
							qualitysettingsdropdown = false;
							usergraphicsettings = false;
						}
					}
				}
				if( i == 4){
					GUI.Box((QualitySettingsDropdown),"Very High",skin.GetStyle ("dropdown"));
					if (QualitySettingsDropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							QualitySettings.SetQualityLevel(4);
							renderdistance = 8000;
							SetNewDrawDistance();
							qualitysettingsdropdown = false;
							usergraphicsettings = false;
						}
					}
				}
				if( i == 5){
					GUI.Box((QualitySettingsDropdown),"Ultra",skin.GetStyle ("dropdown"));
					if (QualitySettingsDropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							QualitySettings.SetQualityLevel(5);
							renderdistance = 10000;
							SetNewDrawDistance();
							qualitysettingsdropdown = false;
							usergraphicsettings = false;
						}
					}
				}
				if( i == 6){
					GUI.Box((QualitySettingsDropdown),"Custom",skin.GetStyle ("dropdown"));
					if (QualitySettingsDropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							QualitySettings.SetQualityLevel(5);
							renderdistance = 10000;
							SetNewDrawDistance();
							qualitysettingsdropdown = false;
							usergraphicsettings = true;
						}
					}
				}
			}

			GUI.EndScrollView();
			
		}
		//this will set weather the user wants full screen or not
		fullscreen = GUI.Toggle(new Rect (Screen.width / 2 - 100, Screen.height / 2 - 30 , 100, 20), fullscreen, "FullScreen");
		Screen.fullScreen = fullscreen;
		VSync = GUI.Toggle(new Rect (Screen.width / 2, Screen.height / 2 - 30 , 100, 20), VSync, "VSync");
		if (VSync == true) {
			QualitySettings.vSyncCount = 1;
				} else {
			QualitySettings.vSyncCount = 0;
				}
	




		Rect resolution = new Rect (Screen.width / 2-35, Screen.height/2 -10, 70,25);



		GUI.Label (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 10 , 200, 30), "Resolution: ");
		int resx, resy;
		resx = 0;
		resy = 0;
		resx = Screen.width;
		resy = Screen.height;
		GUI.Box((resolution),resx + "x"+ resy,skin.GetStyle ("resolutiondropdown"));
		
		if (resolution.Contains (Event.current.mousePosition)) {
			
			if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
				
				resolutiondropdown = !resolutiondropdown;
			}
		}

		if(resolutiondropdown == true){
			Resolutionscroll = GUI.BeginScrollView(new Rect (Screen.width / 2 +45, Screen.height / 2 - 5, 90, 50), Resolutionscroll, new Rect(0, 0, 0, 205));
			//int r = 0;
	//	foreach(Resolution res in resolutions){
			//	r++;

			//}
			for(int i = 0; i < 10; i++){

				Rect Resolutiondropdown = new Rect (0,i*20, 80,25);
				if( i == 0){
					GUI.Box((Resolutiondropdown),"640x480",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(640,480,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if(i == 1){
					GUI.Box((Resolutiondropdown),"720x480",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(720,480,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 2){
					GUI.Box((Resolutiondropdown),"720x576",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(720,576,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 3){
					GUI.Box((Resolutiondropdown),"800x600",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(800,600,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 4){
					GUI.Box((Resolutiondropdown),"1024x768",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1024,768,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 5){
					GUI.Box((Resolutiondropdown),"1280x720",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1280,720,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 6){
					GUI.Box((Resolutiondropdown),"1280x768",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1280,768,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 7){
					GUI.Box((Resolutiondropdown),"1280x800",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1280,800,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 8){
					GUI.Box((Resolutiondropdown),"1280x960",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1280,960,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 9){
					GUI.Box((Resolutiondropdown),"1280x1024",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1280,1024,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 10){
					GUI.Box((Resolutiondropdown),"1600x900",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1600,900,fullscreen);
							resolutiondropdown = false;
						}
					}
				}
				if( i == 11){
					GUI.Box((Resolutiondropdown),"1920x1080",skin.GetStyle ("resolutiondropdown"));
					if (Resolutiondropdown.Contains (Event.current.mousePosition)) {
						
						if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
							Screen.SetResolution(1920,1080,fullscreen);
							resolutiondropdown = false;
						}
					}
				}





			
			}
			
			GUI.EndScrollView();
			}






		//advanced graphics settings
		if(usergraphicsettings == true){
			advancedgraphicsPosition = GUI.BeginScrollView(new Rect(Screen.width / 2-150, Screen.height/2+50, 300, 130), advancedgraphicsPosition, new Rect(0, 0, 0, 500));
		


			Rect TextureQuality = new Rect (141, 0, 55,25);
			//set texture quality 
			GUI.Label (new Rect (50,0 , 200, 30), "Texture Quality:");
			string currenttexturequality = "";
			if(QualitySettings.masterTextureLimit == 0){
				currenttexturequality = "Ultra";
			}
			if(QualitySettings.masterTextureLimit == 1){
				currenttexturequality = "High";
			}
			if(QualitySettings.masterTextureLimit == 2){
				currenttexturequality = "Medium";
			}
			if(QualitySettings.masterTextureLimit == 3){
				currenttexturequality = "Low";
			}
			GUI.Box((TextureQuality),currenttexturequality,skin.GetStyle ("dropdown"));
			if (TextureQuality.Contains (Event.current.mousePosition)) {
				
				if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
					
					texturequalitydropdown = !texturequalitydropdown;
				}
			}
			if(texturequalitydropdown == true){
				TextureQualityScroll = GUI.BeginScrollView(new Rect(210, 0, 100, 50), TextureQualityScroll, new Rect(0, 0, 0, 100));
				for(int i = 0; i < 4; i++){
					Rect TextureQualityDropdown = new Rect (0, 0 +i*20, 60,25);
					if( i == 0){
						GUI.Box((TextureQualityDropdown),"Low",skin.GetStyle ("dropdown"));
						if (TextureQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.masterTextureLimit = 3;
								texturequalitydropdown = false;
							}
						}
					}
					if( i == 1){
						GUI.Box((TextureQualityDropdown),"Medium",skin.GetStyle ("dropdown"));
						if (TextureQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.masterTextureLimit = 2;
								texturequalitydropdown = false;
							}
						}
					}
					if( i == 2){
						GUI.Box((TextureQualityDropdown),"High",skin.GetStyle ("dropdown"));
						if (TextureQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.masterTextureLimit = 1;
								texturequalitydropdown = false;
							}
						}
					}
					if( i == 3){
						GUI.Box((TextureQualityDropdown),"Ultra",skin.GetStyle ("dropdown"));
						if (TextureQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.masterTextureLimit = 0;
								texturequalitydropdown = false;
							}
						}
					}


				}
				GUI.EndScrollView();
			}





			//set AA
			Rect AAQuality = new Rect (141, 50, 55,25);
			//set texture quality 
			GUI.Label (new Rect (50,50 , 200, 30), " AntiAliasing:");
			string AA = "";
			if(QualitySettings.antiAliasing == 0){
				AA = "Disabled";
			}
			if(QualitySettings.antiAliasing == 2){
				AA = "2xMSAA";
			}
			if(QualitySettings.antiAliasing == 4){
				AA = "4xMSAA";
			}
			if(QualitySettings.antiAliasing == 8){
				AA = "8xMSAA";
			}
			GUI.Box((AAQuality),AA,skin.GetStyle ("dropdown"));
			if (AAQuality.Contains (Event.current.mousePosition)) {
				
				if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
					
					AADropDown = !AADropDown;
				}
			}
		if(AADropDown == true){
				AAScroll = GUI.BeginScrollView(new Rect(210, 50, 100, 50), AAScroll, new Rect(0, 0, 0, 100));
				for(int i = 0; i < 4; i++){
					Rect AAQualityDropdown = new Rect (0, 0 +i*20, 60,25);
					if( i == 0){
						GUI.Box((AAQualityDropdown),"Disabled",skin.GetStyle ("dropdown"));
						if (AAQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.antiAliasing = 0;
								AADropDown = false;
							}
						}
					}
					if( i == 1){
						GUI.Box((AAQualityDropdown),"2xMSAA",skin.GetStyle ("dropdown"));
						if (AAQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.antiAliasing = 2;
								AADropDown = false;
							}
						}
					}
					if( i == 2){
						GUI.Box((AAQualityDropdown),"4xMSAA",skin.GetStyle ("dropdown"));
						if (AAQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.antiAliasing = 4;
								AADropDown = false;
							}
						}
					}
					if( i == 3){
						GUI.Box((AAQualityDropdown),"8xMSAA",skin.GetStyle ("dropdown"));
						if (AAQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.antiAliasing = 8;
								AADropDown = false;
							}
						}
					}

			}
				GUI.EndScrollView();
			}


		//set shadows
		shadows = GUI.Toggle(new Rect(50,100, 67, 20), shadows, "Shadows");
		Rect ShadowQuality = new Rect (145, 140, 55,25);
		if(shadows == false){
			QualitySettings.shadowDistance = 0;
		}else{
			//shadowdistance
			GUI.Label (new Rect (50,120 , 200, 30), "Shadow Distance: ");
			shadowdistance = GUI.HorizontalSlider (new Rect (160, 125, 100, 10), shadowdistance, 100, 1000);
			QualitySettings.shadowDistance = shadowdistance;
			//shadowquality
			GUI.Label (new Rect (50, 140 , 200, 30), "Shadow Quality:");
			string currentshadowquality = "";
			if(QualitySettings.shadowCascades == 4){
				currentshadowquality = "High";
			}
			if(QualitySettings.shadowCascades == 2){
				currentshadowquality = "Medium";
			}
			if(QualitySettings.shadowCascades != 4 && QualitySettings.shadowCascades != 2){
				currentshadowquality = "Low";
			}
			GUI.Box((ShadowQuality),currentshadowquality,skin.GetStyle ("dropdown"));
			if (ShadowQuality.Contains (Event.current.mousePosition)) {
	
				if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {

							shadowqualitydropdown = !shadowqualitydropdown;
						}
					}
				if(shadowqualitydropdown == true){

				for(int i = 0; i < 3; i++){
					Rect ShadowQualityDropdown = new Rect (210, 140 +i*20, 60,25);
					if( i == 0){
						GUI.Box((ShadowQualityDropdown),"Low",skin.GetStyle ("dropdown"));
						if (ShadowQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.shadowCascades = 0;
								shadowqualitydropdown = false;
							}
						}
					}
					if( i == 1){
						GUI.Box((ShadowQualityDropdown),"Medium",skin.GetStyle ("dropdown"));
						if (ShadowQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.shadowCascades = 2;
								shadowqualitydropdown = false;
							}
						}
					}
					if( i == 2){
						GUI.Box((ShadowQualityDropdown),"High",skin.GetStyle ("dropdown"));
						if (ShadowQualityDropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								QualitySettings.shadowCascades = 4;
								shadowqualitydropdown = false;
							}
						}
					}
				}

			}





				}

	///end shadows
				
				
			GodRays = GUI.Toggle(new Rect(50,180, 100, 20), shadows, "God Rays");

			if(GodRays == false){
				//CameraNoGodRays.SetActive(true);
			//	CameraGodRays.SetActive(false);
				//CameraNoGodRays.SetActive(true);
			}else{
				//CameraGodRays.SetActive(true);
				//CameraNoGodRays.SetActive(false);

			}


			//start water options



			//GUI.Label (new Rect (50, 220 , 130, 60), "Water Reflection Quality:");

			//waterreflectionquality = GUI.HorizontalSlider (new Rect (170, 230, 100, 10), waterreflectionquality, 1, 512);
			//water.m_TextureSize = (int)waterreflectionquality;

			/*


			string currentwaterquality = "";

			if(water.m_WaterMode == Water.WaterMode.Simple){
				currentwaterquality = "Low";
			}
			if(water.m_WaterMode == Water.WaterMode.Reflective){
				currentwaterquality = "Medium";
			}
			if(water.m_WaterMode == Water.WaterMode.Refractive){
				currentwaterquality = "High";
			}

			GUI.Label (new Rect (50, 280 , 200, 30), "Water Reflect Mode:");
			Rect WaterReflectMode= new Rect (170, 280, 55,25);

			GUI.Box((WaterReflectMode),currentwaterquality,skin.GetStyle ("dropdown"));
			if (WaterReflectMode.Contains (Event.current.mousePosition)) {
				
				if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
					
					waterqualitydropdown = !waterqualitydropdown;
				}
			}


			if(waterqualitydropdown == true){
				
				for(int i = 0; i < 3; i++){
					Rect waterqdropdown = new Rect (60 + i *70, 303, 200, 30);
					if( i == 0){
						GUI.Box((waterqdropdown ),"Low",skin.GetStyle ("dropdown"));
						if (waterqdropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								water.m_WaterMode = Water.WaterMode.Simple;
								waterqualitydropdown = false;
							}
						}
					}
					if( i == 1){
						GUI.Box((waterqdropdown ),"Medium",skin.GetStyle ("dropdown"));
						if (waterqdropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								water.m_WaterMode = Water.WaterMode.Reflective;
								waterqualitydropdown = false;
							}
						}
					}
					if( i == 2){
						GUI.Box((waterqdropdown ),"High",skin.GetStyle ("dropdown"));
						if (waterqdropdown.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								water.m_WaterMode = Water.WaterMode.Refractive;
								waterqualitydropdown = false;
							}
						}
					}
				}
				
			}


			///////
			//water reflect quality

			string currentreflectlayerquality = "";
			
			if(water.m_ReflectLayers.value == 0){
				currentreflectlayerquality = "Low";
			}
			if(water.m_ReflectLayers.value == 1){
				currentreflectlayerquality = "Medium";
			}
			if(water.m_ReflectLayers.value == -1){
				currentreflectlayerquality = "High";
			}
			
			GUI.Label (new Rect (50, 325 , 200, 30), "Water Reflect Quality:");
			Rect WaterReflectQuality= new Rect (178, 325, 55,25);
			
			GUI.Box((WaterReflectQuality),currentreflectlayerquality,skin.GetStyle ("dropdown"));
			if (WaterReflectQuality.Contains (Event.current.mousePosition)) {
				
				if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
					
					waterreflectqualitydropdown = !waterreflectqualitydropdown;
				}
			}
			
			
			if(waterreflectqualitydropdown == true){
				
				for(int i = 0; i < 3; i++){
					Rect waterqualitydropdownr = new Rect (60 + i *70, 348, 200, 30);
					if( i == 0){
						GUI.Box((waterqualitydropdownr ),"Low",skin.GetStyle ("dropdown"));
						if (waterqualitydropdownr.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								water.m_ReflectLayers.value = 0;
								waterreflectqualitydropdown = false;
							}
						}
					}
					if( i == 1){
						GUI.Box((waterqualitydropdownr ),"Medium",skin.GetStyle ("dropdown"));
						if (waterqualitydropdownr.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								water.m_ReflectLayers.value = 1;
								waterreflectqualitydropdown = false;
							}
						}
					}
					if( i == 2){
						GUI.Box((waterqualitydropdownr ),"High",skin.GetStyle ("dropdown"));
						if (waterqualitydropdownr.Contains (Event.current.mousePosition)) {
							
							if (Event.current.button == 0 && Event.current.type == EventType.MouseDown) {
								water.m_ReflectLayers.value = -1;
								waterreflectqualitydropdown = false;
							}
						}
					}
				}
				
			}

			*/



			//end water options

			GUI.Label (new Rect (50,375 , 200, 30), "Render Distance:");
			renderdistance = GUI.HorizontalSlider (new Rect (170,385, 100, 10), renderdistance, 500, 10000);
			GUI.Label (new Rect (190,390 , 200, 30), renderdistance.ToString());
			//render distance
			SetNewDrawDistance();




			//end render distance

















			////end of user graphics settings
		GUI.EndScrollView();
		
	}
		if(GUI.Button(new Rect(Screen.width /2 -100,Screen.height /2 -80,30,20), "<-")){
			showgraphics = false;
			mainmenu = true;
		}
}


void DrawAudioSettings(){


		GUI.skin = skin;
		//scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 2 -20, Screen.height / 2, 200, 200), scrollPosition, new Rect(0, 0, 0, 300));
		GUI.DrawTexture (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 100, 300, 300), Background);
		GUI.Label (new Rect (Screen.width / 2 - 25, Screen.height / 2 - 50, 100, 30), "Master Volume");
		mastervolume = GUI.HorizontalSlider (new Rect (Screen.width / 2-25, Screen.height / 2 - 25, 100, 10), mastervolume, 0, 1);
		GUI.Label (new Rect (Screen.width / 2 - 25, Screen.height / 2 - 15, 100, 30), "Music Volume");
		musicvolume = GUI.HorizontalSlider (new Rect (Screen.width / 2-25, Screen.height / 2+10, 100, 10), musicvolume, 0, 1);
		GUI.Label (new Rect (Screen.width / 2 - 35, Screen.height / 2+25, 150, 30), "Ambient Noise Volume");
		ambientvolume = GUI.HorizontalSlider (new Rect (Screen.width / 2-25, Screen.height / 2+45, 100, 10), ambientvolume, 0, 1);
		GameObject []ambientsources;
		AudioListener.volume = mastervolume;
		GameObject.FindGameObjectWithTag ("music").GetComponent<AudioSource> ().volume = musicvolume;
		ambientsources = GameObject.FindGameObjectsWithTag ("ambiance");

		foreach (GameObject source in ambientsources) {
						source.GetComponent<AudioSource> ().volume = ambientvolume;

				}
		if(GUI.Button(new Rect(Screen.width /2 -100,Screen.height /2 -80,30,20), "<-")){
			showaudio= false;
			mainmenu = true;
		}


		if(GUI.Button(new Rect(Screen.width /2 -100,Screen.height /2 -80,30,20), "<-")){
			showgraphics = false;
			mainmenu = true;
		}
	}
void DrawControlSettings(){
		GUI.skin = skin;

		GUI.DrawTexture (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 100, 300, 300), Background);
	
		if(GUI.Button(new Rect(Screen.width /2 -100,Screen.height /2 -80,30,20), "<-")){
			showcontrols= false;
			mainmenu = true;
		}

		GUI.Label (new Rect (Screen.width / 2 - 50, Screen.height / 2, 200, 30), "Mouse Sensitivety");
		GUI.Label (new Rect (Screen.width / 2 + 50, Screen.height / 2 + 20, 200, 30), ((mousespeed / 100) * 10).ToString ());
		mousespeed = GUI.HorizontalSlider (new Rect (Screen.width / 2 - 50, Screen.height / 2 + 20, 100, 30), mousespeed, 0.1F, 100.0F);
		
		
		if (GUI.Button (new Rect (Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 30), "Apply")) {
			//CameraScript.SendMessage ("Changesensitivety", mousespeed);
			showcontrols = false;
			mainmenu = true;
			
		}






		
	}





}
