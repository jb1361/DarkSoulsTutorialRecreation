using UnityEngine;
using System.Collections;

public class SkyBoxFollowPlayer : MonoBehaviour {
	GameObject skybox;
	// Use this for initialization
	void Start () {
		
		skybox = GameObject.Find ("Sky Dome");
		//skybox.transform.parent = playerspawned.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
		skybox.transform.position = this.transform.position;



	}
}
