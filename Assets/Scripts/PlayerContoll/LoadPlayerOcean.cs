using UnityEngine;
using System.Collections;

public class LoadPlayerOcean : MonoBehaviour {

	// Use this for initialization
	public GameObject Player;
	public GameObject PlayerSpawn;

	GameObject p;
		GameObject s;
	 //this will need to be changed later as a player loads the scene or retirieve their cords
	public GameObject Ship;
	 Vector3 Spawn;
	Vector3 playerspawn;
	// Use this for initialization
	void Start () {
		Application.runInBackground =true;
		Spawn = this.transform.position;
		playerspawn = PlayerSpawn.transform.position;
		s = (GameObject)Instantiate (Ship, Spawn, Quaternion.identity);
		p = (GameObject)Instantiate (Player, playerspawn, Quaternion.identity);
		p.transform.parent = s.transform.GetChild(0).transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
