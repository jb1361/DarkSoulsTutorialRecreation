using UnityEngine;
using System.Collections;

public class GUIPopUp : MonoBehaviour {
	bool popup;
	string popupmessage;
	public GUIStyle style;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	



	}

	void OnGUI(){
		if (popup == true) {
						DrawPopUp ();
				}

	}
	void DrawPopUp(){

		GUI.Label (new Rect (Screen.width / 2-150, Screen.height - 50, 100, 30), popupmessage, style);
	}

	void GetPopUpInfo(string info){
		popupmessage = info;
		if (popupmessage != "") {
			popup = true;
				} else {
			popup = false;
				}
	}
	void NullPopUp(){
		popupmessage = "";
		popup = false;
	}

}
