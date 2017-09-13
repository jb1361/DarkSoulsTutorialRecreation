using UnityEngine;
using System.Collections;

/*
Add a collider to your desired object and mark as trigger
Add this script to that object
Create an empty and name it whatever you desire
Move this empty to where you want the popup to be located
Set the variables in this script to the desired settings
(Message will be thr popups text)
ThreedText is our 3dtext prefab(we will not have to edit the prefab inless its a custom popup)
PopUpArea is the location of our popup

Once those are set and you go within the trigger the popup will appear and dissapear once we leave

*/

public class PopUpMessage : MonoBehaviour {
	public enum InteractType
		{
		popup,use
	}
	public InteractType Interactable;
	public string message;
	public GameObject threedtext;
//	public GameObject Boat;
	public GameObject PopUpArea;
	 TextMesh textcom;
	 GameObject textinstantiated;
	 GameObject player;
	GameObject playercamera;
	// Use this for initialization
	void Start () {
		textcom = threedtext.GetComponent<TextMesh> ();
		//textcom.text = message;
	}
	
	// Update is called once per frame
	void Update () {
	if (player != null && textinstantiated != null) {
			textinstantiated.transform.LookAt(playercamera.transform.position);

			switch(Interactable){
			case InteractType.use:
				if(Input.GetKeyDown(KeyCode.F)){
				player = null;
				Destroy (textinstantiated);
				}
				break;
				}
	}
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "player") {
			player = other.gameObject;
			playercamera = player.transform.FindChild("Main Camera").gameObject;
			textcom.text = message;
			textinstantiated = (GameObject)Instantiate(threedtext,PopUpArea.transform.position,Quaternion.identity);
			textinstantiated.transform.parent = transform;
				}
	}
	void OnTriggerExit(){
		player = null;
		Destroy (textinstantiated);
		}
}
