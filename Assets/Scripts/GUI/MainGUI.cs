using UnityEngine;
using System.Collections;

public class MainGUI : MonoBehaviour {
	public Texture HealthGUI;
	void OnGUI() {
				if (!HealthGUI) {
						Debug.LogError ("You forgot to assign a health texture.");
						return;
				}
				GUI.DrawTexture (new Rect (10, -10, 250, 100), HealthGUI);
		}
	
}
