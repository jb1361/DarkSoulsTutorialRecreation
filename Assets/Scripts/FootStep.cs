using UnityEngine;
using System.Collections;

public class FootStep : MonoBehaviour {

	public AudioClip sound1; // drag the sound 1 here
	public AudioClip sound2; // drag the sound 2 here
	
	void Start () {
		
	}
	
	void Update () {
		
		
		if(Input.GetButtonDown("Vertical")){
			
			//audio.PlayOneShot(sound1);
			GetComponent<AudioSource>().loop = true; GetComponent<AudioSource>().clip = sound1; GetComponent<AudioSource>().Play();
		}
		if(Input.GetButtonUp("Vertical")){
			
			//audio.PlayOneShot(sound1);
			GetComponent<AudioSource>().Stop(); 
		}
		
		
	}
}
