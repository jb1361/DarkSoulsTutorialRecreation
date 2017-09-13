using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {
	public GameObject Player;
	public string PlayerName;
	public GameObject playerText;
	public Camera PlayersCamera;
	// Use this for initialization
	void Start () {
				//if (Steamworks.SteamAPI.IsSteamRunning()==true) {
					//	PlayerName = Steamworks.SteamFriends.GetPersonaName ();
					//	TextMesh tm = playerText.GetComponent<TextMesh> ();
					//	tm.text = PlayerName;
					//	PlayersCamera = Camera.main;
				//} else {
						PlayerName = PlayerPrefs.GetString ("PlayerName");
						TextMesh tm = playerText.GetComponent<TextMesh> ();
						tm.text = PlayerName;
						PlayersCamera = Camera.main;
						//PlayerName = Steamworks.SteamUser.GetSteamID().ToString();
						//Instantiate (playerText, this.transform.position, Quaternion.identity);
				//}
		}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.LookAt (PlayersCamera.gameObject.transform.position);
	}

	void SetPlayerName(){
		TextMesh tm = playerText.GetComponent<TextMesh> ();
		tm.text = PlayerName;
	}
}
