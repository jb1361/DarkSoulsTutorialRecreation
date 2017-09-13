using UnityEngine;
using System.Collections;

public class LoadPlayer : MonoBehaviour {

	public GameObject Player;
	GameObject player;
	public Vector3 Spawn; //this will need to be changed later as a player loads the scene or retirieve their cords

	// Use this for initialization
	void Start () {
		Application.runInBackground =true;
		Spawn = this.transform.position;
	player = (GameObject)Instantiate (Player, Spawn, Quaternion.identity);
		player.name = "Player";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
