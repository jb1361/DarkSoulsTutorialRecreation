using UnityEngine;
using System.Collections;

public class GermanMode : MonoBehaviour {

	// Use this for initialization


		public MovieTexture movTexture;
	//public AudioClip anthem;
		void Start() {
			GetComponent<Renderer>().material.mainTexture = movTexture;
			movTexture.Play ();
	

		}

	// Update is called once per frame
	void Update () {
	
	}
}
