using UnityEngine;
using System.Collections;

public class BFOVersion : MonoBehaviour {
	public GUIStyle Style;
	public string BFOversion;
	public string BFOEdition;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.Box (new Rect (10, Screen.height - 30, 100, 20), BFOversion, Style);
		GUI.Box (new Rect (Screen.width-130, Screen.height - 30, 100, 20), BFOEdition, Style);
	}
}
